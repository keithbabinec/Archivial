using System;
using System.Runtime.Serialization;

namespace ArchivialLibrary.Exceptions
{
    /// <summary>
    /// An exception for when the application database is missing.
    /// </summary>
    public class ApplicationDatabaseNotPresentException : Exception
    {
        public ApplicationDatabaseNotPresentException()
        {
        }

        public ApplicationDatabaseNotPresentException(string message) : base(message)
        {
        }

        public ApplicationDatabaseNotPresentException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ApplicationDatabaseNotPresentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
