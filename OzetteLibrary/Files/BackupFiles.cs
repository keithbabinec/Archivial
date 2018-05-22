using System.Collections.Generic;

namespace OzetteLibrary.Files
{
    /// <summary>
    /// A collection of <c>Target</c> objects.
    /// </summary>
    public class BackupFiles : List<BackupFile>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public BackupFiles()
        {
        }

        /// <summary>
        /// Constructor that accepts a collection of objects.
        /// </summary>
        /// <param name="collection"></param>
        public BackupFiles(IEnumerable<BackupFile> collection) : base(collection)
        {
        }
    }
}
