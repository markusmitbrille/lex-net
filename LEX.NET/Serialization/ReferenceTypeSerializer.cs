using Autrage.LEX.NET.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.LEX.NET.Serialization
{
    public sealed class ReferenceTypeSerializer : ObjectSerializer
    {
        #region Fields

        private Dictionary<object, long> referenceIDs = new Dictionary<object, long>(new ReferenceComparer());
        private Dictionary<long, object> references = new Dictionary<long, object>();

        #endregion Fields

        #region Methods

        public override bool CanHandle(Type type) => type.IsClass;

        public override bool Serialize(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            Type type = instance.GetType();
            if (!CanHandle(type))
            {
                Warning($"{nameof(ReferenceTypeSerializer)} cannot handle type {type}!");
                return false;
            }

            if (referenceIDs.ContainsKey(instance))
            {
                stream.Write(referenceIDs[instance]);
                stream.Write(false);
                return true;
            }

            long referenceID = referenceIDs.Count == 0 ? 0 : referenceIDs.Max(e => e.Value) + 1;
            referenceIDs[instance] = referenceID;
            stream.Write(referenceID);
            stream.Write(true);

            if (!SerializeFields(stream, instance))
            {
                Warning($"Could not serialize {type} instance fields!");
                referenceIDs.Remove(instance);
                return false;
            }

            return true;
        }

        public override object Deserialize(Stream stream, Type type)
        {
            stream.AssertNotNull();
            type.AssertNotNull();

            if (!CanHandle(type))
            {
                Warning($"{nameof(ReferenceTypeSerializer)} cannot handle type {type}!");
                return false;
            }

            long? referenceID = stream.ReadLong();
            if (referenceID == null)
            {
                Warning($"Could not read {type} reference ID!");
                return null;
            }

            object instance = references.GetValueOrDefault(referenceID.Value);
            if (instance == null)
            {
                instance = Instantiate(type);
                references[referenceID.Value] = instance;
            }

            bool? hasFields = stream.ReadBool();
            if (hasFields == null)
            {
                Warning($"Could not read {type} fields value indicator!");
                return null;
            }

            if (hasFields == true && !DeserializeFields(stream, instance))
            {
                Warning($"Could not deserialize {type} instance fields!");
                return null;
            }

            return instance;
        }

        #endregion Methods
    }
}