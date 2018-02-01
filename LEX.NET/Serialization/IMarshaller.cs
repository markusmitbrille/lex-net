using System;
using System.IO;

namespace Autrage.LEX.NET.Serialization
{
    public interface IMarshaller
    {
        #region Methods

        Serializer AddSerializer(Type serializerType);

        T AddSerializer<T>() where T : Serializer, new();

        void Serialize(Stream stream, object instance);

        void Serialize<T>(Stream stream, T instance);

        object Deserialize(Stream stream, Type expectedType);

        T Deserialize<T>(Stream stream);

        #endregion Methods
    }
}