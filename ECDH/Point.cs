using System.Numerics;
using ExtensionMethods;

namespace ECDH
{
    public class Point
    {
        public BigInteger x, y;
        public BigInteger a;// f = x^3 + a*x + b (mod p) 
        public BigInteger p;// modulo p

        public Point() { }

        public Point(BigInteger? x, BigInteger? y, BigInteger a, BigInteger p)
        {
            if (x.HasValue)
            {
                this.x = x.Value;
            }
            if (y.HasValue)
            {
                this.y = y.Value;
            }
            this.a = a;
            this.p = p;
        }

        public static Point NeutralElement(BigInteger a, BigInteger p)
        {
            return new Point(null, null, a, p);
        }

        public override string ToString()
        {
            return $"X:\t{x}\nY:\t{y}";
        }

        public static Point operator +(Point a, Point b)
        {
            BigInteger A = a.a;
            BigInteger P = a.p;
            
            // Dodawanie elementu neutralnego zwraca ten sam punkt
            if (a == Point.NeutralElement(A, P))
            {
                return b;
            }
            else if (b == Point.NeutralElement(A, P))
            {
                return a;
            }
            else if (a.x != b.x)
            {
                BigInteger numerator = (b.y - a.y).Mod(P);
                BigInteger denominator = (b.x - a.x).Mod(P);
                if (denominator.IsZero)
                {
                    return Point.NeutralElement(A, P);
                }
                BigInteger m = (numerator * denominator.ModularInverse(P)).Mod(P);
                BigInteger x = (BigInteger.Pow(m, 2) - a.x - b.x).Mod(P);
                BigInteger y = (m * (a.x - x) - a.y).Mod(P);
                return new Point(x, y, A, P);
            }
            else if (a.x == b.x && a.y != b.y)
            {
                return Point.NeutralElement(A, P);
            }
            else if (a == b && a.y != 0)
            {
                BigInteger numerator = (3 * BigInteger.Pow(a.x, 2) + A).Mod(P);//licznik m
                BigInteger denominator = (2 * a.y).Mod(P);//mianownik m
                if (denominator.IsZero)
                {
                    return Point.NeutralElement(A, P);
                }
                //BigInteger m = numerator / denominator;
                //BigInteger m = numerator * denominator ^ -1;
                BigInteger m = (numerator * denominator.ModularInverse(P)).Mod(P);

                BigInteger x = (BigInteger.Pow(m, 2) - (2 * a.x)).Mod(P);
                BigInteger y = (m * (a.x - x) - a.y).Mod(P);
                return new Point(x, y, A, P);
            }
            else if (a == b && a.y == 0)
            {
                return Point.NeutralElement(A, P);
            }

            return new Point();
        }

        public static bool operator ==(Point a, Point b)
        {
            if (a.x == b.x && a.y == b.y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=(Point a, Point b)
        {
            if (a.x != b.x || a.y != b.y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
