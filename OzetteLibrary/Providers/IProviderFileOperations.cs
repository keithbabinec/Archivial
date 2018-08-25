using OzetteLibrary.Files;
using OzetteLibrary.Folders;
using System.Threading.Tasks;

namespace OzetteLibrary.Providers
{
    /// <summary>
    /// Describes the required file operations for cloud providers.
    /// </summary>
    public interface IProviderFileOperations
    {
        /// <summary>
        /// Returns the status of a file as it exists (or doesn't) in the cloud provider.
        /// </summary>
        /// <param name="file"><c>BackupFile</c></param>
        /// <param name="directory"><c>DirectoryMapItem</c></param>
        /// <returns><c>ProviderFileStatus</c></returns>
        Task<ProviderFileStatus> GetFileStatus(BackupFile file, DirectoryMapItem directory);

        /// <summary>
        /// Uploads a single block of a file to the cloud provider.
        /// </summary>
        /// <remarks>
        /// If this file is the only (or final) block in the file, the file should be committed and/or the transaction finalized.
        /// </remarks>
        /// <param name="file"><c>BackupFile</c></param>
        /// <param name="directory"><c>DirectoryMapItem</c></param>
        /// <param name="data">A byte array stream of file contents/data.</param>
        /// <param name="currentBlock">The block number associated with the specified data.</param>
        /// <param name="totalBlocks">The total number of blocks that this file is made of.</param>
        Task UploadFileBlock(BackupFile file, DirectoryMapItem directory, byte[] data, int currentBlock, int totalBlocks);
    }
}
