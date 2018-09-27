using OzetteLibrary.CommandLine.Arguments;
using OzetteLibrary.Database.LiteDB;
using OzetteLibrary.Logging.Default;
using OzetteLibrary.ServiceCore;
using System;
using System.Diagnostics;

namespace OzetteLibrary.CommandLine.Commands
{
    /// <summary>
    /// A command for listing the configured providers.
    /// </summary>
    public class ListProvidersCommand : ICommand
    {
        /// <summary>
        /// A logging helper instance.
        /// </summary>
        private Logger Logger;

        /// <summary>
        /// Constructor that requires a logging instance.
        /// </summary>
        /// <param name="logger"></param>
        public ListProvidersCommand(Logger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Logger = logger;
        }

        /// <summary>
        /// Runs the list-providers command.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Run(ArgumentBase arguments)
        {
            // arguments is required from the interface definition, but there are no additional parameter arguments for this command.
            // so just ignore it, no validation required here.

            try
            {
                Logger.WriteConsole("--- Starting Ozette Cloud Backup configuration check");

                Logger.WriteConsole("--- Step 1: Query database for the list of configured providers.");
                PrintProviderNames();

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
        private void PrintProviderNames()
        {
            Logger.WriteConsole("Initializing a database connection.");

            var db = new LiteDBClientDatabase(CoreSettings.DatabaseConnectionString);
            db.PrepareDatabase();

            Logger.WriteConsole("Querying for existing providers.");

            var allProviders = db.GetProvidersList();

            Logger.WriteConsole("Number of configured providers: " + allProviders.Count);

            Logger.WriteConsole("--- Results:");

            foreach (var provider in allProviders)
            {
                Logger.WriteConsole(string.Format("Provider: ID={0}, Type={1}", provider.ID, provider.Type));
            }
        }
    }
}
