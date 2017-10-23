using System;
using System.Runtime.Serialization;

namespace OzetteLibrary.Models.Exceptions
{
    /// <summary>
    /// An exception for invalid source location revision count.
    /// </summary>
    public class SourceLocationInvalidRevisionCountException : Exception
    {
        public SourceLocationInvalidRevisionCountException()
        {
        }

        public SourceLocationInvalidRevisionCountException(string message) : base(message)
        {
        }

        public SourceLocationInvalidRevisionCountException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SourceLocationInvalidRevisionCountException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}