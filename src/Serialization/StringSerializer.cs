﻿using Autrage.LEX.NET.Extensions;
using System;
using System.IO;
using System.Text;

using static Autrage.LEX.NET.Bugger;

namespace Autrage.LEX.NET.Serialization
{
    public sealed class StringSerializer : Serializer
    {
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public override bool CanHandle(Type type) => type == typeof(string);

        public override bool Serialize(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            Type type = instance.GetType();
            if (!CanHandle(type))
            {
                Warning($"{nameof(StringSerializer)} cannot handle type {type}!");
                return false;
            }

            stream.Write((string)instance, Encoding);
            return true;
        }

        public override object Deserialize(Stream stream, Type type)
        {
            stream.AssertNotNull();
            type.AssertNotNull();

            if (!CanHandle(type))
            {
                Warning($"{nameof(StringSerializer)} cannot handle type {type}!");
                return null;
            }

            return stream.ReadString(Encoding);
        }
    }
}