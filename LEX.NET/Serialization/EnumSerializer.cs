using Autrage.LEX.NET.Extensions;
using System;
using System.IO;

using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.LEX.NET.Serialization
{
    public sealed class EnumSerializer : Serializer
    {
        #region Methods

        public override bool CanHandle(Type type) => type.IsEnum || (Nullable.GetUnderlyingType(type)?.IsEnum ?? false);

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

            Type enumUnderlyingType = type.GetEnumUnderlyingType();
            if (enumUnderlyingType == null)
            {
                Warning($"Could not get underlying type of enum {type}!");
                return false;
            }

            if (enumUnderlyingType == typeof(byte))
            {
                stream.Write((byte)instance);
                return true;
            }
            if (enumUnderlyingType == typeof(sbyte))
            {
                stream.Write((sbyte)instance);
                return true;
            }
            if (enumUnderlyingType == typeof(short))
            {
                stream.Write((short)instance);
                return true;
            }
            if (enumUnderlyingType == typeof(int))
            {
                stream.Write((int)instance);
                return true;
            }
            if (enumUnderlyingType == typeof(long))
            {
                stream.Write((long)instance);
                return true;
            }
            if (enumUnderlyingType == typeof(ushort))
            {
                stream.Write((ushort)instance);
                return true;
            }
            if (enumUnderlyingType == typeof(uint))
            {
                stream.Write((uint)instance);
                return true;
            }
            if (enumUnderlyingType == typeof(ulong))
            {
                stream.Write((ulong)instance);
                return true;
            }

            Warning($"Underlying type {enumUnderlyingType} not supported!");
            return false;
        }

        public override object Deserialize(Stream stream, Type type)
        {
            stream.AssertNotNull();
            type.AssertNotNull();

            if (!CanHandle(type))
            {
                Warning($"Cannot deserialize type {type}!");
                return false;
            }

            if (Nullable.GetUnderlyingType(type) is Type nullableUnderlyingType)
            {
                type = nullableUnderlyingType;
            }

            Type enumUnderlyingType = type.GetEnumUnderlyingType();
            if (enumUnderlyingType == null)
            {
                Warning($"Could not get underlying type of enum {type}!");
                return type.GetDefault();
            }

            object instance = null;
            if (enumUnderlyingType == typeof(byte))
            {
                instance = stream.Read();
            }
            if (enumUnderlyingType == typeof(sbyte))
            {
                instance = stream.ReadSByte();
            }
            if (enumUnderlyingType == typeof(short))
            {
                instance = stream.ReadShort();
            }
            if (enumUnderlyingType == typeof(int))
            {
                instance = stream.ReadInt();
            }
            if (enumUnderlyingType == typeof(long))
            {
                instance = stream.ReadLong();
            }
            if (enumUnderlyingType == typeof(ushort))
            {
                instance = stream.ReadUShort();
            }
            if (enumUnderlyingType == typeof(uint))
            {
                instance = stream.ReadUInt();
            }
            if (enumUnderlyingType == typeof(ulong))
            {
                instance = stream.ReadULong();
            }

            if (instance == null)
            {
                Warning($"Underlying type {enumUnderlyingType} of {type} not supported!");
                return type.GetDefault();
            }
            else
            {
                return Enum.ToObject(type, instance);
            }
        }

        #endregion Methods
    }
}