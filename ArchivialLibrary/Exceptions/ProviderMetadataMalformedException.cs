using System;
using System.Runtime.Serialization;

namespace ArchivialLibrary.Exceptions
{
    /// <summary>
    /// An exception for when the provider metadata is malformed in the provider storage.
    /// </summary>
    public class ProviderMetadataMalformedException : Exception
    {
        public ProviderMetadataMalformedException()
        {
        }

        public ProviderMetadataMalformedException(string message) : base(message)
        {
        }

        public ProviderMetadataMalformedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ProviderMetadataMalformedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
