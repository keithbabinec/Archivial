using OzetteLibrary.Models;
using System;

namespace OzetteLibrary.Providers.Azure
{
    /// <summary>
    /// Contains file operations for the Azure cloud storage provider.
    /// </summary>
    public class AzureProviderFileOperations : IProviderFileOperations
    {
        /// <summary>
        /// Returns the status of a file as it exists (or doesn't) in Azure cloud storage.
        /// </summary>
        /// <param name="file"><c>BackupFile</c></param>
        /// <returns><c>ProviderFileStatus</c></returns>
        public ProviderFileStatus GetFileStatus(BackupFile file)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Uploads the entire contents of a file to Azure cloud storage.
        /// </summary>
        /// <remarks>
        /// This is useful for small files that fit under the maximum file block transfer size.
        /// For large files, use UploadFileBlock() instead.
        /// </remarks>
        /// <param name="file"><c>BackupFile</c></param>
        /// <param name="data">A byte array stream of file contents/data.</param>
        public void UploadFile(BackupFile file, byte[] data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Uploads a single block of a larger file Azure cloud storage.
        /// </summary>
        /// <remarks>
        /// If this file is the final block in the file, the file should be committed and/or the transaction finalized.
        /// </remarks>
        /// <param name="file"><c>BackupFile</c></param>
        /// <param name="data">A byte array stream of file contents/data.</param>
        /// <param name="currentBlock">The block number associated with the specified data.</param>
        /// <param name="totalBlocks">The total number of blocks that this file is made of.</param>
        public void UploadFileBlock(BackupFile file, byte[] data, int currentBlock, int totalBlocks)
        {
            throw new NotImplementedException();
        }
    }
}
