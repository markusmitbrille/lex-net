using Autrage.LEX.NET.Extensions;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.LEX.NET.Serialization
{
    public sealed class DictionarySerializer : ReferenceTypeSerializer
    {
        #region Methods

        public override bool CanHandle(Type type) => type.GetInterfaces().Any(i => i == typeof(IDictionary));

        protected override bool SerializePayload(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            IDictionary dictionary = (IDictionary)instance;

            stream.Write(dictionary.Count);

            foreach (object key in dictionary.Keys)
            {
                // Recursive call to marshaller for cascading serialization
                Marshaller.Serialize(stream, key);
            }
            foreach (object value in dictionary.Values)
            {
                // Recursive call to marshaller for cascading serialization
                Marshaller.Serialize(stream, value);
            }

            return true;
        }

        protected override bool DeserializePayload(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            int? count = stream.ReadInt();
            if (count == null)
            {
                Warning($"Could not read collection count!");
                return false;
            }

            ArrayList keys = new ArrayList(count.Value);
            for (int i = 0; i < count.Value; i++)
            {
                // Recursive call to marshaller for cascading deserialization
                object key = Marshaller.Deserialize(stream);
                keys.Add(key);
            }

            ArrayList values = new ArrayList(count.Value);
            for (int i = 0; i < count.Value; i++)
            {
                // Recursive call to marshaller for cascading deserialization
                object value = Marshaller.Deserialize(stream);
                values.Add(value);
            }

            IDictionary dictionary = (IDictionary)instance;
            for (int i = 0; i < count.Value; i++)
            {
                try
                {
                    dictionary.Add(keys[i], values[i]);
                }
                catch (Exception)
                {
                    Error($"Could not add entry '{keys[i]}={values[i]}' to {instance.GetType()} dictionary collection!");
                    throw;
                }
            }

            return true;
        }

        #endregion Methods
    }
}