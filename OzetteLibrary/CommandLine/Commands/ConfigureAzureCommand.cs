using OzetteLibrary.CommandLine.Arguments;
using OzetteLibrary.Database.LiteDB;
using OzetteLibrary.Logging.Default;
using OzetteLibrary.Providers;
using OzetteLibrary.Secrets;
using OzetteLibrary.ServiceCore;
using System;
using System.Diagnostics;
using System.Linq;

namespace OzetteLibrary.CommandLine.Commands
{
    /// <summary>
    /// A command for configuring azure provider settings.
    /// </summary>
    /// <remarks>
    /// The reason for elevation requirement: This command uses encryption at the Machine-key scope. We cannot correctly save the encrypted secrets at this level without elevation.
    /// </remarks>
    [RequiresElevation]
    public class ConfigureAzureCommand : ICommand
    {
        /// <summary>
        /// A logging helper instance.
        /// </summary>
        private Logger Logger;

        /// <summary>
        /// Constructor that requires a logging instance.
        /// </summary>
        /// <param name="logger"></param>
        public ConfigureAzureCommand(Logger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Logger = logger;
        }

        /// <summary>
        /// Runs the configure-azure command.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Run(ArgumentBase arguments)
        {
            var configAzArgs = arguments as ConfigureAzureArguments;

            if (configAzArgs == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            try
            {
                Logger.WriteConsole("--- Starting Ozette Cloud Backup Azure configuration");

                Logger.WriteConsole("--- Step 1: Encrypt and save Azure settings.");
                EncryptAndSave(configAzArgs);

                Logger.WriteConsole("--- Step 2: Configure providers list.");
                ConfigureProviders();

                Logger.WriteConsole("--- Azure configuration completed successfully.");

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteConsole("--- Ozette Cloud Backup Azure configuration failed", EventLogEntryType.Error);
                Logger.WriteConsole(ex.ToString(), EventLogEntryType.Error);
                return false;
            }
        }

        /// <summary>
        /// Configures the providers in the client database.
        /// </summary>
        private void ConfigureProviders()
        {
            Logger.WriteConsole("Initializing a database connection.");

            var db = new LiteDBClientDatabase(CoreSettings.DatabaseConnectionString);
            db.PrepareDatabase();

            Logger.WriteConsole("Fetching current providers configuration from the database.");

            var existingProviders = db.GetProvidersList();

            if (existingProviders.Any(x => x.Type == ProviderTypes.Azure) == false)
            {
                Logger.WriteConsole("Azure is not configured as a provider in the client database. Adding it now.");

                existingProviders.Add(
                    new Provider()
                    {
                        Enabled = true,
                        ID = (int)ProviderTypes.Azure,
                        Type = ProviderTypes.Azure
                    });

                db.SetProviders(existingProviders);

                Logger.WriteConsole("Successfully configured Azure as a cloud backup provider.");
            }
            else
            {
                Logger.WriteConsole("Azure is already configured as a provider in the client database. No action required.");
            }
        }

        /// <summary>
        /// Encrypts and saves the Azure configuration settings.
        /// </summary>
        /// <param name="arguments"></param>
        private void EncryptAndSave(ConfigureAzureArguments arguments)
        {
            Logger.WriteConsole("Initializing a database connection.");

            var db = new LiteDBClientDatabase(CoreSettings.DatabaseConnectionString);
            db.PrepareDatabase();

            Logger.WriteConsole("Initializing protected data store.");

            var scope = System.Security.Cryptography.DataProtectionScope.LocalMachine;
            var ivkey = Convert.FromBase64String(CoreSettings.ProtectionIv);
            var pds = new ProtectedDataStore(db, scope, ivkey);

            Logger.WriteConsole("Saving encrypted Azure configuration setting: AzureStorageAccountName.");

            var account = new ServiceOption()
            {
                ID = Constants.OptionIDs.AzureStorageAccountName,
                Name = nameof(Constants.OptionIDs.AzureStorageAccountName),
                Value = arguments.AzureStorageAccountName,
                IsEncryptedOption = true
            };
            pds.SetApplicationSecret(account);

            Logger.WriteConsole("Saving encrypted Azure configuration setting: AzureStorageAccountToken.");

            var token = new ServiceOption()
            {
                ID = Constants.OptionIDs.AzureStorageAccountToken,
                Name = nameof(Constants.OptionIDs.AzureStorageAccountToken),
                Value = arguments.AzureStorageAccountToken,
                IsEncryptedOption = true
            };
            pds.SetApplicationSecret(token);
        }
    }
}
