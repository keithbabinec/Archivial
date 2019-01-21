using OzetteLibrary.Client.Transfer;
using OzetteLibrary.Database;
using OzetteLibrary.Engine;
using OzetteLibrary.Events;
using OzetteLibrary.Files;
using OzetteLibrary.Logging;
using OzetteLibrary.MessagingProviders;
using OzetteLibrary.StorageProviders;
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
        /// <param name="storageProviders">A collection of cloud backup storage provider connections.</param>
        /// <param name="messagingProviders">A collection of messaging provider connections.</param>
        /// <param name="instanceID">A parameter to specify the engine instance ID.</param>
        public BackupEngine(IClientDatabase database,
                            ILogger logger,
                            StorageProviderConnectionsCollection storageProviders,
                            MessagingProviderConnectionsCollection messagingProviders,
                            int instanceID)
            : base(database, logger, storageProviders, messagingProviders, instanceID) { }

        /// <summary>
        /// Begins to start the backup engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStart()
        {
            if (Running == true)
            {
                throw new InvalidOperationException("The engine cannot be started, it is already running.");
            }

            Running = true;
            Sender = new FileSender(Database, Logger, StorageProviders, InstanceID);

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
            if (Running == true)
            {
                Logger.WriteTraceMessage("Backup engine is shutting down.", InstanceID);
                Running = false;
            }
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
                    // check to see if we have any files to backup.
                    // return the next one to backup.

                    BackupFile nextFileToBackup = await SafeGetNextFileToBackupAsync();

                    if (nextFileToBackup != null)
                    {
                        // initiate the file-send operation.

                        var cancel = new CancellationTokenSource();
                        var transferFinished = false;
                        var transferTask = Sender.TransferAsync(nextFileToBackup, cancel.Token);

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

                            if (Running == false)
                            {
                                // stop was requested.
                                // stop the currently in-progress file send operation.
                                cancel.Cancel();
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

                    if (Running == false)
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
                return await Database.FindNextFileToBackupAsync(InstanceID);
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
