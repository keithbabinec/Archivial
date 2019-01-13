using OzetteLibrary.CommandLine.Arguments;
using OzetteLibrary.Logging.Default;
using OzetteLibrary.Secrets;
using OzetteLibrary.ServiceCore;
using System;
using System.Diagnostics;
using System.Linq;
using OzetteLibrary.Providers;
using OzetteLibrary.MessagingProviders;
using OzetteLibrary.Database.SQLServer;

namespace OzetteLibrary.CommandLine.Commands
{
    /// <summary>
    /// A command for configuring twilio provider settings.
    /// </summary>
    /// <remarks>
    /// The reason for elevation requirement: This command uses encryption at the Machine-key scope. We cannot correctly save the encrypted secrets at this level without elevation.
    /// </remarks>
    [RequiresElevation]
    public class ConfigureTwilioCommand : ICommand
    {
        /// <summary>
        /// A logging helper instance.
        /// </summary>
        private Logger Logger;

        /// <summary>
        /// Constructor that requires a logging instance.
        /// </summary>
        /// <param name="logger"></param>
        public ConfigureTwilioCommand(Logger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Logger = logger;
        }

        /// <summary>
        /// Runs the configure-twilio command.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Run(ArgumentBase arguments)
        {
            var configTwilioArgs = arguments as ConfigureTwilioArguments;

            if (configTwilioArgs == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            try
            {
                Logger.WriteConsole("--- Starting Ozette Cloud Backup Twilio Provider configuration");

                Logger.WriteConsole("--- Step 1: Encrypt and save Twilio provider settings.");
                EncryptAndSave(configTwilioArgs);

                Logger.WriteConsole("--- Step 2: Configure providers list.");
                ConfigureProviders();

                Logger.WriteConsole("--- Twilio configuration completed successfully.");

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteConsole("--- Ozette Cloud Backup Twilio configuration failed", EventLogEntryType.Error);
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

            var db = new SQLServerClientDatabase(CoreSettings.DatabaseConnectionString);

            Logger.WriteConsole("Fetching current providers configuration from the database.");

            var existingProviders = db.GetProviders(ProviderTypes.Messaging);

            if (existingProviders.Any(x => x.Name == nameof(MessagingProviderTypes.Twilio)) == false)
            {
                Logger.WriteConsole("Twilio is not configured as a provider in the client database. Adding it now.");

                var newProvider =
                    new Provider()
                    {
                        Type = ProviderTypes.Messaging,
                        Name = nameof(MessagingProviderTypes.Twilio)
                    };

                db.AddProvider(newProvider);

                Logger.WriteConsole("Successfully configured Twilio as a messaging provider.");
            }
            else
            {
                Logger.WriteConsole("Twilio is already configured as a messaging provider in the client database. No action required.");
            }
        }

        /// <summary>
        /// Encrypts and saves the Twilio configuration settings.
        /// </summary>
        /// <param name="arguments"></param>
        private void EncryptAndSave(ConfigureTwilioArguments arguments)
        {
            Logger.WriteConsole("Initializing a database connection.");

            var db = new SQLServerClientDatabase(CoreSettings.DatabaseConnectionString);

            Logger.WriteConsole("Initializing protected data store.");

            var scope = System.Security.Cryptography.DataProtectionScope.LocalMachine;
            var ivkey = Convert.FromBase64String(CoreSettings.ProtectionIv);
            var pds = new ProtectedDataStore(db, scope, ivkey);

            Logger.WriteConsole("Saving encrypted Twilio configuration setting: TwilioAccountID.");
            pds.SetApplicationSecret(Constants.RuntimeSettingNames.TwilioAccountID, arguments.TwilioAccountID);

            Logger.WriteConsole("Saving encrypted Twilio configuration setting: TwilioAuthToken.");
            pds.SetApplicationSecret(Constants.RuntimeSettingNames.TwilioAuthToken, arguments.TwilioAuthToken);

            Logger.WriteConsole("Saving encrypted Twilio configuration setting: TwilioSourcePhone.");
            pds.SetApplicationSecret(Constants.RuntimeSettingNames.TwilioSourcePhone, arguments.TwilioSourcePhone);

            Logger.WriteConsole("Saving encrypted Twilio configuration setting: TwilioDestinationPhones.");
            pds.SetApplicationSecret(Constants.RuntimeSettingNames.TwilioDestinationPhones, arguments.TwilioDestinationPhones);
        }
    }
}
