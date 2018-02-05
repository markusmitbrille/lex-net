using System;

namespace Autrage.LEX.NET.Serialization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class DataContractAttribute : Attribute
    {
        #region Properties

        public string Name { get; }
        public bool SkipConstructor { get; set; }

        #endregion Properties

        #region Constructors

        public DataContractAttribute()
        {
        }

        public DataContractAttribute(string name) => Name = name;

        public DataContractAttribute(string name, bool skipConstructor)
        {
            Name = name;
            SkipConstructor = skipConstructor;
        }

        #endregion Constructors
    }
}