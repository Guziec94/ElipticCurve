using System;
using System.Numerics;
using System.Text;
using ExtensionMethods;

namespace ECDH
{
    class Program
    {
        static void Main(string[] args)
        {
            ElipticCurve elipticCurve = new ElipticCurve(256);
            elipticCurve.GenerateRandomCurve();

            Point randomPoint = elipticCurve.GenerateRandomPointOnCurve();

            BigInteger nA = PrimeHelper.GenerateRandom(64);
            BigInteger nB = PrimeHelper.GenerateRandom(64);

            Point qA = elipticCurve.ScalarMultiplication(randomPoint, nA);
            Point qB = elipticCurve.ScalarMultiplication(randomPoint, nB);

            if(qA == qB)
            {
                Console.WriteLine("Ok");
            }
            else
            {
                Console.WriteLine("Nie ok");
            }

            Console.ReadKey();
        }
    }
}
