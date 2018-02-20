using Autrage.LEX.NET.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using static Autrage.LEX.NET.Bugger;

namespace Autrage.LEX.NET.Serialization
{
    public sealed class GenericCollectionSerializer : ReferenceTypeSerializer
    {
        public override bool CanHandle(Type type) => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));

        protected override bool SerializePayload(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            PropertyInfo countProperty = Cache.GetCountPropertyFrom(instance.GetType());
            if (countProperty == null)
            {
                Warning($"Could not retrieve count property for {instance.GetType()} collection instance!");
                return false;
            }

            stream.Write((int)countProperty.GetValue(instance));

            foreach (object item in (IEnumerable)instance)
            {
                // Recursive call to marshaller for cascading serialization
                Marshaller.Serialize(stream, item);
            }

            return true;
        }

        protected override void DeserializePayload(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            int? count = stream.ReadInt();
            if (count == null)
            {
                Warning($"Could not read collection count!");
                return;
            }

            IDictionary<Type, MethodInfo> addMethods = Cache.GetAddMethodsFrom(instance.GetType());
            if (addMethods == null || !addMethods.Any())
            {
                Warning($"Could not retrieve add methods for {instance.GetType()} collection instance!");
                return;
            }

            for (int i = 0; i < count.Value; i++)
            {
                // Recursive call to marshaller for cascading deserialization
                object item = Marshaller.Deserialize(stream);

                MethodInfo addMethod = addMethods.SingleOrDefault(e => e.Key.IsAssignableFrom(item.GetType())).Value;
                if (addMethod == null)
                {
                    Log($"Could not find add method for {item.GetType()} instance in {instance.GetType()} collection - discarding item.");
                    continue;
                }

                addMethod.Invoke(instance, new object[] { item });
            }
        }
    }
}