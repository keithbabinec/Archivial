using OzetteLibrary.CommandLine.Arguments;
using OzetteLibrary.Database.SQLServer;
using OzetteLibrary.Exceptions;
using OzetteLibrary.Folders;
using OzetteLibrary.Logging.Default;
using OzetteLibrary.ServiceCore;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OzetteLibrary.CommandLine.Commands
{
    /// <summary>
    /// A command for adding network backup sources to the active configuration.
    /// </summary>
    public class AddNetSourceCommand : ICommand
    {
        /// <summary>
        /// A logging helper instance.
        /// </summary>
        private Logger Logger;

        /// <summary>
        /// Constructor that requires a logging instance.
        /// </summary>
        /// <param name="logger"></param>
        public AddNetSourceCommand(Logger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Logger = logger;
        }

        /// <summary>
        /// Runs the add-netsource command.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns>True if successful, otherwise false.</returns>
        public async Task<bool> RunAsync(ArgumentBase arguments)
        {
            var addSrcArgs = arguments as AddNetSourceArguments;

            if (addSrcArgs == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            try
            {
                Logger.WriteConsole("--- Starting Ozette Cloud Backup source configuration");

                Logger.WriteConsole("--- Step 1: Validate the source and save it to the database.");
                await ValidateAndSaveSourceAsync(addSrcArgs).ConfigureAwait(false);

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
        /// Validates the provided source is usable.
        /// </summary>
        /// <param name="arguments"></param>
        private async Task ValidateAndSaveSourceAsync(AddNetSourceArguments arguments)
        {
            Logger.WriteConsole("Initializing a database connection.");

            var db = new SQLServerClientDatabase(CoreSettings.DatabaseConnectionString, Logger);

            Logger.WriteConsole("Querying for existing scan sources to check for duplicates.");

            var allSources = await db.GetSourceLocationsAsync().ConfigureAwait(false);
            var allNetSources = allSources.Where(x => x is NetworkSourceLocation).ToList();

            if (allNetSources.Any(x => string.Equals(x.Path, arguments.UncPath, StringComparison.CurrentCultureIgnoreCase) 
                                    && string.Equals(x.FileMatchFilter, arguments.Matchfilter, StringComparison.CurrentCultureIgnoreCase)))
            {
                // there already exists a source with this folder location and match filter.
                throw new SourceLocationException("Unable to add source: the specified folder and match filter combination is already listed as a source.");
            }
            else
            {
                Logger.WriteConsole("No duplicate sources found.");
            }

            var newSource = new NetworkSourceLocation();
            newSource.Path = arguments.UncPath;
            newSource.CredentialName = arguments.CredentialName;
            newSource.FileMatchFilter = arguments.Matchfilter;
            newSource.RevisionCount = arguments.Revisions;
            newSource.Priority = arguments.Priority;

            Logger.WriteConsole("Validating the source parameters are acceptable.");
            newSource.ValidateParameters();
            Logger.WriteConsole("The specified scan source has normal parameters.");

            Logger.WriteConsole("Saving the source to the database.");
            await db.SetSourceLocationAsync(newSource).ConfigureAwait(false);
            Logger.WriteConsole(string.Format("Successfully saved source ID {0} to the database.", newSource.ID));
        }
    }
}
