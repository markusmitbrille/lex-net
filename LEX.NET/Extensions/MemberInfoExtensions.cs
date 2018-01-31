using System;
using System.Reflection;

using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.LEX.NET.Extensions
{
    public static class MemberInfoExtensions
    {
        #region Methods

        public static Type GetFieldOrPropertyType(this MemberInfo member)
        {
            member.AssertNotNull();

            if (member.MemberType == MemberTypes.Field)
            {
                FieldInfo field = (FieldInfo)member;
                return field.FieldType;
            }
            else if (member.MemberType == MemberTypes.Property)
            {
                PropertyInfo prop = (PropertyInfo)member;
                return prop.PropertyType;
            }

            Fail($"{nameof(member)} must be either {nameof(FieldInfo)} or {nameof(PropertyInfo)}!");
            return null;
        }

        public static object GetFieldOrPropertyValue(this MemberInfo member, object obj)
        {
            member.AssertNotNull();

            if (member.MemberType == MemberTypes.Field)
            {
                FieldInfo field = (FieldInfo)member;
                return field.GetValue(obj);
            }
            else if (member.MemberType == MemberTypes.Property)
            {
                PropertyInfo prop = (PropertyInfo)member;
                return prop.GetValue(obj, null);
            }

            Fail($"{nameof(member)} must be either {nameof(FieldInfo)} or {nameof(PropertyInfo)}!");
            return null;
        }

        public static void SetFieldOrPropertyValue(this MemberInfo member, object obj, object value)
        {
            member.AssertNotNull();

            if (member.MemberType == MemberTypes.Field)
            {
                FieldInfo field = (FieldInfo)member;
                field.SetValue(obj, value);
                return;
            }
            else if (member.MemberType == MemberTypes.Property)
            {
                PropertyInfo prop = (PropertyInfo)member;
                prop.SetValue(obj, value, null);
                return;
            }

            Fail($"{nameof(member)} must be either {nameof(FieldInfo)} or {nameof(PropertyInfo)}!");
        }

        #endregion Methods
    }
}