using Autrage.LEX.NET.Extensions;
using System;
using System.IO;
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

            return SerializeFields(stream, instance);
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

            object instance = Activator.CreateInstance(expectedType);

            if (DeserializeFields(stream, instance))
            {
                Warning($"Could not deserialize {instance.GetType()} instance fields!");
                return expectedType.GetDefault();
            }

            return instance;
        }

        #endregion Methods
    }
}