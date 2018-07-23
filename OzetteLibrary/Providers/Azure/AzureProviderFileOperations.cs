using OzetteLibrary.Files;
using OzetteLibrary.Folders;
using OzetteLibrary.Logging;
using System;

namespace OzetteLibrary.Providers.Azure
{
    /// <summary>
    /// Contains file operations for the Azure cloud storage provider.
    /// </summary>
    public class AzureProviderFileOperations : IProviderFileOperations
    {
        /// <summary>
        /// A reference to the storage account name.
        /// </summary>
        private string StorageAccountName;

        /// <summary>
        /// A reference to the SAS token.
        /// </summary>
        private string StorageAccountSASToken;

        /// <summary>
        /// Constructor that accepts a storage account name and storage account SAS token.
        /// </summary>
        /// <param name="logger">A logging utility instance.</param>
        /// <param name="storageAccountName">The azure storage account name.</param>
        /// <param name="storageAccountSASToken">SAS token for accessing the resource.</param>
        public AzureProviderFileOperations(ILogger logger, string storageAccountName, string storageAccountSASToken)
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

            StorageAccountName = storageAccountName;
            StorageAccountSASToken = storageAccountSASToken;
        }

        /// <summary>
        /// Returns the status of a file as it exists (or doesn't) in Azure cloud storage.
        /// </summary>
        /// <param name="file"><c>BackupFile</c></param>
        /// <param name="directory"><c>DirectoryMapItem</c></param>
        /// <returns><c>ProviderFileStatus</c></returns>
        public ProviderFileStatus GetFileStatus(BackupFile file, DirectoryMapItem directory)
        {
            // calculate my uri

            var uri = GetFileUri(directory.GetRemoteContainerName(ProviderTypes.Azure), file.GetRemoteFileName(ProviderTypes.Azure));

            // does the file exist at the specified uri?

            // if no: return unsynced

            // if yes: 
            // -- query metadata
            // -- determine state from metadata

            // return state

            throw new NotImplementedException();
        }

        /// <summary>
        /// Uploads a single block of a larger file Azure cloud storage.
        /// </summary>
        /// <remarks>
        /// If this file is the final block in the file, the file should be committed and/or the transaction finalized.
        /// </remarks>
        /// <param name="file"><c>BackupFile</c></param>
        /// <param name="directory"><c>DirectoryMapItem</c></param>
        /// <param name="data">A byte array stream of file contents/data.</param>
        /// <param name="currentBlock">The block number associated with the specified data.</param>
        /// <param name="totalBlocks">The total number of blocks that this file is made of.</param>
        public void UploadFileBlock(BackupFile file, DirectoryMapItem directory, byte[] data, int currentBlock, int totalBlocks)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the Azure resource URI.
        /// </summary>
        /// <param name="containerName">The azure storage container name.</param>
        /// <param name="blobName">The azure storage blob name.</param>
        /// <returns>A string formatted as a URI</returns>
        public string GetFileUri(string containerName, string blobName)
        {
            if (string.IsNullOrWhiteSpace(containerName))
            {
                throw new ArgumentNullException(nameof(containerName));
            }
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentNullException(nameof(blobName));
            }

            // example uri:
            // https://myaccount.blob.core.windows.net/mycontainer/myblob 

            return string.Format("https://{0}.blob.core.windows.net/{1}/{2}?{3}", StorageAccountName, containerName, blobName, StorageAccountSASToken);
        }
    }
}
