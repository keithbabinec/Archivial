namespace OzetteLibrary.ServiceCore
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
        /// Failed to configure the logging.
        /// </summary>
        FailedToConfigureLogging = 11
    }
}
