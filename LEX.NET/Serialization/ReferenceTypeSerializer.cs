using Autrage.LEX.NET.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using static Autrage.LEX.NET.DebugUtils;

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

            object instance = references.GetValueOrDefault(referenceID.Value);

            bool? hasPayload = stream.ReadBool();
            if (hasPayload == null)
            {
                Warning($"Could not read {type} payload indicator!");
                return null;
            }
            if (hasPayload == false)
            {
                if (instance == null)
                {
                    Warning($"Neither payload nor already restored reference found for {type} ({referenceID})!");
                }

                return instance;
            }

            if (instance == null)
            {
                instance = Instantiate(type);
                references[referenceID.Value] = instance;
            }

            if (!DeserializePayload(stream, instance))
            {
                Log($"Could not deserialize {type} payload!");
            }

            return instance;
        }

        protected abstract bool SerializePayload(Stream stream, object instance);

        protected abstract bool DeserializePayload(Stream stream, object instance);
    }
}