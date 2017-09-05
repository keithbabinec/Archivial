using System.Collections.Generic;

namespace OzetteLibrary.Shared
{
    /// <summary>
    /// Maintains an index of the files on the system to be synced.
    /// </summary>
    public class BackupIndex
    {
        /// <summary>
        /// A dictionary lookup of the files by source path.
        /// </summary>
        public Dictionary<string, File> FilesBySourcePath { get; set; }

        /// <summary>
        /// A dictionary lookup of the files by hash string.
        /// </summary>
        public Dictionary<string, File> FilesByHashString { get; set; }
    }
}
