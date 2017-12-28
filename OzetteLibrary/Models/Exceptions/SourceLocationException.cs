using System;
using System.Runtime.Serialization;

namespace OzetteLibrary.Models.Exceptions
{
    /// <summary>
    /// A base exception for source location errors.
    /// </summary>
    public class SourceLocationException : Exception
    {
        public SourceLocationException()
        {
        }

        public SourceLocationException(string message) : base(message)
        {
        }

        public SourceLocationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SourceLocationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
