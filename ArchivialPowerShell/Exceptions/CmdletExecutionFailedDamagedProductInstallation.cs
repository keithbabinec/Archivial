using System;
using System.Runtime.Serialization;

namespace ArchivialPowerShell.Exceptions
{
    public class CmdletExecutionFailedDamagedProductInstallation : Exception
    {
        public CmdletExecutionFailedDamagedProductInstallation()
        {
        }

        public CmdletExecutionFailedDamagedProductInstallation(string message) : base(message)
        {
        }

        public CmdletExecutionFailedDamagedProductInstallation(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CmdletExecutionFailedDamagedProductInstallation(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
