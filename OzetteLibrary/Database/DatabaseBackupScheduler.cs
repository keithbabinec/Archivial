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
        /// <remarks>
        /// Backup rules:
        /// - Full Backup every 24 hours.
        /// - Diff Backup every 4 hours.
        /// - Log Backup every 30 minutes.
        /// </remarks>
        /// <param name="RecentBackups"></param>
        /// <returns></returns>
        public DatabaseBackupType? NextDatabaseBackup(DatabaseBackupStatus RecentBackups)
        {
            // When a backup is performed, all backups in lower tiers are flagged as completed for that timestamp.
            // Completing a full backup flags Full, Diff, and Log backups as completed at the same time.
            // Completing a diff backup flags the Diff and Log backups as completed at that time.
            // This 'resets' the clock to make for an simpler algorithm.

            if (RecentBackups.LastFullBackup == null 
                || RecentBackups.LastDifferentialBackup == null
                || RecentBackups.LastTransactionLogBackup == null)
            {
                return DatabaseBackupType.Full;
            }

            var now = DateTime.Now;

            if (RecentBackups.LastFullBackup.Value >= now.AddHours(24))
            {
                return DatabaseBackupType.Full;
            }

            if (RecentBackups.LastDifferentialBackup.Value >= now.AddHours(4))
            {
                return DatabaseBackupType.Differential;
            }

            if (RecentBackups.LastTransactionLogBackup.Value >= now.AddMinutes(30))
            {
                return DatabaseBackupType.TransactionLog;
            }

            return null;
        }
    }
}
