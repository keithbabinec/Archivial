using System;

namespace OzetteLibrary.Database
{
    /// <summary>
    /// Describes when the database backup was last performed.
    /// </summary>
    public class DatabaseBackupStatus
    {
        /// <summary>
        /// The last time a full backup was taken.
        /// </summary>
        public DateTime? LastFullBackup { get; set; }

        /// <summary>
        /// The last time a differential backup was taken.
        /// </summary>
        public DateTime? LastDifferentialBackup { get; set; }

        /// <summary>
        /// The last time a transaction log backup was taken.
        /// </summary>
        public DateTime? LastTransactionLogBackup { get; set; }
    }
}
