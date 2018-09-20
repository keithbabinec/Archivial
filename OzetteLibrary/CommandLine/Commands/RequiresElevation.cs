using System;

namespace OzetteLibrary.CommandLine.Commands
{
    /// <summary>
    /// An attribute that describes if a command requires elevation (admin rights) to execute.
    /// </summary>
    public class RequiresElevation : Attribute
    {
    }
}
