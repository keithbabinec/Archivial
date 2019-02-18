using OzetteLibrary.Client.Transfer;
using OzetteLibrary.Database;
using OzetteLibrary.Engine;
using OzetteLibrary.Events;
using OzetteLibrary.Files;
using OzetteLibrary.Logging;
using OzetteLibrary.Providers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OzetteLibrary.Client
{
    /// <summary>
    /// Contains core backup engine functionality.
    /// </summary>
    public class BackupEngine : BaseEngine
    {
        /// <summary>
        /// Constructor that accepts a database and logger.
        /// </summary>
        /// <param name="database">The client database connection.</param>
        /// <param name="logger">A logging instance.</param>
        /// <param name="instanceID">A parameter to specify the engine instance ID.</param>
        public BackupEngine(IClientDatabase database,
                            ILogger logger,
                            int instanceID)
            : base(database, logger, instanceID) { }

        /// <summary>
        /// Begins to start the backup engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStart()
        {
            Logger.WriteTraceMessage(string.Format("Backup engine is starting up."), InstanceID);

            Thread pl = new Thread(() => ProcessLoopAsync().Wait());
            pl.Start();

            Logger.WriteTraceMessage(string.Format("Backup engine is now running."), InstanceID);
        }

        /// <summary>
        /// Begins to stop the backup engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStop()
        {
            CancelSource.Cancel();
            Logger.WriteTraceMessage("Backup engine is shutting down by request.", InstanceID);
        }

        /// <summary>
        /// The file copy/transfer utility.
        /// </summary>
        private FileSender Sender { get; set; }

        /// <summary>
        /// The last time a heartbeat message or file backup was completed.
        /// </summary>
        private DateTime? LastHeartbeatOrBackupCompleted { get; set; }

        /// <summary>
        /// Core processing loop.
        /// </summary>
        private async Task ProcessLoopAsync()
        {
            try
            {
                while (true)
                {
                    // always ensure we have providers configured.

                    if (await StorageProvidersAreConfiguredAsync())
                    {
                        // check to see if we have any files to backup.
                        // return the next one to backup.

                        BackupFile nextFileToBackup = await SafeGetNextFileToBackupAsync().ConfigureAwait(false);

                        if (nextFileToBackup != null)
                        {
                            // initiate the file-send operation.

                            var transferFinished = false;
                            var transferTask = Sender.TransferAsync(nextFileToBackup, CancelSource.Token);

                            while (!transferFinished)
                            {
                                ThreadSleepWithStopRequestCheck(TimeSpan.FromSeconds(2));

                                switch (transferTask.Status)
                                {
                                    case TaskStatus.RanToCompletion:
                                    case TaskStatus.Faulted:
                                    case TaskStatus.Canceled:
                                        {
                                            // task has completed, failed, or canceled.
                                            // quit the status loop.
                                            transferFinished = true;
                                            LastHeartbeatOrBackupCompleted = DateTime.Now;
                                            break;
                                        }
                                    default:
                                        {
                                            // task is still starting or running.
                                            // do nothing here.
                                            break;
                                        }
                                }

                                if (CancelSource.Token.IsCancellationRequested)
                                {
                                    // stop was requested.
                                    // stop the currently in-progress file send operation.
                                    break;
                                }
                            }

                            // do not sleep here.
                            // immediately move to backing up the next file.
                        }
                        else
                        {
                            ThreadSleepWithStopRequestCheck(TimeSpan.FromSeconds(60));

                            if (LastHeartbeatOrBackupCompleted.HasValue == false || LastHeartbeatOrBackupCompleted.Value < DateTime.Now.Add(TimeSpan.FromMinutes(-1)))
                            {
                                LastHeartbeatOrBackupCompleted = DateTime.Now;
                                Logger.WriteTraceMessage("Backup engine heartbeat: no recent activity.", InstanceID);
                            }
                        }
                    }
                    else
                    {
                        ThreadSleepWithStopRequestCheck(TimeSpan.FromSeconds(60));

                        if (LastHeartbeatOrBackupCompleted.HasValue == false || LastHeartbeatOrBackupCompleted.Value < DateTime.Now.Add(TimeSpan.FromMinutes(-1)))
                        {
                            LastHeartbeatOrBackupCompleted = DateTime.Now;
                            Logger.WriteTraceMessage("Backup engine heartbeat: no recent activity.", InstanceID);
                        }
                    }

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
        /// Checks to see if storage providers are configured.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> StorageProvidersAreConfiguredAsync()
        {
            try
            {
                if (Sender != null)
                {
                    // if we have already setup the sender, we are configured.
                    return true;
                }
                else
                {
                    // otherwise check the database to see if we have any providers.

                    var storageProviders = await Database.GetProvidersAsync(ProviderTypes.Storage);

                    if (storageProviders.Count > 0)
                    {
                        // attemp to configure the providers.
                        var connections = new ProviderConnections(Database);
                        var storageProviderConnections = await connections.ConfigureStorageProviderConnectionsAsync(Logger);

                        if (storageProviderConnections != null)
                        {
                            Sender = new FileSender(Database, Logger, storageProviderConnections, InstanceID);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // no providers setup yet.
                        Logger.WriteTraceWarning("No storage providers have been configured yet. The backup engine(s) won't work until these have been configured.");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteTraceError("Failed to lookup or configure storage providers.", ex, Logger.GenerateFullContextStackTrace());
                return false;
            }
        }

        /// <summary>
        /// Grabs the next file that needs to be backed up.
        /// </summary>
        /// <remarks>
        /// This function is marked as safe and should not throw exceptions.
        /// </remarks>
        /// <returns></returns>
        private async Task<BackupFile> SafeGetNextFileToBackupAsync()
        {
            try
            {
                return await Database.FindNextFileToBackupAsync(InstanceID).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                string err = "Failed to capture the next file ready for backup.";
                Logger.WriteSystemEvent(err, ex, Logger.GenerateFullContextStackTrace(), Constants.EventIDs.FailedToGetNextFileToBackup, true, InstanceID);

                return null;
            }
        }
    }
}
