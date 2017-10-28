using System;

namespace OzetteLibrary.Models
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
        /// The total number of new/updated files found.
        /// </summary>
        public int NewOrUpdatedFilesFound { get; set; }

        /// <summary>
        /// The total number of new/updated file bytes found.
        /// </summary>
        public ulong NewOrUpdatedBytesFound { get; set; }

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
