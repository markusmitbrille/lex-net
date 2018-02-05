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

            Type type = instance.GetType();
            if (!CanHandle(type))
            {
                Warning($"Cannot handle type {type}!");
                return false;
            }

            if (!SerializeFields(stream, instance))
            {
                Warning($"Could not serialize {type} instance fields!");
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
                Warning($"Cannot handle type {type}!");
                return type.GetDefault();
            }

            if (Nullable.GetUnderlyingType(type) is Type underlyingType)
            {
                type = underlyingType;
            }

            object instance = Instantiate(type);

            if (!DeserializeFields(stream, instance))
            {
                Warning($"Could not deserialize {type} instance fields!");
                return type.GetDefault();
            }

            return instance;
        }

        #endregion Methods
    }
}