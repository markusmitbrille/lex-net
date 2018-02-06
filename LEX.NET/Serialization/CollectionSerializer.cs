using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Autrage.LEX.NET.Serialization
{
    internal class CollectionSerializer : ReferenceTypeSerializer
    {
        #region Methods

        public override bool CanHandle(Type type) => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));

        protected override bool SerializePayload(Stream stream, object instance)
        {
            throw new NotImplementedException();
        }

        protected override bool DeserializePayload(Stream stream, object instance)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}