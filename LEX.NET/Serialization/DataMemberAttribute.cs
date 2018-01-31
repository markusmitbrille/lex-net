using System;

namespace Autrage.LEX.NET.Serialization
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class DataMemberAttribute : Attribute
    {
        #region Properties

        public string Name { get; }

        #endregion Properties

        #region Constructors

        public DataMemberAttribute()
        {
        }

        public DataMemberAttribute(string name) => Name = name;

        #endregion Constructors
    }
}