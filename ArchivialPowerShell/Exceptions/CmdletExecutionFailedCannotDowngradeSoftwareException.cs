using System;
using System.Runtime.Serialization;

namespace ArchivialPowerShell.Exceptions
{
    public class CmdletExecutionFailedCannotDowngradeSoftwareException : Exception
    {
        public CmdletExecutionFailedCannotDowngradeSoftwareException()
        {
        }

        public CmdletExecutionFailedCannotDowngradeSoftwareException(string message) : base(message)
        {
        }

        public CmdletExecutionFailedCannotDowngradeSoftwareException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CmdletExecutionFailedCannotDowngradeSoftwareException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
