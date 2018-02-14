using Autrage.LEX.NET.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using static Autrage.LEX.NET.Bugger;

namespace Autrage.LEX.NET.Serialization
{
    public abstract class ReferenceTypeSerializer : ObjectSerializer
    {
        private Dictionary<object, long> referenceIDs = new Dictionary<object, long>(new ReferenceComparer());
        private long nextID = 0;

        private Dictionary<long, object> references = new Dictionary<long, object>();

        public override bool CanHandle(Type type) => type.IsClass;

        public sealed override bool Serialize(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            Type type = instance.GetType();
            if (!CanHandle(type))
            {
                Warning($"{GetType()} cannot handle type {type}!");
                return false;
            }

            if (referenceIDs.ContainsKey(instance))
            {
                stream.Write(referenceIDs[instance]);
                stream.Write(false);
                return true;
            }

            long referenceID = nextID++;
            referenceIDs[instance] = referenceID;
            stream.Write(referenceID);
            stream.Write(true);

            if (!SerializePayload(stream, instance))
            {
                Warning($"Could not serialize {type} payload!");
                referenceIDs.Remove(instance);
                return false;
            }

            return true;
        }

        public sealed override object Deserialize(Stream stream, Type type)
        {
            stream.AssertNotNull();
            type.AssertNotNull();

            if (!CanHandle(type))
            {
                Warning($"{GetType()} cannot handle type {type}!");
                return null;
            }

            long? referenceID = stream.ReadLong();
            if (referenceID == null)
            {
                Warning($"Could not read {type} reference ID!");
                return null;
            }

            bool? hasPayload = stream.ReadBool();
            if (hasPayload == null)
            {
                Warning($"Could not read {type} payload indicator!");
                return null;
            }

            object instance = references.GetValueOrDefault(referenceID.Value);
            if (instance == null)
            {
                instance = Instantiate(type);
                references[referenceID.Value] = instance;
            }

            if (hasPayload == true)
            {
                DeserializePayload(stream, instance);
            }

            return instance;
        }

        protected abstract bool SerializePayload(Stream stream, object instance);

        protected abstract void DeserializePayload(Stream stream, object instance);
    }
}