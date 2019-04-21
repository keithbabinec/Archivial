using System;
using System.Runtime.Serialization;

namespace ArchivialLibrary.Exceptions
{
    /// <summary>
    /// An exception for invalid source location credential name.
    /// </summary>
    public class SourceLocationInvalidCredentialNameException : SourceLocationException
    {
        public SourceLocationInvalidCredentialNameException()
        {
        }

        public SourceLocationInvalidCredentialNameException(string message) : base(message)
        {
        }

        public SourceLocationInvalidCredentialNameException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SourceLocationInvalidCredentialNameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
