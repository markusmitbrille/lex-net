using System;

namespace Autrage.LEX.NET.Extensions
{
    public static class FloatExtensions
    {
        #region Fields

        private const float epsilon = 10e-10f;

        #endregion Fields

        #region Methods

        public static bool IsAlmost(this float number, float other, float epsilon = FloatExtensions.epsilon)
            => Math.Abs(number - other) < epsilon;

        #endregion Methods
    }
}