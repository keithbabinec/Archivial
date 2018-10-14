namespace OzetteLibrary.Files
{
    /// <summary>
    /// Describes the possible file backup states.
    /// </summary>
    public enum FileStatus
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
        Synced = 3,

        /// <summary>
        /// The file has encountered an error at the provider.
        /// </summary>
        ProviderError = 4
    }
}
