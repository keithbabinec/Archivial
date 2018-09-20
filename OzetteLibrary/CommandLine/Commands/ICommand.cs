namespace OzetteLibrary.CommandLine.Commands
{
    /// <summary>
    /// Describes an interface for a command-line command.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Runs the command.
        /// </summary>
        /// <param name="arguments">An arguments object.</param>
        /// <returns>True if successful, otherwise false.</returns>
        bool Run(Arguments arguments);
    }
}
