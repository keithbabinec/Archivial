using System;
using System.Runtime.Serialization;

namespace OzetteLibrary.Exceptions
{
    /// <summary>
    /// An exception for invalid source location file match filter.
    /// </summary>
    public class SourceLocationInvalidFileMatchFilterException : SourceLocationException
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