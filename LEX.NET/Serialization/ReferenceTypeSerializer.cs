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

            if (!CanHandle(instance.GetType()))
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

            string typeName = Cache.GetNameFrom(instance.GetType());
            if (typeName == null)
            {
                Warning($"Could not serialize {instance.GetType()} instance, could not get type name!");
                return false;
            }

            stream.Write(typeName, Marshaller.Encoding);

            if (SerializeFields(stream, instance))
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

            if (!CanHandle(expectedType))
            {
                Warning($"Cannot deserialize type {expectedType}!");
                return false;
            }

            long? referenceID = stream.ReadLong();
            if (referenceID == null)
            {
                Warning($"Could not deserialize {expectedType.Name} reference ID!");
                return null;
            }

            object instance = references.GetValueOrDefault(referenceID.Value);
            if (instance == null)
            {
                instance = Instantiate(stream, expectedType);
                if (instance == null)
                {
                    Warning($"Could not deserialize {expectedType.Name} reference, instantiation failed!");
                    return null;
                }

                references[referenceID.Value] = instance;

                if (DeserializeFields(stream, instance))
                {
                    Warning($"Could not deserialize {instance.GetType().Name} instance fields!");
                    return null;
                }
            }

            return instance;
        }

        private object Instantiate(Stream stream, Type expectedType)
        {
            stream.AssertNotNull();
            expectedType.AssertNotNull();

            Type type = DeserializeType(stream);
            if (type == null)
            {
                Warning($"Could not create {expectedType.Name} instance, type deserialization failed!");
                return null;
            }
            if (!expectedType.IsAssignableFrom(type))
            {
                Warning($"Could not create {expectedType.Name} instance, type mismatch: expected {expectedType.Name}, deserialized {type.Name}!");
                return null;
            }

            object instance = null;
            if (Cache.SkipConstructorOf(type))
            {
                instance = FormatterServices.GetSafeUninitializedObject(type);
                if (instance == null)
                {
                    Warning($"Could not create {expectedType.Name} instance, constructor invokation failed!");
                    return null;
                }
            }
            else
            {
                ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                if (constructor == null)
                {
                    Warning($"Could not create {expectedType.Name} instance, no default constructor found!");
                    return null;
                }

                instance = constructor.Invoke(null);
                if (instance == null)
                {
                    Warning($"Could not create {expectedType.Name} instance, constructor invokation failed!");
                    return null;
                }
            }

            return instance;
        }

        #endregion Methods
    }
}