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
    public class RemoveNetCredentialCommand : ICommand
    {
        /// <summary>
        /// A logging helper instance.
        /// </summary>
        private Logger Logger;

        /// <summary>
        /// Constructor that requires a logging instance.
        /// </summary>
        /// <param name="logger"></param>
        public RemoveNetCredentialCommand(Logger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Logger = logger;
        }

        /// <summary>
        /// Runs the remove-netcredential command.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns>True if successful, otherwise false.</returns>
        public async Task<bool> RunAsync(ArgumentBase arguments)
        {
            var removeProviderArgs = arguments as RemoveNetCredentialArguments;

            if (removeProviderArgs == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            try
            {
                Logger.WriteConsole("--- Starting Ozette Cloud Backup credential configuration");

                Logger.WriteConsole("--- Step 1: Remove the network credential from the database.");
                await RemoveNetCredAsync(removeProviderArgs);

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
        /// Removes the specified credential.
        /// </summary>
        /// <param name="arguments"></param>
        private async Task RemoveNetCredAsync(RemoveNetCredentialArguments arguments)
        {
            Logger.WriteConsole("Initializing a database connection.");

            var db = new SQLServerClientDatabase(CoreSettings.DatabaseConnectionString, Logger);

            Logger.WriteConsole("Querying for existing network credentials to see if the specified credential exists.");

            var allCredentialsList = await db.GetNetCredentialsAsync();
            var credToRemove = allCredentialsList.FirstOrDefault(x => x.CredentialName == arguments.CredentialName);

            if (credToRemove == null)
            {
                // the credential doesn't exist. nothing to do.
                Logger.WriteConsole("No network credential was found with the specified name. Nothing to remove.");
                return;
            }

            Logger.WriteConsole("Found a matching network credential, removing it now.");

            await db.RemoveNetCredentialAsync(credToRemove.CredentialName);

            // remove provider specific secrets
            await db.RemoveApplicationOptionAsync(string.Format(Constants.Formats.NetCredentialUserNameKeyLookup, credToRemove.CredentialName));
            await db.RemoveApplicationOptionAsync(string.Format(Constants.Formats.NetCredentialUserPasswordKeyLookup, credToRemove.CredentialName));

            Logger.WriteConsole("Successfully removed the credential from the database.");
        }
    }
}
