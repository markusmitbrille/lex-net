﻿using System.IO;
using static Autrage.LEX.NET.Bugger;

namespace Autrage.LEX.NET.Serialization
{
    public sealed class ContractSerializer : ReferenceTypeSerializer
    {
        protected override bool SerializePayload(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            if (!SerializeMembers(stream, instance))
            {
                Warning($"Could not serialize {instance.GetType()} instance members!");
                return false;
            }

            return true;
        }

        protected override void DeserializePayload(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            DeserializeMembers(stream, instance);
        }
    }
}