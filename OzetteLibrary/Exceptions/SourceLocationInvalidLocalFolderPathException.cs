using System;
using System.Runtime.Serialization;

namespace OzetteLibrary.Exceptions
{
    /// <summary>
    /// An exception for invalid source location folder path.
    /// </summary>
    public class SourceLocationInvalidLocalFolderPathException : SourceLocationException
    {
        public SourceLocationInvalidLocalFolderPathException()
        {
        }

        public SourceLocationInvalidLocalFolderPathException(string message) : base(message)
        {
        }

        public SourceLocationInvalidLocalFolderPathException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SourceLocationInvalidLocalFolderPathException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}