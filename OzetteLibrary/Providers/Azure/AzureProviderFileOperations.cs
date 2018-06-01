using OzetteLibrary.Files;
using OzetteLibrary.Folders;
using System;

namespace OzetteLibrary.Providers.Azure
{
    /// <summary>
    /// Contains file operations for the Azure cloud storage provider.
    /// </summary>
    public class AzureProviderFileOperations : IProviderFileOperations
    {
        public AzureProviderFileOperations()
        {
            // TODO: input/accept provider options: things like the connection string, storage account name, keys, etc.
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
            var uri = GetFileUri("<storage account>", directory.GetRemoteContainerName(ProviderTypes.Azure), file.FileID.ToString().ToLower());

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
        /// <param name="storageAccountName">The azure storage account name.</param>
        /// <param name="containerName">The azure storage container name.</param>
        /// <param name="blobName">The azure storage blob name.</param>
        /// <returns></returns>
        private string GetFileUri(string storageAccountName, string containerName, string blobName)
        {
            if (string.IsNullOrWhiteSpace(storageAccountName))
            {
                throw new ArgumentNullException(nameof(storageAccountName));
            }
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

            return string.Format("https://{0}.blob.core.windows.net/{1}/{2}", storageAccountName, containerName, blobName);
        }
    }
}
