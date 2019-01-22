using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using OzetteLibrary.Constants;
using OzetteLibrary.Crypto;
using OzetteLibrary.Files;
using OzetteLibrary.Folders;
using OzetteLibrary.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OzetteLibrary.StorageProviders.Azure
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
        /// A reference to the request options we should use on blob operations, which includes the retry policy.
        /// </summary>
        private BlobRequestOptions RequestOptions;

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

            RequestOptions = new BlobRequestOptions()
            {
                RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(1), 3),
                StoreBlobContentMD5 = false
            };
        }

        /// <summary>
        /// Returns the status of a file as it exists (or doesn't) in Azure cloud storage.
        /// </summary>
        /// <param name="file"><c>BackupFile</c></param>
        /// <param name="directory"><c>DirectoryMapItem</c></param>
        /// <returns><c>ProviderFileStatus</c></returns>
        public async Task<StorageProviderFileStatus> GetFileStatusAsync(BackupFile file, DirectoryMapItem directory)
        {
            // calculate my uri

            var sasBlobUri = ProviderUtilities.GetFileUri(AzureStorage.Credentials.AccountName, directory.GetRemoteContainerName(StorageProviderTypes.Azure), file.GetRemoteFileName(StorageProviderTypes.Azure));

            // the default state for a freshly initialized file status object is unsynced.
            // if the blob doesn't exist, the file is unsynced.

            CloudBlockBlob blob = new CloudBlockBlob(new Uri(sasBlobUri), AzureStorage.Credentials);
            var fileStatus = new StorageProviderFileStatus(StorageProviderTypes.Azure);

            // does the file exist at the specified uri?

            if (await blob.ExistsAsync(RequestOptions, null).ConfigureAwait(false))
            {
                // -- query metadata
                // -- determine state from metadata
                
                await blob.FetchAttributesAsync(null, RequestOptions, null).ConfigureAwait(false);

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
        /// <param name="directory"><c>DirectoryMapItem</c></param>
        /// <param name="data">A byte array stream of file contents/data.</param>
        /// <param name="currentBlockIndex">The block number associated with the specified data.</param>
        /// <param name="totalBlocks">The total number of blocks that this file is made of.</param>
        public async Task UploadFileBlockAsync(BackupFile file, DirectoryMapItem directory, byte[] data, int currentBlockIndex, int totalBlocks)
        {
            var containerName = directory.GetRemoteContainerName(StorageProviderTypes.Azure);
            var containerUri = ProviderUtilities.GetContainerUri(AzureStorage.Credentials.AccountName, containerName);
            var currentBlockNumber = currentBlockIndex + 1;

            var isFirstBlock = (currentBlockNumber == 1);
            var isLastBlock = (currentBlockNumber == totalBlocks);

            await CreateBlobContainerIfMissingAsync(containerName, containerUri, directory).ConfigureAwait(false);

            // calculate my uri

            var blobName = file.GetRemoteFileName(StorageProviderTypes.Azure);
            var sasBlobUri = ProviderUtilities.GetFileUri(AzureStorage.Credentials.AccountName, containerName, blobName);

            if (isFirstBlock)
            {
                Logger.WriteTraceMessage(string.Format("Azure destination: {0}/{1}", containerName, blobName));
            }

            Logger.WriteTraceMessage(string.Format("Uploading file block ({0} of {1}) to Azure storage.", currentBlockNumber, totalBlocks));

            // get a current reference to the blob and it's attributes.
            // upload the block.

            CloudBlockBlob blob = new CloudBlockBlob(new Uri(sasBlobUri), AzureStorage.Credentials);

            using (var stream = new MemoryStream(data))
            {
                var encodedBlockIdString = ProviderUtilities.GenerateBlockIdentifierBase64String(file.FileID, currentBlockIndex);
                await blob.PutBlockAsync(encodedBlockIdString, stream, null, null, RequestOptions, null).ConfigureAwait(false);
            }

            // is this the first block or the last block? then run the block commit.
            // >> we need to commit once at the beginning to create the blob, which allows us to set metadata.
            // >> we need to commit at the end/final block as well to commit the complete block list.

            if (isFirstBlock || isLastBlock)
            {
                await CommitBlocksAsync(file, currentBlockIndex, blob).ConfigureAwait(false);
            }

            // set the metadata (all situations).

            await SetBlobMetadataAsync(file, currentBlockIndex, totalBlocks, blob).ConfigureAwait(false);

            if (isLastBlock)
            {
                if (!blob.Properties.StandardBlobTier.HasValue
                    || blob.Properties.StandardBlobTier.Value != StandardBlobTier.Archive)
                {
                    // set blob tier access.
                    // we only need to set this one time, at the time the upload is completed.
                    await blob.SetStandardBlobTierAsync(StandardBlobTier.Archive, null, RequestOptions, null).ConfigureAwait(false);
                }

                Logger.WriteTraceMessage("File successfully uploaded to Azure storage: " + file.FullSourcePath);
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

            if (!await container.ExistsAsync(RequestOptions, null).ConfigureAwait(false))
            {
                Logger.WriteTraceMessage(string.Format("Azure container [{0}] does not exist. Creating it now.", containerName));

                await container.CreateAsync(BlobContainerPublicAccessType.Off, RequestOptions, null).ConfigureAwait(false);

                container.Metadata[ProviderMetadata.ContainerLocalFolderPathKeyName] = System.Web.HttpUtility.UrlEncode(directory.LocalPath);
                container.Metadata[ProviderMetadata.LocalHostNameKeyName] = Environment.MachineName;
                await container.SetMetadataAsync(null, RequestOptions, null).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Commits a list of committed and uncommitted blocks to a block blob.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="currentBlockIndex"></param>
        /// <param name="blob"></param>
        /// <returns></returns>
        private async Task CommitBlocksAsync(BackupFile file, int currentBlockIndex, CloudBlockBlob blob)
        {
            // after the block has been uploaded it is in an uncommitted state.
            // commit this block (plus any previously committed blocks).

            var blockListToCommit = ProviderUtilities.GenerateListOfBlocksToCommit(file.FileID, currentBlockIndex);
            await blob.PutBlockListAsync(blockListToCommit, null, RequestOptions, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Sets a blob's metadata properties.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="currentBlockIndex"></param>
        /// <param name="totalBlocks"></param>
        /// <param name="blob"></param>
        /// <returns></returns>
        private async Task SetBlobMetadataAsync(BackupFile file, int currentBlockIndex, int totalBlocks, CloudBlockBlob blob)
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

            // commit the metadata changes to Azure.
            await blob.SetMetadataAsync(null, RequestOptions, null).ConfigureAwait(false);
        }
    }
}
