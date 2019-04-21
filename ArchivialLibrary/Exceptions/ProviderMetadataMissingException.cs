using System;
using System.Runtime.Serialization;

namespace OzetteLibrary.Exceptions
{
    /// <summary>
    /// An exception for when the provider metadata is missing from the provider storage.
    /// </summary>
    public class ProviderMetadataMissingException : Exception
    {
        public ProviderMetadataMissingException()
        {
        }

        public ProviderMetadataMissingException(string message) : base(message)
        {
        }

        public ProviderMetadataMissingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ProviderMetadataMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
