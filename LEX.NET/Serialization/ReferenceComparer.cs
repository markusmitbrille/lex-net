using System.Collections.Generic;

namespace Autrage.LEX.NET.Serialization
{
    public sealed class ReferenceComparer : IEqualityComparer<object>
    {
        #region Methods

        public new bool Equals(object x, object y) => ReferenceEquals(x, y);

        public int GetHashCode(object obj) => obj?.GetHashCode() ?? 0;

        #endregion Methods
    }
}