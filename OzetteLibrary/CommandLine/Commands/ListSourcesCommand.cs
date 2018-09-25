using OzetteLibrary.Database.LiteDB;
using OzetteLibrary.Logging.Default;
using OzetteLibrary.ServiceCore;
using System;
using System.Diagnostics;

namespace OzetteLibrary.CommandLine.Commands
{
    /// <summary>
    /// A command for listing the configured sources.
    /// </summary>
    public class ListSourcesCommand : ICommand
    {
        /// <summary>
        /// A logging helper instance.
        /// </summary>
        private Logger Logger;

        /// <summary>
        /// Constructor that requires a logging instance.
        /// </summary>
        /// <param name="logger"></param>
        public ListSourcesCommand(Logger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Logger = logger;
        }

        /// <summary>
        /// Runs the list-sources command.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Run(Arguments arguments)
        {
            // arguments is required from the interface definition, but there are no additional parameter arguments for this command.
            // so just ignore it, no validation required here.

            try
            {
                Logger.WriteConsole("--- Starting Ozette Cloud Backup configuration check");

                Logger.WriteConsole("--- Step 1: Query database for the list of configured backup sources.");
                PrintBackupSources();

                Logger.WriteConsole("--- Configuration check completed successfully.");

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
        /// Prints the providers.
        /// </summary>
        private void PrintBackupSources()
        {
            Logger.WriteConsole("Initializing a database connection.");

            var db = new LiteDBClientDatabase(CoreSettings.DatabaseConnectionString);
            db.PrepareDatabase();

            Logger.WriteConsole("Querying for existing backup sources.");

            var allSources = db.GetAllSourceLocations();

            Logger.WriteConsole("Number of configured backup sources: " + allSources.Count);

            foreach (var source in allSources)
            {
                Logger.WriteConsole("Source: " + source.ToString());
            }
        }
    }
}
