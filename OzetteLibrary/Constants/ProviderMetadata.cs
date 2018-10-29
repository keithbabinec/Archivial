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
        public const string FileHashKeyName = "OzetteFileHash";

        /// <summary>
        /// The name of the metadata key for the file's hash algorithm.
        /// </summary>
        public const string FileHashAlgorithmKeyName = "OzetteFileHashAlgorithm";

        /// <summary>
        /// The name of the metadata key for the folder's local path.
        /// </summary>
        public const string ContainerLocalFolderPathKeyName = "OzetteFolderLocalPath";

        /// <summary>
        /// The name of the metadata key for the backup host computer.
        /// </summary>
        public const string LocalHostNameKeyName = "OzetteBackupHost";

        /// <summary>
        /// The name of the properties key for file hydration state.
        /// </summary>
        public const string HydrationStateKeyName = "HydrationState";

        /// <summary>
        /// The name of the properties key for file revision.
        /// </summary>
        public const string RevisionTagKeyName = "RevisionTag";
    }
}
