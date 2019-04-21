using System;
using System.Runtime.Serialization;

namespace ArchivialLibrary.Exceptions
{
    /// <summary>
    /// An exception for invalid source location folder path.
    /// </summary>
    public class SourceLocationInvalidUncFolderPathException : SourceLocationException
    {
        public SourceLocationInvalidUncFolderPathException()
        {
        }

        public SourceLocationInvalidUncFolderPathException(string message) : base(message)
        {
        }

        public SourceLocationInvalidUncFolderPathException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SourceLocationInvalidUncFolderPathException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}