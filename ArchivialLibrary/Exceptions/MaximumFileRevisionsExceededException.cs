using System;
using System.Runtime.Serialization;

namespace ArchivialLibrary.Exceptions
{
    /// <summary>
    /// An exception for when the maximum number of trackable file revisions for a file has been exceeded.
    /// </summary>
    public class MaximumFileRevisionsExceededException : Exception
    {
        public MaximumFileRevisionsExceededException()
        {
        }

        public MaximumFileRevisionsExceededException(string message) : base(message)
        {
        }

        public MaximumFileRevisionsExceededException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MaximumFileRevisionsExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
