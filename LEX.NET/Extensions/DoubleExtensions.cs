namespace Autrage.LEX.NET.Extensions
{
    public static class DoubleExtensions
    {
        #region Methods

        public static bool IsBetween(this double value, double bound1, double bound2) =>
            bound1 < bound2 ? bound1 < value && value < bound2 : bound2 < value && value < bound1;

        #endregion Methods
    }
}