using System;
using System.Collections.Generic;

namespace Autrage.LEX.NET.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Splits a flags enum value into its atmoic flags and returns them as an IEnumerable of integers.
        /// </summary>
        /// <param name="source">The combination of bitflags to split up.</param>
        /// <returns>An IEnumerable of integers that contains the integer representation of the bitflags set in <paramref name="source"/>.</returns>
        public static IEnumerable<int> GetAtomicFlags(this Enum source)
        {
            source.AssertNotNull(nameof(source));

            int numFlags = Convert.ToInt32(source);

            for (int currentFlag = 1; numFlags != 0; currentFlag = currentFlag << 1)
            {
                if ((numFlags & currentFlag) == currentFlag)
                {
                    yield return currentFlag;
                    numFlags ^= currentFlag;
                }
            }
        }
    }
}