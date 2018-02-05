using Autrage.LEX.NET.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.LEX.NET.Serialization
{
    public sealed class ValueTypeSerializer : ObjectSerializer
    {
        #region Methods

        public override bool CanHandle(Type type) => type.IsValueType;

        public override bool Serialize(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            if (!CanHandle(instance.GetType()))
            {
                Warning($"Cannot serialize type {instance.GetType()}!");
                return false;
            }

            return SerializeObject(stream, instance);
        }

        public override object Deserialize(Stream stream, Type expectedType)
        {
            stream.AssertNotNull();
            expectedType.AssertNotNull();

            if (!CanHandle(expectedType))
            {
                Warning($"Cannot deserialize type {expectedType}!");
                return false;
            }

            if (Nullable.GetUnderlyingType(expectedType) is Type underlyingType)
            {
                expectedType = underlyingType;
            }

            object instance = Instantiate(stream, expectedType);
            if (instance == null)
            {
                Warning($"Could not deserialize {expectedType} instance, instantiation failed!");
                return expectedType.GetDefault();
            }

            IEnumerable<Field> fields = DeserializeFields(stream);
            if (fields == null)
            {
                Warning($"Could not deserialize {instance.GetType()} instance fields!");
                return expectedType.GetDefault();
            }

            SetFields(instance, fields);

            return instance;
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

            return Activator.CreateInstance(expectedType);
        }

        #endregion Methods
    }
}