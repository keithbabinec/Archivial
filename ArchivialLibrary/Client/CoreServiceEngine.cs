using ArchivialLibrary.Database;
using ArchivialLibrary.Engine;
using ArchivialLibrary.Events;
using ArchivialLibrary.Logging;
using ArchivialLibrary.ServiceCore;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ArchivialLibrary.Client
{
    /// <summary>
    /// Contains functionality for the core windows service, such as core heartbeat and database backups.
    /// </summary>
    public class CoreServiceEngine : BaseEngine
    {
        /// <summary>
        /// Constructor that accepts a database and logger.
        /// </summary>
        /// <param name="database">The client database connection.</param>
        /// <param name="logger">A logging instance.</param>
        /// <param name="instanceID">A parameter to specify the engine instance ID.</param>
        /// <param name="coreSettings">The core settings accessor.</param>
        public CoreServiceEngine(IClientDatabase database,
                            ILogger logger,
                            int instanceID,
                            ICoreSettings coreSettings)
            : base(database, logger, instanceID, coreSettings) { }

        /// <summary>
        /// Begins to start the core service engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStart()
        {
            Thread hb = new Thread(() => CoreServiceHeartbeatAsync().Wait());
            hb.Start();

            Thread dbBackup = new Thread(() => CoreServiceClientDatabaseBackupsAsync().Wait());
            dbBackup.Start();

            Thread fm = new Thread(() => CoreServiceMetaFileManagementAsync().Wait());
            fm.Start();
        }

        /// <summary>
        /// Begins to stop the core service engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStop()
        {
            CancelSource.Cancel();
            Logger.WriteTraceMessage("Core service engine is shutting down by request.", InstanceID);
        }

        /// <summary>
        /// Performs the core service heartbeat.
        /// </summary>
        private async Task CoreServiceHeartbeatAsync()
        {
            try
            {
                while (true)
                {
                    // drop a heartbeat message in the log.

                    Logger.WriteTraceMessage("Archivial core service heartbeat.");

                    // wait 15 minutes or until an engine stop was requested.
                    // then restart the loop.

                    await WaitAsync(TimeSpan.FromMinutes(15)).ConfigureAwait(false);

                    if (CancelSource.Token.IsCancellationRequested)
                    {
                        OnStopped(new EngineStoppedEventArgs(EngineStoppedReason.StopRequested, InstanceID));
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                OnStopped(new EngineStoppedEventArgs(ex, InstanceID));
            }
        }

        /// <summary>
        /// Performs client database backups in a long running thread.
        /// </summary>
        private async Task CoreServiceClientDatabaseBackupsAsync()
        {
            var scheduler = new DatabaseBackupScheduler();

            try
            {
                while (true)
                {
                    try
                    {
                        // pull the timestamps that recents backups have been completed.
                        // ask the schedule which backup type (if any) should be performed based onthe recent history.
                        var recentBackups = await Database.GetClientDatabaseBackupStatusAsync().ConfigureAwait(false);
                        var backupToPerform = scheduler.NextDatabaseBackup(recentBackups);

                        if (backupToPerform != null)
                        {
                            // if a backup type was returned, then we can perform that backup type now.
                            await Database.CreateDatabaseBackupAsync(backupToPerform.Value).ConfigureAwait(false);
                            await Database.SetClientDatabaseBackupCompletedAsync(backupToPerform.Value).ConfigureAwait(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteTraceError("Failed to check for, or perform, a regularly scheduled backup of the client database.", ex, Logger.GenerateFullContextStackTrace());
                    }

                    await WaitAsync(TimeSpan.FromSeconds(60)).ConfigureAwait(false);

                    if (CancelSource.Token.IsCancellationRequested)
                    {
                        OnStopped(new EngineStoppedEventArgs(EngineStoppedReason.StopRequested, InstanceID));
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                OnStopped(new EngineStoppedEventArgs(ex, InstanceID));
            }
        }

        /// <summary>
        /// Performs meta-file management in a long running thread.
        /// </summary>
        private async Task CoreServiceMetaFileManagementAsync()
        {
            try
            {
                while (true)
                {
                    MoveCompletedLogFiles();

                    await DeleteOldDatabaseBackupsAsync().ConfigureAwait(false);

                    await DeleteOldLogFilesAsync().ConfigureAwait(false);

                    await WaitAsync(TimeSpan.FromMinutes(15)).ConfigureAwait(false);

                    if (CancelSource.Token.IsCancellationRequested)
                    {
                        OnStopped(new EngineStoppedEventArgs(EngineStoppedReason.StopRequested, InstanceID));
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                OnStopped(new EngineStoppedEventArgs(ex, InstanceID));
            }
        }

        /// <summary>
        /// The expected log file extension.
        /// </summary>
        private const string LogFileExtension = ".log";

        /// <summary>
        /// The expected log file fragment.
        /// </summary>
        private const string LogFileNameFragment = "Archivial";

        /// <summary>
        /// The expected database backup file extension.
        /// </summary>
        private const string DatabaseBackupFileExtension = ".bak";

        /// <summary>
        /// Moves completed log files to the archive directory.
        /// </summary>
        private void MoveCompletedLogFiles()
        {
            try
            {
                var dirInfo = new DirectoryInfo(CoreSettings.GetLogFilesDirectory());
                var files = dirInfo.EnumerateFiles();
                var oldFileDate = DateTime.Now.AddHours(-24);

                foreach (var file in files)
                {
                    if (file.Name.StartsWith(LogFileNameFragment) && file.Extension == LogFileExtension && file.LastWriteTime < oldFileDate)
                    {
                        var newFileName = Path.Combine(CoreSettings.GetLogFilesArchiveDirectory(), file.Name);
                        Logger.WriteTraceMessage(string.Format("Moving completed log file {0} to the archive folder.", file.Name), InstanceID);
                        file.MoveTo(newFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteTraceError("Failed to move completed log files to the log archive folder.", ex, Logger.GenerateFullContextStackTrace(), InstanceID);
            }
        }

        /// <summary>
        /// Deletes old database backup files outside of the retention period.
        /// </summary>
        private async Task DeleteOldDatabaseBackupsAsync()
        {
            try
            {
                var settingName = Constants.RuntimeSettingNames.DatabaseBackupsRetentionInDays;
                var backupsRetentionInDays = Convert.ToInt32(await Database.GetApplicationOptionAsync(settingName).ConfigureAwait(false));

                var dirInfo = new DirectoryInfo(CoreSettings.GetDatabaseBackupsDirectory());
                var files = dirInfo.EnumerateFiles();
                var oldFileDate = DateTime.Now.AddDays(-backupsRetentionInDays);

                foreach (var file in files)
                {
                    if (file.Name.StartsWith(Constants.Database.DatabaseName) && file.Extension == DatabaseBackupFileExtension && file.LastWriteTime < oldFileDate)
                    {
                        Logger.WriteTraceMessage(string.Format("Removing database backup file [{0}] that has exceeded the retention period.", file.Name), InstanceID);
                        file.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteTraceError("Failed to delete old database backup files from the database backups folder.", ex, Logger.GenerateFullContextStackTrace(), InstanceID);
            }
        }

        /// <summary>
        /// Deletes old log files outside of the retention period.
        /// </summary>
        private async Task DeleteOldLogFilesAsync()
        {
            try
            {
                var settingName = Constants.RuntimeSettingNames.LogFilesRetentionInDays;
                var logFilesRetentionInDays = Convert.ToInt32(await Database.GetApplicationOptionAsync(settingName).ConfigureAwait(false));

                var dirInfo = new DirectoryInfo(CoreSettings.GetLogFilesArchiveDirectory());
                var files = dirInfo.EnumerateFiles();
                var oldFileDate = DateTime.Now.AddDays(-logFilesRetentionInDays);

                foreach (var file in files)
                {
                    if (file.Name.StartsWith(LogFileNameFragment) && file.Extension == LogFileExtension && file.LastWriteTime < oldFileDate)
                    {
                        var newFileName = Path.Combine(CoreSettings.GetLogFilesArchiveDirectory(), file.Name);
                        Logger.WriteTraceMessage(string.Format("Removing log file [{0}] that has exceeded the retention period.", file.Name), InstanceID);
                        file.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteTraceError("Failed to delete old log files from the log archive folder.", ex, Logger.GenerateFullContextStackTrace(), InstanceID);
            }
        }
    }
}
