using System;
using System.Numerics;
using ExtensionMethods;

namespace ECDH
{
    public class ElipticCurve
    {
        public BigInteger p, a, b, pMinus1, delta;
        private int primeSize = 256;
        private Random RandomGenerator = new Random(DateTime.Now.Millisecond);
        
        public ElipticCurve() { }
        public ElipticCurve (int primeNumberSize)
        {
            primeSize = primeNumberSize;
        }

        public ElipticCurve (BigInteger a, BigInteger b, BigInteger p)
        {
            this.a = a;
            this.b = b;
            this.p = p;
        }

        /// <summary>
        /// Calculates delta: Delta = 4a^3 + 27b^2 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public BigInteger CalculateDelta(BigInteger a, BigInteger b)
        {
            BigInteger temp1, temp2;

            temp1 = BigInteger.Pow(a, 3);
            temp1 = BigInteger.Multiply(4, temp1);
            temp2 = BigInteger.Pow(b, 2);
            temp2 = BigInteger.Multiply(27, temp2);

            return BigInteger.Add(temp1, temp2);
        }

        /// <summary>
        /// Calculates: x^3 + a*x + b (mod p)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="x"></param>
        public BigInteger CalculateF(BigInteger a, BigInteger b, BigInteger x, BigInteger p)
        {
            BigInteger temp1, temp2;

            temp1 = BigInteger.Pow(x, 3);// x^3
            temp2 = BigInteger.Multiply(a, x);// a*x

            return (temp1 + temp2 + b).Mod(p);
        }

        /// <summary>
        /// Calculates Lagrange Symbol. Returns -1 / 0 / 1.
        /// </summary>
        /// <param name="a">"Numerator" - number above a line</param>
        /// <param name="p">"Denominator" - number below a line</param>
        /// <returns></returns>
        public BigInteger CalculateLegendreSymbol(BigInteger a, BigInteger p)
        {
            BigInteger temp;
            // a^((p-1)/2) mod p
            temp = BigInteger.Subtract(p, BigInteger.One);//p - 1
            temp = BigInteger.Divide(temp, 2);//(p - 1) / 2
            return BigInteger.ModPow(a, temp, p);
        }

        public void GenerateRandomCurve()
        {
            // 1 - generowanie liczby pierwszej przystającej do 3 modulo 4 
            p = PrimeHelper.GenerateRandomPrimeForElipticCurve(primeSize);

            pMinus1 = BigInteger.Subtract(p, BigInteger.One);
            while (true)
            {
                // 2 - losowanie a, b, chi
                a = PrimeHelper.GenerateRandom(RandomGenerator.Next(2, primeSize - 1));
                b = PrimeHelper.GenerateRandom(RandomGenerator.Next(2, primeSize - 1));

                // 2.1 - jeśli delta przystaje do 0 to skok do punktu 2
                delta = CalculateDelta(a, b);
                if (delta.Mod(p) != 0)
                {
                    // Krzywa gotowa, można szukać punktu należącego do krzywej
                    break;
                }
            }


            Console.WriteLine($"p:\t{p}");
            Console.WriteLine($"a:\t{a}");
            Console.WriteLine($"b:\t{b}");
            Console.WriteLine($"delta:\t{delta}");
        }

        public bool CheckIfPointBelongsToCurve(Point point)
        {
            Point neutral = Point.NeutralElement(a, p);
            if(point == neutral)
            {
                return true;
            }
            BigInteger left = BigInteger.Pow(point.y, 2).Mod(p);
            BigInteger right = (BigInteger.Pow(point.x, 3) + a * point.x + b).Mod(p);
            return left == right;
        }

        public Point GenerateRandomPointOnCurve()
        {
            BigInteger x, y, f, legendreFP;

            while (true)
            {
                x = PrimeHelper.GenerateRandom(RandomGenerator.Next(2, primeSize - 1));

                f = CalculateF(a, b, x, p);
                legendreFP = CalculateLegendreSymbol(f, p);
                if (legendreFP == BigInteger.One)
                {
                    y = BigInteger.ModPow(f, (p + 1) / 4, p);
                    BigInteger left, right;
                    left = BigInteger.ModPow(y, 2, p);
                    right = (BigInteger.Pow(x, 3) + a * x + b).Mod(p);
                    bool test = left == right;
                    if (test)
                    {
                        break;
                    }
                }
            }

            return new Point(x, y, a, p);
        }

        public Point ScalarMultiplication(Point P, BigInteger n)
        {
            // 1)
            Point Q = P;

            // 2)
            string binaryString = PrimeHelper.BigintegerToBinaryString(n);

            Point R = binaryString[0] == '0' 
                ? Point.NeutralElement(P.a, P.p)
                : P;

            // 3)
            for (int i = 1; i < binaryString.Length; i++)
            {
                Q = Q + Q;
                if(binaryString[i] == '1')
                {
                    R = R + Q;
                }
            }

            return R;
        }
    }
}
