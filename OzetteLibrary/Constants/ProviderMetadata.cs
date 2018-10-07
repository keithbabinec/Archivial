namespace OzetteLibrary.Constants
{
    /// <summary>
    /// A constants class for provider metadata.
    /// </summary>
    public class ProviderMetadata
    {
        /// <summary>
        /// The name of the metadata key for provider sync status.
        /// </summary>
        public const string ProviderSyncStatusKeyName = "OzetteSyncStatus";

        /// <summary>
        /// The name of the metadata key for last completed file block index.
        /// </summary>
        public const string ProviderLastCompletedFileBlockIndexKeyName = "OzetteLastCompletedFileBlockIndex";

        /// <summary>
        /// The name of the metadata key for the file's source path.
        /// </summary>
        public const string FullSourcePathKeyName = "OzetteFileSourcePath";

        /// <summary>
        /// The name of the metadata key for the file's computed hash.
        /// </summary>
        public const string FileHash = "OzetteFileHash";

        /// <summary>
        /// The name of the metadata key for the file's hash algorithm.
        /// </summary>
        public const string FileHashAlgorithm = "OzetteFileHashAlgorithm";

        /// <summary>
        /// The name of the metadata key for the folder's local path.
        /// </summary>
        public const string ContainerLocalFolderPath = "OzetteFolderLocalPath";
    }
}
