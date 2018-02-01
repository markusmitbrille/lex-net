using Autrage.LEX.NET.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.LEX.NET.Serialization
{
    public class Serializer
    {
        #region Fields

        private Dictionary<object, long> referenceIDs = new Dictionary<object, long>(new ReferenceComparer());
        private Dictionary<long, object> references = new Dictionary<long, object>();

        #endregion Fields

        #region Properties

        private Encoding Encoding { get; } = Encoding.UTF8;

        #endregion Properties

        #region Constructors

        private Serializer()
        {
        }

        private Serializer(Encoding encoding)
        {
            Encoding = encoding;
        }

        #endregion Constructors

        #region Methods

        public static void Serialize<T>(Stream stream, T instance)
        {
            stream.AssertNotNull();

            Serialize(stream, instance, Encoding.UTF8);
        }

        public static void Serialize<T>(Stream stream, T instance, Encoding encoding)
        {
            stream.AssertNotNull();
            encoding.AssertNotNull();

            new Serializer(encoding).Serialize(stream, (object)instance);
        }

        public static T Deserialize<T>(Stream stream)
        {
            stream.AssertNotNull();

            return Deserialize<T>(stream, Encoding.UTF8);
        }

        public static T Deserialize<T>(Stream stream, Encoding encoding)
        {
            stream.AssertNotNull();
            encoding.AssertNotNull();

            object instance = new Serializer(encoding).Deserialize(stream, typeof(T));
            if (instance == null)
            {
                return default;
            }
            else
            {
                return (T)instance;
            }
        }

        private void Serialize(Stream stream, object instance)
        {
            stream.AssertNotNull();

            if (instance == null)
            {
                stream.Write(false);
                return;
            }

            using (MemoryStream payload = new MemoryStream())
            {
                bool wasSuccess;
                Type type = instance.GetType();
                if (type == typeof(string))
                {
                    payload.Write((string)instance, Encoding);
                    wasSuccess = true;
                }
                else if (type.IsPrimitive)
                {
                    wasSuccess = SerializePrimitive(payload, instance);
                }
                else if (type.IsEnum)
                {
                    wasSuccess = SerializeEnum(payload, instance);
                }
                else if (type.IsValueType)
                {
                    wasSuccess = SerializeInstance(payload, instance);
                }
                else if (typeof(IEnumerable<object>).IsAssignableFrom(type))
                {
                    wasSuccess = SerializeCollection(payload, instance);
                }
                else
                {
                    wasSuccess = SerializeReference(payload, instance);
                }

                if (wasSuccess)
                {
                    stream.Write(true);

                    byte[] payloadBuffer = payload.ToArray();
                    stream.Write(payloadBuffer.Length);
                    stream.Write(payloadBuffer);
                }
                else
                {
                    Warning($"{type} instance serialization not successful!");
                    stream.Write(false);
                }
            }
        }

        private bool SerializePrimitive(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            Type type = instance.GetType();
            if (type == typeof(byte))
            {
                stream.Write((byte)instance);
                return true;
            }
            if (type == typeof(sbyte))
            {
                stream.Write((sbyte)instance);
                return true;
            }
            if (type == typeof(short))
            {
                stream.Write((short)instance);
                return true;
            }
            if (type == typeof(int))
            {
                stream.Write((int)instance);
                return true;
            }
            if (type == typeof(long))
            {
                stream.Write((long)instance);
                return true;
            }
            if (type == typeof(ushort))
            {
                stream.Write((ushort)instance);
                return true;
            }
            if (type == typeof(uint))
            {
                stream.Write((uint)instance);
                return true;
            }
            if (type == typeof(ulong))
            {
                stream.Write((ulong)instance);
                return true;
            }
            if (type == typeof(float))
            {
                stream.Write((float)instance);
                return true;
            }
            if (type == typeof(double))
            {
                stream.Write((double)instance);
                return true;
            }
            if (type == typeof(decimal))
            {
                stream.Write((decimal)instance);
                return true;
            }
            if (type == typeof(bool))
            {
                stream.Write((bool)instance);
                return true;
            }
            if (type == typeof(char))
            {
                stream.Write((char)instance);
                return true;
            }

            Warning($"Primitive type {type} not supported!");
            return false;
        }

        private bool SerializeEnum(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            Type underlyingType = instance.GetType().GetEnumUnderlyingType();
            if (underlyingType == typeof(byte))
            {
                stream.Write((byte)instance);
                return true;
            }
            if (underlyingType == typeof(sbyte))
            {
                stream.Write((sbyte)instance);
                return true;
            }
            if (underlyingType == typeof(short))
            {
                stream.Write((short)instance);
                return true;
            }
            if (underlyingType == typeof(int))
            {
                stream.Write((int)instance);
                return true;
            }
            if (underlyingType == typeof(long))
            {
                stream.Write((long)instance);
                return true;
            }
            if (underlyingType == typeof(ushort))
            {
                stream.Write((ushort)instance);
                return true;
            }
            if (underlyingType == typeof(uint))
            {
                stream.Write((uint)instance);
                return true;
            }
            if (underlyingType == typeof(ulong))
            {
                stream.Write((ulong)instance);
                return true;
            }

            Warning($"Underlying type {underlyingType} not supported!");
            return false;
        }

        private bool SerializeCollection(Stream stream, object instance)
        {
            throw new NotImplementedException();
        }

        private bool SerializeReference(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            if (referenceIDs.ContainsKey(instance))
            {
                stream.Write(referenceIDs[instance]);
                return true;
            }

            long referenceID = referenceIDs.Max(e => e.Value) + 1;
            referenceIDs[instance] = referenceID;
            stream.Write(referenceID);

            if (SerializeInstance(stream, instance))
            {
                return true;
            }
            else
            {
                referenceIDs.Remove(instance);
                return false;
            }
        }

        private bool SerializeInstance(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            string typeName = Cache.GetNameFrom(instance.GetType());
            if (typeName == null)
            {
                Warning($"Could not serialize {instance.GetType()} instance, could not get type name!");
                return false;
            }

            stream.Write(typeName, Encoding);

            return SerializeFields(stream, instance);
        }

        private bool SerializeFields(Stream stream, object instance)
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

        private object Deserialize(Stream stream, Type expectedType)
        {
            stream.AssertNotNull();
            expectedType.AssertNotNull();

            bool? hasValue = stream.ReadBool();
            if (hasValue == null)
            {
                Warning($"Could not deserialize {expectedType.Name} value indicator!");
                return expectedType.GetDefault();
            }
            if (hasValue == false)
            {
                return expectedType.GetDefault();
            }

            int? payloadBufferLength = stream.ReadInt();
            if (payloadBufferLength == null)
            {
                Warning($"Could not deserialize {expectedType.Name} instance payload buffer length!");
                return expectedType.GetDefault();
            }

            byte[] payloadBuffer = stream.Read(payloadBufferLength.Value);
            if (payloadBuffer == null)
            {
                Warning($"Could not deserialize {expectedType.Name} instance payload!");
                return expectedType.GetDefault();
            }

            using (MemoryStream payload = new MemoryStream(payloadBuffer))
            {
                Type underlyingType = Nullable.GetUnderlyingType(expectedType);
                if (underlyingType?.IsPrimitive ?? false)
                {
                    return DeserializePrimitive(payload, underlyingType);
                }
                if (underlyingType?.IsEnum ?? false)
                {
                    return DeserializeEnum(payload, underlyingType);
                }
                if (underlyingType?.IsValueType ?? false)
                {
                    return DeserializeInstance(payload, underlyingType);
                }
                if (expectedType == typeof(string))
                {
                    return payload.ReadString(Encoding);
                }
                if (expectedType.IsPrimitive)
                {
                    return DeserializePrimitive(payload, expectedType);
                }
                if (expectedType.IsEnum)
                {
                    return DeserializeEnum(payload, expectedType);
                }
                if (expectedType.IsValueType)
                {
                    return DeserializeInstance(payload, expectedType);
                }
                if (typeof(IEnumerable<object>).IsAssignableFrom(expectedType))
                {
                    return DeserializeCollection(payload, expectedType);
                }

                return DeserializeReference(payload, expectedType);
            }
        }

        private object DeserializePrimitive(Stream stream, Type expectedType)
        {
            stream.AssertNotNull();
            expectedType.AssertNotNull();
            expectedType.Assert(t => t.IsPrimitive);

            object instance = null;
            if (expectedType == typeof(byte))
            {
                instance = stream.Read();
            }
            if (expectedType == typeof(sbyte))
            {
                instance = stream.ReadSByte();
            }
            if (expectedType == typeof(short))
            {
                instance = stream.ReadShort();
            }
            if (expectedType == typeof(int))
            {
                instance = stream.ReadInt();
            }
            if (expectedType == typeof(long))
            {
                instance = stream.ReadLong();
            }
            if (expectedType == typeof(ushort))
            {
                instance = stream.ReadUShort();
            }
            if (expectedType == typeof(uint))
            {
                instance = stream.ReadUInt();
            }
            if (expectedType == typeof(ulong))
            {
                instance = stream.ReadULong();
            }
            if (expectedType == typeof(float))
            {
                instance = stream.ReadFloat();
            }
            if (expectedType == typeof(double))
            {
                instance = stream.ReadDouble();
            }
            if (expectedType == typeof(decimal))
            {
                instance = stream.ReadDecimal();
            }
            if (expectedType == typeof(bool))
            {
                instance = stream.ReadBool();
            }
            if (expectedType == typeof(char))
            {
                instance = stream.ReadChar();
            }

            if (instance == null)
            {
                Warning($"Primitive type {expectedType.Name} not supported!");
                return expectedType.GetDefault();
            }
            else
            {
                return instance;
            }
        }

        private object DeserializeEnum(Stream stream, Type expectedType)
        {
            stream.AssertNotNull();
            expectedType.AssertNotNull();
            expectedType.Assert(t => t.IsEnum);

            Type underlyingType = expectedType.GetEnumUnderlyingType();
            if (underlyingType == null)
            {
                Warning($"Could not get underlying type of enum {expectedType.Name}!");
                return expectedType.GetDefault();
            }
            if (!underlyingType.IsValueType)
            {
                Warning($"Underlying type {underlyingType.Name} of {expectedType.Name} not supported!");
                return expectedType.GetDefault();
            }

            object instance = null;
            if (underlyingType == typeof(byte))
            {
                instance = stream.Read();
            }
            if (underlyingType == typeof(sbyte))
            {
                instance = stream.ReadSByte();
            }
            if (underlyingType == typeof(short))
            {
                instance = stream.ReadShort();
            }
            if (underlyingType == typeof(int))
            {
                instance = stream.ReadInt();
            }
            if (underlyingType == typeof(long))
            {
                instance = stream.ReadLong();
            }
            if (underlyingType == typeof(ushort))
            {
                instance = stream.ReadUShort();
            }
            if (underlyingType == typeof(uint))
            {
                instance = stream.ReadUInt();
            }
            if (underlyingType == typeof(ulong))
            {
                instance = stream.ReadULong();
            }

            if (instance == null)
            {
                Warning($"Underlying type {underlyingType.Name} of {expectedType.Name} not supported!");
                return expectedType.GetDefault();
            }
            else
            {
                return instance;
            }
        }

        private object DeserializeCollection(Stream stream, Type type)
        {
            throw new NotImplementedException();
        }

        private object DeserializeComponent(Stream stream, Type type)
        {
            throw new NotImplementedException();
        }

        private object DeserializeReference(Stream stream, Type expectedType)
        {
            stream.AssertNotNull();
            expectedType.AssertNotNull();
            expectedType.Assert(t => !t.IsValueType);

            long? referenceID = stream.ReadLong();
            if (referenceID == null)
            {
                Warning($"Could not deserialize {expectedType.Name} reference ID!");
                return expectedType.GetDefault();
            }

            object instance = references.GetValueOrDefault(referenceID.Value);
            if (instance == null)
            {
                instance = Instantiate(stream, expectedType);
                if (instance == null)
                {
                    Warning($"Could not deserialize {expectedType.Name} reference, instantiation failed!");
                    return expectedType.GetDefault();
                }

                references[referenceID.Value] = instance;

                IEnumerable<Field> fields = DeserializeFields(stream);
                if (fields == null)
                {
                    Warning($"Could not deserialize {instance.GetType().Name} instance fields!");
                    return expectedType.GetDefault();
                }

                SetFields(instance, fields);
            }

            return instance;
        }

        private object DeserializeInstance(Stream stream, Type expectedType)
        {
            stream.AssertNotNull();
            expectedType.AssertNotNull();

            object instance = Instantiate(stream, expectedType);
            if (instance == null)
            {
                Warning($"Could not deserialize {expectedType.Name} instance, instantiation failed!");
                return expectedType.GetDefault();
            }

            IEnumerable<Field> fields = DeserializeFields(stream);
            if (fields == null)
            {
                Warning($"Could not deserialize {instance.GetType().Name} instance fields!");
                return expectedType.GetDefault();
            }

            SetFields(instance, fields);

            return instance;
        }

        private Type DeserializeType(Stream stream)
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

        private object Instantiate(Stream stream, Type expectedType)
        {
            stream.AssertNotNull();
            expectedType.AssertNotNull();

            Type type = DeserializeType(stream);
            if (type == null)
            {
                Warning($"Could not create {expectedType.Name} instance, type deserialization failed!");
                return expectedType.GetDefault();
            }
            if (!expectedType.IsAssignableFrom(type))
            {
                Warning($"Could not create {expectedType.Name} instance, type mismatch: expected {expectedType.Name}, deserialized {type.Name}!");
                return expectedType.GetDefault();
            }

            object instance = null;
            if (Cache.SkipConstructorOf(type))
            {
                instance = FormatterServices.GetSafeUninitializedObject(type);
                if (instance == null)
                {
                    Warning($"Could not create {expectedType.Name} instance, constructor invokation failed!");
                    return expectedType.GetDefault();
                }
            }
            else
            {
                ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                if (constructor == null)
                {
                    Warning($"Could not create {expectedType.Name} instance, no default constructor found!");
                    return expectedType.GetDefault();
                }

                instance = constructor.Invoke(null);
                if (instance == null)
                {
                    Warning($"Could not create {expectedType.Name} instance, constructor invokation failed!");
                    return expectedType.GetDefault();
                }
            }

            return instance;
        }

        private IEnumerable<Field> DeserializeFields(Stream stream)
        {
            stream.AssertNotNull();

            int? fieldCount = stream.ReadInt();
            if (fieldCount == null)
            {
                Warning($"Could not deserialize field count!");
                return null;
            }

            List<Field> fields = new List<Field>();
            for (int i = 0; i < fieldCount; i++)
            {
                Field field = DeserializeField(stream);
                if (field == null)
                {
                    Warning($"Could not deserialize field [{i}]!");
                    return null;
                }

                fields.Add(field);
            }

            return fields;
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

        #region Classes

        private static class Cache
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

        private class Field
        {
            #region Properties

            public string Name { get; }
            public Type Type { get; }
            public object Value { get; }

            #endregion Properties

            #region Constructors

            public Field(string name, Type type, object value)
            {
                name.AssertNotNull();
                type.AssertNotNull();

                Name = name;
                Type = type;
                Value = value;
            }

            #endregion Constructors
        }

        #endregion Classes
    }
}