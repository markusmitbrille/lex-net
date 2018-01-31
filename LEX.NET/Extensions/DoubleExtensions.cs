namespace Autrage.LEX.NET.Extensions
{
    public static class DoubleExtensions
    {
        #region Methods

        public static bool IsBetween(this double value, double bound1, double bound2)
        {
            if (bound1 < bound2)
            {
                return bound1 < value && value < bound2;
            }
            else
            {
                return bound2 < value && value < bound1;
            }
        }

        #endregion Methods
    }
}