using System.Collections.Generic;

namespace Autrage.LEX.NET.Extensions
{
    public static class KeyValuePairExtensions
    {
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
        {
            pair.AssertNotNull();

            key = pair.Key;
            value = pair.Value;
        }
    }
}