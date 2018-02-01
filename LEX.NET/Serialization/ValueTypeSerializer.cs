using Autrage.LEX.NET.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.LEX.NET.Serialization
{
    public sealed class ValueTypeSerializer : ObjectSerializer
    {
        #region Methods

        public override bool CanSerialize(Type type) => type.IsValueType;

        public override bool Serialize(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            if (!CanSerialize(instance.GetType()))
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

            if (!CanSerialize(expectedType))
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

        #endregion Methods
    }
}