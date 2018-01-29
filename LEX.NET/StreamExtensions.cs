using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autrage.LEX.NET
{
    public static class StreamExtensions
    {
        public static void Reset(this Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            stream.Position = 0;
        }

        public static Stream Write(this Stream stream, byte instance)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            stream.WriteByte(instance);
            return stream;
        }

        public static Stream Write(this Stream stream, short instance)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            byte[] buffer = BitConverter.GetBytes(instance);
            stream.Write(buffer, 0, buffer.Length);

            return stream;
        }

        public static Stream Write(this Stream stream, int instance)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            byte[] buffer = BitConverter.GetBytes(instance);
            stream.Write(buffer, 0, buffer.Length);

            return stream;
        }

        public static Stream Write(this Stream stream, long instance)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            byte[] buffer = BitConverter.GetBytes(instance);
            stream.Write(buffer, 0, buffer.Length);

            return stream;
        }

        public static Stream Write(this Stream stream, sbyte instance)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            stream.WriteByte((byte)instance);
            return stream;
        }

        public static Stream Write(this Stream stream, ushort instance)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            byte[] buffer = BitConverter.GetBytes(instance);
            stream.Write(buffer, 0, buffer.Length);

            return stream;
        }

        public static Stream Write(this Stream stream, uint instance)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            byte[] buffer = BitConverter.GetBytes(instance);
            stream.Write(buffer, 0, buffer.Length);

            return stream;
        }

        public static Stream Write(this Stream stream, ulong instance)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            byte[] buffer = BitConverter.GetBytes(instance);
            stream.Write(buffer, 0, buffer.Length);

            return stream;
        }

        public static Stream Write(this Stream stream, float instance)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            byte[] buffer = BitConverter.GetBytes(instance);
            stream.Write(buffer, 0, buffer.Length);

            return stream;
        }

        public static Stream Write(this Stream stream, double instance)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            byte[] buffer = BitConverter.GetBytes(instance);
            stream.Write(buffer, 0, buffer.Length);

            return stream;
        }

        public static Stream Write(this Stream stream, decimal instance)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

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
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            byte[] buffer = BitConverter.GetBytes(instance);
            stream.Write(buffer, 0, buffer.Length);

            return stream;
        }

        public static Stream Write(this Stream stream, char instance)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            byte[] buffer = BitConverter.GetBytes(instance);
            stream.Write(buffer, 0, buffer.Length);

            return stream;
        }

        public static byte? Read(this Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            const int length = 1;
            byte[] buffer = new byte[length];
            if (stream.Read(buffer, 0, length) < length)
            {
                return null;
            }

            return buffer[0];
        }

        public static short? ReadShort(this Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            const int length = 2;
            byte[] buffer = new byte[length];
            if (stream.Read(buffer, 0, length) < length)
            {
                return null;
            }

            return BitConverter.ToInt16(buffer, 0);
        }

        public static int? ReadInt(this Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            const int length = 4;
            byte[] buffer = new byte[length];
            if (stream.Read(buffer, 0, length) < length)
            {
                return null;
            }

            return BitConverter.ToInt32(buffer, 0);
        }

        public static long? ReadLong(this Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            const int length = 8;
            byte[] buffer = new byte[length];
            if (stream.Read(buffer, 0, length) < length)
            {
                return null;
            }

            return BitConverter.ToInt64(buffer, 0);
        }

        public static sbyte? ReadSByte(this Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            const int length = 1;
            byte[] buffer = new byte[length];
            if (stream.Read(buffer, 0, length) < length)
            {
                return null;
            }

            return (sbyte)buffer[0];
        }

        public static ushort? ReadUShort(this Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            const int length = 2;
            byte[] buffer = new byte[length];
            if (stream.Read(buffer, 0, length) < length)
            {
                return null;
            }

            return BitConverter.ToUInt16(buffer, 0);
        }

        public static uint? ReadUInt(this Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            const int length = 4;
            byte[] buffer = new byte[length];
            if (stream.Read(buffer, 0, length) < length)
            {
                return null;
            }

            return BitConverter.ToUInt32(buffer, 0);
        }

        public static ulong? ReadULong(this Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            const int length = 8;
            byte[] buffer = new byte[length];
            if (stream.Read(buffer, 0, length) < length)
            {
                return null;
            }

            return BitConverter.ToUInt64(buffer, 0);
        }

        public static float? ReadFloat(this Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            const int length = 4;
            byte[] buffer = new byte[length];
            if (stream.Read(buffer, 0, length) < length)
            {
                return null;
            }

            return BitConverter.ToSingle(buffer, 0);
        }

        public static double? ReadDouble(this Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            const int length = 8;
            byte[] buffer = new byte[length];
            if (stream.Read(buffer, 0, length) < length)
            {
                return null;
            }

            return BitConverter.ToDouble(buffer, 0);
        }

        public static decimal? ReadDecimal(this Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            int? count = stream.ReadInt();
            if (count == null)
            {
                return null;
            }

            int[] bits = new int[count.Value];
            for (int i = 0; i < count.Value; i++)
            {
                int? bit = stream.ReadInt();
                if (bit == null)
                {
                    return null;
                }

                bits[i] = bit.Value;
            }

            return new decimal(bits);
        }

        public static bool? ReadBool(this Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            const int length = 1;
            byte[] buffer = new byte[length];
            if (stream.Read(buffer, 0, length) < length)
            {
                return null;
            }

            return BitConverter.ToBoolean(buffer, 0);
        }

        public static char? ReadChar(this Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            const int length = 2;
            byte[] buffer = new byte[length];
            if (stream.Read(buffer, 0, length) < length)
            {
                return null;
            }

            return BitConverter.ToChar(buffer, 0);
        }

        public static Stream Write(this Stream stream, string instance) => Write(stream, instance, Encoding.UTF8);
        public static Stream Write(this Stream stream, string instance, Encoding encoding)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            byte[] buffer = encoding.GetBytes(instance);
            stream.Write(buffer.Length);
            stream.Write(buffer, 0, buffer.Length);

            return stream;
        }

        public static string ReadString(this Stream stream) => ReadString(stream, Encoding.UTF8);

        public static string ReadString(this Stream stream, Encoding encoding)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            int? length = stream.ReadInt();
            if (length == null)
            {
                return null;
            }

            byte[] buffer = new byte[length.Value];
            if (stream.Read(buffer, 0, length.Value) < length.Value)
            {
                return null;
            }

            return encoding.GetString(buffer);
        }
    }
}
