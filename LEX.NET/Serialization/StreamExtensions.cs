﻿using System;
using System.IO;
using System.Text;

namespace Autrage.LEX.NET.Serialization
{
    public static class StreamExtensions
    {
        #region Methods

        public static void Reset(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            stream.Position = 0;
        }

        public static byte[] Read(this Stream stream, int count)
        {
            stream.AssertNotNull(nameof(stream));
            count.Assert(i => i > 0);

            byte[] buffer = new byte[count];
            if (stream.Read(buffer, 0, count) < count)
            {
                return null;
            }
            else
            {
                return buffer;
            }
        }

        public static byte? Read(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            const int length = 1;
            if (stream.Read(length) is byte[] buffer)
            {
                return buffer[0];
            }
            else
            {
                return null;
            }
        }

        public static sbyte? ReadSByte(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            const int length = 1;
            if (stream.Read(length) is byte[] buffer)
            {
                return (sbyte)buffer[0];
            }
            else
            {
                return null;
            }
        }

        public static short? ReadShort(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            const int length = 2;
            if (stream.Read(length) is byte[] buffer)
            {
                return BitConverter.ToInt16(buffer, 0);
            }
            else
            {
                return null;
            }
        }

        public static int? ReadInt(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            const int length = 4;
            if (stream.Read(length) is byte[] buffer)
            {
                return BitConverter.ToInt32(buffer, 0);
            }
            else
            {
                return null;
            }
        }

        public static long? ReadLong(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            const int length = 8;
            if (stream.Read(length) is byte[] buffer)
            {
                return BitConverter.ToInt64(buffer, 0);
            }
            else
            {
                return null;
            }
        }

        public static ushort? ReadUShort(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            const int length = 2;
            if (stream.Read(length) is byte[] buffer)
            {
                return BitConverter.ToUInt16(buffer, 0);
            }
            else
            {
                return null;
            }
        }

        public static uint? ReadUInt(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            const int length = 4;
            if (stream.Read(length) is byte[] buffer)
            {
                return BitConverter.ToUInt32(buffer, 0);
            }
            else
            {
                return null;
            }
        }

        public static ulong? ReadULong(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            const int length = 8;
            if (stream.Read(length) is byte[] buffer)
            {
                return BitConverter.ToUInt64(buffer, 0);
            }
            else
            {
                return null;
            }
        }

        public static float? ReadFloat(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            const int length = 4;
            if (stream.Read(length) is byte[] buffer)
            {
                return BitConverter.ToSingle(buffer, 0);
            }
            else
            {
                return null;
            }
        }

        public static double? ReadDouble(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            const int length = 8;
            if (stream.Read(length) is byte[] buffer)
            {
                return BitConverter.ToDouble(buffer, 0);
            }
            else
            {
                return null;
            }
        }

        public static decimal? ReadDecimal(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            if (!(stream.ReadInt() is int count))
            {
                return null;
            }

            int[] bits = new int[count];
            for (int i = 0; i < count; i++)
            {
                if (stream.ReadInt() is int bit)
                {
                    bits[i] = bit;
                }
                else
                {
                    return null;
                }
            }

            return new decimal(bits);
        }

        public static bool? ReadBool(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            const int length = 1;
            if (stream.Read(length) is byte[] buffer)
            {
                return BitConverter.ToBoolean(buffer, 0);
            }
            else
            {
                return null;
            }
        }

        public static char? ReadChar(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            const int length = 2;
            if (stream.Read(length) is byte[] buffer)
            {
                return BitConverter.ToChar(buffer, 0);
            }
            else
            {
                return null;
            }
        }

        public static string ReadString(this Stream stream) => ReadString(stream, Encoding.UTF8);

        public static string ReadString(this Stream stream, Encoding encoding)
        {
            stream.AssertNotNull(nameof(stream));

            if (!(stream.ReadInt() is int length))
            {
                return null;
            }

            if (stream.Read(length) is byte[] buffer)
            {
                return encoding.GetString(buffer);
            }
            else
            {
                return null;
            }
        }

        public static Stream Write(this Stream stream, byte[] buffer)
        {
            stream.AssertNotNull(nameof(stream));
            buffer.AssertNotNull(nameof(buffer));

            stream.Write(buffer, 0, buffer.Length);
            return stream;
        }

        public static Stream Write(this Stream stream, byte instance)
        {
            stream.AssertNotNull(nameof(stream));

            stream.WriteByte(instance);
            return stream;
        }

        public static Stream Write(this Stream stream, short instance)
        {
            stream.AssertNotNull(nameof(stream));

            stream.Write(BitConverter.GetBytes(instance));
            return stream;
        }

        public static Stream Write(this Stream stream, int instance)
        {
            stream.AssertNotNull(nameof(stream));

            stream.Write(BitConverter.GetBytes(instance));
            return stream;
        }

        public static Stream Write(this Stream stream, long instance)
        {
            stream.AssertNotNull(nameof(stream));

            stream.Write(BitConverter.GetBytes(instance));
            return stream;
        }

        public static Stream Write(this Stream stream, sbyte instance)
        {
            stream.AssertNotNull(nameof(stream));

            stream.WriteByte((byte)instance);
            return stream;
        }

        public static Stream Write(this Stream stream, ushort instance)
        {
            stream.AssertNotNull(nameof(stream));

            stream.Write(BitConverter.GetBytes(instance));
            return stream;
        }

        public static Stream Write(this Stream stream, uint instance)
        {
            stream.AssertNotNull(nameof(stream));

            stream.Write(BitConverter.GetBytes(instance));
            return stream;
        }

        public static Stream Write(this Stream stream, ulong instance)
        {
            stream.AssertNotNull(nameof(stream));

            stream.Write(BitConverter.GetBytes(instance));
            return stream;
        }

        public static Stream Write(this Stream stream, float instance)
        {
            stream.AssertNotNull(nameof(stream));

            stream.Write(BitConverter.GetBytes(instance));
            return stream;
        }

        public static Stream Write(this Stream stream, double instance)
        {
            stream.AssertNotNull(nameof(stream));

            stream.Write(BitConverter.GetBytes(instance));
            return stream;
        }

        public static Stream Write(this Stream stream, decimal instance)
        {
            stream.AssertNotNull(nameof(stream));

            int[] bits = decimal.GetBits(instance);
            stream.Write(bits.Length);
            foreach (int bit in bits)
            {
                stream.Write(bit);
            }

            return stream;
        }

        public static Stream Write(this Stream stream, bool instance)
        {
            stream.AssertNotNull(nameof(stream));

            stream.Write(BitConverter.GetBytes(instance));
            return stream;
        }

        public static Stream Write(this Stream stream, char instance)
        {
            stream.AssertNotNull(nameof(stream));

            stream.Write(BitConverter.GetBytes(instance));
            return stream;
        }

        public static Stream Write(this Stream stream, string instance) => Write(stream, instance, Encoding.UTF8);

        public static Stream Write(this Stream stream, string instance, Encoding encoding)
        {
            stream.AssertNotNull(nameof(stream));

            byte[] buffer = encoding.GetBytes(instance);
            stream.Write(buffer.Length);
            stream.Write(buffer);

            return stream;
        }

        #endregion Methods
    }
}