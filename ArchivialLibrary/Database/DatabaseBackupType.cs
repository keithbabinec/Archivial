namespace OzetteLibrary.Database
{
    /// <summary>
    /// Describes the different types of database backups that can be performed.
    /// </summary>
    public enum DatabaseBackupType
    {
        /// <summary>
        /// Take a full database backup.
        /// </summary>
        Full = 1,

        /// <summary>
        /// Take a diffential database backup.
        /// </summary>
        Differential = 2,

        /// <summary>
        /// Take a transaction log backup.
        /// </summary>
        TransactionLog = 3
    }
}
