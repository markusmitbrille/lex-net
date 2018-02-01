using System;

namespace Autrage.LEX.NET.Serialization
{
    internal class Field
    {
        #region Properties

        public string Name { get; }
        public Type Type { get; }
        public object Value { get; }

        #endregion Properties

        #region Constructors

        public Field(string name, Type type, object value)
        {
            name.AssertNotNull();
            type.AssertNotNull();

            Name = name;
            Type = type;
            Value = value;
        }

        #endregion Constructors
    }
}