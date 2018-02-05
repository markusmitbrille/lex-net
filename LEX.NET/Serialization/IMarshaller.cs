using System;
using System.IO;
using System.Text;

namespace Autrage.LEX.NET.Serialization
{
    public interface IMarshaller
    {
        #region Properties

        Encoding Encoding { get; set; }

        #endregion Properties

        #region Methods

        Serializer AddSerializer(Type serializerType);

        T AddSerializer<T>() where T : Serializer, new();

        void Serialize(Stream stream, object instance);

        void Serialize<T>(Stream stream, T instance);

        object Deserialize(Stream stream);

        T Deserialize<T>(Stream stream);

        #endregion Methods
    }
}