using System;
using System.Collections.Generic;
using System.Linq;

namespace Autrage.LEX.NET.Extensions
{
    public static class IEnumerableExtensions
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source) => new HashSet<T>(source);

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source)
            {
                action(item);
            }
        }

        public static bool None<T>(this IEnumerable<T> source) => !source.Any();

        public static bool None<T>(this IEnumerable<T> source, Predicate<T> predicate) => source.All(e => !predicate(e));
    }
}