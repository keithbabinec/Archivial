using System;
using System.Runtime.Serialization;

namespace ArchivialLibrary.Exceptions
{
    /// <summary>
    /// An exception for when the application secret is not found in the secret store.
    /// </summary>
    public class ApplicationSecretMissingException : Exception
    {
        public ApplicationSecretMissingException()
        {
        }

        public ApplicationSecretMissingException(string message) : base(message)
        {
        }

        public ApplicationSecretMissingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ApplicationSecretMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
