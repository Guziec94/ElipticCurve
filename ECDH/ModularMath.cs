using System.Numerics;
using ExtensionMethods;

namespace ECDH
{
    public static class ModularMath
    {
        public static BigInteger Modulo(BigInteger a, BigInteger modulo)
        {
            return (a % modulo + modulo) % modulo;
        }

        public static BigInteger ModularInverse(BigInteger a, BigInteger modulo)
        {
            // https://rosettacode.org/wiki/Modular_inverse#C.23
            if (modulo == 1)
            {
                return 0;
            }

            BigInteger m0 = modulo;
            BigInteger x = 1;
            BigInteger y = 0;
            BigInteger q;

            while (a > 1)
            {
                q = a / modulo;
                (a, modulo) = (modulo, a % modulo);
                (x, y) = (y, x - q * y);
            }

            return x < 0
                ? x + m0
                : x;
        }
    }
}
