using System;
using OzetteLibrary.Database;
using OzetteLibrary.Logging;
using System.Collections.Generic;
using System.Threading;
using OzetteLibrary.Crypto;
using System.IO;
using OzetteLibrary.Files;
using OzetteLibrary.Providers;
using System.Threading.Tasks;
using OzetteLibrary.Constants;

namespace OzetteLibrary.Client.Transfer
{
    public class FileSender
    {
        /// <summary>
        /// A constructor that accepts a database and logger.
        /// </summary>
        /// <param name="database">The client database connection.</param>
        /// <param name="logger">A logging instance.</param>
        /// <param name="providers">A collection of cloud backup providers.</param>
        public FileSender(IClientDatabase database, ILogger logger, Dictionary<ProviderTypes, IProviderFileOperations> providers)
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
        /// A reference to the provider connections.
        /// </summary>
        private Dictionary<ProviderTypes, IProviderFileOperations> Providers { get; set; }

        /// <summary>
        /// Performs a transfer of the specified file to the target providers.
        /// </summary>
        /// <param name="File"></param>
        /// <param name="CancelToken"></param>
        public async Task TransferAsync(BackupFile File, CancellationToken CancelToken)
        {
            if (File == null)
            {
                throw new ArgumentNullException(nameof(File));
            }

            Logger.WriteTraceMessage(string.Format("Starting transfer operation for file: {0}", File.ToString()));

            try
            {
                // step 1: open up a filestream to the specified file.
                // use a read-only lock: this prevents the file from being modified while this lock is open.
                // but others can still open for read.

                using (FileStream fs = new FileStream(File.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    // step 2: verify/update the hash.
                    // if the file contents have changed between the time it was scanned and now, then this would result in a hash mismatch. 
                    // re-verify the hash of this file, and update the database if needed.

                    UpdateHashIfFileHasChangedRecently(File, fs);

                    // step 3: see if this file is already on the destination target provider(s).
                    // this avoids resending the file if for some reason the client DB/states got wiped out.

                    await UpdateFileCopyStateIfFileAlreadyExistsAtProvidersAsync(File, fs).ConfigureAwait(false);

                    // step 4: while the file has data that needs to be transferred- transfer it.
                    // this includes transferring to each potential target that needs this same file block.

                    while (File.HasDataToTransfer())
                    {
                        // step 4A: generate the next transfer data block.
                        var payload = File.GenerateNextTransferPayload(fs, Hasher);

                        // step 4B: send the transfer payload.
                        await SendTransferPayloadToFileTargetsAsync(File, payload).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteTraceError("An error occurred while preparing to transfer a file.", ex, Logger.GenerateFullContextStackTrace());
            }
        }

        /// <summary>
        /// Sends the transfer payload to the target providers.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        private async Task SendTransferPayloadToFileTargetsAsync(BackupFile file, TransferPayload payload)
        {
            var directory = Database.GetDirectoryMapItem(file.Directory);

            foreach (var providerName in payload.DestinationProviders)
            {
                var destination = Providers[providerName];

                try
                {
                    // upload this chunk to the destination cloud provider.
                    // note: the provider implementation will automatically handle retries of transient issues.
                    await destination.UploadFileBlockAsync(file, directory,
                        payload.Data, (int)payload.CurrentBlockNumber, (int)payload.TotalBlocks).ConfigureAwait(false);

                    // flag the chunk as sent in the file status.
                    file.SetBlockAsSent((int)payload.CurrentBlockNumber, providerName);
                }
                catch (Exception ex)
                {
                    Logger.WriteTraceError("An error occurred during a file transfer.", ex, Logger.GenerateFullContextStackTrace());
                    file.SetProviderToFailed(providerName);
                }
                finally
                {
                    // commit the status changes to the local state database.
                    Database.UpdateBackupFile(file);
                }
            }
        }

        /// <summary>
        /// Updates the local file copy state if the file has already been transferred (and local DB doesn't know about it).
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fs"></param>
        private async Task UpdateFileCopyStateIfFileAlreadyExistsAtProvidersAsync(BackupFile file, FileStream fs)
        {
            // for each provider that needs this file:
            // > double check that we haven't already transferred this whole file.
            // > this avoids a full upload if for some reason we have lost our client database state.

            Logger.WriteTraceMessage("Checking the status of this file on supported providers.");

            foreach (var provider in Providers)
            {
                var directory = Database.GetDirectoryMapItem(file.Directory);
                var providerState = await provider.Value.GetFileStatusAsync(file, directory);

                // mismatch: the provider file is synced, but our local state does not reflect this.

                if (file.CopyState.ContainsKey(provider.Key) 
                    && file.CopyState[provider.Key].SyncStatus == FileStatus.Unsynced 
                    && providerState.SyncStatus == FileStatus.Synced
                    && providerState.Metadata[ProviderMetadata.FileHash] == file.FileHashString)
                {
                    Logger.WriteTraceMessage(string.Format("Found a sync mismatch: this file is already synced at the provider [{0}]. Updating our local status.", provider.Key));

                    file.SetProviderToCompleted(provider.Key);
                    Database.UpdateBackupFile(file);
                }
            }
        }

        /// <summary>
        /// Verifies that the existing hash for the specified file is correct in the database.
        /// </summary>
        /// <param name="File"></param>
        /// <param name="Stream"></param>
        private void UpdateHashIfFileHasChangedRecently(BackupFile File, FileStream Stream)
        {
            if (File == null)
            {
                throw new ArgumentNullException(nameof(File));
            }
            if (Stream == null)
            {
                throw new ArgumentNullException(nameof(Stream));
            }

            var currentHash = Hasher.GenerateFileHash(File.GetFileHashAlgorithm(), Stream);

            if (currentHash.Length != 0)
            {
                if (Hasher.CheckTwoByteHashesAreTheSame(File.GetFileHash(), currentHash) == false)
                {
                    File.SetFileHashWithAlgorithm(currentHash, File.GetFileHashAlgorithm());
                    File.SetLastCheckedTimeStamp();
                    File.ResetCopyState(Database.GetProvidersList());
                    Database.UpdateBackupFile(File);
                }
            }
            else
            {
                throw new Exception("Failed to calculate the file hash.");
            }
        }
    }
}
