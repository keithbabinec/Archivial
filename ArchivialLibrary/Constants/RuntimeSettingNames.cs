namespace ArchivialLibrary.Constants
{
    /// <summary>
    /// A constants class for the runtime setting names.
    /// </summary>
    /// <remarks>
    /// Runtime settings are stored in the client database and are not critical to launching the application.
    /// </remarks>
    public class RuntimeSettingNames
    {
        /// <summary>
        /// Option: ProtectionIV.
        /// </summary>
        public const string ProtectionIV = "ProtectionIV";

        /// <summary>
        /// Option: LogFilesRetentionInDays.
        /// </summary>
        public const string LogFilesRetentionInDays = "LogFilesRetentionInDays";

        /// <summary>
        /// Option: DatabaseBackupsRetentionInDays.
        /// </summary>
        public const string DatabaseBackupsRetentionInDays = "DatabaseBackupsRetentionInDays";

        /// <summary>
        /// Option: BackupEngineInstancesCount.
        /// </summary>
        public const string BackupEngineInstancesCount = "BackupEngineInstancesCount";

        /// <summary>
        /// Option: BackupEngineStartupDelayInSeconds.
        /// </summary>
        public const string BackupEngineStartupDelayInSeconds = "BackupEngineStartupDelayInSeconds";

        /// <summary>
        /// Option: CleanupEngineInstancesCount.
        /// </summary>
        public const string CleanupEngineInstancesCount = "CleanupEngineInstancesCount";

        /// <summary>
        /// Option: LowPriorityScanFrequencyInHours.
        /// </summary>
        public const string LowPriorityScanFrequencyInHours = "LowPriorityScanFrequencyInHours";

        /// <summary>
        /// Option: MedPriorityScanFrequencyInHours.
        /// </summary>
        public const string MedPriorityScanFrequencyInHours = "MedPriorityScanFrequencyInHours";

        /// <summary>
        /// Option: HighPriorityScanFrequencyInHours.
        /// </summary>
        public const string HighPriorityScanFrequencyInHours = "HighPriorityScanFrequencyInHours";

        /// <summary>
        /// Option: MetaPriorityScanFrequencyInHours.
        /// </summary>
        public const string MetaPriorityScanFrequencyInHours = "MetaPriorityScanFrequencyInHours";

        /// <summary>
        /// Option: StatusUpdateSchedule.
        /// </summary>
        public const string StatusUpdateSchedule = "StatusUpdateSchedule";

        /// <summary>
        /// Option: AzureStorageAccountName.
        /// </summary>
        public const string AzureStorageAccountName = "AzureStorageAccountName";

        /// <summary>
        /// Option: AzureStorageAccountToken.
        /// </summary>
        public const string AzureStorageAccountToken = "AzureStorageAccountToken";

        /// <summary>
        /// Option: TwilioAccountID.
        /// </summary>
        public const string TwilioAccountID = "TwilioAccountID";

        /// <summary>
        /// Option: TwilioAuthToken.
        /// </summary>
        public const string TwilioAuthToken = "TwilioAuthToken";

        /// <summary>
        /// Option: TwilioSourcePhone.
        /// </summary>
        public const string TwilioSourcePhone = "TwilioSourcePhone";

        /// <summary>
        /// Option: TwilioDestinationPhones.
        /// </summary>
        public const string TwilioDestinationPhones = "TwilioDestinationPhones";

        /// <summary>
        /// Option: MasterExclusionMatches.
        /// </summary>
        public const string MasterExclusionMatches = "MasterExclusionMatches";
    }
}
