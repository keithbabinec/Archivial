using OzetteLibrary.Client;
using OzetteLibrary.Database.LiteDB;
using OzetteLibrary.Logging;
using OzetteLibrary.Logging.Default;
using OzetteLibrary.Providers;
using OzetteLibrary.Providers.Azure;
using OzetteLibrary.Secrets;
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

            CoreLog = new Logger(OzetteLibrary.Constants.Logging.CoreServiceComponentName);

            CoreLog.Start(
                Properties.Settings.Default.EventlogName, 
                Properties.Settings.Default.EventlogName, 
                Properties.Settings.Default.LogFilesDirectory);

            CoreLog.WriteSystemEvent(
                string.Format("Starting {0} client service.", OzetteLibrary.Constants.Logging.AppName), 
                EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StartingService);

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
                EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StartedService);
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
                    EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StoppingService);
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
                    EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StoppedService);
            }
        }

        /// <summary>
        /// Configures the cloud storage provider connections.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        private bool ConfigureProviderConnections()
        {
            var startingMessage = "Configuring cloud storage provider connections.";
            CoreLog.WriteTraceMessage(startingMessage);
            CoreLog.WriteSystemEvent(startingMessage, EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.ConfiguringCloudProviderConnections);

            ProviderConnections = new ProviderConnectionsCollection();

            try
            {
                // establish the database and protected store.

                var db = new LiteDBClientDatabase(Properties.Settings.Default.DatabaseConnectionString, CoreLog);
                db.PrepareDatabase();

                var ivEncodedString = Environment.GetEnvironmentVariable(Properties.Settings.Default.ProtectionIVSecretName);

                if (string.IsNullOrWhiteSpace(ivEncodedString))
                {
                    throw new Exception("Protection IV secret not found in environment variables.");
                }

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
                                    string storageAccountName = protectedStore.GetApplicationSecret(OzetteLibrary.Constants.OptionIDs.AzureStorageAccountName);
                                    string storageAccountToken = protectedStore.GetApplicationSecret(OzetteLibrary.Constants.OptionIDs.AzureStorageAccountToken);

                                    CoreLog.WriteTraceMessage("Initializing Azure cloud storage provider.");
                                    AzureProviderFileOperations azureConnection = new AzureProviderFileOperations(CoreLog, storageAccountName, storageAccountToken);
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
                    throw new Exception("No cloud storage providers are listed in the database, or no cloud storage providers are currently enabled.");
                }

                var completedMessage = "Successfully configured cloud storage provider connections.";
                CoreLog.WriteTraceMessage(completedMessage);
                CoreLog.WriteSystemEvent(completedMessage, EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.ConfiguredCloudProviderConnections);
                return true;
            }
            catch (Exception ex)
            {
                var message = "Failed to configure cloud storage provider connections.";
                var context = CoreLog.GenerateFullContextStackTrace();
                CoreLog.WriteTraceError(message, ex, context);
                CoreLog.WriteSystemEvent(message, ex, context, OzetteLibrary.Constants.EventIDs.FailedToConfigureCloudProviderConnections);
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
                var log = new Logger(OzetteLibrary.Constants.Logging.ScanningComponentName);

                log.Start(
                    Properties.Settings.Default.EventlogName,
                    Properties.Settings.Default.EventlogName,
                    Properties.Settings.Default.LogFilesDirectory);

                var db = new LiteDBClientDatabase(Properties.Settings.Default.DatabaseConnectionString, log);
                db.PrepareDatabase();

                Scan = new ScanEngine(db, log, ProviderConnections);
                Scan.Stopped += Scan_Stopped;
                Scan.BeginStart();

                CoreLog.WriteTraceMessage("Scanning Engine has started.");

                CoreLog.WriteSystemEvent(
                    string.Format("Scanning Engine has started."),
                    EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StartedScanEngine);

                return true;
            }
            catch (Exception ex)
            {
                var message = "Failed to start the scanning engine.";
                var context = CoreLog.GenerateFullContextStackTrace();
                CoreLog.WriteTraceError(message, ex, context);
                CoreLog.WriteSystemEvent(message, ex, context, OzetteLibrary.Constants.EventIDs.FailedScanEngine);
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
                CoreLog.WriteTraceError("Scanning Engine has failed.", e.Exception, CoreLog.GenerateFullContextStackTrace());

                CoreLog.WriteSystemEvent(
                    string.Format("Scanning Engine has failed."),
                    e.Exception,
                    CoreLog.GenerateFullContextStackTrace(),
                    OzetteLibrary.Constants.EventIDs.FailedScanEngine);
            }
            else if (e.Reason == OzetteLibrary.Events.EngineStoppedReason.StopRequested)
            {
                CoreLog.WriteTraceMessage("Scanning Engine has stopped.");

                CoreLog.WriteSystemEvent(
                    string.Format("Scanning Engine has stopped."),
                    EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StoppedScanEngine);
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
                var log = new Logger(OzetteLibrary.Constants.Logging.BackupComponentName);

                log.Start(
                    Properties.Settings.Default.EventlogName,
                    Properties.Settings.Default.EventlogName,
                    Properties.Settings.Default.LogFilesDirectory);

                var db = new LiteDBClientDatabase(Properties.Settings.Default.DatabaseConnectionString, log);
                db.PrepareDatabase();

                Backup = new BackupEngine(db, log, ProviderConnections);
                Backup.Stopped += Backup_Stopped;
                Backup.BeginStart();

                CoreLog.WriteTraceMessage("Backup Engine has started.");

                CoreLog.WriteSystemEvent(
                    string.Format("Backup Engine has started."),
                    EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StartedBackupEngine);

                return true;
            }
            catch (Exception ex)
            {
                var message = "Failed to start the backup engine.";
                var context = CoreLog.GenerateFullContextStackTrace();
                CoreLog.WriteTraceError(message, ex, context);
                CoreLog.WriteSystemEvent(message, ex, context, OzetteLibrary.Constants.EventIDs.FailedBackupEngine);
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
                CoreLog.WriteTraceError("Backup Engine has failed.", e.Exception, CoreLog.GenerateFullContextStackTrace());

                CoreLog.WriteSystemEvent(
                    string.Format("Backup Engine has failed."),
                    e.Exception,
                    CoreLog.GenerateFullContextStackTrace(),
                    OzetteLibrary.Constants.EventIDs.FailedBackupEngine);
            }
            else if (e.Reason == OzetteLibrary.Events.EngineStoppedReason.StopRequested)
            {
                CoreLog.WriteTraceMessage("Backup Engine has stopped.");

                CoreLog.WriteSystemEvent(
                    string.Format("Backup Engine has stopped."),
                    EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StoppedBackupEngine);
            }
            else
            {
                throw new InvalidOperationException("Unexpected EngineStoppedReason: " + e.Reason);
            }
        }
    }
}
