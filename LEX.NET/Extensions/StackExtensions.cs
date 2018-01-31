using System.Collections.Generic;

namespace Autrage.LEX.NET.Extensions
{
    public static class StackExtensions
    {
        #region Methods

        public static void PushAll<T>(this Stack<T> stack, IEnumerable<T> items)
        {
            stack.AssertNotNull(nameof(stack));
            items.AssertNotNull(nameof(items));

            foreach (T item in items)
            {
                stack.Push(item);
            }
        }

        public static T PopOrDefault<T>(this Stack<T> stack)
        {
            stack.AssertNotNull(nameof(stack));

            return stack.Count > 0 ? stack.Pop() : default(T);
        }

        #endregion Methods
    }
}