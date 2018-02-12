using System;

namespace Autrage.LEX.NET.Extensions
{
    public static class TypeExtensions
    {
        public static object GetDefault(this Type type)
        {
            type.AssertNotNull(nameof(type));
            return type.IsValueType ? Activator.CreateInstance(type, true) : null;
        }
    }
}