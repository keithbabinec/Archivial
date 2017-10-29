using OzetteLibrary.Models.Exceptions;

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
