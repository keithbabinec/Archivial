using System.Collections.Generic;

namespace ArchivialLibrary.Files
{
    /// <summary>
    /// Contains the result of a backup file lookup.
    /// </summary>
    /// <remarks>
    /// This result is used internally by the scanning engine to return file lookup results
    /// that determined if the file exists in the index as unchanged, changed, or new to the index.
    /// </remarks>
    public class BackupFileLookup
    {
        /// <summary>
        /// The resulting backup file, if found, otherwise null.
        /// </summary>
        public BackupFile File { get; set; }

        /// <summary>
        /// Contains a list of partial matches, if more than one duplicate is found.
        /// </summary>
        public List<BackupFile> PartialMatches { get; set; }

        /// <summary>
        /// The result of the lookup operation.
        /// </summary>
        public BackupFileLookupResult Result { get; set; }
    }
}
