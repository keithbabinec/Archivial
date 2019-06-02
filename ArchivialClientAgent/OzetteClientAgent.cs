using ArchivialLibrary.Client;
using ArchivialLibrary.Logging;
using ArchivialLibrary.Logging.Default;
using ArchivialLibrary.ServiceCore;
using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Collections.Generic;
using ArchivialLibrary.Database.SQLServer;
using System.Threading.Tasks;
using ArchivialLibrary.Database;
using System.Reflection;

namespace ArchivialClientAgent
{
    /// <summary>
    /// Contains service functionality.
    /// </summary>
    public partial class ArchivialClientAgent : ServiceBase
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ArchivialClientAgent()
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
        private ILogger ScanEngineLog { get; set; }

        /// <summary>
        /// A reference to the client database instance.
        /// </summary>
        private IClientDatabase ClientDatabase { get; set; }

        /// <summary>
        /// A reference to the core service engine instance.
        /// </summary>
        private CoreServiceEngine CoreServiceEngineInstance { get; set; }

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
        /// A reference to the cleanup engine instances.
        /// </summary>
        private List<CleanupEngine> CleanupEngineInstances { get; set; }

        /// <summary>
        /// Core application start.
        /// </summary>
        private void CoreStart()
        {
            StartLoggers();

            CoreLog.WriteSystemEvent(
                string.Format("Starting {0} client service version {1}.", ArchivialLibrary.Constants.Logging.AppName, Assembly.GetExecutingAssembly().GetName().Version.ToString()),
                EventLogEntryType.Information, ArchivialLibrary.Constants.EventIDs.StartingService, true);

            var dbTask = ConfigureDatabaseAsync();
            dbTask.Wait();
            
            if (!dbTask.Result)
            {
                Stop();
                return;
            }

            if (!StartCoreServiceEngine())
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

            var beTask = StartBackupEnginesAsync();
            beTask.Wait();

            if (!beTask.Result)
            {
                Stop();
                return;
            }

            var cleTask = StartCleanupEnginesAsync();
            cleTask.Wait();

            if (!cleTask.Result)
            {
                Stop();
                return;
            }

            CoreLog.WriteSystemEvent(
                string.Format("Successfully started {0} client service.", ArchivialLibrary.Constants.Logging.AppName),
                EventLogEntryType.Information, ArchivialLibrary.Constants.EventIDs.StartedService, true);
        }

