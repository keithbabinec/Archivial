using System;
using System.Runtime.Serialization;

namespace ArchivialPowerShell.Exceptions
{
    public class CmdletExecutionFailedProductAlreadyInstalled : Exception
    {
        public CmdletExecutionFailedProductAlreadyInstalled()
        {
        }

        public CmdletExecutionFailedProductAlreadyInstalled(string message) : base(message)
        {
        }

        public CmdletExecutionFailedProductAlreadyInstalled(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CmdletExecutionFailedProductAlreadyInstalled(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
