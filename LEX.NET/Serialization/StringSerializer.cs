using Autrage.LEX.NET.Extensions;
using System;
using System.IO;
using System.Text;

using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.LEX.NET.Serialization
{
    public sealed class StringSerializer : Serializer
    {
        #region Properties

        public Encoding Encoding { get; set; } = Encoding.UTF8;

        #endregion Properties

        #region Methods

        public override bool CanHandle(Type type) => type == typeof(string);

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

            stream.Write((string)instance, Encoding);
            return true;
        }

        public override object Deserialize(Stream stream, Type expectedType)
        {
            stream.AssertNotNull();
            expectedType.AssertNotNull();

            if (!CanHandle(expectedType))
            {
                Warning($"Cannot deserialize type {expectedType}!");
                return false;
            }

            return stream.ReadString(Encoding);
        }

        #endregion Methods
    }
}