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
        /// Event ID: Failed to write to the trace log.
        /// </summary>
        public const int FailedToWriteToTraceLog = 2000;
    }
}
