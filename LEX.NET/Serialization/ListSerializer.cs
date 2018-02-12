using Autrage.LEX.NET.Extensions;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.LEX.NET.Serialization
{
    public sealed class ListSerializer : ReferenceTypeSerializer
    {
        public override bool CanHandle(Type type) => type.GetInterfaces().Any(i => i == typeof(IList));

        protected override bool SerializePayload(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            IList list = (IList)instance;

            stream.Write(list.Count);

            foreach (object item in list)
            {
                // Recursive call to marshaller for cascading serialization
                Marshaller.Serialize(stream, item);
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

            IList list = (IList)instance;
            for (int i = 0; i < count.Value; i++)
            {
                object item = Marshaller.Deserialize(stream);

                try
                {
                    list.Add(item);
                }
                catch (Exception)
                {
                    Error($"Could not add item '{item}' to {instance.GetType()} list collection!");
                    throw;
                }
            }

            return true;
        }
    }
}