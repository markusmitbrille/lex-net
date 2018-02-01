using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Autrage.LEX.NET.Serialization
{
    internal static class Cache
    {
        #region Properties

        private static Dictionary<Type, string> NamesByType { get; } = new Dictionary<Type, string>();

        private static Dictionary<Type, bool> SkipConstructorOfType { get; } = new Dictionary<Type, bool>();

        private static Dictionary<FieldInfo, string> NamesByField { get; } = new Dictionary<FieldInfo, string>();

        private static Dictionary<Type, IEnumerable<FieldInfo>> FieldsByType { get; } = new Dictionary<Type, IEnumerable<FieldInfo>>();

        private static Dictionary<string, Type> TypesByName { get; } = new Dictionary<string, Type>();

        private static Dictionary<(Type type, string name), FieldInfo> FieldsByNameAndType { get; } = new Dictionary<(Type, string), FieldInfo>();

        #endregion Properties

        #region Methods

        public static string GetNameFrom(Type type)
        {
            type.AssertNotNull();

            if (!NamesByType.ContainsKey(type))
            {
                DataContractAttribute contract = type.GetCustomAttribute<DataContractAttribute>();
                if (contract == null || string.IsNullOrEmpty(contract.Name))
                {
                    NamesByType[type] = type.FullName;
                }
                else
                {
                    NamesByType[type] = contract.Name;
                }
            }

            return NamesByType[type];
        }

        public static bool SkipConstructorOf(Type type)
        {
            type.AssertNotNull();

            if (!SkipConstructorOfType.ContainsKey(type))
            {
                DataContractAttribute contract = type.GetCustomAttribute<DataContractAttribute>();
                if (contract == null)
                {
                    SkipConstructorOfType[type] = false;
                }
                else
                {
                    SkipConstructorOfType[type] = contract.SkipConstructor;
                }
            }

            return SkipConstructorOfType[type];
        }

        public static string GetNameFrom(FieldInfo field)
        {
            field.AssertNotNull();

            if (!NamesByField.ContainsKey(field))
            {
                DataMemberAttribute member = field.GetCustomAttribute<DataMemberAttribute>();
                if (member == null || string.IsNullOrEmpty(member.Name))
                {
                    NamesByField[field] = field.Name;
                }
                else
                {
                    NamesByField[field] = member.Name;
                }
            }

            return NamesByField[field];
        }

        public static IEnumerable<FieldInfo> GetFieldsFrom(Type type)
        {
            type.AssertNotNull();

            if (!FieldsByType.ContainsKey(type))
            {
                if (type.IsDefined(typeof(DataContractAttribute)))
                {
                    FieldsByType[type] =
                        from field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        where field.IsDefined(typeof(DataMemberAttribute))
                        select field;
                }
                else
                {
                    FieldsByType[type] = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
                }
            }

            return FieldsByType[type];
        }

        public static Type GetTypeFrom(string name)
        {
            name.AssertNotNull();

            if (!TypesByName.ContainsKey(name))
            {
                IEnumerable<Type> allTypes =
                    from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from t in assembly.GetTypes()
                    select t;

                Type type = allTypes.SingleOrDefault(t => t.GetCustomAttribute<DataContractAttribute>()?.Name == name);
                if (type == null)
                {
                    type = allTypes.SingleOrDefault(t => t.FullName == name);
                }

                TypesByName[name] = type;
            }

            return TypesByName[name];
        }

        public static FieldInfo GetFieldFrom(Type type, string name)
        {
            type.AssertNotNull();
            name.AssertNotNull();

            if (!FieldsByNameAndType.ContainsKey((type, name)))
            {
                IEnumerable<FieldInfo> fields = GetFieldsFrom(type);

                FieldInfo field = fields.SingleOrDefault(f => f.GetCustomAttribute<DataMemberAttribute>()?.Name == name);
                if (field == null)
                {
                    field = fields.SingleOrDefault(f => f.Name == name);
                }

                FieldsByNameAndType[(type, name)] = field;
            }

            return FieldsByNameAndType[(type, name)];
        }

        #endregion Methods
    }
}