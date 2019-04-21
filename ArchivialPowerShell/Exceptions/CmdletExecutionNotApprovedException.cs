using System;
using System.Runtime.Serialization;

namespace ArchivialPowerShell.Exceptions
{
    public class CmdletExecutionNotApprovedException : Exception
    {
        public CmdletExecutionNotApprovedException()
        {
        }

        public CmdletExecutionNotApprovedException(string message) : base(message)
        {
        }

        public CmdletExecutionNotApprovedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CmdletExecutionNotApprovedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
