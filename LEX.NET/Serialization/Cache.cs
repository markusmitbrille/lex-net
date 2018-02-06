﻿using System;
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

        private static Dictionary<Type, IDictionary<Type, MethodInfo>> AddMethodsByType { get; } = new Dictionary<Type, IDictionary<Type, MethodInfo>>();

        private static Dictionary<Type, PropertyInfo> CountPropertiesByType { get; } = new Dictionary<Type, PropertyInfo>();

        #endregion Properties

        #region Methods

        public static string GetNameFrom(Type type)
        {
            type.AssertNotNull();

            if (!NamesByType.ContainsKey(type))
            {
                NamesByType[type] = type.AssemblyQualifiedName;
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
                             where field.IsDefined(typeof(DataMemberAttribute))
                             let name = field.Name
                             select new { name, field })
                             .ToDictionary(e => e.name, e => e.field);
                    }
                    else
                    {
                        FieldsByType[type] =
                            (from field in type.GetFields(BindingFlags.Instance | BindingFlags.Public)
                             let name = field.Name
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

        public static Type GetTypeFrom(string name, Func<AssemblyName, Assembly> assemblyResolver, Func<Assembly, string, bool, Type> typeResolver)
        {
            name.AssertNotNull();

            if (!TypesByName.ContainsKey(name))
            {
                TypesByName[name] = Type.GetType(name, assemblyResolver, typeResolver, false, false);
            }

            return TypesByName[name];
        }

        public static IDictionary<Type, MethodInfo> GetAddMethodsFrom(Type type)
        {
            type.AssertNotNull();

            if (!AddMethodsByType.ContainsKey(type))
            {
                IDictionary<Type, MethodInfo> addMethods =
                    (from i in type.GetInterfaces()
                     where i.IsGenericType
                     where i.GetGenericTypeDefinition() == typeof(ICollection<>)
                     let itemType = i.GenericTypeArguments.SingleOrDefault()
                     where itemType != null
                     let addMethod = i.GetMethod(nameof(ICollection<object>.Add), new Type[] { itemType })
                     select new { itemType, addMethod })
                     .ToDictionary(e => e.itemType, e => e.addMethod);

                AddMethodsByType[type] = addMethods;
            }

            return AddMethodsByType[type];
        }

        public static PropertyInfo GetCountPropertyFrom(Type type)
        {
            type.AssertNotNull();

            if (!CountPropertiesByType.ContainsKey(type))
            {
                CountPropertiesByType[type] = type.GetProperty(nameof(ICollection<object>.Count));
            }

            return CountPropertiesByType[type];
        }

        #endregion Methods
    }
}