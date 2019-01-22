using OzetteLibrary.CommandLine.Arguments;
using OzetteLibrary.Database.SQLServer;
using OzetteLibrary.Logging.Default;
using OzetteLibrary.ServiceCore;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OzetteLibrary.CommandLine.Commands
{
    public class ListNetCredentialsCommand : ICommand
    {
        /// <summary>
        /// A logging helper instance.
        /// </summary>
        private Logger Logger;

        /// <summary>
        /// Constructor that requires a logging instance.
        /// </summary>
        /// <param name="logger"></param>
        public ListNetCredentialsCommand(Logger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Logger = logger;
        }

        /// <summary>
        /// Runs the list-netcredential command.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns>True if successful, otherwise false.</returns>
        public async Task<bool> RunAsync(ArgumentBase arguments)
        {
            var listCredsArgs = arguments as ListNetCredentialsArguments;

            if (listCredsArgs == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            try
            {
                Logger.WriteConsole("--- Starting Ozette Cloud Backup credential configuration");

                Logger.WriteConsole("--- Step 1: List the network credentials from the database.");
                await ListNetCredsAsync(listCredsArgs).ConfigureAwait(false);

                Logger.WriteConsole("--- Credential configuration completed successfully.");

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteConsole("--- Ozette Cloud Backup credential configuration failed", EventLogEntryType.Error);
                Logger.WriteConsole(ex.ToString(), EventLogEntryType.Error);
                return false;
            }
        }

        /// <summary>
        /// Lists net credentials
        /// </summary>
        /// <param name="arguments"></param>
        private async Task ListNetCredsAsync(ListNetCredentialsArguments arguments)
        {
            Logger.WriteConsole("Initializing a database connection.");

            var db = new SQLServerClientDatabase(CoreSettings.DatabaseConnectionString, Logger);

            Logger.WriteConsole("Querying for existing network credentials.");

            var allNetCreds = await db.GetNetCredentialsAsync().ConfigureAwait(false);

            Logger.WriteConsole("Number of configured credentials: " + allNetCreds.Count);

            Logger.WriteConsole("--- Results:");

            foreach (var provider in allNetCreds)
            {
                Logger.WriteConsole(string.Format("Credential: ID={0}, Name={1}", provider.ID, provider.CredentialName));
            }
        }
    }
}
