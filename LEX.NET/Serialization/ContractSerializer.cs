using System.IO;
using static Autrage.LEX.NET.DebugUtils;

namespace Autrage.LEX.NET.Serialization
{
    public sealed class ContractSerializer : ReferenceTypeSerializer
    {
        #region Methods

        protected override bool SerializePayload(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            if (!SerializeFields(stream, instance))
            {
                Warning($"Could not serialize {instance.GetType()} instance fields!");
                return false;
            }

            return true;
        }

        protected override bool DeserializePayload(Stream stream, object instance)
        {
            stream.AssertNotNull();
            instance.AssertNotNull();

            if (!DeserializeFields(stream, instance))
            {
                Warning($"Could not deserialize {instance.GetType()} instance fields!");
                return false;
            }

            return true;
        }

        #endregion Methods
    }
}