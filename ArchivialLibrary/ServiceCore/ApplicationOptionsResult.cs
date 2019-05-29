namespace ArchivialLibrary.ServiceCore
{
    /// <summary>
    /// A results class for the configured application options.
    /// </summary>
    public class ApplicationOptionsResult
    {
        /// <summary>
        /// Option: HighPriorityScanFrequencyInHours
        /// </summary>
        public int HighPriorityScanFrequencyInHours { get; set; }

        /// <summary>
        /// Option: ProtectionIV
        /// </summary>
        public string ProtectionIV { get; set; }

        /// <summary>
        /// Option: StatusUpdateSchedule
        /// </summary>
        public string StatusUpdateSchedule { get; set; }

        /// <summary>
        /// Option: MedPriorityScanFrequencyInHours
        /// </summary>
        public int MedPriorityScanFrequencyInHours { get; set; }

        /// <summary>
        /// Option: LowPriorityScanFrequencyInHours
        /// </summary>
        public int LowPriorityScanFrequencyInHours { get; set; }

        /// <summary>
        /// Option: BackupEngineInstancesCount
        /// </summary>
        public int BackupEngineInstancesCount { get; set; }

        /// <summary>
        /// Option: BackupEngineStartupDelayInSeconds
        /// </summary>
        public int BackupEngineStartupDelayInSeconds { get; set; }

        /// <summary>
        /// Option: DatabaseBackupsRetentionInDays
        /// </summary>
        public int DatabaseBackupsRetentionInDays { get; set; }

        /// <summary>
        /// Option: LogFilesRetentionInDays
        /// </summary>
        public int LogFilesRetentionInDays { get; set; }

        /// <summary>
        /// Option: MasterExclusionMatches
        /// </summary>
        public string[] MasterExclusionMatches { get; set; }
    }
}
