using OzetteLibrary.Providers;
using System.Collections.Generic;

namespace OzetteLibrary.ServiceCore
{
    /// <summary>
    /// Describes the overall backup progress status.
    /// </summary>
    public class BackupProgress
    {
        /// <summary>
        /// The overall backup completion percentage.
        /// </summary>
        public double OverallPercentage { get; set; }

        /// <summary>
        /// The overall backup completion percentage, grouped by provider.
        /// </summary>
        public Dictionary<ProviderTypes, double> PercentageByProvider { get; set; }

        /// <summary>
        /// The total number of all files.
        /// </summary>
        public long TotalFileCount { get; set; }

        /// <summary>
        /// The total size of all files.
        /// </summary>
        public string TotalFileSize { get; set; }

        /// <summary>
        /// The total number of all backed up files.
        /// </summary>
        public long BackedUpFileCount { get; set; }

        /// <summary>
        /// The total size of all backed up files.
        /// </summary>
        public string BackedUpFileSize { get; set; }

        /// <summary>
        /// The total number of remaining files.
        /// </summary>
        public long RemainingFileCount { get; set; }

        /// <summary>
        /// The total size of remaining files.
        /// </summary>
        public string RemainingFileSize { get; set; }

        /// <summary>
        /// The total number of failed files.
        /// </summary>
        public long FailedFileCount { get; set; }

        /// <summary>
        /// The total size of failed files.
        /// </summary>
        public string FailedFileSize { get; set; }
    }
}
