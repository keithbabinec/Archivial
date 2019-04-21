using System;
using System.Runtime.Serialization;

namespace ArchivialLibrary.Exceptions
{
    public class SourceLocationInvalidIDException : SourceLocationException
    {
        public SourceLocationInvalidIDException()
        {
        }

        public SourceLocationInvalidIDException(string message) : base(message)
        {
        }

        public SourceLocationInvalidIDException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SourceLocationInvalidIDException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
