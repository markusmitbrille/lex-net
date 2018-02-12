using System;

namespace Autrage.LEX.NET.Extensions
{
    public static class DoubleExtensions
    {
        public static bool IsBetween(this double value, double bound1, double bound2) =>
            bound1 < bound2 ? bound1 < value && value < bound2 : bound2 < value && value < bound1;

        public static double Clamp(ref this double value, double bound1, double bound2)
        {
            double min = Math.Min(bound1, bound2);
            double max = Math.Max(bound1, bound2);

            if (value < min)
            {
                return value = min;
            }
            else if (value > max)
            {
                return value = max;
            }
            else
            {
                return value;
            }
        }

        public static double Clamp01(ref this double value)
        {
            if (value < 0)
            {
                return value = 0;
            }
            else if (value > 1)
            {
                return value = 1;
            }
            else
            {
                return value;
            }
        }
    }
}