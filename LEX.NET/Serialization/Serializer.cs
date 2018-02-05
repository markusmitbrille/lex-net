using Autrage.LEX.NET.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.LEX.NET.Serialization
{
    public abstract class Serializer
    {
        #region Properties

        public IMarshaller Marshaller { get; private set; }

        #endregion Properties

        #region Methods

        public static IMarshaller CreateMarshaller() => new SerializationMarshaller();

        public abstract bool CanSerialize(Type type);

        public abstract bool Serialize(Stream stream, object instance);

        public abstract object Deserialize(Stream stream, Type expectedType);

        #endregion Methods

        #region Classes

        private class SerializationMarshaller : IMarshaller
        {
            #region Fields

            private List<Serializer> serializers = new List<Serializer>();

            #endregion Fields

            #region Methods

            public Serializer AddSerializer(Type serializerType)
            {
                if (!serializerType.IsSubclassOf(typeof(Serializer))) return null;
                if (serializers.Any(s => s.GetType() == serializerType)) return null;

                Serializer serializer = (Serializer)Activator.CreateInstance(serializerType, true);
                serializer.Marshaller = this;
                serializers.Add(serializer);

                return serializer;
            }

            public T AddSerializer<T>() where T : Serializer, new()
            {
                if (serializers.Any(s => s is T)) return default;

                T serializer = new T() { Marshaller = this };
                serializers.Add(serializer);

                return serializer;
            }

            public void Serialize(Stream stream, object instance)
            {
                stream.AssertNotNull();

                if (instance == null)
                {
                    stream.Write(false);
                    return;
                }

                using (MemoryStream payload = new MemoryStream())
                {
                    Serializer serializer = serializers.FirstOrDefault(s => s.CanSerialize(instance.GetType()));
                    if (serializer == null)
                    {
                        Warning($"Not suitable serializer specified for {instance.GetType()}!");
                        stream.Write(false);
                        return;
                    }

                    if (serializer.Serialize(payload, instance))
                    {
                        stream.Write(true);

                        byte[] payloadBuffer = payload.ToArray();
                        stream.Write(payloadBuffer.Length);
                        stream.Write(payloadBuffer);
                    }
                    else
                    {
                        Warning($"{instance.GetType()} instance serialization not successful!");
                        stream.Write(false);
                    }
                }
            }

            public void Serialize<T>(Stream stream, T instance)
            {
                Serialize(stream, (object)instance);
            }

            public object Deserialize(Stream stream, Type expectedType)
            {
                stream.AssertNotNull();
                expectedType.AssertNotNull();

                bool? hasValue = stream.ReadBool();
                if (hasValue == null)
                {
                    Warning($"Could not deserialize {expectedType} value indicator!");
                    return expectedType.GetDefault();
                }
                if (hasValue == false)
                {
                    return expectedType.GetDefault();
                }

                int? payloadBufferLength = stream.ReadInt();
                if (payloadBufferLength == null)
                {
                    Warning($"Could not deserialize {expectedType} instance payload buffer length!");
                    return expectedType.GetDefault();
                }

                byte[] payloadBuffer = stream.Read(payloadBufferLength.Value);
                if (payloadBuffer == null)
                {
                    Warning($"Could not deserialize {expectedType} instance payload!");
                    return expectedType.GetDefault();
                }

                using (MemoryStream payload = new MemoryStream(payloadBuffer))
                {
                    foreach (Serializer serializer in serializers)
                    {
                        if (serializer.CanSerialize(expectedType))
                        {
                            return serializer.Deserialize(payload, expectedType);
                        }
                    }

                    Warning($"No suitable serializer specified for {expectedType}!");
                    return expectedType.GetDefault();
                }
            }

            public T Deserialize<T>(Stream stream)
            {
                if (Deserialize(stream, typeof(T)) is T instance)
                {
                    return instance;
                }
                else
                {
                    return default;
                }
            }

            #endregion Methods
        }

        #endregion Classes
    }
}