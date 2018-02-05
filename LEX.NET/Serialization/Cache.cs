using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.LEX.NET.Serialization
{
    internal static class Cache
    {
        #region Properties

        private static Dictionary<Type, string> NamesByType { get; } = new Dictionary<Type, string>();

        private static Dictionary<Type, bool> SkipConstructorOfType { get; } = new Dictionary<Type, bool>();

        private static Dictionary<Type, IDictionary<string, FieldInfo>> FieldsByType { get; } = new Dictionary<Type, IDictionary<string, FieldInfo>>();

        private static Dictionary<string, Type> TypesByName { get; } = new Dictionary<string, Type>();

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

        public static IDictionary<string, FieldInfo> GetFieldsFrom(Type type)
        {
            type.AssertNotNull();

            if (!FieldsByType.ContainsKey(type))
            {
                try
                {
                    if (type.IsDefined(typeof(DataContractAttribute)))
                    {
                        FieldsByType[type] =
                            (from field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                             let attribute = field.GetCustomAttribute<DataMemberAttribute>()
                             where attribute != null
                             let name = string.IsNullOrEmpty(attribute.Name) ? field.Name : attribute.Name
                             select new { name, field })
                             .ToDictionary(e => e.name, e => e.field);
                    }
                    else
                    {
                        FieldsByType[type] =
                            (from field in type.GetFields(BindingFlags.Instance | BindingFlags.Public)
                             let attribute = field.GetCustomAttribute<DataMemberAttribute>()
                             let name = string.IsNullOrEmpty(attribute?.Name) ? field.Name : attribute.Name
                             select new { name, field })
                             .ToDictionary(e => e.name, e => e.field);
                    }
                }
                catch (ArgumentException)
                {
                    Error($"Duplicate names found for fields in {type}!");
                    throw;
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

        #endregion Methods
    }
}