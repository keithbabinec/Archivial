using System;
using System.Runtime.Serialization;

namespace ArchivialPowerShell.Exceptions
{
    public class CmdletExecutionFailedDamagedProductInstallationException : Exception
    {
        public CmdletExecutionFailedDamagedProductInstallationException()
        {
        }

        public CmdletExecutionFailedDamagedProductInstallationException(string message) : base(message)
        {
        }

        public CmdletExecutionFailedDamagedProductInstallationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CmdletExecutionFailedDamagedProductInstallationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
