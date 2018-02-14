using Autrage.LEX.NET.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using static Autrage.LEX.NET.Bugger;

namespace Autrage.LEX.NET.Serialization
{
    public abstract class ObjectSerializer : Serializer
    {
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

        private protected bool SerializeMembers(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            return SerializeFields(stream, instance) && SerializeProperties(stream, instance);
        }

        private protected void DeserializeMembers(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            DeserializeFields(stream, instance);
            DeserializeProperties(stream, instance);
        }

        private bool SerializeFields(Stream stream, object instance)
        {
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

        private bool SerializeProperties(Stream stream, object instance)
        {
            IDictionary<string, PropertyInfo> properties = Cache.GetPropertiesFrom(instance.GetType());
            if (properties == null)
            {
                Warning($"Could not retrieve properties of {instance.GetType()} from cache!");
                return false;
            }

            stream.Write(properties.Count());

            foreach (var (name, info) in properties)
            {
                stream.Write(name, Marshaller.Encoding);

                // Recursive call to marshaller for cascading serialization
                Marshaller.Serialize(stream, info.GetValue(instance));
            }

            return true;
        }

        private void DeserializeFields(Stream stream, object instance)
        {
            int? count = stream.ReadInt();
            if (count == null)
            {
                Warning($"Could not read field count!");
                return;
            }

            Type type = instance.GetType();
            IDictionary<string, FieldInfo> fields = Cache.GetFieldsFrom(type);
            for (int i = 0; i < count; i++)
            {
                string name = stream.ReadString(Marshaller.Encoding);
                if (name == null)
                {
                    Warning($"Could not read field name!");
                    return;
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
        }

        private void DeserializeProperties(Stream stream, object instance)
        {
            int? count = stream.ReadInt();
            if (count == null)
            {
                Warning($"Could not read property count!");
                return;
            }

            Type type = instance.GetType();
            IDictionary<string, PropertyInfo> properties = Cache.GetPropertiesFrom(type);
            for (int i = 0; i < count; i++)
            {
                string name = stream.ReadString(Marshaller.Encoding);
                if (name == null)
                {
                    Warning($"Could not read property name!");
                    return;
                }

                // Recursive call to marshaller for cascading deserialization
                object value = Marshaller.Deserialize(stream);

                PropertyInfo info = properties.GetValueOrDefault(name);
                if (info == null)
                {
                    Log($"Deserialized value for property {type}.{name}, but failed to retrieve property info from cache - discarding value.");
                    continue;
                }
                if (!info.PropertyType.IsInstanceOfType(value))
                {
                    Log($"Deserialized value for property {type}.{name}, but value is not instance of property type {info.PropertyType} - discarding value.");
                    continue;
                }

                info.SetValue(instance, value);
            }
        }
    }
}