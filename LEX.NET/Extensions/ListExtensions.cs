using System.Collections.Generic;

namespace Autrage.LEX.NET.Extensions
{
    public static class ListExtensions
    {
        #region Methods

        public static T Dequeue<T>(this List<T> source)
        {
            source.AssertNotNull(nameof(source));

            if (source.Count == 0)
            {
                return default(T);
            }

            T obj = source[0];
            source.RemoveAt(0);

            return obj;
        }

        public static T Pop<T>(this List<T> source)
        {
            source.AssertNotNull(nameof(source));

            if (source.Count == 0)
            {
                return default(T);
            }

            T obj = source[source.Count - 1];
            source.RemoveAt(source.Count - 1);

            return obj;
        }

        public static T GetValueOrDefault<T>(this List<T> source, int index)
        {
            if (source.Count == 0)
            {
                return default(T);
            }

            if (index < 0 || index >= source.Count)
            {
                return default(T);
            }

            return source[index];
        }

        public static bool Swap<T>(this List<T> source, T item1, T item2)
        {
            if (item1 == null ||
                item2 == null ||
                item1.Equals(item2))
            {
                return false;
            }

            return source.Swap(source.IndexOf(item1), source.IndexOf(item2));
        }

        public static bool Swap<T>(this List<T> source, int index1, int index2)
        {
            if (index1 < 0 ||
                index2 < 0 ||
                index1 >= source.Count ||
                index2 >= source.Count ||
                index1 == index2)
            {
                return false;
            }

            T tmp = source[index1];
            source[index1] = source[index2];
            source[index2] = tmp;

            return true;
        }

        public static bool MoveElementForward<T>(this List<T> source, T element)
        {
            int index = source.IndexOf(element);
            return source.Swap(index, index + 1);
        }

        public static bool MoveElementBackward<T>(this List<T> source, T element)
        {
            int index = source.IndexOf(element);
            return source.Swap(index, index - 1);
        }

        public static bool MoveElementForward<T>(this List<T> source, int index)
        {
            return source.Swap(index, index + 1);
        }

        public static bool MoveElementBackward<T>(this List<T> source, int index)
        {
            return source.Swap(index, index - 1);
        }

        #endregion Methods
    }
}