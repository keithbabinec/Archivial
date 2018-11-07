using OzetteLibrary.Client;
using OzetteLibrary.Database.LiteDB;
using OzetteLibrary.Exceptions;
using OzetteLibrary.Logging;
using OzetteLibrary.Logging.Default;
using OzetteLibrary.Providers;
using OzetteLibrary.Providers.Azure;
using OzetteLibrary.Secrets;
using OzetteLibrary.ServiceCore;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Threading;

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
            Thread t = new Thread(() => CoreStart());
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
        /// A reference to the scanning engine instance.
        /// </summary>
        private ScanEngine Scan { get; set; }

        /// <summary>
        /// A reference to the backup engine instance.
        /// </summary>
        private BackupEngine Backup { get; set; }

        /// <summary>
        /// A reference to the backup provider connections.
        /// </summary>
        private ProviderConnectionsCollection ProviderConnections { get; set; }

        /// <summary>
        /// Core application start.
        /// </summary>
        private void CoreStart()
        {
            // in the client agent the core loop consists of two pieces.
            // first is the scan engine, and the second is the backup engine.
            // each one lives under it's own long-running thread and class.
            // prepare the database and then start both engines.

            StartLoggers();

            CoreLog.WriteSystemEvent(
                string.Format("Starting {0} client service.", OzetteLibrary.Constants.Logging.AppName),
                EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StartingService, true);

            if (!ConfigureProviderConnections())
            {
                Stop();
                return;
            }

            if (!StartScanEngine())
            {
                Stop();
                return;
            }

            if (!StartBackupEngine())
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

            if (Scan != null)
            {
                Scan.BeginStop();
            }
            if (Backup != null)
            {
                Backup.BeginStop();
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
        /// Configures the cloud storage provider connections.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        private bool ConfigureProviderConnections()
        {
            CoreLog.WriteSystemEvent("Configuring cloud storage provider connections.", EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.ConfiguringCloudProviderConnections, true);

            ProviderConnections = new ProviderConnectionsCollection();

            try
            {
                // establish the database and protected store.

                var db = new LiteDBClientDatabase(CoreSettings.DatabaseConnectionString);
                db.PrepareDatabase();

                var ivEncodedString = CoreSettings.ProtectionIv;
                var ivBytes = Convert.FromBase64String(ivEncodedString);

                ProtectedDataStore protectedStore = new ProtectedDataStore(db, DataProtectionScope.LocalMachine, ivBytes);

                // configure the provider implementation instances.
                // add each to the collection of providers.

                var providersList = db.GetProvidersList();

                foreach (var provider in providersList)
                {
                    CoreLog.WriteTraceMessage(
                        string.Format("A cloud provider was found in the configuration database: Name: {0}, Enabled: {1}, ID: {2}",
                            provider.Type.ToString(), provider.Enabled.ToString(), provider.ID));

                    if (provider.Enabled)
                    {
                        switch (provider.Type)
                        {
                            case ProviderTypes.Azure:
                                {
                                    CoreLog.WriteTraceMessage("Checking for Azure cloud storage provider connection settings.");
                                    string storageAccountName = protectedStore.GetApplicationSecret(OzetteLibrary.Constants.RuntimeSettingNames.AzureStorageAccountName);
                                    string storageAccountToken = protectedStore.GetApplicationSecret(OzetteLibrary.Constants.RuntimeSettingNames.AzureStorageAccountToken);

                                    CoreLog.WriteTraceMessage("Initializing Azure cloud storage provider.");
                                    AzureProviderFileOperations azureConnection = new AzureProviderFileOperations(BackupEngineLog, storageAccountName, storageAccountToken);
                                    ProviderConnections.Add(ProviderTypes.Azure, azureConnection);
                                    CoreLog.WriteTraceMessage("Successfully initialized the cloud storage provider.");

                                    break;
                                }
                            default:
                                {
                                    throw new NotImplementedException("Unexpected provider type specified: " + provider.Type.ToString());
                                }
                        }
                    }
                }

                if (ProviderConnections.Count == 0)
                {
                    CoreLog.WriteSystemEvent("Failed to configure cloud storage provider connections: No providers were listed in the database, or providers are not enabled.",
                        EventLogEntryType.Error, OzetteLibrary.Constants.EventIDs.FailedToConfigureProvidersNoFoundOrEnabled, true);

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
                CoreLog.WriteSystemEvent(message, ex, context, OzetteLibrary.Constants.EventIDs.FailedToConfigureCloudProviderConnections, true);
                return false;
            }
        }

        /// <summary>
        /// Starts the scanning engine.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        private bool StartScanEngine()
        {
            // note: each engine can get it's own instance of the LiteDBClientDatabase wrapper.
            // LiteDB is thread safe, but the wrapper is not; so give threads their own DB wrappers.

            try
            {
                var db = new LiteDBClientDatabase(CoreSettings.DatabaseConnectionString);
                db.PrepareDatabase();

                Scan = new ScanEngine(db, ScanEngineLog, ProviderConnections);
                Scan.Stopped += Scan_Stopped;
                Scan.BeginStart();

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
                    string.Format("Scanning Engine has failed."),
                    e.Exception,
                    CoreLog.GenerateFullContextStackTrace(),
                    OzetteLibrary.Constants.EventIDs.FailedScanEngine, true);
            }
            else if (e.Reason == OzetteLibrary.Events.EngineStoppedReason.StopRequested)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Scanning Engine has stopped."),
                    EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StoppedScanEngine, true);
            }
            else
            {
                throw new InvalidOperationException("Unexpected EngineStoppedReason: " + e.Reason);
            }
        }

        /// <summary>
        /// Starts the backup engine.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        private bool StartBackupEngine()
        {
            // note: each engine can get it's own instance of the LiteDBClientDatabase wrapper.
            // LiteDB is thread safe, but the wrapper is not; so give threads their own DB wrappers.

            try
            {
                var db = new LiteDBClientDatabase(CoreSettings.DatabaseConnectionString);
                db.PrepareDatabase();

                Backup = new BackupEngine(db, BackupEngineLog, ProviderConnections);
                Backup.Stopped += Backup_Stopped;
                Backup.BeginStart();

                CoreLog.WriteSystemEvent(
                    string.Format("Backup Engine has started."),
                    EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StartedBackupEngine, true);

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
                    string.Format("Backup Engine has failed."),
                    e.Exception,
                    CoreLog.GenerateFullContextStackTrace(),
                    OzetteLibrary.Constants.EventIDs.FailedBackupEngine, true);
            }
            else if (e.Reason == OzetteLibrary.Events.EngineStoppedReason.StopRequested)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Backup Engine has stopped."),
                    EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StoppedBackupEngine, true);
            }
            else
            {
                throw new InvalidOperationException("Unexpected EngineStoppedReason: " + e.Reason);
            }
        }
    }
}
