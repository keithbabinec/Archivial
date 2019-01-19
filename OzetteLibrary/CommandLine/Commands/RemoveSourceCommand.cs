using OzetteLibrary.CommandLine.Arguments;
using OzetteLibrary.Database.SQLServer;
using OzetteLibrary.Logging.Default;
using OzetteLibrary.ServiceCore;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OzetteLibrary.CommandLine.Commands
{
    public class RemoveSourceCommand : ICommand
    {
        /// <summary>
        /// A logging helper instance.
        /// </summary>
        private Logger Logger;

        /// <summary>
        /// Constructor that requires a logging instance.
        /// </summary>
        /// <param name="logger"></param>
        public RemoveSourceCommand(Logger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Logger = logger;
        }

        /// <summary>
        /// Runs the remove-source command.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns>True if successful, otherwise false.</returns>
        public async Task<bool> RunAsync(ArgumentBase arguments)
        {
            var removeSrcArgs = arguments as RemoveSourceArguments;

            if (removeSrcArgs == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            try
            {
                Logger.WriteConsole("--- Starting Ozette Cloud Backup source configuration");

                Logger.WriteConsole("--- Step 1: Remove the source from the database.");
                await RemoveSourceAsync(removeSrcArgs);

                Logger.WriteConsole("--- Source configuration completed successfully.");

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteConsole("--- Ozette Cloud Backup source configuration failed", EventLogEntryType.Error);
                Logger.WriteConsole(ex.ToString(), EventLogEntryType.Error);
                return false;
            }
        }

        /// <summary>
        /// Removes the specified source.
        /// </summary>
        /// <param name="arguments"></param>
        private async Task RemoveSourceAsync(RemoveSourceArguments arguments)
        {
            Logger.WriteConsole("Initializing a database connection.");

            var db = new SQLServerClientDatabase(CoreSettings.DatabaseConnectionString, Logger);

            Logger.WriteConsole("Querying for existing scan sources to see if the specified source exists.");

            var allSources = await db.GetSourceLocationsAsync();
            var sourceToRemove = allSources.FirstOrDefault(x => x.ID == arguments.SourceID);

            if (sourceToRemove == null)
            {
                // the source doesn't exist. nothing to do.
                Logger.WriteConsole("No source was found with the specified ID. Nothing to remove.");
                return;
            }

            Logger.WriteConsole("Found a matching backup source, removing it now.");

            await db.RemoveSourceLocationAsync(sourceToRemove);

            Logger.WriteConsole("Successfully removed the source from the database.");
        }
    }
}
