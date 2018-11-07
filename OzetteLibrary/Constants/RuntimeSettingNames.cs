namespace OzetteLibrary.Constants
{
    /// <summary>
    /// A constants class for the runtime setting names.
    /// </summary>
    /// <remarks>
    /// Runtime settings are stored in the client database and are not critical to launching the application.
    /// </remarks>
    public static class RuntimeSettingNames
    {
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
        /// Option: SourcesFilePath.
        /// </summary>
        public const string SourcesFilePath = "SourcesFilePath";

        /// <summary>
        /// Option: ProvidersFilePath.
        /// </summary>
        public const string ProvidersFilePath = "ProvidersFilePath";

        /// <summary>
        /// Option: AzureStorageAccountName.
        /// </summary>
        public const string AzureStorageAccountName = "AzureStorageAccountName";

        /// <summary>
        /// Option: AzureStorageAccountToken.
        /// </summary>
        public const string AzureStorageAccountToken = "AzureStorageAccountToken";
    }
}
