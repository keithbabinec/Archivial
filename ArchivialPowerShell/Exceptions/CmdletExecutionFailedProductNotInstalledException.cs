using System;
using System.Runtime.Serialization;

namespace ArchivialPowerShell.Exceptions
{
    public class CmdletExecutionFailedProductNotInstalledException : Exception
    {
        public CmdletExecutionFailedProductNotInstalledException()
        {
        }

        public CmdletExecutionFailedProductNotInstalledException(string message) : base(message)
        {
        }

        public CmdletExecutionFailedProductNotInstalledException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CmdletExecutionFailedProductNotInstalledException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
