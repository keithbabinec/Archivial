using System;

namespace OzetteLibrary.Folders
{
    /// <summary>
    /// A results object containing the results of a source scan operation.
    /// </summary>
    public class ScanResults
    {
        /// <summary>
        /// An exception that prevented the scan from running or completing.
        /// </summary>
        public Exception Error { get; set; }

        /// <summary>
        /// The total number of new files found.
        /// </summary>
        public int NewFilesFound { get; set; }

        /// <summary>
        /// The total number of new file bytes found.
        /// </summary>
        public ulong NewBytesFound { get; set; }

        /// <summary>
        /// The total number of updated files found.
        /// </summary>
        public int UpdatedFilesFound { get; set; }

        /// <summary>
        /// The total number of updated file bytes found.
        /// </summary>
        public ulong UpdatedBytesFound { get; set; }

        /// <summary>
        /// The total number of all files found.
        /// </summary>
        public int TotalFilesFound { get; set; }

        /// <summary>
        /// The total number of all file bytes found.
        /// </summary>
        public ulong TotalBytesFound { get; set; }

        /// <summary>
        /// The total number of scanned directories.
        /// </summary>
        public int ScannedDirectoriesCount { get; set; }
    }
}
