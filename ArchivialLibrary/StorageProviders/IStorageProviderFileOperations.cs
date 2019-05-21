using ArchivialLibrary.Files;
using ArchivialLibrary.Folders;
using System.Threading;
using System.Threading.Tasks;

namespace ArchivialLibrary.StorageProviders
{
    /// <summary>
    /// Describes the required file operations for cloud providers.
    /// </summary>
    public interface IStorageProviderFileOperations
    {
        /// <summary>
        /// Returns the status of a file as it exists (or doesn't) in the cloud storage provider.
        /// </summary>
        /// <param name="file"><c>BackupFile</c></param>
        /// <param name="sourceLocation"><c>SourceLocation</c></param>
        /// <param name="directory"><c>DirectoryMapItem</c></param>
        /// <returns><c>ProviderFileStatus</c></returns>
        Task<StorageProviderFileStatus> GetFileStatusAsync(BackupFile file, SourceLocation sourceLocation, DirectoryMapItem directory);

        /// <summary>
        /// Uploads a single block of a file to the cloud storage provider.
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
        Task UploadFileBlockAsync(BackupFile file, SourceLocation sourceLocation, DirectoryMapItem directory, byte[] data, int currentBlockIndex, int totalBlocks, CancellationToken cancelToken);

        /// <summary>
        /// Deletes a single file revision from the cloud storage provider.
        /// </summary>
        /// <param name="file"><c>BackupFile</c></param>
        /// <param name="sourceLocation"><c>SourceLocation</c></param>
        /// <param name="directory"><c>DirectoryMapItem</c></param>
        /// <param name="cancelToken">The canellation token.</param>
        Task DeleteFileRevisionAsync(BackupFile file, SourceLocation sourceLocation, DirectoryMapItem directory, CancellationToken cancelToken);
    }
}
