using System;
using System.IO;

namespace Autrage.LEX.NET.Serialization
{
    public abstract class Serializer
    {
        public Marshaller Marshaller { get; internal set; }

        public abstract bool CanHandle(Type type);

        public abstract bool Serialize(Stream stream, object instance);

        public abstract object Deserialize(Stream stream, Type type);
    }
}