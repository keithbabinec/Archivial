using OzetteLibrary.CommandLine.Arguments;
using OzetteLibrary.Database.SQLServer;
using OzetteLibrary.Logging.Default;
using OzetteLibrary.ServiceCore;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OzetteLibrary.CommandLine.Commands
{
    /// <summary>
    /// A command for showing the backup status.
    /// </summary>
    public class ShowStatusCommand : ICommand
    {
        /// <summary>
        /// A logging helper instance.
        /// </summary>
        private Logger Logger;

        /// <summary>
        /// Constructor that requires a logging instance.
        /// </summary>
        /// <param name="logger"></param>
        public ShowStatusCommand(Logger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Logger = logger;
        }

        /// <summary>
        /// Runs the show-status command.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns>True if successful, otherwise false.</returns>
        public async Task<bool> RunAsync(ArgumentBase arguments)
        {
            // arguments is required from the interface definition, but there are no additional parameter arguments for this command.
            // so just ignore it, no validation required here.

            try
            {
                Logger.WriteConsole("--- Starting Ozette Cloud Backup status check");

                Logger.WriteConsole("--- Step 1: Querying database for backup status.");
                await PrintStatusAsync();

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteConsole("--- Ozette Cloud Backup configuration check failed", EventLogEntryType.Error);
                Logger.WriteConsole(ex.ToString(), EventLogEntryType.Error);
                return false;
            }
        }

        /// <summary>
        /// Prints the backup status.
        /// </summary>
        private async Task PrintStatusAsync()
        {
            Logger.WriteConsole("Initializing a database connection.");

            var db = new SQLServerClientDatabase(CoreSettings.DatabaseConnectionString);

            Logger.WriteConsole("Checking backup file status...");

            var progress = await db.GetBackupProgressAsync();

            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Completion Percentage");
            Console.WriteLine("\tOverall: " + progress.OverallPercentage);
            Console.WriteLine();
            Console.WriteLine("Files");
            Console.WriteLine("\tTotal File Count: " + progress.TotalFileCount);
            Console.WriteLine("\tTotal File Size: " + progress.TotalFileSize);
            Console.WriteLine();
            Console.WriteLine("\tBacked Up Count: " + progress.BackedUpFileCount);
            Console.WriteLine("\tBacked Up Size: " + progress.BackedUpFileSize);
            Console.WriteLine();
            Console.WriteLine("\tRemaining Count: " + progress.RemainingFileCount);
            Console.WriteLine("\tRemaining Size: " + progress.RemainingFileSize);
            Console.WriteLine();
            Console.WriteLine("\tFailed Count: " + progress.FailedFileCount);
            Console.WriteLine("\tFailed Size: " + progress.FailedFileSize);
            Console.WriteLine("------------------------------------------------");
        }
    }
}
