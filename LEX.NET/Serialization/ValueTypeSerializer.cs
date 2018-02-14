using Autrage.LEX.NET.Extensions;
using System;
using System.IO;
using static Autrage.LEX.NET.Bugger;

namespace Autrage.LEX.NET.Serialization
{
    public sealed class ValueTypeSerializer : ObjectSerializer
    {
        public override bool CanHandle(Type type) => type.IsValueType;

        public override bool Serialize(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            Type type = instance.GetType();
            if (!CanHandle(type))
            {
                Warning($"{nameof(ValueTypeSerializer)} cannot handle type {type}!");
                return false;
            }

            if (!SerializeMembers(stream, instance))
            {
                Warning($"Could not serialize {type} instance members!");
                return false;
            }

            return true;
        }

        public override object Deserialize(Stream stream, Type type)
        {
            stream.AssertNotNull();
            type.AssertNotNull();

            if (!CanHandle(type))
            {
                Warning($"{nameof(ValueTypeSerializer)} cannot handle type {type}!");
                return type.GetDefault();
            }

            if (Nullable.GetUnderlyingType(type) is Type underlyingType)
            {
                type = underlyingType;
            }

            object instance = Instantiate(type);

            DeserializeMembers(stream, instance);

            return instance;
        }
    }
}