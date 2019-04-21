using System;
using System.Runtime.Serialization;

namespace ArchivialLibrary.Exceptions
{
    /// <summary>
    /// An exception for provider option errors.
    /// </summary>
    public class ProviderOptionsException : Exception
    {
        public ProviderOptionsException()
        {
        }

        public ProviderOptionsException(string message) : base(message)
        {
        }

        public ProviderOptionsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ProviderOptionsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
