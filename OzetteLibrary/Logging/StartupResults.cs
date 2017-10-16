namespace OzetteLibrary.Logging
{
    /// <summary>
    /// Describes possible startup/initialization results.
    /// </summary>
    public enum StartupResults
    {
        /// <summary>
        /// Has not yet completed or has not been set.
        /// </summary>
        NotSet = -1,

        /// <summary>
        /// Completed successfully.
        /// </summary>
        Success = 0,

        /// <summary>
        /// Failed to parse the service options.
        /// </summary>
        FailedToParseServiceOptions = 10,

        /// <summary>
        /// Failed to configure the logs folder.
        /// </summary>
        FailedToConfigureLogsFolder = 11,

        /// <summary>
        /// Failed to configure the custom event log.
        /// </summary>
        FailedToConfigureCustomEventLog = 12,
    }
}
