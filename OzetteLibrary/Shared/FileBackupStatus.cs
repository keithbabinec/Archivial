namespace OzetteLibrary.Shared
{
    /// <summary>
    /// Describes the possible file backup states.
    /// </summary>
    public enum FileBackupStatus
    {
        /// <summary>
        /// The file has not been backed up.
        /// </summary>
        Unsynced = 0,

        /// <summary>
        /// The file has been backed up previously, but is now out-of-date.
        /// </summary>
        OutOfDate = 1,
        
        /// <summary>
        /// The file is currently being backed up.
        /// </summary>
        InProgress = 2,

        /// <summary>
        /// The file has been synced.
        /// </summary>
        Synced = 3
    }
}
