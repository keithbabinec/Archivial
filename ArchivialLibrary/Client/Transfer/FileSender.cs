using System;
using ArchivialLibrary.Database;
using ArchivialLibrary.Logging;
using System.Threading;
using ArchivialLibrary.Crypto;
using System.IO;
using ArchivialLibrary.Files;
using ArchivialLibrary.StorageProviders;
using System.Threading.Tasks;
using ArchivialLibrary.Constants;
using ArchivialLibrary.Folders;

namespace ArchivialLibrary.Client.Transfer
{
    public class FileSender
    {
        /// <summary>
        /// A constructor that accepts a database and logger.
        /// </summary>
        /// <param name="database">The client database connection.</param>
        /// <param name="logger">A logging instance.</param>
        /// <param name="providers">A collection of cloud backup providers.</param>
        /// <param name="instanceID">An engine instance ID.</param>
        public FileSender(IClientDatabase database, ILogger logger, StorageProviderConnectionsCollection providers, int instanceID)
        {
            if (database == null)
            {
                throw new ArgumentNullException(nameof(database));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            if (providers == null)
            {
                throw new ArgumentNullException(nameof(providers));
            }
            if (providers.Count == 0)
            {
                throw new ArgumentException(nameof(providers) + " must be provided.");
            }

            Database = database;
            Logger = logger;
            Providers = providers;
            Hasher = new Hasher(Logger);
            InstanceID = instanceID;
        }

        /// <summary>
        /// A reference to the database.
        /// </summary>
        private IClientDatabase Database { get; set; }

        /// <summary>
        /// A reference to the logger.
        /// </summary>
        private ILogger Logger { get; set; }

        /// <summary>
        /// A reference to the hasher.
        /// </summary>
        private Hasher Hasher { get; set; }

        /// <summary>
        /// An instance ID for the engine.
        /// </summary>
        private int InstanceID { get; set; }

        /// <summary>
        /// A reference to the provider connections.
        /// </summary>
        private StorageProviderConnectionsCollection Providers { get; set; }

        /// <summary>
        /// Performs a transfer of the specified file to the target providers.
        /// </summary>
        /// <param name="File"></param>
        /// <param name="CancelToken"></param>
        public async Task TransferAsync(BackupFile File, SourceLocation Source, CancellationToken CancelToken)
        {
            if (File == null)
            {
                throw new ArgumentNullException(nameof(File));
            }
            if (Source == null)
            {
                throw new ArgumentNullException(nameof(Source));
            }
            if (CancelToken == null)
            {
                throw new ArgumentNullException(nameof(CancelToken));
            }

            Logger.WriteTraceMessage(string.Format("Starting transfer operation for file: {0}", File.ToString()), InstanceID);

            try
            {
                // step 1: safety checks.
                // bail out if the file is missing or contents are empty.

                var info = new FileInfo(File.FullSourcePath);
                if (!info.Exists)
                {
                    Logger.WriteTraceMessage(string.Format("Unable to backup file ({0}). It has been deleted or is no longer accessible since it was scanned.", File.FullSourcePath), InstanceID);
                    await Database.DeleteBackupFileAsync(File).ConfigureAwait(false);
                    await Database.RemoveFileFromBackupQueueAsync(File).ConfigureAwait(false);
                    return;
                }

                if (info.Length == 0)
                {
                    var message = string.Format("Unable to backup file ({0}). It is empty (has no contents).", File.FullSourcePath);
                    Logger.WriteTraceMessage(message, InstanceID);
                    await Database.SetBackupFileAsFailedAsync(File, message).ConfigureAwait(false);
                    await Database.RemoveFileFromBackupQueueAsync(File).ConfigureAwait(false);
                    return;
                }

                // cancel if the caller is shutting down.
                // add these in a few different places to ensure we aren't holding the threads open too long.
                CancelToken.ThrowIfCancellationRequested();

                // step 2: open up a filestream to the specified file.
                // use a read-only lock: this prevents the file from being modified while this lock is open.
                // but others can still open for read.

                using (FileStream fs = new FileStream(File.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    // step 3: calculate and save the hash.

                    CancelToken.ThrowIfCancellationRequested();
                    await UpdateFileHashInDatabaseAsync(File, fs).ConfigureAwait(false);

                    // step 4: see if this file is already on the destination target provider(s).
                    // this avoids resending the file if for some reason the client DB/states got wiped out.

                    CancelToken.ThrowIfCancellationRequested();
                    await UpdateFileCopyStateIfFileAlreadyExistsAtProvidersAsync(File, Source, fs).ConfigureAwait(false);

                    // step 5: while the file has data that needs to be transferred- transfer it.
                    // this includes transferring to each potential target that needs this same file block.

                    while (File.HasDataToTransfer())
                    {
                        // step 5A: generate the next transfer data block.
                        CancelToken.ThrowIfCancellationRequested();
                        var payload = File.GenerateNextTransferPayload(fs, Hasher);

                        // step 5B: send the transfer payload.
                        CancelToken.ThrowIfCancellationRequested();
                        await SendTransferPayloadToFileTargetsAsync(File, Source, payload).ConfigureAwait(false);
                    }

                    // no more data to transfer, remove the file from the backup queue.
                    await Database.RemoveFileFromBackupQueueAsync(File).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                // the caller has requested that we stop.
                Logger.WriteTraceWarning("An in-progress file transfer has been cancelled by request of the Backup Engine. It will be resumed the next time the engine starts up.", InstanceID);
            }
            catch (Exception ex)
            {
                var message = "An error occurred while preparing to transfer a file.";
                Logger.WriteTraceError(message, ex, Logger.GenerateFullContextStackTrace(), InstanceID);
                await Database.SetBackupFileAsFailedAsync(File, (message + ex.ToString())).ConfigureAwait(false);
                await Database.RemoveFileFromBackupQueueAsync(File).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Sends the transfer payload to the target providers.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        private async Task SendTransferPayloadToFileTargetsAsync(BackupFile file, SourceLocation source, TransferPayload payload)
        {
            var directory = await Database.GetDirectoryMapItemAsync(file.Directory).ConfigureAwait(false);

            foreach (var providerName in payload.DestinationProviders)
            {
                var destination = Providers[providerName];

                try
                {
                    // upload this chunk to the destination cloud provider.
                    // note: the provider implementation will automatically handle retries of transient issues.
                    await destination.UploadFileBlockAsync(file, source, directory,
                        payload.Data, (int)payload.CurrentBlockNumber, (int)payload.TotalBlocks).ConfigureAwait(false);

                    // flag the chunk as sent in the file status.
                    file.SetBlockAsSent((int)payload.CurrentBlockNumber, providerName);
                }
                catch (Exception ex)
                {
                    Logger.WriteTraceError("An error occurred during a file transfer.", ex, Logger.GenerateFullContextStackTrace(), InstanceID);
                    file.SetProviderToFailed(providerName);

                    // sets the error message/stack trace
                    await Database.SetBackupFileAsFailedAsync(file, ex.ToString()).ConfigureAwait(false);
                    await Database.RemoveFileFromBackupQueueAsync(file).ConfigureAwait(false);
                }
                finally
                {
                    // commit the status changes to the local state database.
                    await Database.UpdateBackupFileCopyStateAsync(file).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Updates the local file copy state if the file has already been transferred (and local DB doesn't know about it).
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fs"></param>
        private async Task UpdateFileCopyStateIfFileAlreadyExistsAtProvidersAsync(BackupFile file, SourceLocation source, FileStream fs)
        {
            // for each provider that needs this file:
            // > double check that we haven't already transferred this whole file.
            // > this avoids a full upload if for some reason we have lost our client database state.

            bool stateChanged = false;

            foreach (var provider in Providers)
            {
                var directory = await Database.GetDirectoryMapItemAsync(file.Directory).ConfigureAwait(false);
                var providerState = await provider.Value.GetFileStatusAsync(file, source, directory).ConfigureAwait(false);

                // compare the results from the remote provider to the local known state.
                // if things are different, then apply the remote state to known local and save the changes.

                if (file.UpdateLocalStateIfRemoteStateDoesNotMatch(providerState))
                {
                    stateChanged = true;
                }
            }

            if (stateChanged)
            {
                Logger.WriteTraceMessage("Found a sync mismatch. A provider state does not match local status, updating now.", InstanceID);
                await Database.UpdateBackupFileCopyStateAsync(file).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Calculates the hash for the specified file and saves it to the database.
        /// </summary>
        /// <param name="File"></param>
        /// <param name="Stream"></param>
        private async Task UpdateFileHashInDatabaseAsync(BackupFile File, FileStream Stream)
        {
            if (File == null)
            {
                throw new ArgumentNullException(nameof(File));
            }
            if (Stream == null)
            {
                throw new ArgumentNullException(nameof(Stream));
            }

            var hashAlgo = Hasher.GetDefaultHashAlgorithm(File.Priority);
            var currentHash = Hasher.GenerateFileHash(hashAlgo, Stream);

            if (currentHash.Length != 0)
            {
                File.SetFileHashWithAlgorithm(currentHash, hashAlgo);
                await Database.SetBackupFileHashAsync(File).ConfigureAwait(false);
            }
            else
            {
                throw new Exception("Failed to calculate the file hash.");
            }
        }
    }
}
