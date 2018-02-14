using Autrage.LEX.NET.Extensions;
using System;
using System.IO;

using static Autrage.LEX.NET.Bugger;

namespace Autrage.LEX.NET.Serialization
{
    public sealed class PrimitiveSerializer : Serializer
    {
        public override bool CanHandle(Type type) => type.IsPrimitive || (Nullable.GetUnderlyingType(type)?.IsPrimitive ?? false);

        public override bool Serialize(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            Type type = instance.GetType();
            if (!CanHandle(type))
            {
                Warning($"{nameof(PrimitiveSerializer)} cannot handle type {type}!");
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

        public override object Deserialize(Stream stream, Type type)
        {
            stream.AssertNotNull();
            type.AssertNotNull();

            if (!CanHandle(type))
            {
                Warning($"{nameof(PrimitiveSerializer)} cannot handle type {type}!");
                return type.GetDefault();
            }

            if (Nullable.GetUnderlyingType(type) is Type underlyingType)
            {
                type = underlyingType;
            }

            object instance = null;
            if (type == typeof(byte))
            {
                instance = stream.Read();
            }
            if (type == typeof(sbyte))
            {
                instance = stream.ReadSByte();
            }
            if (type == typeof(short))
            {
                instance = stream.ReadShort();
            }
            if (type == typeof(int))
            {
                instance = stream.ReadInt();
            }
            if (type == typeof(long))
            {
                instance = stream.ReadLong();
            }
            if (type == typeof(ushort))
            {
                instance = stream.ReadUShort();
            }
            if (type == typeof(uint))
            {
                instance = stream.ReadUInt();
            }
            if (type == typeof(ulong))
            {
                instance = stream.ReadULong();
            }
            if (type == typeof(float))
            {
                instance = stream.ReadFloat();
            }
            if (type == typeof(double))
            {
                instance = stream.ReadDouble();
            }
            if (type == typeof(decimal))
            {
                instance = stream.ReadDecimal();
            }
            if (type == typeof(bool))
            {
                instance = stream.ReadBool();
            }
            if (type == typeof(char))
            {
                instance = stream.ReadChar();
            }

            if (instance == null)
            {
                Warning($"Primitive type {type} not supported!");
                return type.GetDefault();
            }
            else
            {
                return instance;
            }
        }
    }
}