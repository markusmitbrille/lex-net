using System;
using System.Reflection;

namespace Autrage.LEX.NET.Extensions
{
    public static class MemberInfoExtensions
    {
        public static Type GetFieldOrPropertyType(this MemberInfo member)
        {
            member.AssertNotNull();

            if (member is FieldInfo)
            {
                return ((FieldInfo)member).FieldType;
            }
            if (member is PropertyInfo)
            {
                return ((PropertyInfo)member).PropertyType;
            }

            throw new Exception($"{nameof(member)} must be either {nameof(FieldInfo)} or {nameof(PropertyInfo)}!");
        }

        public static object GetFieldOrPropertyValue(this MemberInfo member, object obj)
        {
            member.AssertNotNull();

            if (member is FieldInfo)
            {
                return ((FieldInfo)member).GetValue(obj);
            }
            if (member is PropertyInfo)
            {
                return ((PropertyInfo)member).GetValue(obj);
            }

            throw new Exception($"{nameof(member)} must be either {nameof(FieldInfo)} or {nameof(PropertyInfo)}!");
        }

        public static void SetFieldOrPropertyValue(this MemberInfo member, object obj, object value)
        {
            member.AssertNotNull();

            if (member is FieldInfo)
            {
                ((FieldInfo)member).SetValue(obj, value);
                return;
            }
            if (member is PropertyInfo)
            {
                ((PropertyInfo)member).SetValue(obj, value);
                return;
            }

            throw new Exception($"{nameof(member)} must be either {nameof(FieldInfo)} or {nameof(PropertyInfo)}!");
        }
    }
}