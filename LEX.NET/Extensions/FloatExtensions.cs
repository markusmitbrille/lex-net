using System;

namespace Autrage.LEX.NET.Extensions
{
    public static class FloatExtensions
    {
        private const float epsilon = 10e-10f;

        public static bool IsAlmost(this float number, float other, float epsilon = FloatExtensions.epsilon)
            => Math.Abs(number - other) < epsilon;
    }
}