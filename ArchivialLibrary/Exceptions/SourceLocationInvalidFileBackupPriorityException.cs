using System;
using System.Runtime.Serialization;

namespace OzetteLibrary.Exceptions
{
    /// <summary>
    /// An exception for invalid source location file backup priority.
    /// </summary>
    public class SourceLocationInvalidFileBackupPriorityException : SourceLocationException
    {
        public SourceLocationInvalidFileBackupPriorityException()
        {
        }

        public SourceLocationInvalidFileBackupPriorityException(string message) : base(message)
        {
        }

        public SourceLocationInvalidFileBackupPriorityException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SourceLocationInvalidFileBackupPriorityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
