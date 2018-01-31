using System;
using System.Reflection;

namespace Autrage.LEX.NET.Extensions
{
    public static class MemberInfoExtensions
    {
        #region Methods

        public static Type GetFieldOrPropertyType(this MemberInfo member)
        {
            member.AssertNotNull();

            if (member is FieldInfo field)
            {
                return field.FieldType;
            }
            if (member is PropertyInfo property)
            {
                return property.PropertyType;
            }

            throw new Exception($"{nameof(member)} must be either {nameof(FieldInfo)} or {nameof(PropertyInfo)}!");
        }

        public static object GetFieldOrPropertyValue(this MemberInfo member, object obj)
        {
            member.AssertNotNull();

            if (member is FieldInfo field)
            {
                return field.GetValue(obj);
            }
            if (member is PropertyInfo property)
            {
                return property.GetValue(obj);
            }

            throw new Exception($"{nameof(member)} must be either {nameof(FieldInfo)} or {nameof(PropertyInfo)}!");
        }

        public static void SetFieldOrPropertyValue(this MemberInfo member, object obj, object value)
        {
            member.AssertNotNull();

            if (member is FieldInfo field)
            {
                field.SetValue(obj, value);
                return;
            }
            if (member is PropertyInfo property)
            {
                property.SetValue(obj, value);
                return;
            }

            throw new Exception($"{nameof(member)} must be either {nameof(FieldInfo)} or {nameof(PropertyInfo)}!");
        }

        #endregion Methods
    }
}