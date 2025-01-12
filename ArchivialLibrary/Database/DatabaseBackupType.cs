namespace ArchivialLibrary.Database
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
        Differential = 2
    }
}
