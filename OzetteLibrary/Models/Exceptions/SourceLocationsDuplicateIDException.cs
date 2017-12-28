using System;
using System.Runtime.Serialization;

namespace OzetteLibrary.Models.Exceptions
{
    /// <summary>
    /// An exception for duplicate source location IDs.
    /// </summary>
    public class SourceLocationsDuplicateIDException : SourceLocationException
    {
        public SourceLocationsDuplicateIDException()
        {
        }

        public SourceLocationsDuplicateIDException(string message) : base(message)
        {
        }

        public SourceLocationsDuplicateIDException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SourceLocationsDuplicateIDException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
