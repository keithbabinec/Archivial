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
        public string OverallPercentage { get; set; }

        /// <summary>
        /// The total number of all files.
        /// </summary>
        public long TotalFileCount { get; set; }

        /// <summary>
        /// The total size of all files in bytes.
        /// </summary>
        public long TotalFileSizeBytes { get; set; }

        /// <summary>
        /// The total size of all files expressed as a friendly string.
        /// </summary>
        public string TotalFileSize
        {
            get
            {
                return GetBytesAsFriendlyString(TotalFileSizeBytes);
            }
        }

        /// <summary>
        /// The total number of all backed up files.
        /// </summary>
        public long BackedUpFileCount { get; set; }

        /// <summary>
        /// The total size of all backed up files in bytes.
        /// </summary>
        public long BackedUpFileSizeBytes { get; set; }

        /// <summary>
        /// The total size of all backed up files expressed as a friendly string.
        /// </summary>
        public string BackedUpFileSize
        {
            get
            {
                return GetBytesAsFriendlyString(BackedUpFileSizeBytes);
            }
        }

        /// <summary>
        /// The total number of remaining files.
        /// </summary>
        public long RemainingFileCount { get; set; }

        /// <summary>
        /// The total size of remaining files in bytes.
        /// </summary>
        public long RemainingFileSizeBytes { get; set; }

        /// <summary>
        /// The total size of remaining files expressed as a friendly string.
        /// </summary>
        public string RemainingFileSize
        {
            get
            {
                return GetBytesAsFriendlyString(RemainingFileSizeBytes);
            }
        }

        /// <summary>
        /// The total number of failed files.
        /// </summary>
        public long FailedFileCount { get; set; }

        /// <summary>
        /// The total size of failed files in bytes.
        /// </summary>
        public long FailedFileSizeBytes { get; set; }

        /// <summary>
        /// The total size of failed files expressed as a friendly string.
        /// </summary>
        public string FailedFileSize
        {
            get
            {
                return GetBytesAsFriendlyString(FailedFileSizeBytes);
            }
        }

        /// <summary>
        /// Returns a byte length formatted as a friendly string.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private string GetBytesAsFriendlyString(long bytes)
        {
            if (bytes < 1048576)
            {
                var dBytes = (double)bytes / 1024;
                return string.Format("{0} KB", dBytes.ToString("0.00"));
            }
            else if (bytes < 1073741824)
            {
                var dBytes = (double)bytes / 1024 / 1024;
                return string.Format("{0} MB", dBytes.ToString("0.00"));
            }
            else if (bytes < 1099511627776)
            {
                var dBytes = (double)bytes / 1024 / 1024 / 1024;
                return string.Format("{0} GB", dBytes.ToString("0.00"));
            }
            else if (bytes < 1125899906842624)
            {
                var dBytes = (double)bytes / 1024 / 1024 / 1024 / 1024;
                return string.Format("{0} TB", dBytes.ToString("0.00"));
            }
            else
            {
                var dBytes = (double)bytes / 1024 / 1024 / 1024 / 1024 / 1024;
                return string.Format("{0} PB", dBytes.ToString("0.00"));
            }
        }
    }
}
