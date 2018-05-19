using System.Collections.Generic;

namespace OzetteLibrary.Models
{
    /// <summary>
    /// Contains the result of a Client file lookup.
    /// </summary>
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
