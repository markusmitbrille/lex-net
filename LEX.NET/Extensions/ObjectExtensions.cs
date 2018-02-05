﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Autrage.LEX.NET.Extensions
{
    public static class ObjectExtensions
    {
        public static void Print(this object obj, bool instances = true, bool statics = false, bool publics = true, bool nonPublics = false)
        {
            obj.AssertNotNull(nameof(obj));

            BindingFlags bindingFlags = BindingFlags.Default;
            if (instances)
                bindingFlags |= BindingFlags.Instance;
            if (statics)
                bindingFlags |= BindingFlags.Static;
            if (publics)
                bindingFlags |= BindingFlags.Public;
            if (nonPublics)
                bindingFlags |= BindingFlags.NonPublic;

            Console.Write(obj);
            PrintFields(obj, bindingFlags);
        }

        private static void PrintFields(object obj, BindingFlags bindingFlags)
        {
            IEnumerable<FieldInfo> fields = obj.GetType().GetFields(bindingFlags);
            if (fields.Count() < 5 && fields.All(f => f.FieldType.IsPrimitive || f.FieldType.IsEnum))
            {
                PrintFieldsSimple(obj, fields);
            }
            else
            {
                PrintFieldsComplex(obj, fields);
            }
        }

        private static void PrintFieldsSimple(object obj, IEnumerable<FieldInfo> fields)
        {
            Console.Write("Fields:");
            foreach (FieldInfo field in fields)
            {
                Console.Write($" {field.Name}={field.GetValue(obj)};");
            }
            Console.WriteLine();
        }

        private static void PrintFieldsComplex(object obj, IEnumerable<FieldInfo> fields)
        {
            Console.WriteLine();
            Console.WriteLine("Fields:");
            foreach (FieldInfo field in fields)
            {
                Console.WriteLine($"\t{field.Name}={field.GetValue(obj)};");
            }
            Console.WriteLine();
        }

        private static void PrintProperties(object obj, BindingFlags bindingFlags)
        {
            IEnumerable<PropertyInfo> properties = obj.GetType().GetProperties(bindingFlags).Where(p => p.CanRead);
            if (properties.Count() < 5 && properties.All(p => p.PropertyType.IsPrimitive || p.PropertyType.IsEnum))
            {
                PrintPropertiesSimple(obj, properties);
            }
            else
            {
                PrintPropertiesComplex(obj, properties);
            }
        }

        private static void PrintPropertiesSimple(object obj, IEnumerable<PropertyInfo> properties)
        {
            Console.Write("Properties:");
            foreach (PropertyInfo property in properties)
            {
                Console.Write($" {property.Name}={property.GetValue(obj)};");
            }
            Console.WriteLine();
        }

        private static void PrintPropertiesComplex(object obj, IEnumerable<PropertyInfo> properties)
        {
            Console.WriteLine();
            Console.WriteLine("Properties:");
            foreach (PropertyInfo property in properties)
            {
                Console.WriteLine($"\t{property.Name}={property.GetValue(obj)};");
            }
            Console.WriteLine();
        }
    }
}
