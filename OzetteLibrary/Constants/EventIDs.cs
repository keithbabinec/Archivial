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
        /// Event ID: Started scanning engine.
        /// </summary>
        public const int StartedScanEngine = 1007;

        /// <summary>
        /// Event ID: Started backup engine.
        /// </summary>
        public const int StartedBackupEngine = 1008;

        /// <summary>
        /// Event ID: Configuring cloud provider connections.
        /// </summary>
        public const int ConfiguringCloudProviderConnections = 1009;

        /// <summary>
        /// Event ID: Configured cloud provider connections.
        /// </summary>
        public const int ConfiguredCloudProviderConnections = 1010;

        /// <summary>
        /// Event ID: Started connection engine.
        /// </summary>
        public const int StartedConnectionEngine = 1011;

        /// <summary>
        /// Event ID: Stopped connection engine.
        /// </summary>
        public const int StoppedConnectionEngine = 1012;

        /// <summary>
        /// Event ID: Started status engine.
        /// </summary>
        public const int StartedStatusEngine = 1013;

        /// <summary>
        /// Event ID: Stopped status engine.
        /// </summary>
        public const int StoppedStatusEngine = 1014;

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

        /// <summary>
        /// Event ID: Failed to grab the next file that needs to be backed up.
        /// </summary>
        public const int FailedToGetNextFileToBackup = 2005;

        /// <summary>
        /// Event ID: Failed to load provider options.
        /// </summary>
        public const int FailedToLoadProviderOptions = 2006;

        /// <summary>
        /// Event ID: Failed to validate provider options.
        /// </summary>
        public const int FailedToValidateProviderOptions = 2007;

        /// <summary>
        /// Event ID: Failed to configure cloud provider connections.
        /// </summary>
        public const int FailedToConfigureCloudProviderConnections = 2008;

        /// <summary>
        /// Event ID: Failed to configure cloud provider connections due to missing settings.
        /// </summary>
        public const int FailedToConfigureProvidersMissingSettings = 2009;

        /// <summary>
        /// Event ID: Failed to configure cloud provider connections due to no providers being listed in the database.
        /// </summary>
        public const int FailedToConfigureProvidersNotFound = 2010;

        /// <summary>
        /// Event ID: A core application setting is missing. 
        /// </summary>
        public const int CoreSettingMissing = 2011;

        /// <summary>
        /// Event ID: Failed connection engine.
        /// </summary>
        public const int FailedConnectionEngine = 2012;

        /// <summary>
        /// Event ID: Failed to save scan sources.
        /// </summary>
        public const int FailedToSaveScanSources = 2013;

        /// <summary>
        /// Event ID: Failed status engine.
        /// </summary>
        public const int FailedStatusEngine = 2014;
    }
}
