using Autrage.LEX.NET.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.LEX.NET.Serialization
{
    public abstract class ObjectSerializer : Serializer
    {
        #region Methods

        private protected bool SerializeFields(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            IDictionary<string, FieldInfo> fields = Cache.GetFieldsFrom(instance.GetType());
            if (fields == null)
            {
                Warning($"Could not retrieve fields of {instance.GetType()} from cache!");
                return false;
            }

            stream.Write(fields.Count());

            foreach (var (name, info) in fields)
            {
                stream.Write(name, Marshaller.Encoding);

                // Recursive call to marshaller for cascading serialization
                Marshaller.Serialize(stream, info.GetValue(instance));
            }

            return true;
        }

        private protected bool DeserializeFields(Stream stream, object instance)
        {
            stream.AssertNotNull();

            int? fieldCount = stream.ReadInt();
            if (fieldCount == null)
            {
                Warning($"Could not read field count!");
                return false;
            }

            Type type = instance.GetType();
            IDictionary<string, FieldInfo> fields = Cache.GetFieldsFrom(type);
            for (int i = 0; i < fieldCount; i++)
            {
                string name = stream.ReadString(Marshaller.Encoding);
                if (name == null)
                {
                    Warning($"Could not read field name!");
                    return false;
                }

                // Recursive call to marshaller for cascading deserialization
                object value = Marshaller.Deserialize(stream);

                FieldInfo info = fields.GetValueOrDefault(name);
                if (info == null)
                {
                    Log($"Deserialized value for field {type}.{name}, but failed to retrieve field info from cache - discarding value.");
                    continue;
                }
                if (!info.FieldType.IsInstanceOfType(value))
                {
                    Log($"Deserialized value for field {type}.{name}, but value is not instance of field type {info.FieldType} - discarding value.");
                    continue;
                }

                info.SetValue(instance, value);
            }

            return true;
        }

        private protected static object Instantiate(Type type)
        {
            type.AssertNotNull();

            if (Cache.SkipConstructorOf(type))
            {
                return FormatterServices.GetSafeUninitializedObject(type);
            }
            else
            {
                try
                {
                    return Activator.CreateInstance(type, true);
                }
                catch (MissingMethodException)
                {
                    Error($"No parameterless constructor found for {type}!");
                    throw;
                }
            }
        }

        #endregion Methods
    }
}