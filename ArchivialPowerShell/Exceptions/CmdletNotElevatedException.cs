using System;
using System.Runtime.Serialization;

namespace OzettePowerShell.Exceptions
{
    public class CmdletNotElevatedException : Exception
    {
        public CmdletNotElevatedException()
        {
        }

        public CmdletNotElevatedException(string message) : base(message)
        {
        }

        public CmdletNotElevatedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CmdletNotElevatedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
