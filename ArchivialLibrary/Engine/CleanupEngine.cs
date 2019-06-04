using ArchivialLibrary.Database;
using ArchivialLibrary.Events;
using ArchivialLibrary.Files;
using ArchivialLibrary.Folders;
using ArchivialLibrary.Logging;
using ArchivialLibrary.Providers;
using ArchivialLibrary.ServiceCore;
using ArchivialLibrary.StorageProviders;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ArchivialLibrary.Engine
{
    /// <summary>
    /// Contains core cleanup engine functionality.
    /// </summary>
    public class CleanupEngine : BaseEngine
    {
        /// <summary>
        /// Constructor that accepts a database and logger.
        /// </summary>
        /// <param name="database">The client database connection.</param>
        /// <param name="logger">A logging instance.</param>
        /// <param name="instanceID">A parameter to specify the engine instance ID.</param>
        /// <param name="coreSettings">The core settings accessor.</param>
        public CleanupEngine(IClientDatabase database,
                            ILogger logger,
                            int instanceID,
                            ICoreSettings coreSettings)
            : base(database, logger, instanceID, coreSettings) { }

        /// <summary>
        /// Begins to start the cleanup engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStart()
        {
            Logger.WriteTraceMessage(string.Format("Cleanup engine is starting up."), InstanceID);

            Thread pl = new Thread(() => ProcessLoopAsync().Wait());
            pl.Start();

            Logger.WriteTraceMessage(string.Format("Cleanup engine is now running."), InstanceID);
        }

        /// <summary>
        /// Begins to stop the cleanup engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStop()
        {
            CancelSource.Cancel();
            Logger.WriteTraceMessage("Cleanup engine is shutting down by request.", InstanceID);
        }

        /// <summary>
        /// The last time a heartbeat message or file backup was completed.
        /// </summary>
        private DateTime? LastHeartbeatOrBackupCompleted { get; set; }

        /// <summary>
        /// A reference to the provider connections.
        /// </summary>
        private StorageProviderConnectionsCollection Providers { get; set; }

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

                        var nextFileToCleanup = await SafeGetNextFileToCleanupAsync().ConfigureAwait(false);

                        if (nextFileToCleanup != null)
                        {
                            var nextFileMessage = string.Format("Detected a file revision to cleanup. FileName={0}, Revision={1}", nextFileToCleanup.FullSourcePath, nextFileToCleanup.FileRevisionNumber);
                            Logger.WriteTraceMessage(nextFileMessage, InstanceID);

                            // lookup the source information for this file.
                            // it is required for the cleanup.

                            var nextFileSource = await SafeGetSourceFromSourceIdAndType(nextFileToCleanup.SourceID, nextFileToCleanup.SourceType).ConfigureAwait(false);

                            if (nextFileSource == null)
                            {
                                var message = string.Format("Unable to cleanup file revision ({0}). An error has occurred while looking up the Source information.", nextFileToCleanup.FullSourcePath);
                                Logger.WriteTraceMessage(message, InstanceID);

                                await Database.RemoveFileFromCleanupQueueAsync(nextFileToCleanup).ConfigureAwait(false);
                            }

                            // initiate the file revision cleanup operation.

                            await DeleteFileRevisionInStorageProvidersAsync(nextFileToCleanup, nextFileSource, CancelSource.Token).ConfigureAwait(false);
                            await Database.RemoveFileFromCleanupQueueAsync(nextFileToCleanup).ConfigureAwait(false);

                            // do not sleep here.
                            // immediately move to backing up the next file.
                        }
                        else
                        {
                            await WaitAsync(TimeSpan.FromSeconds(60)).ConfigureAwait(false);

                            if (LastHeartbeatOrBackupCompleted.HasValue == false || LastHeartbeatOrBackupCompleted.Value < DateTime.Now.Add(TimeSpan.FromMinutes(-1)))
                            {
                                LastHeartbeatOrBackupCompleted = DateTime.Now;
                                Logger.WriteTraceMessage("Cleanup engine heartbeat: no recent activity.", InstanceID);
                            }
                        }
                    }
                    else
                    {
                        await WaitAsync(TimeSpan.FromSeconds(60)).ConfigureAwait(false);

                        if (LastHeartbeatOrBackupCompleted.HasValue == false || LastHeartbeatOrBackupCompleted.Value < DateTime.Now.Add(TimeSpan.FromMinutes(-1)))
                        {
                            LastHeartbeatOrBackupCompleted = DateTime.Now;
                            Logger.WriteTraceMessage("Cleanup engine heartbeat: no recent activity.", InstanceID);
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
        /// Removes the specified file revision in the configured cloud storage providers.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="source"></param>
        /// <param name="cancelToken"></param>
        /// <returns></returns>
        private async Task DeleteFileRevisionInStorageProvidersAsync(BackupFile file, SourceLocation source, CancellationToken cancelToken)
        {
            var directory = await Database.GetDirectoryMapItemAsync(file.Directory).ConfigureAwait(false);

            foreach (var provider in Providers)
            {
                try
                {
                    await provider.Value.DeleteFileRevisionAsync(file, source, directory, cancelToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    // cancellation was requested.
                    // bubble up to the next level.
                    throw;
                }
                catch (Exception ex)
                {
                    Logger.WriteTraceError("An error occurred during a file revision cleanup.", ex, Logger.GenerateFullContextStackTrace(), InstanceID);

                    // sets the error message/stack trace
                    await Database.RemoveFileFromBackupQueueAsync(file).ConfigureAwait(false);
                }
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
                if (Providers != null)
                {
                    // providers are already configured.
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
                            Providers = storageProviderConnections;
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
                        Logger.WriteTraceWarning("No storage providers have been configured yet. The cleanup engine(s) won't work until these have been configured.");
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
        /// Grabs the next file that needs to be cleaned up.
        /// </summary>
        /// <remarks>
        /// This function is marked as safe and should not throw exceptions.
        /// </remarks>
        /// <returns></returns>
        private async Task<BackupFile> SafeGetNextFileToCleanupAsync()
        {
            try
            {
                return await Database.FindNextFileToCleanupAsync(InstanceID).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                string err = "Failed to capture the next file revision ready for cleanup.";
                Logger.WriteSystemEvent(err, ex, Logger.GenerateFullContextStackTrace(), Constants.EventIDs.FailedToGetNextFileToCleanup, true, InstanceID);

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
                Logger.WriteSystemEvent(err, ex, Logger.GenerateFullContextStackTrace(), Constants.EventIDs.FailedToGetSourceLocationForCleanupFile, true, InstanceID);

                return null;
            }
        }
    }
}
