using System.Collections.Generic;

namespace Autrage.LEX.NET.Extensions
{
    public static class DictionaryExtension
    {
        #region Methods

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            source.AssertNotNull(nameof(source));

            TValue value = default(TValue);
            source.TryGetValue(key, out value);
            return value;
        }

        #endregion Methods
    }
}