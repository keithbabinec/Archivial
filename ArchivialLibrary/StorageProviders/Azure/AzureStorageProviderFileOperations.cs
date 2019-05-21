using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using ArchivialLibrary.Constants;
using ArchivialLibrary.Crypto;
using ArchivialLibrary.Files;
using ArchivialLibrary.Folders;
using ArchivialLibrary.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace ArchivialLibrary.StorageProviders.Azure
{
    /// <summary>
    /// Implements file operations for the Azure cloud storage provider.
    /// </summary>
    public class AzureStorageProviderFileOperations : IStorageProviderFileOperations
    {
        /// <summary>
        /// A reference to the logging utility.
        /// </summary>
        private ILogger Logger;

        /// <summary>
        /// A reference to the hashing helper instance.
        /// </summary>
        private Hasher Hasher;

        /// <summary>
        /// A reference to the provider utilities helper instance.
        /// </summary>
        private AzureStorageProviderUtilities ProviderUtilities;

        /// <summary>
        /// A reference to the authenticated Azure Storage account.
        /// </summary>
        private CloudStorageAccount AzureStorage;

        /// <summary>
        /// A set of blob request options for writing or reading metadata.
        /// </summary>
        private BlobRequestOptions MetaDataRequestOptions = new BlobRequestOptions()
        {
            RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(5), 3),
            StoreBlobContentMD5 = false,
            ServerTimeout = TimeSpan.FromSeconds(30)
        };

        /// <summary>
        /// A set of blob request options for creating new containers.
        /// </summary>
        private BlobRequestOptions NewContainerRequestOptions = new BlobRequestOptions()
        {
            RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(15), 3),
            StoreBlobContentMD5 = false,
            ServerTimeout = TimeSpan.FromSeconds(30)
        };

        /// <summary>
        /// A set of blob request options for writing block lists.
        /// </summary>
        private BlobRequestOptions WriteBlockListRequestOptions = new BlobRequestOptions()
        {
            RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(15), 3),
            StoreBlobContentMD5 = false,
            ServerTimeout = TimeSpan.FromSeconds(60)
        };

        /// <summary>
        /// A set of blob request options for writing blocks.
        /// </summary>
        private BlobRequestOptions WriteBlockRequestOptions = new BlobRequestOptions()
        {
            RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(15), 3),
            StoreBlobContentMD5 = false,

            // Azure allows 10 minutes per MB. Commit blocks are 2MB in size.
            ServerTimeout = TimeSpan.FromMinutes(20)
        };

        /// <summary>
        /// Constructor that accepts a storage account name and storage account SAS token.
        /// </summary>
        /// <param name="logger">A logging utility instance.</param>
        /// <param name="storageAccountName">The azure storage account name.</param>
        /// <param name="storageAccountSASToken">SAS token for accessing the resource.</param>
        public AzureStorageProviderFileOperations(ILogger logger, string storageAccountName, string storageAccountSASToken)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            if (string.IsNullOrWhiteSpace(storageAccountName))
            {
                throw new ArgumentException(nameof(storageAccountName) + " must be provided.");
            }
            if (string.IsNullOrWhiteSpace(storageAccountSASToken))
            {
                throw new ArgumentException(nameof(storageAccountSASToken) + " must be provided.");
            }

            Logger = logger;
            Hasher = new Hasher(logger);
            ProviderUtilities = new AzureStorageProviderUtilities();

            var storageCredentials = new StorageCredentials(storageAccountName, storageAccountSASToken);
            AzureStorage = new CloudStorageAccount(storageCredentials, true);
        }

        /// <summary>
        /// Returns the status of a file as it exists (or doesn't) in Azure cloud storage.
        /// </summary>
        /// <param name="file"><c>BackupFile</c></param>
        /// <param name="sourceLocation"><c>SourceLocation</c></param>
        /// <param name="directory"><c>DirectoryMapItem</c></param>
        /// <returns><c>ProviderFileStatus</c></returns>
        public async Task<StorageProviderFileStatus> GetFileStatusAsync(BackupFile file, SourceLocation sourceLocation, DirectoryMapItem directory)
        {
            // calculate my uri
            string sasBlobUri = null;

            if (sourceLocation.Priority == FileBackupPriority.Meta)
            {
                // file has a specific destination container. this is reserved for meta folders.
                // use that specific container and filename instead of a guid-based uri.
                sasBlobUri = ProviderUtilities.GetFileUri(AzureStorage.Credentials.AccountName, sourceLocation.DestinationContainerName, file.Filename.ToLower());
            }
            else
            {
                sasBlobUri = ProviderUtilities.GetFileUri(AzureStorage.Credentials.AccountName, directory.GetRemoteContainerName(StorageProviderTypes.Azure), file.GetRemoteFileName(StorageProviderTypes.Azure));
            }

            // the default state for a freshly initialized file status object is unsynced.
            // if the blob doesn't exist, the file is unsynced.

            CloudBlockBlob blob = new CloudBlockBlob(new Uri(sasBlobUri), AzureStorage.Credentials);
            var fileStatus = new StorageProviderFileStatus(StorageProviderTypes.Azure);

            // does the file exist at the specified uri?

            if (await blob.ExistsAsync(MetaDataRequestOptions, null).ConfigureAwait(false))
            {
                // -- query metadata
                // -- determine state from metadata
                
                await blob.FetchAttributesAsync(null, MetaDataRequestOptions, null).ConfigureAwait(false);

                var allPropsAndMetadata = new Dictionary<string, string>(blob.Metadata);
                allPropsAndMetadata.Add(ProviderMetadata.HydrationStateKeyName, ProviderUtilities.GetHydrationStatusFromAzureState(blob.Properties.RehydrationStatus));

                fileStatus.ApplyMetadataToState(allPropsAndMetadata);
            }
            
            return fileStatus;
        }

        /// <summary>
        /// Uploads a single block of a file to Azure cloud storage.
        /// </summary>
        /// <remarks>
        /// If this file is the only (or final) block in the file, the file should be committed and/or the transaction finalized.
        /// </remarks>
        /// <param name="file"><c>BackupFile</c></param>
        /// <param name="sourceLocation"><c>SourceLocation</c></param>
        /// <param name="directory"><c>DirectoryMapItem</c></param>
        /// <param name="data">A byte array stream of file contents/data.</param>
        /// <param name="currentBlockIndex">The block number associated with the specified data.</param>
        /// <param name="totalBlocks">The total number of blocks that this file is made of.</param>
        /// <param name="cancelToken">The cancellation token.</param>
        public async Task UploadFileBlockAsync(BackupFile file, SourceLocation sourceLocation, DirectoryMapItem directory, byte[] data, int currentBlockIndex, int totalBlocks, CancellationToken cancelToken)
        {
            string containerName = null;

            if (sourceLocation.Priority == FileBackupPriority.Meta)
            {
                // special handling for meta/reserved folders
                containerName = sourceLocation.DestinationContainerName;
            }
            else
            {
                containerName = directory.GetRemoteContainerName(StorageProviderTypes.Azure);
            }

            var containerUri = ProviderUtilities.GetContainerUri(AzureStorage.Credentials.AccountName, containerName);
            var currentBlockNumber = currentBlockIndex + 1;

            var isFirstBlock = (currentBlockNumber == 1);
            var isLastBlock = (currentBlockNumber == totalBlocks);

            if (isFirstBlock)
            {
                await CreateBlobContainerIfMissingWithRetryAsync(containerName, containerUri, directory, cancelToken).ConfigureAwait(false);
            }

            cancelToken.ThrowIfCancellationRequested();

            string blobName = null;
            string sasBlobUri = null;

            if (sourceLocation.Priority == FileBackupPriority.Meta)
            {
                // special handling for meta/reserved folders
                blobName = file.Filename.ToLower();
                sasBlobUri = ProviderUtilities.GetFileUri(AzureStorage.Credentials.AccountName, sourceLocation.DestinationContainerName, blobName);
            }
            else
            {
                blobName = file.GetRemoteFileName(StorageProviderTypes.Azure);
                sasBlobUri = ProviderUtilities.GetFileUri(AzureStorage.Credentials.AccountName, containerName, blobName);
            }

            if (isFirstBlock)
            {
                Logger.WriteTraceMessage(string.Format("Azure destination: {0}/{1}", containerName, blobName));
            }

            Logger.WriteTraceMessage(string.Format("Uploading file block ({0} of {1}) to Azure storage.", currentBlockNumber, totalBlocks));

            // get a current reference to the blob and it's attributes.
            // upload the block.

            CloudBlockBlob blob = new CloudBlockBlob(new Uri(sasBlobUri), AzureStorage.Credentials);

            await UploadFileBlockWithRetryAsync(file, blob, data, currentBlockIndex, cancelToken).ConfigureAwait(false);

            // is this the first block or the last block? then run the block commit.
            // >> we need to commit once at the beginning to create the blob, which allows us to set metadata.
            // >> we need to commit at the end/final block as well to commit the complete block list.

            if (isFirstBlock || isLastBlock)
            {
                await CommitBlocksWithRetryAsync(file, currentBlockIndex, blob, cancelToken).ConfigureAwait(false);
            }

            // set the metadata (all situations).

            await SetBlobMetadataWithRetryAsync(file, currentBlockIndex, totalBlocks, blob, cancelToken).ConfigureAwait(false);

            if (isLastBlock)
            {
                if (!blob.Properties.StandardBlobTier.HasValue
                    || blob.Properties.StandardBlobTier.Value != StandardBlobTier.Archive)
                {
                    // set blob tier access.
                    // we only need to set this one time, at the time the upload is completed.
                    if (sourceLocation.Priority == FileBackupPriority.Meta)
                    {
                        await blob.SetStandardBlobTierAsync(StandardBlobTier.Cool, null, MetaDataRequestOptions, null).ConfigureAwait(false);
                    }
                    else
                    {
                        await blob.SetStandardBlobTierAsync(StandardBlobTier.Archive, null, MetaDataRequestOptions, null).ConfigureAwait(false);
                    }
                }

                Logger.WriteTraceMessage("File successfully uploaded to Azure storage: " + file.FullSourcePath);
            }
        }

        /// <summary>
        /// Deletes a single file revision from the Azure cloud storage provider.
        /// </summary>
        /// <param name="file"><c>BackupFile</c></param>
        /// <param name="sourceLocation"><c>SourceLocation</c></param>
        /// <param name="directory"><c>DirectoryMapItem</c></param>
        /// <param name="cancelToken">The canellation token.</param>
        public async Task DeleteFileRevisionAsync(BackupFile file, SourceLocation sourceLocation, DirectoryMapItem directory, CancellationToken cancelToken)
        {
            int maxAttempts = 3;

            var sasBlobUri = ProviderUtilities.GetFileUri(AzureStorage.Credentials.AccountName, 
                    directory.GetRemoteContainerName(StorageProviderTypes.Azure),
                    file.GetRemoteFileName(StorageProviderTypes.Azure));

            for (int currentAttempt = 1; currentAttempt <= maxAttempts; currentAttempt++)
            {
                cancelToken.ThrowIfCancellationRequested();

                try
                {
                    CloudBlockBlob blob = new CloudBlockBlob(new Uri(sasBlobUri), AzureStorage.Credentials);
                    await blob.DeleteIfExistsAsync(cancelToken).ConfigureAwait(false);

                    break;
                }
                catch (StorageException ex)
                {
                    if (ex.InnerException != null && ex.InnerException is TimeoutException)
                    {
                        if (currentAttempt == maxAttempts)
                        {
                            // retries exhausted
                            throw;
                        }

                        // special case handling:
                        // a windows azure storage timeout has occurred. give it a minute and try again.

                        Logger.WriteTraceMessage("An Azure storage timeout exception has occurred while trying to check (or delete) the blob. Waiting a minute before trying again.");
                        await Task.Delay(TimeSpan.FromSeconds(60), cancelToken).ConfigureAwait(false);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Creates the blob container if it is missing, with retries.
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="containerUri"></param>
        /// <param name="directory"></param>
        /// <param name="cancelToken"></param>
        /// <returns></returns>
        private async Task CreateBlobContainerIfMissingWithRetryAsync(string containerName, string containerUri, DirectoryMapItem directory, CancellationToken cancelToken)
        {
            int maxAttempts = 3;

            for (int currentAttempt = 1; currentAttempt <= maxAttempts; currentAttempt++)
            {
                cancelToken.ThrowIfCancellationRequested();

                try
                {
                    await CreateBlobContainerIfMissingAsync(containerName, containerUri, directory).ConfigureAwait(false);
                    break;
                }
                catch (StorageException ex)
                {
                    if (ex.InnerException != null && ex.InnerException is TimeoutException)
                    {
                        if (currentAttempt == maxAttempts)
                        {
                            // retries exhausted
                            throw;
                        }

                        // special case handling:
                        // a windows azure storage timeout has occurred. give it a minute and try again.

                        Logger.WriteTraceMessage("An Azure storage timeout exception has occurred while trying to check (or create) the container. Waiting a minute before trying again.");
                        await Task.Delay(TimeSpan.FromSeconds(60), cancelToken).ConfigureAwait(false);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Creates the blob container if it is missing.
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="containerUri"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        private async Task CreateBlobContainerIfMissingAsync(string containerName, string containerUri, DirectoryMapItem directory)
        {
            CloudBlobContainer container = new CloudBlobContainer(new Uri(containerUri), AzureStorage.Credentials);

            if (!await container.ExistsAsync(MetaDataRequestOptions, null).ConfigureAwait(false))
            {
                try
                {
                    Logger.WriteTraceMessage(string.Format("Azure container [{0}] does not exist. Creating it now.", containerName));

                    await container.CreateAsync(BlobContainerPublicAccessType.Off, NewContainerRequestOptions, null).ConfigureAwait(false);

                    container.Metadata[ProviderMetadata.ContainerLocalFolderPathKeyName] = System.Web.HttpUtility.UrlEncode(directory.LocalPath);
                    container.Metadata[ProviderMetadata.LocalHostNameKeyName] = Environment.MachineName;
                    await container.SetMetadataAsync(null, MetaDataRequestOptions, null).ConfigureAwait(false);
                }
                catch (StorageException ex)
                {
                    if (ex.RequestInformation.HttpStatusCode == 409) // 409 == Conflict
                    {
                        // special case handling.
                        // if multiple backup files start transferring at the same moment, and those files have the same folder, and that folder
                        // hasn't had a container created yet, then there is a race condition to create the container.
                        // not much we can do here but ignore this particular error but ignore it and continue.

                        Logger.WriteTraceMessage("Azure container has been created already.");
                    }
                    else
                    {
                        // wasn't a 409 (should re-throw).
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Uploads a file block with retry logic.
        /// </summary>
        /// <param name="file">The backup file information.</param>
        /// <param name="blob">The cloud block blob reference.</param>
        /// <param name="data">The block of data to upload.</param>
        /// <param name="currentBlockIndex">The block number to upload.</param>
        /// <param name="cancelToken">The cancellation token.</param>
        /// <returns></returns>
        private async Task UploadFileBlockWithRetryAsync(BackupFile file, CloudBlockBlob blob, byte[] data, int currentBlockIndex, CancellationToken cancelToken)
        {
            using (var stream = new MemoryStream(data))
            {
                var encodedBlockIdString = ProviderUtilities.GenerateBlockIdentifierBase64String(file.FileID, currentBlockIndex);
                int maxAttempts = 3;

                for (int currentAttempt = 1; currentAttempt <= maxAttempts; currentAttempt++)
                {
                    cancelToken.ThrowIfCancellationRequested();

                    try
                    {
                        await blob.PutBlockAsync(encodedBlockIdString, stream, null, null, WriteBlockRequestOptions, null, cancelToken).ConfigureAwait(false);
                        break;
                    }
                    catch (StorageException ex)
                    {
                        if (ex.InnerException != null && ex.InnerException is TimeoutException)
                        {
                            if (currentAttempt == maxAttempts)
                            {
                                // retries exhausted
                                throw;
                            }

                            // common scenario:
                            // a windows azure storage timeout has occurred. give it a minute and try again.

                            Logger.WriteTraceMessage("An Azure storage timeout exception has occurred while trying to upload the file block. Waiting a minute before trying again.");
                            await Task.Delay(TimeSpan.FromSeconds(60), cancelToken).ConfigureAwait(false);
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Commits a list of committed and uncommitted blocks to a block blob.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="currentBlockIndex"></param>
        /// <param name="blob"></param>
        /// <param name="cancelToken"></param>
        /// <returns></returns>
        private async Task CommitBlocksWithRetryAsync(BackupFile file, int currentBlockIndex, CloudBlockBlob blob, CancellationToken cancelToken)
        {
            int maxAttempts = 3;

            for (int currentAttempt = 1; currentAttempt <= maxAttempts; currentAttempt++)
            {
                cancelToken.ThrowIfCancellationRequested();

                try
                {
                    // after the block has been uploaded it is in an uncommitted state.
                    // commit this block (plus any previously committed blocks).

                    var blockListToCommit = ProviderUtilities.GenerateListOfBlocksToCommit(file.FileID, currentBlockIndex);
                    await blob.PutBlockListAsync(blockListToCommit, null, WriteBlockListRequestOptions, null).ConfigureAwait(false);
                    break;
                }
                catch (StorageException ex)
                {
                    if (ex.InnerException != null && ex.InnerException is TimeoutException)
                    {
                        if (currentAttempt == maxAttempts)
                        {
                            // retries exhausted
                            throw;
                        }

                        // special case handling:
                        // a windows azure storage timeout has occurred. give it a minute and try again.

                        Logger.WriteTraceMessage("An Azure storage timeout exception has occurred while trying to commit the block list. Waiting a minute before trying again.");
                        await Task.Delay(TimeSpan.FromSeconds(60), cancelToken).ConfigureAwait(false);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Sets a blob's metadata properties.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="currentBlockIndex"></param>
        /// <param name="totalBlocks"></param>
        /// <param name="blob"></param>
        /// <param name="cancelToken"></param>
        /// <returns></returns>
        private async Task SetBlobMetadataWithRetryAsync(BackupFile file, int currentBlockIndex, int totalBlocks, CloudBlockBlob blob, CancellationToken cancelToken)
        {
            // set the metadata properties.
            blob.Metadata[ProviderMetadata.ProviderSyncStatusKeyName] =
                ((currentBlockIndex + 1) == totalBlocks ?
                    FileStatus.Synced.ToString() :
                    FileStatus.InProgress.ToString());

            blob.Metadata[ProviderMetadata.ProviderLastCompletedFileBlockIndexKeyName] = currentBlockIndex.ToString();
            blob.Metadata[ProviderMetadata.FullSourcePathKeyName] = System.Web.HttpUtility.UrlEncode(file.FullSourcePath);
            blob.Metadata[ProviderMetadata.FileHashKeyName] = file.FileHashString;
            blob.Metadata[ProviderMetadata.FileHashAlgorithmKeyName] = file.HashAlgorithmType;

            int maxAttempts = 3;

            for (int currentAttempt = 1; currentAttempt <= maxAttempts; currentAttempt++)
            {
                cancelToken.ThrowIfCancellationRequested();

                try
                {
                    // commit the metadata changes to Azure.
                    await blob.SetMetadataAsync(null, MetaDataRequestOptions, null).ConfigureAwait(false);

                    break;
                }
                catch (StorageException ex)
                {
                    if (ex.InnerException != null && ex.InnerException is TimeoutException)
                    {
                        if (currentAttempt == maxAttempts)
                        {
                            // retries exhausted
                            throw;
                        }

                        // special case handling:
                        // a windows azure storage timeout has occurred. give it a minute and try again.

                        Logger.WriteTraceMessage("An Azure storage timeout exception has occurred while trying to set the blob metadata. Waiting a minute before trying again.");
                        await Task.Delay(TimeSpan.FromSeconds(60), cancelToken).ConfigureAwait(false);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
    }
}
