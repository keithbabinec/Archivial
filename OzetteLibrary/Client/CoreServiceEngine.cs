using OzetteLibrary.Database;
using OzetteLibrary.Engine;
using OzetteLibrary.Events;
using OzetteLibrary.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OzetteLibrary.Client
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
        public CoreServiceEngine(IClientDatabase database,
                            ILogger logger,
                            int instanceID)
            : base(database, logger, instanceID) { }

        /// <summary>
        /// Begins to start the core service engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStart()
        {
            Thread hb = new Thread(() => CoreServiceHeartbeat());
            hb.Start();

            Thread dbBackup = new Thread(() => CoreServiceClientDatabaseBackupsAsync().Wait());
            dbBackup.Start();
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
        private void CoreServiceHeartbeat()
        {
            try
            {
                while (true)
                {
                    // drop a heartbeat message in the log.

                    Logger.WriteTraceMessage("Ozette core service heartbeat.");

                    // wait 15 minutes or until an engine stop was requested.
                    // then restart the loop.

                    ThreadSleepWithStopRequestCheck(TimeSpan.FromMinutes(15));

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
                        var recentBackups = await Database.GetClientDatabaseBackupStatusAsync();
                        var backupToPerform = scheduler.NextDatabaseBackup(recentBackups);

                        if (backupToPerform != null)
                        {
                            // if a backup type was returned, then we can perform that backup type now.
                            await Database.CreateDatabaseBackupAsync(backupToPerform.Value);
                            await Database.SetClientDatabaseBackupCompletedAsync(backupToPerform.Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteTraceError("Failed to check for, or perform, a regularly scheduled backup of the client database.", ex, Logger.GenerateFullContextStackTrace());
                    }

                    ThreadSleepWithStopRequestCheck(TimeSpan.FromSeconds(60));

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
    }
}
