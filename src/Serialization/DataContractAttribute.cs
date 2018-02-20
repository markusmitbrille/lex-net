using System;

namespace Autrage.LEX.NET.Serialization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class DataContractAttribute : Attribute
    {
        public bool SkipConstructor { get; set; }

        public DataContractAttribute()
        {
        }

        public DataContractAttribute(bool skipConstructor)
        {
            SkipConstructor = skipConstructor;
        }
    }
}