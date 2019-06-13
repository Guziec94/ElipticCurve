using System.Numerics;
using ECDH;

namespace ExtensionMethods
{
    public static class MyExtension
    {
        public static BigInteger Mod(this BigInteger a, BigInteger b)
        {
            return ModularMath.Modulo(a, b);
        }

        public static BigInteger ModularInverse(this BigInteger a, BigInteger modulo)
        {
            return ModularMath.ModularInverse(a, modulo);
        }
    }
}