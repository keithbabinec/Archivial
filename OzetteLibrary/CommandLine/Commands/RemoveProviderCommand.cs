using OzetteLibrary.CommandLine.Arguments;
using OzetteLibrary.Database.SQLServer;
using OzetteLibrary.Logging.Default;
using OzetteLibrary.Providers;
using OzetteLibrary.ServiceCore;
using OzetteLibrary.StorageProviders;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OzetteLibrary.CommandLine.Commands
{
    /// <summary>
    /// A command for removing a stored provider.
    /// </summary>
    public class RemoveProviderCommand : ICommand
    {
        /// <summary>
        /// A logging helper instance.
        /// </summary>
        private Logger Logger;

        /// <summary>
        /// Constructor that requires a logging instance.
        /// </summary>
        /// <param name="logger"></param>
        public RemoveProviderCommand(Logger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Logger = logger;
        }

        /// <summary>
        /// Runs the remove-provider command.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns>True if successful, otherwise false.</returns>
        public async Task<bool> RunAsync(ArgumentBase arguments)
        {
            var removeProviderArgs = arguments as RemoveProviderArguments;

            if (removeProviderArgs == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            try
            {
                Logger.WriteConsole("--- Starting Ozette Cloud Backup provider configuration");

                Logger.WriteConsole("--- Step 1: Remove the provider from the database.");
                await RemoveProviderAsync(removeProviderArgs).ConfigureAwait(false);

                Logger.WriteConsole("--- Provider configuration completed successfully.");

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteConsole("--- Ozette Cloud Backup provider configuration failed", EventLogEntryType.Error);
                Logger.WriteConsole(ex.ToString(), EventLogEntryType.Error);
                return false;
            }
        }

        /// <summary>
        /// Removes the specified provider.
        /// </summary>
        /// <param name="arguments"></param>
        private async Task RemoveProviderAsync(RemoveProviderArguments arguments)
        {
            Logger.WriteConsole("Initializing a database connection.");

            var db = new SQLServerClientDatabase(CoreSettings.DatabaseConnectionString, Logger);

            Logger.WriteConsole("Querying for existing cloud providers to see if the specified provider exists.");

            var allProviders = await db.GetProvidersAsync(ProviderTypes.Any).ConfigureAwait(false);
            var providerToRemove = allProviders.FirstOrDefault(x => x.ID == arguments.ProviderID);

            if (providerToRemove == null)
            {
                // the source doesn't exist. nothing to do.
                Logger.WriteConsole("No provider was found with the specified ID. Nothing to remove.");
                return;
            }

            Logger.WriteConsole("Found a matching provider, removing it now.");
            await db.RemoveProviderAsync(providerToRemove.Name).ConfigureAwait(false);

            if (providerToRemove.Name == StorageProviderTypes.Azure.ToString())
            {
                // remove provider specific secrets
                await db.RemoveApplicationOptionAsync(Constants.RuntimeSettingNames.AzureStorageAccountName).ConfigureAwait(false);
                await db.RemoveApplicationOptionAsync(Constants.RuntimeSettingNames.AzureStorageAccountToken).ConfigureAwait(false);
            }
            else
            {
                throw new NotImplementedException("unexpected provider type.");
            }

            Logger.WriteConsole("Successfully removed the cloud provider from the database.");
        }
    }
}
