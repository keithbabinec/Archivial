using OzetteLibrary.Exceptions;
using OzetteLibrary.Files;
using System;

namespace OzetteLibrary.Folders
{
    /// <summary>
    /// Describes an abstract requested source backup location.
    /// </summary>
    public abstract class SourceLocation
    {
        /// <summary>
        /// An identifier for the database.
        /// </summary>
        public int ID { get; set; }

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

        /// <summary>
        /// The last time a scan of this source was completed.
        /// </summary>
        public DateTime? LastCompletedScan { get; set; }

        /// <summary>
        /// Checks to see if a source location is ready to scan again.
        /// </summary>
        /// <remarks>
        /// The source location is ready to scan again once enough time has elapsed since the previous scan.
        /// </remarks>
        /// <returns></returns>
        public bool ShouldScan(ScanFrequencies options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (LastCompletedScan == null)
            {
                // no scan has completed before
                return true;
            }

            DateTime lastAcceptableScanPoint;

            if (Priority == FileBackupPriority.Low)
            {
                lastAcceptableScanPoint = DateTime.Now.AddHours(-options.LowPriorityScanFrequencyInHours);
            }
            else if (Priority == FileBackupPriority.Medium)
            {
                lastAcceptableScanPoint = DateTime.Now.AddHours(-options.MedPriorityScanFrequencyInHours);
            }
            else if (Priority == FileBackupPriority.High)
            {
                lastAcceptableScanPoint = DateTime.Now.AddHours(-options.HighPriorityScanFrequencyInHours);
            }
            else
            {
                throw new InvalidOperationException("Unexpected backup priority specified: " + Priority);
            }

            if (LastCompletedScan.Value < lastAcceptableScanPoint)
            {
                // a scan hasn't been completed recently.
                return true;
            }
            else
            {
                // scan already completed recently.
                return false;
            }
        }

        /// <summary>
        /// Formats the Source Location string properties.
        /// </summary>
        /// <returns></returns>
        public abstract override string ToString();

        /// <summary>
        /// Validates that source configuration parameters are valid, but does not validate if the source exists on disk.
        /// </summary>
        public abstract void ValidateParameters();

        /// <summary>
        /// Validates that a file match filter is usable.
        /// </summary>
        internal void ValidateFileMatchFilter()
        {
            if (FileMatchFilter != null && FileMatchFilter.Length > 0)
            {
                if (!FileMatchFilter.Contains("*") && !FileMatchFilter.Contains("?"))
                {
                    // filter was provided, but has no wildcard.
                    throw new SourceLocationInvalidFileMatchFilterException(this.ToString());
                }
            }
        }

        /// <summary>
        /// Validates that a file revision count setting is usable.
        /// </summary>
        internal void ValidateRevisionCount()
        {
            if (RevisionCount <= 0)
            {
                throw new SourceLocationInvalidRevisionCountException(this.ToString());
            }
        }

        /// <summary>
        /// Validates that a source location ID is valid/usable.
        /// </summary>
        internal void ValidateID()
        {
            if (ID <= 0)
            {
                throw new SourceLocationInvalidIDException(this.ToString());
            }
        }
    }
}
