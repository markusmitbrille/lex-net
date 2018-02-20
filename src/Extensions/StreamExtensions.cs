using System;
using System.IO;
using System.Text;

namespace Autrage.LEX.NET.Extensions
{
    public static class StreamExtensions
    {
        public static void Reset(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            stream.Position = 0;
        }

        public static byte[] Read(this Stream stream, int count)
        {
            stream.AssertNotNull(nameof(stream));

            if (count == 0)
            {
                return new byte[0];
            }

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
            byte[] buffer = stream.Read(length);
            return buffer == null ? null : (byte?)buffer[0];
        }

        public static sbyte? ReadSByte(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            const int length = 1;
            byte[] buffer = stream.Read(length);
            return buffer == null ? null : (sbyte?)buffer[0];
        }

        public static short? ReadShort(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            const int length = 2;
            byte[] buffer = stream.Read(length);
            return buffer == null ? null : (short?)BitConverter.ToInt16(buffer, 0);
        }

        public static int? ReadInt(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            const int length = 4;
            byte[] buffer = stream.Read(length);
            return buffer == null ? null : (int?)BitConverter.ToInt32(buffer, 0);
        }

        public static long? ReadLong(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            const int length = 8;
            byte[] buffer = stream.Read(length);
            return buffer == null ? null : (long?)BitConverter.ToInt64(buffer, 0);
        }

        public static ushort? ReadUShort(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            const int length = 2;
            byte[] buffer = stream.Read(length);
            return buffer == null ? null : (ushort?)BitConverter.ToUInt16(buffer, 0);
        }

        public static uint? ReadUInt(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            const int length = 4;
            byte[] buffer = stream.Read(length);
            return buffer == null ? null : (uint?)BitConverter.ToUInt32(buffer, 0);
        }

        public static ulong? ReadULong(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            const int length = 8;
            byte[] buffer = stream.Read(length);
            return buffer == null ? null : (ulong?)BitConverter.ToUInt64(buffer, 0);
        }

        public static float? ReadFloat(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            const int length = 4;
            byte[] buffer = stream.Read(length);
            return buffer == null ? null : (float?)BitConverter.ToSingle(buffer, 0);
        }

        public static double? ReadDouble(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            const int length = 8;
            byte[] buffer = stream.Read(length);
            return buffer == null ? null : (double?)BitConverter.ToDouble(buffer, 0);
        }

        public static decimal? ReadDecimal(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            int? count = stream.ReadInt();
            if (count == null)
            {
                return null;
            }

            int[] bits = new int[count.Value];
            for (int i = 0; i < count; i++)
            {
                int? bit = stream.ReadInt();
                if (bit == null)
                {
                    return null;
                }
                else
                {
                    bits[i] = bit.Value;
                }
            }

            return new decimal(bits);
        }

        public static bool? ReadBool(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            const int length = 1;
            byte[] buffer = stream.Read(length);
            return buffer == null ? null : (bool?)BitConverter.ToBoolean(buffer, 0);
        }

        public static char? ReadChar(this Stream stream)
        {
            stream.AssertNotNull(nameof(stream));

            const int length = 2;
            byte[] buffer = stream.Read(length);
            return buffer == null ? null : (char?)BitConverter.ToChar(buffer, 0);
        }

        public static string ReadString(this Stream stream) => ReadString(stream, Encoding.UTF8);

        public static string ReadString(this Stream stream, Encoding encoding)
        {
            stream.AssertNotNull(nameof(stream));

            int? length = stream.ReadInt();
            if (length == null)
            {
                return null;
            }

            byte[] buffer = stream.Read(length.Value);
            return buffer == null ? null : encoding.GetString(buffer);
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
    }
}