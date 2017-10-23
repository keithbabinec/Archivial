using System;
using System.Runtime.Serialization;

namespace OzetteLibrary.Models.Exceptions
{
    /// <summary>
    /// An exception for invalid source location file match filter.
    /// </summary>
    public class SourceLocationInvalidFileMatchFilterException : Exception
    {
        public SourceLocationInvalidFileMatchFilterException()
        {
        }

        public SourceLocationInvalidFileMatchFilterException(string message) : base(message)
        {
        }

        public SourceLocationInvalidFileMatchFilterException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SourceLocationInvalidFileMatchFilterException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}