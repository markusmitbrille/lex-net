using System;

namespace Autrage.LEX.NET.Extensions
{
    public static class FloatExtensions
    {
        private const float epsilon = 10e-10f;

        public static bool IsAlmost(this float number, float other, float epsilon = FloatExtensions.epsilon)
            => Math.Abs(number - other) < epsilon;

        public static bool IsBetween(this float value, float bound1, float bound2) =>
               bound1 < bound2 ? bound1 < value && value < bound2 : bound2 < value && value < bound1;

        public static double Clamp(/* ref */ this float value, float bound1, float bound2)
        {
            float min = Math.Min(bound1, bound2);
            float max = Math.Max(bound1, bound2);

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

        public static float Clamp01(/* ref */ this float value)
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