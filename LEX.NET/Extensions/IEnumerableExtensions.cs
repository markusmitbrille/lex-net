using System.Collections.Generic;

namespace Autrage.LEX.NET.Extensions
{
    public static class IEnumerableExtensions
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source) => new HashSet<T>(source);
    }
}