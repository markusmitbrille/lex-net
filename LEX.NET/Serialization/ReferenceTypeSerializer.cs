using Autrage.LEX.NET.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public override bool CanSerialize(Type type) => type.IsClass;

        public override bool Serialize(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            if (!CanSerialize(instance.GetType()))
            {
                Warning($"Cannot serialize type {instance.GetType()}!");
                return false;
            }

            if (referenceIDs.ContainsKey(instance))
            {
                stream.Write(referenceIDs[instance]);
                return true;
            }

            long referenceID = referenceIDs.Count == 0 ? 0 : referenceIDs.Max(e => e.Value) + 1;
            referenceIDs[instance] = referenceID;
            stream.Write(referenceID);

            if (SerializeObject(stream, instance))
            {
                return true;
            }
            else
            {
                referenceIDs.Remove(instance);
                return false;
            }
        }

        public override object Deserialize(Stream stream, Type expectedType)
        {
            stream.AssertNotNull();
            expectedType.AssertNotNull();
            expectedType.Assert(t => !t.IsValueType);

            if (!CanSerialize(expectedType))
            {
                Warning($"Cannot deserialize type {expectedType}!");
                return false;
            }

            long? referenceID = stream.ReadLong();
            if (referenceID == null)
            {
                Warning($"Could not deserialize {expectedType.Name} reference ID!");
                return expectedType.GetDefault();
            }

            object instance = references.GetValueOrDefault(referenceID.Value);
            if (instance == null)
            {
                instance = Instantiate(stream, expectedType);
                if (instance == null)
                {
                    Warning($"Could not deserialize {expectedType.Name} reference, instantiation failed!");
                    return expectedType.GetDefault();
                }

                references[referenceID.Value] = instance;

                IEnumerable<Field> fields = DeserializeFields(stream);
                if (fields == null)
                {
                    Warning($"Could not deserialize {instance.GetType().Name} instance fields!");
                    return expectedType.GetDefault();
                }

                SetFields(instance, fields);
            }

            return instance;
        }

        #endregion Methods
    }
}