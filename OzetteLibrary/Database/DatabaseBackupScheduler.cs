using System;

namespace OzetteLibrary.Database
{
    /// <summary>
    /// Contains functionality for scheduling backups.
    /// </summary>
    public class DatabaseBackupScheduler
    {
        /// <summary>
        /// Determines which backup can be performed, if any, based on the recent backup status.
        /// </summary>
        /// <param name="RecentBackups"></param>
        /// <returns></returns>
        public DatabaseBackupType? NextDatabaseBackup(DatabaseBackupStatus RecentBackups)
        {
            throw new NotImplementedException();
        }
    }
}
