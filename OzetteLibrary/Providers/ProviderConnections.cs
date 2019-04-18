using OzetteLibrary.Database;
using OzetteLibrary.Exceptions;
using OzetteLibrary.Logging;
using OzetteLibrary.MessagingProviders;
using OzetteLibrary.MessagingProviders.Twilio;
using OzetteLibrary.Secrets;
using OzetteLibrary.ServiceCore;
using OzetteLibrary.StorageProviders;
using OzetteLibrary.StorageProviders.Azure;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace OzetteLibrary.Providers
{
    /// <summary>
    /// Contains functionality for setting up provider connections.
    /// </summary>
    public class ProviderConnections
    {
        /// <summary>
        /// Constructor that accepts the database connection.
        /// </summary>
        /// <param name="database"></param>
        public ProviderConnections(IClientDatabase database)
        {
            if (database == null)
            {
                throw new ArgumentNullException(nameof(database));
            }

            Database = database;
        }

        private IClientDatabase Database { get; set; }

        /// <summary>
        /// Configures the cloud storage provider connections.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        public async Task<StorageProviderConnectionsCollection> ConfigureStorageProviderConnectionsAsync(ILogger Log)
        {
            Log.WriteSystemEvent("Configuring cloud storage provider connections.", EventLogEntryType.Information, Constants.EventIDs.ConfiguringCloudProviderConnections, true);

            var storageConnections = new StorageProviderConnectionsCollection();

            try
            {
                // establish the protected store.

                var settingName = Constants.RuntimeSettingNames.ProtectionIV;
                var protectionIvEncodedString = await Database.GetApplicationOptionAsync(settingName).ConfigureAwait(false);
                var ivBytes = Convert.FromBase64String(protectionIvEncodedString);

                ProtectedDataStore protectedStore = new ProtectedDataStore(Database, DataProtectionScope.LocalMachine, ivBytes);

                // configure the provider implementation instances.
                // add each to the collection of providers.

                var providersList = await Database.GetProvidersAsync(ProviderTypes.Storage).ConfigureAwait(false);

                foreach (var provider in providersList)
                {
                    Log.WriteTraceMessage(string.Format("A storage provider was found in the configuration database: Name: {0}", provider.Name));

                    switch (provider.Name)
                    {
                        case nameof(StorageProviderTypes.Azure):
                            {
                                Log.WriteTraceMessage("Checking for Azure cloud storage provider connection settings.");
                                string storageAccountName = await protectedStore.GetApplicationSecretAsync(Constants.RuntimeSettingNames.AzureStorageAccountName).ConfigureAwait(false);
                                string storageAccountToken = await protectedStore.GetApplicationSecretAsync(Constants.RuntimeSettingNames.AzureStorageAccountToken).ConfigureAwait(false);

                                Log.WriteTraceMessage("Initializing Azure cloud storage provider.");
                                var azureConnection = new AzureStorageProviderFileOperations(Log, storageAccountName, storageAccountToken);
                                storageConnections.Add(StorageProviderTypes.Azure, azureConnection);
                                Log.WriteTraceMessage("Successfully initialized the cloud storage provider.");

                                break;
                            }
                        default:
                            {
                                throw new NotImplementedException("Unexpected provider type specified: " + provider.Type.ToString());
                            }
                    }
                }

                if (storageConnections.Count == 0)
                {
                    return null;
                }
                else
                {
                    Log.WriteSystemEvent("Successfully configured cloud storage provider connections.", EventLogEntryType.Information, Constants.EventIDs.ConfiguredCloudProviderConnections, true);
                    return storageConnections;
                }
            }
            catch (ApplicationCoreSettingMissingException ex)
            {
                Log.WriteSystemEvent("A core application setting has not been configured yet: " + ex.Message,
                    EventLogEntryType.Error, Constants.EventIDs.CoreSettingMissing, true);

                return null;
            }
            catch (ApplicationCoreSettingInvalidValueException ex)
            {
                Log.WriteSystemEvent("A core application setting has an invalid value specified: " + ex.Message,
                    EventLogEntryType.Error, Constants.EventIDs.CoreSettingInvalid, true);

                return null;
            }
            catch (ApplicationSecretMissingException)
            {
                Log.WriteSystemEvent("Failed to configure cloud storage provider connections: A cloud storage provider is missing required connection settings.",
                    EventLogEntryType.Error, Constants.EventIDs.FailedToConfigureProvidersMissingSettings, true);

                return null;
            }
            catch (Exception ex)
            {
                var message = "Failed to configure cloud storage provider connections.";
                var context = Log.GenerateFullContextStackTrace();
                Log.WriteSystemEvent(message, ex, context, Constants.EventIDs.FailedToConfigureStorageProviderConnections, true);
                return null;
            }
        }

        /// <summary>
        /// Configures the messaging provider connections.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        public async Task<MessagingProviderConnectionsCollection> ConfigureMessagingProviderConnectionsAsync(ILogger Log)
        {
            Log.WriteSystemEvent("Configuring messaging provider connections.", EventLogEntryType.Information, Constants.EventIDs.ConfiguringMessagingProviderConnections, true);

            var messagingConnections = new MessagingProviderConnectionsCollection();

            try
            {
                // establish the database and protected store.
                var settingName = Constants.RuntimeSettingNames.ProtectionIV;
                var protectionIvEncodedString = await Database.GetApplicationOptionAsync(settingName).ConfigureAwait(false);
                var ivBytes = Convert.FromBase64String(protectionIvEncodedString);

                ProtectedDataStore protectedStore = new ProtectedDataStore(Database, DataProtectionScope.LocalMachine, ivBytes);

                // configure the provider implementation instances.
                // add each to the collection of providers.

                var providersList = await Database.GetProvidersAsync(ProviderTypes.Messaging).ConfigureAwait(false);

                foreach (var provider in providersList)
                {
                    Log.WriteTraceMessage(string.Format("A messaging provider was found in the configuration database: Name: {0}", provider.Name));

                    switch (provider.Name)
                    {
                        case nameof(MessagingProviderTypes.Twilio):
                            {
                                Log.WriteTraceMessage("Checking for Twilio messaging provider connection settings.");
                                string accountID = await protectedStore.GetApplicationSecretAsync(Constants.RuntimeSettingNames.TwilioAccountID).ConfigureAwait(false);
                                string authToken = await protectedStore.GetApplicationSecretAsync(Constants.RuntimeSettingNames.TwilioAuthToken).ConfigureAwait(false);
                                string sourcePhone = await protectedStore.GetApplicationSecretAsync(Constants.RuntimeSettingNames.TwilioSourcePhone).ConfigureAwait(false);
                                string destPhones = await protectedStore.GetApplicationSecretAsync(Constants.RuntimeSettingNames.TwilioDestinationPhones).ConfigureAwait(false);

                                Log.WriteTraceMessage("Initializing Twilio messaging provider.");
                                var twilioConnection = new TwilioMessagingProviderOperations(Log, accountID, authToken, sourcePhone, destPhones);
                                messagingConnections.Add(MessagingProviderTypes.Twilio, twilioConnection);
                                Log.WriteTraceMessage("Successfully initialized the messaging provider.");

                                break;
                            }
                        default:
                            {
                                throw new NotImplementedException("Unexpected provider type specified: " + provider.Type.ToString());
                            }
                    }
                }

                if (messagingConnections.Count == 0)
                {
                    Log.WriteSystemEvent("No messaging providers are configured. This isn't a problem, but they are nice to have.", EventLogEntryType.Information, Constants.EventIDs.NoMessagingProviderConnections, true);
                    return null;
                }
                else
                {
                    Log.WriteSystemEvent("Successfully configured messaging provider connections.", EventLogEntryType.Information, Constants.EventIDs.ConfiguredMessagingProviderConnections, true);
                    return messagingConnections;
                }
            }
            catch (ApplicationCoreSettingMissingException ex)
            {
                Log.WriteSystemEvent("A core application setting has not been configured yet: " + ex.Message,
                    EventLogEntryType.Error, Constants.EventIDs.CoreSettingMissing, true);

                return null;
            }
            catch (ApplicationCoreSettingInvalidValueException ex)
            {
                Log.WriteSystemEvent("A core application setting has an invalid value specified: " + ex.Message,
                    EventLogEntryType.Error, Constants.EventIDs.CoreSettingInvalid, true);

                return null;
            }
            catch (ApplicationSecretMissingException)
            {
                Log.WriteSystemEvent("Failed to configure messaging provider connections: A messaging provider is missing required connection settings.",
                    EventLogEntryType.Error, Constants.EventIDs.FailedToConfigureProvidersMissingSettings, true);

                return null;
            }
            catch (Exception ex)
            {
                var message = "Failed to configure messaging provider connections.";
                var context = Log.GenerateFullContextStackTrace();
                Log.WriteSystemEvent(message, ex, context, Constants.EventIDs.FailedToConfigureMessagingProviderConnections, true);
                return null;
            }
        }
    }
}
