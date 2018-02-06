using System;

namespace Autrage.LEX.NET.Serialization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class DataContractAttribute : Attribute
    {
        #region Properties

        public bool SkipConstructor { get; set; }

        #endregion Properties

        #region Constructors

        public DataContractAttribute()
        {
        }

        public DataContractAttribute(bool skipConstructor)
        {
            SkipConstructor = skipConstructor;
        }

        #endregion Constructors
    }
}