        /// <summary>
        /// Runs when the service stop is triggered.
        /// </summary>
        protected override void OnStop()
        {
            if (CoreLog != null)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Stopping {0} client service.", ArchivialLibrary.Constants.Logging.AppName),
                    EventLogEntryType.Information, ArchivialLibrary.Constants.EventIDs.StoppingService, true);
            }

            if (CoreServiceEngineInstance != null)
            {
                CoreServiceEngineInstance.BeginStop();
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
            if (CleanupEngineInstances != null)
            {
                foreach (var instance in CleanupEngineInstances)
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
                    string.Format("Successfully stopped {0} client service.", ArchivialLibrary.Constants.Logging.AppName),
                    EventLogEntryType.Information, ArchivialLibrary.Constants.EventIDs.StoppedService, true);
            }
        }

        /// <summary>
        /// Starts the shared logging instances.
        /// </summary>
        private void StartLoggers()
        {
            CoreLog = new Logger(ArchivialLibrary.Constants.Logging.CoreServiceComponentName);
            CoreLog.Start(
                CoreSettings.EventlogName,
                CoreSettings.EventlogName,
                CoreSettings.LogFilesDirectory);

            ScanEngineLog = new Logger(ArchivialLibrary.Constants.Logging.ScanningComponentName);
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
                ClientDatabase = new SQLServerClientDatabase(CoreSettings.DatabaseConnectionString, CoreLog);
                await ClientDatabase.PrepareDatabaseAsync().ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                var message = "Failed to configure client database.";
                var context = CoreLog.GenerateFullContextStackTrace();
                CoreLog.WriteSystemEvent(message, ex, context, ArchivialLibrary.Constants.EventIDs.FailedToPrepareClientDatabase, true);
                return false;
            }
        }

        /// <summary>
        /// Starts the core service engine.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        private bool StartCoreServiceEngine()
        {
            try
            {
                CoreServiceEngineInstance = new CoreServiceEngine(ClientDatabase, CoreLog, 0);
                CoreServiceEngineInstance.Stopped += Connection_Stopped;
                CoreServiceEngineInstance.BeginStart();

                CoreLog.WriteSystemEvent(
                    string.Format("Core Service Engine has started."),
                    EventLogEntryType.Information, ArchivialLibrary.Constants.EventIDs.StartedCoreServiceEngine, true);

                return true;
            }
            catch (Exception ex)
            {
                var message = "Failed to start the core service engine.";
                var context = CoreLog.GenerateFullContextStackTrace();
                CoreLog.WriteSystemEvent(message, ex, context, ArchivialLibrary.Constants.EventIDs.FailedCoreServiceEngine, true);
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
                ConnectionEngineInstance = new ConnectionEngine(ClientDatabase, CoreLog);
                ConnectionEngineInstance.Stopped += Connection_Stopped;
                ConnectionEngineInstance.BeginStart();

                CoreLog.WriteSystemEvent(
                    string.Format("Connection Engine has started."),
                    EventLogEntryType.Information, ArchivialLibrary.Constants.EventIDs.StartedConnectionEngine, true);

                return true;
            }
            catch (Exception ex)
            {
                var message = "Failed to start the connection engine.";
                var context = CoreLog.GenerateFullContextStackTrace();
                CoreLog.WriteSystemEvent(message, ex, context, ArchivialLibrary.Constants.EventIDs.FailedConnectionEngine, true);
                return false;
            }
        }

        /// <summary>
        /// Callback event for when connection engine has stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connection_Stopped(object sender, ArchivialLibrary.Events.EngineStoppedEventArgs e)
        {
            if (e.Reason == ArchivialLibrary.Events.EngineStoppedReason.Failed)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Connection Engine instance {0} has failed.", e.EngineID),
                    e.Exception,
                    CoreLog.GenerateFullContextStackTrace(),
                    ArchivialLibrary.Constants.EventIDs.FailedConnectionEngine, true);
            }
            else if (e.Reason == ArchivialLibrary.Events.EngineStoppedReason.StopRequested)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Connection Engine instance {0} has stopped.", e.EngineID),
                    EventLogEntryType.Information, ArchivialLibrary.Constants.EventIDs.StoppedConnectionEngine, true);
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
                StatusEngineInstance = new StatusEngine(ClientDatabase, CoreLog, 0);
                StatusEngineInstance.Stopped += Status_Stopped;
                StatusEngineInstance.BeginStart();

                CoreLog.WriteSystemEvent(
                    string.Format("Status Engine has started."),
                    EventLogEntryType.Information, ArchivialLibrary.Constants.EventIDs.StartedStatusEngine, true);

                return true;
            }
            catch (Exception ex)
            {
                var message = "Failed to start the status engine.";
                var context = CoreLog.GenerateFullContextStackTrace();
                CoreLog.WriteSystemEvent(message, ex, context, ArchivialLibrary.Constants.EventIDs.FailedStatusEngine, true);
                return false;
            }
        }
        
        /// <summary>
        /// Callback event for when status engine has stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Status_Stopped(object sender, ArchivialLibrary.Events.EngineStoppedEventArgs e)
        {
            if (e.Reason == ArchivialLibrary.Events.EngineStoppedReason.Failed)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Status Engine instance {0} has failed.", e.EngineID),
                    e.Exception,
                    CoreLog.GenerateFullContextStackTrace(),
                    ArchivialLibrary.Constants.EventIDs.FailedStatusEngine, true);
            }
            else if (e.Reason == ArchivialLibrary.Events.EngineStoppedReason.StopRequested)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Status Engine instance {0} has stopped.", e.EngineID),
                    EventLogEntryType.Information, ArchivialLibrary.Constants.EventIDs.StoppedStatusEngine, true);
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
                ScanEngineInstance = new ScanEngine(ClientDatabase, ScanEngineLog, 0);
                ScanEngineInstance.Stopped += Scan_Stopped;
                ScanEngineInstance.BeginStart();

                CoreLog.WriteSystemEvent(
                    string.Format("Scanning Engine has started."),
                    EventLogEntryType.Information, ArchivialLibrary.Constants.EventIDs.StartedScanEngine, true);

                return true;
            }
            catch (Exception ex)
            {
                var message = "Failed to start the scanning engine.";
                var context = CoreLog.GenerateFullContextStackTrace();
                CoreLog.WriteSystemEvent(message, ex, context, ArchivialLibrary.Constants.EventIDs.FailedScanEngine, true);
                return false;
            }
        }

        /// <summary>
        /// Callback event for when scanning engine has stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Scan_Stopped(object sender, ArchivialLibrary.Events.EngineStoppedEventArgs e)
        {
            if (e.Reason == ArchivialLibrary.Events.EngineStoppedReason.Failed)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Scanning Engine instance {0} has failed.", e.EngineID),
                    e.Exception,
                    CoreLog.GenerateFullContextStackTrace(),
                    ArchivialLibrary.Constants.EventIDs.FailedScanEngine, true);
            }
            else if (e.Reason == ArchivialLibrary.Events.EngineStoppedReason.StopRequested)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Scanning Engine instance {0} has stopped.", e.EngineID),
                    EventLogEntryType.Information, ArchivialLibrary.Constants.EventIDs.StoppedScanEngine, true);
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
        private async Task<bool> StartBackupEnginesAsync()
        {
            // each backup engine instance shares the same logger.
            // this means a single log file for all engine instances- and each engine will prepend its log messages with a context tag.

            try
            {
                BackupEngineInstances = new List<BackupEngine>();

                var instanceCountSettingName = ArchivialLibrary.Constants.RuntimeSettingNames.BackupEngineInstancesCount;
                var instanceCount = Convert.ToInt32(await ClientDatabase.GetApplicationOptionAsync(instanceCountSettingName).ConfigureAwait(false));

                var startupDelaySettingName = ArchivialLibrary.Constants.RuntimeSettingNames.BackupEngineStartupDelayInSeconds;
                var startupDelaySeconds = Convert.ToInt32(await ClientDatabase.GetApplicationOptionAsync(startupDelaySettingName).ConfigureAwait(false));

                for (int i = 0; i < instanceCount; i++)
                {
                    CoreLog.WriteSystemEvent(
                        string.Format("Waiting {0} seconds between Backup Engine starts to reduce sudden filesystem load.", startupDelaySeconds),
                        EventLogEntryType.Information, ArchivialLibrary.Constants.EventIDs.BackupEngineWaitingForNextStart, true);

                    await Task.Delay(TimeSpan.FromSeconds(startupDelaySeconds)).ConfigureAwait(false);

                    var engineLog = new Logger(string.Format("{0}-{1}", ArchivialLibrary.Constants.Logging.BackupComponentName, i));
                    engineLog.Start(
                        CoreSettings.EventlogName,
                        CoreSettings.EventlogName,
                        CoreSettings.LogFilesDirectory);

                    var instance = new BackupEngine(ClientDatabase, engineLog, i);
                    instance.Stopped += Backup_Stopped;
                    instance.BeginStart();

                    BackupEngineInstances.Add(instance);

                    CoreLog.WriteSystemEvent(
                        string.Format("Backup Engine instance {0} has started.", i),
                        EventLogEntryType.Information, ArchivialLibrary.Constants.EventIDs.StartedBackupEngine, true);
                }

                return true;
            }
            catch (Exception ex)
            {
                var message = "Failed to start the backup engine.";
                var context = CoreLog.GenerateFullContextStackTrace();
                CoreLog.WriteSystemEvent(message, ex, context, ArchivialLibrary.Constants.EventIDs.FailedBackupEngine, true);
                return false;
            }
        }

        /// <summary>
        /// Callback event for when the backup engine has stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Backup_Stopped(object sender, ArchivialLibrary.Events.EngineStoppedEventArgs e)
        {
            if (e.Reason == ArchivialLibrary.Events.EngineStoppedReason.Failed)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Backup Engine instance {0} has failed.", e.EngineID),
                    e.Exception,
                    CoreLog.GenerateFullContextStackTrace(),
                    ArchivialLibrary.Constants.EventIDs.FailedBackupEngine, true);
            }
            else if (e.Reason == ArchivialLibrary.Events.EngineStoppedReason.StopRequested)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Backup Engine instance {0} has stopped.", e.EngineID),
                    EventLogEntryType.Information, ArchivialLibrary.Constants.EventIDs.StoppedBackupEngine, true);
            }
            else
            {
                throw new InvalidOperationException("Unexpected EngineStoppedReason: " + e.Reason);
            }
        }

        /// <summary>
        /// Starts the cleanup engine(s).
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        private async Task<bool> StartCleanupEnginesAsync()
        {
            // each cleanup engine instance shares the same logger.
            // this means a single log file for all engine instances- and each engine will prepend its log messages with a context tag.

            try
            {
                CleanupEngineInstances = new List<CleanupEngine>();

                var settingName = ArchivialLibrary.Constants.RuntimeSettingNames.CleanupEngineInstancesCount;
                var instanceCount = Convert.ToInt32(await ClientDatabase.GetApplicationOptionAsync(settingName).ConfigureAwait(false));

                for (int i = 0; i < instanceCount; i++)
                {
                    var engineLog = new Logger(string.Format("{0}-{1}", ArchivialLibrary.Constants.Logging.CleanupComponentName, i));
                    engineLog.Start(
                        CoreSettings.EventlogName,
                        CoreSettings.EventlogName,
                        CoreSettings.LogFilesDirectory);

                    var instance = new CleanupEngine(ClientDatabase, engineLog, i);
                    instance.Stopped += Cleanup_Stopped;
                    instance.BeginStart();

                    CleanupEngineInstances.Add(instance);

                    CoreLog.WriteSystemEvent(
                        string.Format("Cleanup Engine instance {0} has started.", i),
                        EventLogEntryType.Information, ArchivialLibrary.Constants.EventIDs.StartedCleanupEngine, true);
                }

                return true;
            }
            catch (Exception ex)
            {
                var message = "Failed to start the cleanup engine(s).";
                var context = CoreLog.GenerateFullContextStackTrace();
                CoreLog.WriteSystemEvent(message, ex, context, ArchivialLibrary.Constants.EventIDs.FailedCleanupEngine, true);
                return false;
            }
        }

        /// <summary>
        /// Callback event for when the cleanup engine has stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cleanup_Stopped(object sender, ArchivialLibrary.Events.EngineStoppedEventArgs e)
        {
            if (e.Reason == ArchivialLibrary.Events.EngineStoppedReason.Failed)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Cleanup Engine instance {0} has failed.", e.EngineID),
                    e.Exception,
                    CoreLog.GenerateFullContextStackTrace(),
                    ArchivialLibrary.Constants.EventIDs.FailedCleanupEngine, true);
            }
            else if (e.Reason == ArchivialLibrary.Events.EngineStoppedReason.StopRequested)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Cleanup Engine instance {0} has stopped.", e.EngineID),
                    EventLogEntryType.Information, ArchivialLibrary.Constants.EventIDs.StoppedCleanupEngine, true);
            }
            else
            {
                throw new InvalidOperationException("Unexpected EngineStoppedReason: " + e.Reason);
            }
        }
    }
}
