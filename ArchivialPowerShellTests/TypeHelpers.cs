using System;

namespace ArchivialPowerShellTests
{
    /// <summary>
    /// Contains funcitonality for type checking.
    /// </summary>
    public class TypeHelpers
    {
        /// <summary>
        /// Checks if a cmdlet properties has the specified attribute.
        /// </summary>
        /// <param name="cmdletType"></param>
        /// <param name="cmdletProperty"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static bool CmdletParameterHasAttribute(Type cmdletType, string cmdletProperty, Type attribute)
        {
            var property = cmdletType.GetProperty(cmdletProperty);
            return Attribute.IsDefined(property, attribute);
        }
    }
}
