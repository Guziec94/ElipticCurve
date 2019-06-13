using System;
using System.Collections.Generic;
using System.Numerics;
using System.Collections;
using System.Text;

namespace ECDH
{
    public enum NumberType
    {
        Composite,
        Prime
    }

    public static class PrimeHelper
    {
        public static string BigintegerToBinaryString(BigInteger bigint)
        {
            var bytes = bigint.ToByteArray();
            var idx = bytes.Length - 1;

            // Create a StringBuilder having appropriate capacity.
            var base2 = new StringBuilder(bytes.Length * 8);

            // Convert first byte to binary.
            var binary = Convert.ToString(bytes[idx], 2);

            // Append binary string to StringBuilder.
            base2.Append(binary);

            // Convert remaining bytes adding leading zeros.
            for (idx--; idx >= 0; idx--)
            {
                base2.Append(Convert.ToString(bytes[idx], 2).PadLeft(8, '0'));
            }

            return base2.ToString();
        }


        /// <summary>
        /// Generate BigInteger number (higher than 1) with specified size.
        /// </summary>
        /// <param name="size">Size of number in bits</param>
        /// <returns></returns>
        public static BigInteger GenerateRandom(int size)
        {
            Random randomGenerator = new Random(DateTime.Now.Millisecond);
            string numberAsBits = "1";

            for (int i = 0; i < size - 1; i++)
            {
                numberAsBits += randomGenerator.Next(0, 2);
            }

            BigInteger result = 0;
            foreach (char bit in numberAsBits)
            {
                result <<= 1;
                result += (bit == '1') ? 1 : 0;
            }

            return result;
        }

        /// <summary>
        /// Generates a random BigInteger between min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static BigInteger GenerateRandom(BigInteger min, BigInteger max)
        {
            byte[] maxBytes = max.ToByteArray();
            BitArray maxBits = new BitArray(maxBytes);
            Random randomGenerator = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i < maxBits.Length; i++)
            {
                // Randomly set the bit
                int randomInt = randomGenerator.Next();
                if ((randomInt % 2) == 0)
                {
                    // Reverse the bit
                    maxBits[i] = !maxBits[i];
                }
            }

            BigInteger result = new BigInteger();

            // Convert the bits back to a BigInteger
            for (int k = (maxBits.Count - 1); k >= 0; k--)
            {
                BigInteger bitValue = 0;

                if (maxBits[k])
                {
                    bitValue = BigInteger.Pow(2, k);
                }

                result = BigInteger.Add(result, bitValue);
            }

            // Generate the random number
            BigInteger randomBigInt = BigInteger.ModPow(result, 1, BigInteger.Add(max, min));
            return randomBigInt;
        }

        /// <summary>
        /// Generate prime number with specified size
        /// </summary>
        /// <param name="size">Size of number in bits</param>
        /// <returns></returns>
        public static BigInteger GenerateRandomPrime(int size)
        {
            BigInteger primeNumber = 0;

            bool primeFound = false;
            while (!primeFound)
            {
                BigInteger temp = GenerateRandom(size);

                if (IsPrimeMillerRabin(temp))
                {
                    primeFound = true;
                    primeNumber = temp;
                }
            }
            return primeNumber;
        }

        /// <summary>
        /// Generate prime number P with specified size. P is equal to 4 * k + 3
        /// </summary>
        /// <param name="size">Size of number in bits</param>
        /// <returns></returns>
        public static BigInteger GenerateRandomPrimeForElipticCurve(int size)
        {
            BigInteger primeNumber = 0;

            bool primeFound = false;
            while (!primeFound)
            {
                BigInteger temp = GenerateRandom(size);

                temp = BigInteger.Multiply(temp, 4) + 3;

                if (IsPrimeMillerRabin(temp))
                {
                    primeFound = true;
                    primeNumber = temp;
                }
            }
            return primeNumber;
        }


        public static bool IsPrimeMillerRabin(BigInteger integer)
        {
            NumberType type = MillerRabin(integer, 4000);
            return type == NumberType.Prime;
        }

        /// <summary>
        /// Check number primality based on Miller-Rabin
        /// </summary>
        /// <param name="number">Number to test</param>
        /// <param name="s">Number of iterations. Higher number improves probability.</param>
        /// <returns></returns>
        public static NumberType MillerRabin(BigInteger number, int s)
        {
            BigInteger nMinusOne = BigInteger.Subtract(number, 1);

            for (int j = 1; j <= s; j++)
            {
                BigInteger a = GenerateRandom(1, nMinusOne);

                if (Witness(a, number))
                {
                    return NumberType.Composite;
                }
            }

            return NumberType.Prime;
        }

        public static bool Witness(BigInteger a, BigInteger n)
        {
            KeyValuePair<int, BigInteger> tAndU = GetTAndU(BigInteger.Subtract(n, 1));
            int t = tAndU.Key;
            BigInteger u = tAndU.Value;
            BigInteger[] x = new BigInteger[t + 1];

            x[0] = BigInteger.ModPow(a, u, n);

            for (int i = 1; i <= t; i++)
            {
                // x[i] = x[i-1]^2 mod n
                x[i] = BigInteger.ModPow(BigInteger.Multiply(x[i - 1], x[i - 1]), 1, n);
                BigInteger minus = BigInteger.Subtract(x[i - 1], BigInteger.Subtract(n, 1));

                if (x[i] == 1 && x[i - 1] != 1 && !minus.IsZero)
                {
                    return true;
                }
            }

            if (!x[t].IsOne)
            {
                return true;
            }

            return false;
        }

        public static KeyValuePair<int, BigInteger> GetTAndU(BigInteger nMinusOne)
        {
            // Convert n - 1 to a byte array
            byte[] nBytes = nMinusOne.ToByteArray();
            BitArray bits = new BitArray(nBytes);
            int t = 0;
            BigInteger u = new BigInteger();

            int n = bits.Count - 1;
            bool lastBit = bits[n];

            // Calculate t
            while (!lastBit)
            {
                t++;
                n--;
                lastBit = bits[n];
            }

            for (int k = ((bits.Count - 1) - t); k >= 0; k--)
            {
                BigInteger bitValue = 0;

                if (bits[k])
                {
                    bitValue = BigInteger.Pow(2, k);
                }

                u = BigInteger.Add(u, bitValue);
            }

            KeyValuePair<int, BigInteger> tAndU = new KeyValuePair<int, BigInteger>(t, u);
            return tAndU;
        }
    }
}