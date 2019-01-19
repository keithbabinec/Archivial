using OzetteLibrary.Client;
using OzetteLibrary.Exceptions;
using OzetteLibrary.Logging;
using OzetteLibrary.Logging.Default;
using OzetteLibrary.StorageProviders;
using OzetteLibrary.StorageProviders.Azure;
using OzetteLibrary.Secrets;
using OzetteLibrary.ServiceCore;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Threading;
using OzetteLibrary.Providers;
using OzetteLibrary.MessagingProviders;
using OzetteLibrary.MessagingProviders.Twilio;
using System.Collections.Generic;
using OzetteLibrary.Database.SQLServer;
using System.Threading.Tasks;
using OzetteLibrary.Database;

namespace OzetteClientAgent
{
    /// <summary>
    /// Contains service functionality.
    /// </summary>
    public partial class OzetteClientAgent : ServiceBase
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public OzetteClientAgent()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// Runs when the service start is triggered.
        /// </summary>
        /// <remarks>
        /// Long running initialization code can confuse the service control manager (thinks it may be a hang).
        /// Instead launch the initialization tasks in a seperate thread so control returns to the SCM immediately.
        /// </remarks>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            Thread t = new Thread(() => (Task.Run(() => CoreStartAsync())).Wait());
            t.Start();
        }

        /// <summary>
        /// A reference to the core service log.
        /// </summary>
        private ILogger CoreLog { get; set; }

        /// <summary>
        /// A reference to the backup engine log.
        /// </summary>
        private ILogger BackupEngineLog { get; set; }

        /// <summary>
        /// A reference to the backup engine log.
        /// </summary>
        private ILogger ScanEngineLog { get; set; }

        /// <summary>
        /// A reference to the client database instance.
        /// </summary>
        private IClientDatabase ClientDatabase { get; set; }

        /// <summary>
        /// A reference to the connection engine instance.
        /// </summary>
        private ConnectionEngine ConnectionEngineInstance { get; set; }

        /// <summary>
        /// A reference to the status engine instance.
        /// </summary>
        private StatusEngine StatusEngineInstance { get; set; }

        /// <summary>
        /// A reference to the scanning engine instance.
        /// </summary>
        private ScanEngine ScanEngineInstance { get; set; }

        /// <summary>
        /// A reference to the backup engine instances.
        /// </summary>
        private List<BackupEngine> BackupEngineInstances { get; set; }

        /// <summary>
        /// A collection of storage provider connections.
        /// </summary>
        private StorageProviderConnectionsCollection StorageConnections { get; set; }

        /// <summary>
        /// A collection of messaging provider connections.
        /// </summary>
        private MessagingProviderConnectionsCollection MessagingConnections { get; set; }

        /// <summary>
        /// Core application start.
        /// </summary>
        private async Task CoreStartAsync()
        {
            StartLoggers();

            CoreLog.WriteSystemEvent(
                string.Format("Starting {0} client service.", OzetteLibrary.Constants.Logging.AppName),
                EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StartingService, true);

            if (!(await ConfigureDatabaseAsync()))
            {
                Stop();
                return;
            }

            if (!(await ConfigureStorageProviderConnectionsAsync()))
            {
                Stop();
                return;
            }

            if (!(await ConfigureMessagingProviderConnectionsAsync()))
            {
                Stop();
                return;
            }

            if (!StartConnectionEngine())
            {
                Stop();
                return;
            }

            if (!StartStatusEngine())
            {
                Stop();
                return;
            }

            if (!StartScanEngine())
            {
                Stop();
                return;
            }

            if (!StartBackupEngines())
            {
                Stop();
                return;
            }

            CoreLog.WriteSystemEvent(
                string.Format("Successfully started {0} client service.", OzetteLibrary.Constants.Logging.AppName),
                EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StartedService, true);
        }

        /// <summary>
        /// Runs when the service stop is triggered.
        /// </summary>
        protected override void OnStop()
        {
            if (CoreLog != null)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Stopping {0} client service.", OzetteLibrary.Constants.Logging.AppName),
                    EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StoppingService, true);
            }

            if (ScanEngineInstance != null)
            {
                ScanEngineInstance.BeginStop();
            }
            if (BackupEngineInstances != null)
            {
                foreach (var instance in BackupEngineInstances)
                {
                    instance.BeginStop();
                }
            }
            if (ConnectionEngineInstance != null)
            {
                ConnectionEngineInstance.BeginStop();
            }
            if (StatusEngineInstance != null)
            {
                StatusEngineInstance.BeginStop();
            }

            if (CoreLog != null)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Successfully stopped {0} client service.", OzetteLibrary.Constants.Logging.AppName),
                    EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StoppedService, true);
            }
        }

        /// <summary>
        /// Starts the shared logging instances.
        /// </summary>
        private void StartLoggers()
        {
            CoreLog = new Logger(OzetteLibrary.Constants.Logging.CoreServiceComponentName);
            CoreLog.Start(
                CoreSettings.EventlogName,
                CoreSettings.EventlogName,
                CoreSettings.LogFilesDirectory);

            BackupEngineLog = new Logger(OzetteLibrary.Constants.Logging.BackupComponentName);
            BackupEngineLog.Start(
                CoreSettings.EventlogName,
                CoreSettings.EventlogName,
                CoreSettings.LogFilesDirectory);

            ScanEngineLog = new Logger(OzetteLibrary.Constants.Logging.ScanningComponentName);
            ScanEngineLog.Start(
                CoreSettings.EventlogName,
                CoreSettings.EventlogName,
                CoreSettings.LogFilesDirectory);
        }

        /// <summary>
        /// Configures the database instance.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ConfigureDatabaseAsync()
        {
            try
            {
                ClientDatabase = new SQLServerClientDatabase(CoreSettings.DatabaseConnectionString);
                await ClientDatabase.PrepareDatabaseAsync(CoreLog);

                return true;
            }
            catch (Exception ex)
            {
                var message = "Failed to configure client database.";
                var context = CoreLog.GenerateFullContextStackTrace();
                CoreLog.WriteSystemEvent(message, ex, context, OzetteLibrary.Constants.EventIDs.FailedToPrepareClientDatabase, true);
                return false;
            }
        }

        /// <summary>
        /// Configures the cloud storage provider connections.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        private async Task<bool> ConfigureStorageProviderConnectionsAsync()
        {
            CoreLog.WriteSystemEvent("Configuring cloud storage provider connections.", EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.ConfiguringCloudProviderConnections, true);

            StorageConnections = new StorageProviderConnectionsCollection();

            try
            {
                // establish the protected store.
                var ivEncodedString = CoreSettings.ProtectionIv;
                var ivBytes = Convert.FromBase64String(ivEncodedString);

                ProtectedDataStore protectedStore = new ProtectedDataStore(ClientDatabase, DataProtectionScope.LocalMachine, ivBytes);

                // configure the provider implementation instances.
                // add each to the collection of providers.

                var providersList = await ClientDatabase.GetProvidersAsync(ProviderTypes.Storage);

                foreach (var provider in providersList)
                {
                    CoreLog.WriteTraceMessage(string.Format("A storage provider was found in the configuration database: Name: {0}", provider.Name));

                    switch (provider.Name)
                    {
                        case nameof(StorageProviderTypes.Azure):
                            {
                                CoreLog.WriteTraceMessage("Checking for Azure cloud storage provider connection settings.");
                                string storageAccountName = await protectedStore.GetApplicationSecretAsync(OzetteLibrary.Constants.RuntimeSettingNames.AzureStorageAccountName);
                                string storageAccountToken = await protectedStore.GetApplicationSecretAsync(OzetteLibrary.Constants.RuntimeSettingNames.AzureStorageAccountToken);

                                CoreLog.WriteTraceMessage("Initializing Azure cloud storage provider.");
                                var azureConnection = new AzureStorageProviderFileOperations(BackupEngineLog, storageAccountName, storageAccountToken);
                                StorageConnections.Add(StorageProviderTypes.Azure, azureConnection);
                                CoreLog.WriteTraceMessage("Successfully initialized the cloud storage provider.");

                                break;
                            }
                        default:
                            {
                                throw new NotImplementedException("Unexpected provider type specified: " + provider.Type.ToString());
                            }
                    }
                }

                if (StorageConnections.Count == 0)
                {
                    CoreLog.WriteSystemEvent("Failed to configure cloud storage provider connections: No storage providers were listed in the database.",
                        EventLogEntryType.Error, OzetteLibrary.Constants.EventIDs.FailedToConfigureProvidersNotFound, true);

                    return false;
                }

                CoreLog.WriteSystemEvent("Successfully configured cloud storage provider connections.", EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.ConfiguredCloudProviderConnections, true);
                return true;
            }
            catch (ApplicationCoreSettingMissingException ex)
            {
                CoreLog.WriteSystemEvent("A core application setting has not been configured yet: " + ex.Message,
                    EventLogEntryType.Error, OzetteLibrary.Constants.EventIDs.CoreSettingMissing, true);

                return false;
            }
            catch (ApplicationCoreSettingInvalidValueException ex)
            {
                CoreLog.WriteSystemEvent("A core application setting has an invalid value specified: " + ex.Message,
                    EventLogEntryType.Error, OzetteLibrary.Constants.EventIDs.CoreSettingInvalid, true);

                return false;
            }
            catch (ApplicationSecretMissingException)
            {
                CoreLog.WriteSystemEvent("Failed to configure cloud storage provider connections: A cloud storage provider is missing required connection settings.",
                    EventLogEntryType.Error, OzetteLibrary.Constants.EventIDs.FailedToConfigureProvidersMissingSettings, true);

                return false;
            }
            catch (Exception ex)
            {
                var message = "Failed to configure cloud storage provider connections.";
                var context = CoreLog.GenerateFullContextStackTrace();
                CoreLog.WriteSystemEvent(message, ex, context, OzetteLibrary.Constants.EventIDs.FailedToConfigureStorageProviderConnections, true);
                return false;
            }
        }

        /// <summary>
        /// Configures the messaging provider connections.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        private async Task<bool> ConfigureMessagingProviderConnectionsAsync()
        {
            CoreLog.WriteSystemEvent("Configuring messaging provider connections.", EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.ConfiguringMessagingProviderConnections, true);

            MessagingConnections = new MessagingProviderConnectionsCollection();

            try
            {
                // establish the database and protected store.
                var ivEncodedString = CoreSettings.ProtectionIv;
                var ivBytes = Convert.FromBase64String(ivEncodedString);

                ProtectedDataStore protectedStore = new ProtectedDataStore(ClientDatabase, DataProtectionScope.LocalMachine, ivBytes);

                // configure the provider implementation instances.
                // add each to the collection of providers.

                var providersList = await ClientDatabase.GetProvidersAsync(ProviderTypes.Messaging);

                foreach (var provider in providersList)
                {
                    CoreLog.WriteTraceMessage(string.Format("A messaging provider was found in the configuration database: Name: {0}", provider.Name));

                    switch (provider.Name)
                    {
                        case nameof(MessagingProviderTypes.Twilio):
                            {
                                CoreLog.WriteTraceMessage("Checking for Twilio messaging provider connection settings.");
                                string accountID = await protectedStore.GetApplicationSecretAsync(OzetteLibrary.Constants.RuntimeSettingNames.TwilioAccountID);
                                string authToken = await protectedStore.GetApplicationSecretAsync(OzetteLibrary.Constants.RuntimeSettingNames.TwilioAuthToken);
                                string sourcePhone = await protectedStore.GetApplicationSecretAsync(OzetteLibrary.Constants.RuntimeSettingNames.TwilioSourcePhone);
                                string destPhones = await protectedStore.GetApplicationSecretAsync(OzetteLibrary.Constants.RuntimeSettingNames.TwilioDestinationPhones);

                                CoreLog.WriteTraceMessage("Initializing Twilio messaging provider.");
                                var twilioConnection = new TwilioMessagingProviderOperations(CoreLog, accountID, authToken, sourcePhone, destPhones);
                                MessagingConnections.Add(MessagingProviderTypes.Twilio, twilioConnection);
                                CoreLog.WriteTraceMessage("Successfully initialized the messaging provider.");

                                break;
                            }
                        default:
                            {
                                throw new NotImplementedException("Unexpected provider type specified: " + provider.Type.ToString());
                            }
                    }
                }

                if (MessagingConnections.Count == 0)
                {
                    CoreLog.WriteSystemEvent("No messaging providers are configured. This isn't a problem, but they are nice to have.", EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.NoMessagingProviderConnections, true);
                    return true;
                }
                else
                {
                    CoreLog.WriteSystemEvent("Successfully configured messaging provider connections.", EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.ConfiguredMessagingProviderConnections, true);
                    return true;
                }
            }
            catch (ApplicationCoreSettingMissingException ex)
            {
                CoreLog.WriteSystemEvent("A core application setting has not been configured yet: " + ex.Message,
                    EventLogEntryType.Error, OzetteLibrary.Constants.EventIDs.CoreSettingMissing, true);

                return false;
            }
            catch (ApplicationCoreSettingInvalidValueException ex)
            {
                CoreLog.WriteSystemEvent("A core application setting has an invalid value specified: " + ex.Message,
                    EventLogEntryType.Error, OzetteLibrary.Constants.EventIDs.CoreSettingInvalid, true);

                return false;
            }
            catch (ApplicationSecretMissingException)
            {
                CoreLog.WriteSystemEvent("Failed to configure messaging provider connections: A messaging provider is missing required connection settings.",
                    EventLogEntryType.Error, OzetteLibrary.Constants.EventIDs.FailedToConfigureProvidersMissingSettings, true);

                return false;
            }
            catch (Exception ex)
            {
                var message = "Failed to configure messaging provider connections.";
                var context = CoreLog.GenerateFullContextStackTrace();
                CoreLog.WriteSystemEvent(message, ex, context, OzetteLibrary.Constants.EventIDs.FailedToConfigureMessagingProviderConnections, true);
                return false;
            }
        }

        /// <summary>
        /// Starts the connection engine.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        private bool StartConnectionEngine()
        {
            try
            {
                ConnectionEngineInstance = new ConnectionEngine(ClientDatabase, CoreLog, StorageConnections, MessagingConnections);
                ConnectionEngineInstance.Stopped += Connection_Stopped;
                ConnectionEngineInstance.BeginStart();

                CoreLog.WriteSystemEvent(
                    string.Format("Connection Engine has started."),
                    EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StartedConnectionEngine, true);

                return true;
            }
            catch (Exception ex)
            {
                var message = "Failed to start the connection engine.";
                var context = CoreLog.GenerateFullContextStackTrace();
                CoreLog.WriteSystemEvent(message, ex, context, OzetteLibrary.Constants.EventIDs.FailedConnectionEngine, true);
                return false;
            }
        }

        /// <summary>
        /// Callback event for when connection engine has stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connection_Stopped(object sender, OzetteLibrary.Events.EngineStoppedEventArgs e)
        {
            if (e.Reason == OzetteLibrary.Events.EngineStoppedReason.Failed)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Connection Engine instance {0} has failed.", e.EngineID),
                    e.Exception,
                    CoreLog.GenerateFullContextStackTrace(),
                    OzetteLibrary.Constants.EventIDs.FailedConnectionEngine, true);
            }
            else if (e.Reason == OzetteLibrary.Events.EngineStoppedReason.StopRequested)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Connection Engine instance {0} has stopped.", e.EngineID),
                    EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StoppedConnectionEngine, true);
            }
            else
            {
                throw new InvalidOperationException("Unexpected EngineStoppedReason: " + e.Reason);
            }
        }

        /// <summary>
        /// Starts the status engine.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        private bool StartStatusEngine()
        {
            try
            {
                StatusEngineInstance = new StatusEngine(ClientDatabase, CoreLog, StorageConnections, MessagingConnections, 0);
                StatusEngineInstance.Stopped += Status_Stopped;
                StatusEngineInstance.BeginStart();

                CoreLog.WriteSystemEvent(
                    string.Format("Status Engine has started."),
                    EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StartedStatusEngine, true);

                return true;
            }
            catch (Exception ex)
            {
                var message = "Failed to start the status engine.";
                var context = CoreLog.GenerateFullContextStackTrace();
                CoreLog.WriteSystemEvent(message, ex, context, OzetteLibrary.Constants.EventIDs.FailedStatusEngine, true);
                return false;
            }
        }
        
        /// <summary>
        /// Callback event for when status engine has stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Status_Stopped(object sender, OzetteLibrary.Events.EngineStoppedEventArgs e)
        {
            if (e.Reason == OzetteLibrary.Events.EngineStoppedReason.Failed)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Status Engine instance {0} has failed.", e.EngineID),
                    e.Exception,
                    CoreLog.GenerateFullContextStackTrace(),
                    OzetteLibrary.Constants.EventIDs.FailedStatusEngine, true);
            }
            else if (e.Reason == OzetteLibrary.Events.EngineStoppedReason.StopRequested)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Status Engine instance {0} has stopped.", e.EngineID),
                    EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StoppedStatusEngine, true);
            }
            else
            {
                throw new InvalidOperationException("Unexpected EngineStoppedReason: " + e.Reason);
            }
        }

        /// <summary>
        /// Starts the scanning engine.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        private bool StartScanEngine()
        {
            try
            {
                ScanEngineInstance = new ScanEngine(ClientDatabase, ScanEngineLog, StorageConnections, MessagingConnections, 0);
                ScanEngineInstance.Stopped += Scan_Stopped;
                ScanEngineInstance.BeginStart();

                CoreLog.WriteSystemEvent(
                    string.Format("Scanning Engine has started."),
                    EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StartedScanEngine, true);

                return true;
            }
            catch (Exception ex)
            {
                var message = "Failed to start the scanning engine.";
                var context = CoreLog.GenerateFullContextStackTrace();
                CoreLog.WriteSystemEvent(message, ex, context, OzetteLibrary.Constants.EventIDs.FailedScanEngine, true);
                return false;
            }
        }

        /// <summary>
        /// Callback event for when scanning engine has stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Scan_Stopped(object sender, OzetteLibrary.Events.EngineStoppedEventArgs e)
        {
            if (e.Reason == OzetteLibrary.Events.EngineStoppedReason.Failed)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Scanning Engine instance {0} has failed.", e.EngineID),
                    e.Exception,
                    CoreLog.GenerateFullContextStackTrace(),
                    OzetteLibrary.Constants.EventIDs.FailedScanEngine, true);
            }
            else if (e.Reason == OzetteLibrary.Events.EngineStoppedReason.StopRequested)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Scanning Engine instance {0} has stopped.", e.EngineID),
                    EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StoppedScanEngine, true);
            }
            else
            {
                throw new InvalidOperationException("Unexpected EngineStoppedReason: " + e.Reason);
            }
        }

        /// <summary>
        /// Starts the backup engines.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        private bool StartBackupEngines()
        {
            // each backup engine instance shares the same logger.
            // this means a single log file for all engine instances- and each engine will prepend its log messages with a context tag.

            try
            {
                BackupEngineInstances = new List<BackupEngine>();
                var instanceCount = CoreSettings.BackupEngineInstanceCount;

                for (int i = 0; i < instanceCount; i++)
                {
                    var instance = new BackupEngine(ClientDatabase, BackupEngineLog, StorageConnections, MessagingConnections, i);
                    instance.Stopped += Backup_Stopped;
                    instance.BeginStart();

                    BackupEngineInstances.Add(instance);

                    CoreLog.WriteSystemEvent(
                        string.Format("Backup Engine instance {0} has started.", i),
                        EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StartedBackupEngine, true);
                }

                return true;
            }
            catch (Exception ex)
            {
                var message = "Failed to start the backup engine.";
                var context = CoreLog.GenerateFullContextStackTrace();
                CoreLog.WriteSystemEvent(message, ex, context, OzetteLibrary.Constants.EventIDs.FailedBackupEngine, true);
                return false;
            }
        }

        /// <summary>
        /// Callback event for when the backup engine has stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Backup_Stopped(object sender, OzetteLibrary.Events.EngineStoppedEventArgs e)
        {
            if (e.Reason == OzetteLibrary.Events.EngineStoppedReason.Failed)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Backup Engine instance {0} has failed.", e.EngineID),
                    e.Exception,
                    CoreLog.GenerateFullContextStackTrace(),
                    OzetteLibrary.Constants.EventIDs.FailedBackupEngine, true);
            }
            else if (e.Reason == OzetteLibrary.Events.EngineStoppedReason.StopRequested)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Backup Engine instance {0} has stopped.", e.EngineID),
                    EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StoppedBackupEngine, true);
            }
            else
            {
                throw new InvalidOperationException("Unexpected EngineStoppedReason: " + e.Reason);
            }
        }
    }
}
