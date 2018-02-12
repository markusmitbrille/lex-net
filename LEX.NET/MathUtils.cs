namespace Autrage.LEX.NET
{
    public static class MathUtils
    {
        private const int HashCodeCombinationPrime1 = 17;
        private const int HashCodeCombinationPrime2 = 31;

        /// <summary>
        /// Combines mutliple hashcodes into one.
        /// </summary>
        public static int CombineHashCodes(params int[] hashes)
        {
            unchecked
            {
                int hash = HashCodeCombinationPrime1;

                for (int i = 0; i < hashes.Length; i++)
                {
                    hash = hash * HashCodeCombinationPrime2 + hashes[i];
                }

                return hash;
            }
        }

        /// <summary>
        /// Returns the next higher power of 2.
        /// </summary>
        public static uint NextPowerOf2(this uint number)
        {
            number--;
            number |= number >> 1;
            number |= number >> 2;
            number |= number >> 4;
            number |= number >> 8;
            number |= number >> 16;
            return ++number;
        }

        /// <summary>
        /// Returns the next higher power of 2.
        /// </summary>
        public static int NextPowerOf2(this int number)
        {
            if (number < 0)
            {
                number *= -1;
                number--;
                number |= number >> 1;
                number |= number >> 2;
                number |= number >> 4;
                number |= number >> 8;
                number |= number >> 16;
                number++;
                number >>= 2;
                number *= -1;
            }
            else
            {
                number--;
                number |= number >> 1;
                number |= number >> 2;
                number |= number >> 4;
                number |= number >> 8;
                number |= number >> 16;
                number++;
            }
            return number;
        }
    }
}