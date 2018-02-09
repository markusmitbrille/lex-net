using Autrage.LEX.NET.Extensions;
using Microsoft.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.LEX.NET.Serialization
{
    class DelegateSerializer : ReferenceTypeSerializer
    {
        private static CSharpCodeProvider Provider { get; } = new CSharpCodeProvider();

        public override bool CanHandle(Type type) => typeof(MulticastDelegate).IsAssignableFrom(type);

        protected override bool SerializePayload(Stream stream, object instance)
        {
            MulticastDelegate multiDel = (MulticastDelegate)instance;
            var methods =
                from del in multiDel.GetInvocationList()
                let method = del.Method
                where Provider.IsValidIdentifier(method.Name)
                select method;

            stream.Write(methods.Count());
            foreach (var method in methods)
            {
                stream.Write(Cache.GetNameFrom(method.DeclaringType));
                stream.Write(method.Name);
            }

            return true;
        }

        protected override bool DeserializePayload(Stream stream, object instance)
        {
            MulticastDelegate multiDel = (MulticastDelegate)instance;

            int? methodCount = stream.ReadInt();
            if(methodCount==null)
            {
                Warning("Could not read multicast delegate method count!");
                return false;
            }

            for (int i = 0; i < methodCount.Value; i++)
            {
                string declaringType = stream.ReadString();
                if (declaringType == null)
                {
                    Warning("Could not read multicast delegate method declaring type!");
                    return false;
                }

                string name = stream.ReadString();
                if (name == null)
                {
                    Warning("Could not read multicast delegate method name!");
                    return false;
                }

                Type type = Cache.GetTypeFrom(declaringType, Marshaller.AssemblyResolver, Marshaller.TypeResolver);
                MethodInfo method = type.GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                // Not working like this
            }

            return true;
        }
    }
}
