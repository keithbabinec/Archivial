namespace OzetteLibrary.Constants
{
    /// <summary>
    /// A constants class that contains event log IDs.
    /// </summary>
    public static class EventIDs
    {
        /// <summary>
        /// Event ID: Starting the service.
        /// </summary>
        public const int StartingService = 1000;

        /// <summary>
        /// Event ID: Started service event.
        /// </summary>
        public const int StartedService = 1001;

        /// <summary>
        /// Event ID: Stopping the service.
        /// </summary>
        public const int StoppingService = 1002;

        /// <summary>
        /// Event ID: Stopped service event.
        /// </summary>
        public const int StoppedService = 1003;

        /// <summary>
        /// Event ID: Stopped scanning engine.
        /// </summary>
        public const int StoppedScanEngine = 1004;

        /// <summary>
        /// Event ID: Stopped backup engine.
        /// </summary>
        public const int StoppedBackupEngine = 1005;

        /// <summary>
        /// Event ID: Logging initialized.
        /// </summary>
        public const int LoggingInitialized = 1006;

        /// <summary>
        /// Event ID: Failed to write to the trace log.
        /// </summary>
        public const int FailedToWriteToTraceLog = 2000;

        /// <summary>
        /// Event ID: Failed to load scan sources.
        /// </summary>
        public const int FailedToLoadScanSources = 2001;

        /// <summary>
        /// Event ID: Failed to validate scan sources.
        /// </summary>
        public const int FailedToValidateScanSources = 2002;

        /// <summary>
        /// Event ID: Failed scanning engine.
        /// </summary>
        public const int FailedScanEngine = 2003;

        /// <summary>
        /// Event ID: Failed backup engine.
        /// </summary>
        public const int FailedBackupEngine = 2004;
    }
}
