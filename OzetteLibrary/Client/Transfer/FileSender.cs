﻿using System;
using OzetteLibrary.Database;
using OzetteLibrary.Logging;
using System.Collections.Generic;
using System.Threading;
using OzetteLibrary.Crypto;
using System.IO;
using OzetteLibrary.Files;
using OzetteLibrary.StorageProviders;
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
        public FileSender(IClientDatabase database, ILogger logger, Dictionary<StorageProviderTypes, IStorageProviderFileOperations> providers)
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
        private Dictionary<StorageProviderTypes, IStorageProviderFileOperations> Providers { get; set; }

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
                // step 1: safety checks.
                // bail out if the file is missing or contents are empty.

                var info = new FileInfo(File.FullSourcePath);
                if (!info.Exists)
                {
                    Logger.WriteTraceMessage(string.Format("Unable to backup file ({0}). It has been deleted or is no longer accessible since it was scanned.", File.FullSourcePath));
                    File.SetFileAsDeleted();
                    Database.UpdateBackupFile(File);
                    return;
                }

                if (info.Length == 0)
                {
                    Logger.WriteTraceMessage(string.Format("Unable to backup file ({0}). It is empty (has no contents).", File.FullSourcePath));
                    File.SetFileAsReadOrBackupFailed();
                    Database.UpdateBackupFile(File);
                    return;
                }

                // step 2: open up a filestream to the specified file.
                // use a read-only lock: this prevents the file from being modified while this lock is open.
                // but others can still open for read.

                using (FileStream fs = new FileStream(File.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    // step 3: calculate and save the hash.

                    UpdateFileHashInDatabase(File, fs);

                    // step 4: see if this file is already on the destination target provider(s).
                    // this avoids resending the file if for some reason the client DB/states got wiped out.

                    await UpdateFileCopyStateIfFileAlreadyExistsAtProvidersAsync(File, fs).ConfigureAwait(false);

                    // step 5: while the file has data that needs to be transferred- transfer it.
                    // this includes transferring to each potential target that needs this same file block.

                    while (File.HasDataToTransfer())
                    {
                        // step 5A: generate the next transfer data block.
                        var payload = File.GenerateNextTransferPayload(fs, Hasher);

                        // step 5B: send the transfer payload.
                        await SendTransferPayloadToFileTargetsAsync(File, payload).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteTraceError("An error occurred while preparing to transfer a file.", ex, Logger.GenerateFullContextStackTrace());
                File.SetFileAsReadOrBackupFailed();
                Database.UpdateBackupFile(File);
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

            foreach (var provider in Providers)
            {
                var directory = Database.GetDirectoryMapItem(file.Directory);
                var providerState = await provider.Value.GetFileStatusAsync(file, directory);

                // mismatch: the provider file is synced, but our local state does not reflect this.

                if (file.CopyState.ContainsKey(provider.Key) 
                    && file.CopyState[provider.Key].SyncStatus == FileStatus.Unsynced 
                    && providerState.SyncStatus == FileStatus.Synced
                    && providerState.Metadata[ProviderMetadata.FileHashKeyName] == file.FileHashString)
                {
                    Logger.WriteTraceMessage(string.Format("Found a sync mismatch: this file is already synced at the provider [{0}]. Updating our local status.", provider.Key));

                    file.SetProviderToCompleted(provider.Key);
                    Database.UpdateBackupFile(file);
                }
            }
        }

        /// <summary>
        /// Calculates the hash for the specified file and saves it to the database.
        /// </summary>
        /// <param name="File"></param>
        /// <param name="Stream"></param>
        private void UpdateFileHashInDatabase(BackupFile File, FileStream Stream)
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
                File.SetLastCheckedTimeStamp();
                Database.UpdateBackupFile(File);
            }
            else
            {
                throw new Exception("Failed to calculate the file hash.");
            }
        }
    }
}
