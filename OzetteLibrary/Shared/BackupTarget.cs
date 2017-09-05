using System;

namespace OzetteLibrary.Shared
{
    /// <summary>
    /// Contains data and functionality for a single backup target.
    /// </summary>
    public class BackupTarget
    {
        /// <summary>
        /// The backup target's unique identifier.
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// The name of the backup target.
        /// </summary>
        public string Name { get; set; }
    }
}
