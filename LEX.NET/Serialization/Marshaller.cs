using System;
using System.IO;

namespace Autrage.LEX.NET.Serialization
{
    public static class Marshaller
    {
        #region Methods

        public static IMarshaller Create()
        {
            return Serializer.CreateMarshaller();
        }

        public static IMarshaller Create(params Type[] serializers)
        {
            IMarshaller marshaller = Serializer.CreateMarshaller();
            foreach (Type serializer in serializers)
            {
                marshaller.AddSerializer(serializer);
            }

            return marshaller;
        }

        public static void Serialize(Stream stream, object instance, params Type[] serializers)
        {
            IMarshaller marshaller = Serializer.CreateMarshaller();
            foreach (Type serializer in serializers)
            {
                marshaller.AddSerializer(serializer);
            }

            marshaller.Serialize(stream, instance);
        }

        public static void Serialize<T>(Stream stream, T instance, params Type[] serializers)
        {
            Serialize(stream, (object)instance, serializers);
        }

        public static object Deserialize(Stream stream, params Type[] serializers)
        {
            IMarshaller marshaller = Serializer.CreateMarshaller();
            foreach (Type serializer in serializers)
            {
                marshaller.AddSerializer(serializer);
            }

            return marshaller.Deserialize(stream);
        }

        public static T Deserialize<T>(Stream stream, params Type[] serializers)
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

        #endregion Methods
    }
}