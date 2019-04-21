using System;
using System.Runtime.Serialization;

namespace ArchivialLibrary.Exceptions
{
    /// <summary>
    /// An exception for when the application core setting value is invalid.
    /// </summary>
    public class ApplicationCoreSettingInvalidValueException : Exception
    {
        public ApplicationCoreSettingInvalidValueException()
        {
        }

        public ApplicationCoreSettingInvalidValueException(string message) : base(message)
        {
        }

        public ApplicationCoreSettingInvalidValueException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ApplicationCoreSettingInvalidValueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
