using System;
using System.Runtime.Serialization;

namespace OzetteLibrary.Models.Exceptions
{
    public class SourceLocationInvalidIDException : Exception
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
