using System;

namespace Autrage.LEX.NET.Extensions
{
    public static class Rnd
    {
        public static int Int() => Singleton<Random>.Instance.Next();

        public static int Int(int max) => Singleton<Random>.Instance.Next(max);

        public static int Int(int min, int max) => Singleton<Random>.Instance.Next(min, max);

        public static double Double() => Singleton<Random>.Instance.NextDouble();

        public static bool Choice() => Singleton<Random>.Instance.NextDouble() < 0.5;

        /// <summary>
        /// Returns <code>true</code> with a probability of <paramref name="probability"/>.
        /// </summary>
        /// <param name="probability">Prabability of <code>true</code> being returned, ranging from 0 to 1 (100%).</param>
        public static bool Chance(double probability) => Singleton<Random>.Instance.NextDouble() < probability;
    }
}