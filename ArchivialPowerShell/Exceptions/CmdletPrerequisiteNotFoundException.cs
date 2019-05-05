using System;
using System.Runtime.Serialization;

namespace ArchivialPowerShell.Exceptions
{
    public class CmdletPrerequisiteNotFoundException : Exception
    {
        public CmdletPrerequisiteNotFoundException()
        {
        }

        public CmdletPrerequisiteNotFoundException(string message) : base(message)
        {
        }

        public CmdletPrerequisiteNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CmdletPrerequisiteNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
