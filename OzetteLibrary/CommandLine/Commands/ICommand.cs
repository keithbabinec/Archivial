using OzetteLibrary.CommandLine.Arguments;
using System.Threading.Tasks;

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
        Task<bool> RunAsync(ArgumentBase arguments);
    }
}
