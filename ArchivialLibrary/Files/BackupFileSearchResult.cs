using System;

namespace ArchivialLibrary.Files
{
    /// <summary>
    /// Describes a backup file found from search results.
    /// </summary>
    /// <remarks>
    /// This result is passed into the restore commands to initiate a file 
    /// restore from the cloud storage provider.
    /// </remarks>
    public class BackupFileSearchResult
    {
        /// <summary>
        /// A unique file identifier.
        /// </summary>
        public Guid FileID { get; set; }

        /// <summary>
        /// The filename for the source file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The directory path for the source file.
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// The full path for the source file (directory + filename).
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// The size of the file expressed in bytes.
        /// </summary>
        public long FileSizeBytes { get; set; }

        /// <summary>
        /// The date/time that the file was last modified.
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// The file revision number.
        /// </summary>
        public int RevisionNumber { get; set; }

        /// <summary>
        /// File hash expressed as hex formatted string.
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// The name of the hashing algorithm used.
        /// </summary>
        public string HashAlgorithm { get; set; }
    }
}
