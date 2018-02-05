using Autrage.LEX.NET.Extensions;
using System;
using System.IO;

using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.LEX.NET.Serialization
{
    public sealed class PrimitiveSerializer : Serializer
    {
        #region Methods

        public override bool CanHandle(Type type) => type.IsPrimitive || (Nullable.GetUnderlyingType(type)?.IsPrimitive ?? false);

        public override bool Serialize(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            Type type = instance.GetType();
            if (!CanHandle(type))
            {
                Warning($"Cannot serialize type {type}!");
                return false;
            }

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
                Warning($"Primitive type {expectedType} not supported!");
                return expectedType.GetDefault();
            }
            else
            {
                return instance;
            }
        }

        #endregion Methods
    }
}