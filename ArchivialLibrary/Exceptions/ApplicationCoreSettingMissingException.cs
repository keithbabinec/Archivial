using System;
using System.Runtime.Serialization;

namespace ArchivialLibrary.Exceptions
{
    /// <summary>
    /// An exception for when the application core setting is missing.
    /// </summary>
    public class ApplicationCoreSettingMissingException : Exception
    {
        public ApplicationCoreSettingMissingException()
        {
        }

        public ApplicationCoreSettingMissingException(string message) : base(message)
        {
        }

        public ApplicationCoreSettingMissingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ApplicationCoreSettingMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}