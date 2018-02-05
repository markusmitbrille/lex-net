using Autrage.LEX.NET.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.LEX.NET.Serialization
{
    public abstract class ObjectSerializer : Serializer
    {
        #region Properties

        public Encoding Encoding { get; set; } = Encoding.UTF8;

        #endregion Properties

        #region Methods

        private protected bool SerializeFields(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            IEnumerable<FieldInfo> fields = Cache.GetFieldsFrom(instance.GetType());
            if (fields == null)
            {
                Warning($"Could not serialize {instance.GetType()} fields, could not get fields!");
                return false;
            }

            stream.Write(fields.Count());

            foreach (FieldInfo field in fields)
            {
                if (!SerializeField(stream, instance, field)) return false;
            }

            return true;
        }

        private protected bool DeserializeFields(Stream stream, object instance)
        {
            stream.AssertNotNull();

            int? fieldCount = stream.ReadInt();
            if (fieldCount == null)
            {
                Warning($"Could not deserialize field count!");
                return false;
            }

            List<Field> fields = new List<Field>();
            for (int i = 0; i < fieldCount; i++)
            {
                Field field = DeserializeField(stream);
                if (field == null)
                {
                    Warning($"Could not deserialize field [{i}]!");
                    return false;
                }

                fields.Add(field);
            }

            SetFields(instance, fields);

            return true;
        }

        private protected Type DeserializeType(Stream stream)
        {
            string name = stream.ReadString(Encoding);
            if (name == null)
            {
                Warning($"Could not deserialize type name!");
                return null;
            }

            Type type = Cache.GetTypeFrom(name);
            if (type == null)
            {
                Warning($"Could not deserialize type {name}!");
                return null;
            }

            return type;
        }

        private bool SerializeField(Stream stream, object instance, FieldInfo field)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            string fieldName = Cache.GetNameFrom(field);
            if (fieldName == null)
            {
                Warning($"Could not serialize {field.FieldType} field {field.Name}, could not get field name!");
                return false;
            }

            string fieldTypeName = Cache.GetNameFrom(field.FieldType);
            if (fieldTypeName == null)
            {
                Warning($"Could not serialize {field.FieldType} field {field.Name}, could not get field type name!");
                return false;
            }

            stream.Write(fieldName, Encoding);
            stream.Write(fieldTypeName, Encoding);
            Serialize(stream, field.GetValue(instance));

            return true;
        }

        private Field DeserializeField(Stream stream)
        {
            stream.AssertNotNull();

            string name = stream.ReadString(Encoding);
            if (name == null)
            {
                Warning($"Could not deserialize field name!");
                return null;
            }

            Type type = DeserializeType(stream);
            if (type == null)
            {
                Warning($"Could not deserialize {name} field type!");
                return null;
            }

            object value = Deserialize(stream, type);

            return new Field(name, type, value);
        }

        private void SetFields(object instance, IEnumerable<Field> fields)
        {
            instance.AssertNotNull();
            fields.AssertNotNull();

            Type type = instance.GetType();

            foreach (Field field in fields)
            {
                FieldInfo info = Cache.GetFieldFrom(type, field.Name);
                if (info == null)
                {
                    Warning($"Could not set {field.Type} field {field.Name} to {field.Value}, no such field found in {type}!");
                    continue;
                }
                if (!info.FieldType.IsAssignableFrom(field.Type))
                {
                    Warning($"Could not set {field.Type} field {field.Name} to {field.Value}, type mismatch: expected {field.Type}, was {info.FieldType}!");
                    continue;
                }

                info.SetValue(instance, field.Value);
            }
        }

        #endregion Methods
    }
}