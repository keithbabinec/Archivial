using System;
using System.Runtime.Serialization;

namespace ArchivialPowerShell.Exceptions
{
    public class CmdletExecutionFailedProductAlreadyInstalledException : Exception
    {
        public CmdletExecutionFailedProductAlreadyInstalledException()
        {
        }

        public CmdletExecutionFailedProductAlreadyInstalledException(string message) : base(message)
        {
        }

        public CmdletExecutionFailedProductAlreadyInstalledException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CmdletExecutionFailedProductAlreadyInstalledException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
