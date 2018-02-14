using Autrage.LEX.NET.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using static Autrage.LEX.NET.Bugger;

namespace Autrage.LEX.NET.Serialization
{
    public sealed class Marshaller : IEnumerable<Serializer>, IEnumerable
    {
        private List<Serializer> serializers = new List<Serializer>();

        public Encoding Encoding { get; set; } = Encoding.UTF8;
        public Func<AssemblyName, Assembly> AssemblyResolver { get; set; }
        public Func<Assembly, string, bool, Type> TypeResolver { get; set; }

        public void Add(Serializer serializer)
        {
            serializer.Marshaller = this;
            serializers.Add(serializer);
        }

        public static void Serialize(Stream stream, object instance, params Serializer[] serializers)
        {
            Marshaller marshaller = new Marshaller();
            foreach (Serializer serializer in serializers)
            {
                marshaller.Add(serializer);
            }

            marshaller.Serialize(stream, instance);
        }

        public static object Deserialize(Stream stream, params Serializer[] serializers)
        {
            Marshaller marshaller = new Marshaller();
            foreach (Serializer serializer in serializers)
            {
                marshaller.Add(serializer);
            }

            return marshaller.Deserialize(stream);
        }

        public static T Deserialize<T>(Stream stream, params Serializer[] serializers)
        {
            if (Deserialize(stream, serializers) is T instance)
            {
                return instance;
            }
            else
            {
                return default;
            }
        }

        public void Serialize(Stream stream, object instance)
        {
            stream.AssertNotNull();

            if (instance == null)
            {
                stream.Write(false);
                return;
            }

            string typeName = Cache.GetNameFrom(instance.GetType());
            if (typeName == null)
            {
                Warning($"Could not retrieve name for {instance.GetType()} from cache!");
                stream.Write(false);
                return;
            }

            byte[] payloadBuffer;
            using (MemoryStream payload = new MemoryStream())
            {
                Serializer serializer = serializers.FirstOrDefault(s => s.CanHandle(instance.GetType()));
                if (serializer == null)
                {
                    Warning($"No suitable serializer specified for {instance.GetType()}!");
                    stream.Write(false);
                    return;
                }

                if (!serializer.Serialize(payload, instance))
                {
                    Warning($"{instance.GetType()} instance serialization not successful!");
                    stream.Write(false);
                    return;
                }

                payloadBuffer = payload.ToArray();
            }

            stream.Write(true);
            stream.Write(typeName, Encoding);
            stream.Write(payloadBuffer.Length);
            stream.Write(payloadBuffer);
        }

        public void Serialize<T>(Stream stream, T instance)
        {
            Serialize(stream, (object)instance);
        }

        public object Deserialize(Stream stream)
        {
            stream.AssertNotNull();

            bool? hasValue = stream.ReadBool();
            if (hasValue == null)
            {
                Warning($"Could not read value indicator!");
                return null;
            }
            if (hasValue == false)
            {
                return null;
            }

            string typeName = stream.ReadString(Encoding);
            if (typeName == null)
            {
                Warning($"Could not read type name!");
                return null;
            }

            Type type = Cache.GetTypeFrom(typeName, AssemblyResolver, TypeResolver);
            if (type == null)
            {
                Warning($"Could not retrieve type for {typeName} from cache!");
                return null;
            }

            int? payloadBufferLength = stream.ReadInt();
            if (payloadBufferLength == null)
            {
                Warning($"Could not read {type} instance payload buffer length!");
                return null;
            }

            byte[] payloadBuffer = stream.Read(payloadBufferLength.Value);
            if (payloadBuffer == null)
            {
                Warning($"Could not read {type} instance payload!");
                return null;
            }

            object result = null;
            using (MemoryStream payload = new MemoryStream(payloadBuffer))
            {
                Serializer serializer = serializers.FirstOrDefault(s => s.CanHandle(type));
                if (serializer == null)
                {
                    Warning($"No suitable serializer specified for {type}!");
                    return null;
                }

                result = serializer.Deserialize(payload, type);
            }

            if (type.IsInstanceOfType(result))
            {
                return result;
            }
            else
            {
                return type.GetDefault();
            }
        }

        public T Deserialize<T>(Stream stream)
        {
            if (Deserialize(stream) is T instance)
            {
                return instance;
            }
            else
            {
                return default;
            }
        }

        public IEnumerator<Serializer> GetEnumerator() => serializers.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => serializers.GetEnumerator();
    }
}