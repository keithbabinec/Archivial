namespace OzetteLibrary.CommandLine.Arguments
{
    /// <summary>
    /// A set of Ozette Azure configuration arguments.
    /// </summary>
    public class ConfigureAzureArguments : ArgumentBase
    {
        /// <summary>
        /// The Azure cloud storage account name.
        /// </summary>
        public string AzureStorageAccountName { get; set; }

        /// <summary>
        /// The Azure cloud storage account access token.
        /// </summary>
        public string AzureStorageAccountToken { get; set; }
    }
}
