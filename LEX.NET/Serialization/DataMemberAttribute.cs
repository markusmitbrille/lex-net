using System;

namespace Autrage.LEX.NET.Serialization
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class DataMemberAttribute : Attribute
    {
    }
}