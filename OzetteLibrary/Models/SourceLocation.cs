using OzetteLibrary.Models.Exceptions;
using OzetteLibrary.ServiceCore;
using System;

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
        public bool ShouldScan(ServiceOptions options)
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
        public override string ToString()
        {
            return string.Format("Path='{0}', Filter='{1}', Priority='{2}', RevisionCount='{3}'",
                FolderPath,
                FileMatchFilter == null ? "(none)" : FileMatchFilter,
                Priority,
                RevisionCount);
        }

        /// <summary>
        /// Validates that a source configuration is usable.
        /// </summary>
        public void Validate()
        {
            ValidateFolderPath();
            ValidateFileMatchFilter();
            ValidateRevisionCount();
        }

        /// <summary>
        /// Validates that the folder path is usable.
        /// </summary>
        private void ValidateFolderPath()
        {
            if (string.IsNullOrWhiteSpace(FolderPath))
            {
                throw new SourceLocationInvalidFolderPathException();
            }
            if (!System.IO.Directory.Exists(FolderPath))
            {
                throw new SourceLocationInvalidFolderPathException();
            }
        }

        /// <summary>
        /// Validates that a file match filter is usable.
        /// </summary>
        private void ValidateFileMatchFilter()
        {
            if (FileMatchFilter != null && FileMatchFilter.Length > 0)
            {
                if (!FileMatchFilter.Contains("*") && !FileMatchFilter.Contains("?"))
                {
                    // filter was provided, but has no wildcard.
                    throw new SourceLocationInvalidFileMatchFilterException();
                }
            }
        }

        /// <summary>
        /// Validates that a file revision count setting is usable.
        /// </summary>
        private void ValidateRevisionCount()
        {
            if (RevisionCount <= 0)
            {
                throw new SourceLocationInvalidRevisionCountException();
            }
        }
    }
}
