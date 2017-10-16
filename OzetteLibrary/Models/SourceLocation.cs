namespace OzetteLibrary.Models
{
    /// <summary>
    /// Describes a single requested source backup location.
    /// </summary>
    public class SourceLocation
    {
        /// <summary>
        /// The folder path to backup.
        /// </summary>
        public string FolderPath { get; set; }

        /// <summary>
        /// The file match filter for this folder.
        /// </summary>
        /// <remarks>
        /// Can use * wildcard for all files.
        /// </remarks>
        public string FileMatchFilter { get; set; }

        /// <summary>
        /// The priority in which this file should be backed up.
        /// </summary>
        public FileBackupPriority Priority { get; set; }

        /// <summary>
        /// The number of revisions to store.
        /// </summary>
        public int RevisionCount { get; set; }
    }
}
