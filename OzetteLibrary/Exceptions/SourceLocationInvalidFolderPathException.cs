using System;
using System.Runtime.Serialization;

namespace OzetteLibrary.Exceptions
{
    /// <summary>
    /// An exception for invalid source location folder path.
    /// </summary>
    public class SourceLocationInvalidFolderPathException : SourceLocationException
    {
        public SourceLocationInvalidFolderPathException()
        {
        }

        public SourceLocationInvalidFolderPathException(string message) : base(message)
        {
        }

        public SourceLocationInvalidFolderPathException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SourceLocationInvalidFolderPathException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}