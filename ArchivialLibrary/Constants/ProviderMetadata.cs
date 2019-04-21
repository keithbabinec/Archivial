namespace ArchivialLibrary.Constants
{
    /// <summary>
    /// A constants class for provider metadata.
    /// </summary>
    public class ProviderMetadata
    {
        /// <summary>
        /// The name of the metadata key for provider sync status.
        /// </summary>
        public const string ProviderSyncStatusKeyName = "ArchivialSyncStatus";

        /// <summary>
        /// The name of the metadata key for last completed file block index.
        /// </summary>
        public const string ProviderLastCompletedFileBlockIndexKeyName = "ArchivialLastCompletedFileBlockIndex";

        /// <summary>
        /// The name of the metadata key for the file's source path.
        /// </summary>
        public const string FullSourcePathKeyName = "ArchivialFileSourcePath";

        /// <summary>
        /// The name of the metadata key for the file's computed hash.
        /// </summary>
        public const string FileHashKeyName = "ArchivialFileHash";

        /// <summary>
        /// The name of the metadata key for the file's hash algorithm.
        /// </summary>
        public const string FileHashAlgorithmKeyName = "ArchivialFileHashAlgorithm";

        /// <summary>
        /// The name of the metadata key for the folder's local path.
        /// </summary>
        public const string ContainerLocalFolderPathKeyName = "ArchivialFolderLocalPath";

        /// <summary>
        /// The name of the metadata key for the backup host computer.
        /// </summary>
        public const string LocalHostNameKeyName = "ArchivialBackupHost";

        /// <summary>
        /// The name of the properties key for file hydration state.
        /// </summary>
        public const string HydrationStateKeyName = "HydrationState";
    }
}
