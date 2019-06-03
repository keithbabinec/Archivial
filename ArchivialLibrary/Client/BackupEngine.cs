using ArchivialLibrary.Client.Transfer;
using ArchivialLibrary.Database;
using ArchivialLibrary.Engine;
using ArchivialLibrary.Events;
using ArchivialLibrary.Files;
using ArchivialLibrary.Folders;
using ArchivialLibrary.Logging;
using ArchivialLibrary.Providers;
using ArchivialLibrary.ServiceCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ArchivialLibrary.Client
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
        /// <param name="coreSettings">The core settings accessor.</param>
        public BackupEngine(IClientDatabase database,
                            ILogger logger,
                            int instanceID,
                            ICoreSettings coreSettings)
            : base(database, logger, instanceID, coreSettings) { }

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
        /// An ordered collection of backup queues to query for files to backup.
        /// </summary>
        private FileBackupPriority[] OrderedBackupQueues = new FileBackupPriority[] 
        {
            FileBackupPriority.Meta,
            FileBackupPriority.High,
            FileBackupPriority.Medium,
            FileBackupPriority.Low
        };

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

                    if (await StorageProvidersAreConfiguredAsync().ConfigureAwait(false))
                    {
                        // check to see if we have any files to backup.
                        // return the next one to backup.

                        var nextFileToBackup = await SafeGetNextFileToBackupAsync().ConfigureAwait(false);

                        if (nextFileToBackup != null)
                        {
                            // lookup the source information for this file.
                            // it is required for the backup.

                            var nextFileSource = await SafeGetSourceFromSourceIdAndType(nextFileToBackup.SourceID, nextFileToBackup.SourceType).ConfigureAwait(false);

                            if (nextFileSource == null)
                            {
                                var message = string.Format("Unable to backup file ({0}). An error has occurred while looking up the Source information.", nextFileToBackup.FullSourcePath);
                                Logger.WriteTraceMessage(message, InstanceID);

                                await Database.SetBackupFileAsFailedAsync(nextFileToBackup, message).ConfigureAwait(false);
                                await Database.RemoveFileFromBackupQueueAsync(nextFileToBackup).ConfigureAwait(false);
                            }

                            // initiate the file-send operation.

                            var transferFinished = false;
                            var transferTask = Sender.TransferAsync(nextFileToBackup, nextFileSource, CancelSource.Token);

                            while (!transferFinished)
                            {
                                await WaitAsync(TimeSpan.FromSeconds(2)).ConfigureAwait(false);

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
                            await WaitAsync(TimeSpan.FromSeconds(60)).ConfigureAwait(false);

                            if (LastHeartbeatOrBackupCompleted.HasValue == false || LastHeartbeatOrBackupCompleted.Value < DateTime.Now.Add(TimeSpan.FromMinutes(-1)))
                            {
                                LastHeartbeatOrBackupCompleted = DateTime.Now;
                                Logger.WriteTraceMessage("Backup engine heartbeat: no recent activity.", InstanceID);
                            }
                        }
                    }
                    else
                    {
                        await WaitAsync(TimeSpan.FromSeconds(60)).ConfigureAwait(false);

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

                    var storageProviders = await Database.GetProvidersAsync(ProviderTypes.Storage).ConfigureAwait(false);

                    if (storageProviders.Count > 0)
                    {
                        // attemp to configure the providers.
                        var connections = new ProviderConnections(Database);
                        var storageProviderConnections = await connections.ConfigureStorageProviderConnectionsAsync(Logger).ConfigureAwait(false);

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
                // there are 4 different priority queues: Low, Med, High, and Meta (system/reserved).
                // check the queues in priority order to determine the next file to backup.

                BackupFile nextFile = null;

                foreach (var queuePriority in OrderedBackupQueues)
                {
                    nextFile = await Database.FindNextFileToBackupAsync(InstanceID, queuePriority).ConfigureAwait(false);

                    if (nextFile != null)
                    {
                        // no need to look at additional queues-- we found one.
                        break;
                    }
                }

                return nextFile;
            }
            catch (Exception ex)
            {
                string err = "Failed to capture the next file ready for backup.";
                Logger.WriteSystemEvent(err, ex, Logger.GenerateFullContextStackTrace(), Constants.EventIDs.FailedToGetNextFileToBackup, true, InstanceID);

                return null;
            }
        }

        /// <summary>
        /// Returns the source from the specified source ID and type.
        /// </summary>
        /// <param name="sourceID">The ID of the source location.</param>
        /// <param name="sourceType">The source location type.</param>
        /// <returns></returns>
        private async Task<SourceLocation> SafeGetSourceFromSourceIdAndType(int sourceID, SourceLocationType sourceType)
        {
            try
            {
                return await Database.GetSourceLocationAsync(sourceID, sourceType).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                string err = string.Format("Failed to lookup the source {0} of type {1}.", sourceID, sourceType);
                Logger.WriteSystemEvent(err, ex, Logger.GenerateFullContextStackTrace(), Constants.EventIDs.FailedToGetSourceLocationForBackupFile, true, InstanceID);

                return null;
            }
        }
    }
}
