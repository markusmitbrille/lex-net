using Autrage.LEX.NET.Extensions;
using Microsoft.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using static Autrage.LEX.NET.Bugger;

namespace Autrage.LEX.NET.Serialization
{
    public sealed class DelegateSerializer : Serializer
    {
        private Dictionary<MulticastDelegate, long> referenceIDs = new Dictionary<MulticastDelegate, long>(new ReferenceComparer());
        private long nextID = 0;

        private Dictionary<long, MulticastDelegate> references = new Dictionary<long, MulticastDelegate>();

        private static CSharpCodeProvider Provider { get; } = new CSharpCodeProvider();

        public override bool CanHandle(Type type) => typeof(MulticastDelegate).IsAssignableFrom(type);

        public override bool Serialize(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            Type type = instance.GetType();
            if (!CanHandle(type))
            {
                Warning($"{nameof(DelegateSerializer)} cannot handle type {type}!");
                return false;
            }

            MulticastDelegate multiDel = (MulticastDelegate)instance;

            if (referenceIDs.ContainsKey(multiDel))
            {
                stream.Write(referenceIDs[multiDel]);
                stream.Write(false);
                return true;
            }

            long referenceID = nextID++;
            referenceIDs[multiDel] = referenceID;
            stream.Write(referenceID);
            stream.Write(true);

            var methods =
                from del in multiDel.GetInvocationList()
                let method = del.Method
                where Provider.IsValidIdentifier(method.Name)
                let declaringTypeName = Cache.GetNameFrom(method.DeclaringType)
                where declaringTypeName != null
                let paramTypeNames =
                    from parameter in method.GetParameters()
                    let paramTypeName = Cache.GetNameFrom(parameter.ParameterType)
                    select paramTypeName
                where !paramTypeNames.Contains(null)
                select new { name = method.Name, declaringTypeName, paramTypeNames };

            stream.Write(methods.Count());
            foreach (var method in methods)
            {
                stream.Write(method.name, Marshaller.Encoding);
                stream.Write(method.declaringTypeName, Marshaller.Encoding);

                stream.Write(method.paramTypeNames.Count());
                foreach (string paramTypeName in method.paramTypeNames)
                {
                    stream.Write(paramTypeName, Marshaller.Encoding);
                }
            }

            return true;
        }

        public override object Deserialize(Stream stream, Type type)
        {
            stream.AssertNotNull();
            type.AssertNotNull();

            if (!CanHandle(type))
            {
                Warning($"{nameof(DelegateSerializer)} cannot handle type {type}!");
                return null;
            }

            long? referenceID = stream.ReadLong();
            if (referenceID == null)
            {
                Warning($"Could not read {type} reference ID!");
                return null;
            }

            MulticastDelegate instance = references.GetValueOrDefault(referenceID.Value);

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

            int? methodCount = stream.ReadInt();
            if (methodCount == null)
            {
                Warning("Could not read multicast delegate method count!");
                return null;
            }

            Delegate[] dels = new Delegate[methodCount.Value];
            for (int i = 0; i < methodCount.Value; i++)
            {
                string name = stream.ReadString(Marshaller.Encoding);
                if (name == null)
                {
                    Warning("Could not read multicast delegate method name!");
                    return null;
                }

                string declaringTypeName = stream.ReadString(Marshaller.Encoding);
                if (declaringTypeName == null)
                {
                    Warning("Could not read multicast delegate method declaring type!");
                    return null;
                }

                Type declaringType = Cache.GetTypeFrom(declaringTypeName, Marshaller.AssemblyResolver, Marshaller.TypeResolver);
                if (declaringType == null)
                {
                    Warning($"Could not retrieve type for {declaringTypeName} from cache!");
                    return null;
                }

                int? paramTypeNameCount = stream.ReadInt();
                if (paramTypeNameCount == null)
                {
                    Warning("Could not read multicast delegate method parameter type name count!");
                    return null;
                }

                Type[] paramTypes = new Type[paramTypeNameCount.Value];
                for (int j = 0; j < paramTypeNameCount.Value; j++)
                {
                    string paramTypeName = stream.ReadString(Marshaller.Encoding);
                    if (paramTypeName == null)
                    {
                        Warning("Could not read multicast delegate method parameter type name!");
                        return null;
                    }

                    Type paramType = Cache.GetTypeFrom(paramTypeName, Marshaller.AssemblyResolver, Marshaller.TypeResolver);
                    if (paramType == null)
                    {
                        Warning($"Could not retrieve type for {paramTypeName} from cache!");
                        return null;
                    }

                    paramTypes[j] = paramType;
                }

                const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
                MethodInfo method = declaringType.GetMethod(name, bindingFlags, null, paramTypes, null);
                if (method == null)
                {
                    Warning($"Could not find method {name} on type {declaringType}!");
                    return null;
                }

                dels[i] = Delegate.CreateDelegate(type, method);
            }

            if (instance == null)
            {
                instance = (MulticastDelegate)Delegate.Combine(dels);
                references[referenceID.Value] = instance;
            }

            return instance;
        }
    }
}