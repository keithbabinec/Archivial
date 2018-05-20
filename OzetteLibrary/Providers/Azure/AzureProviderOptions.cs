namespace OzetteLibrary.Providers.Azure
{
    /// <summary>
    /// Describes the options required for the Azure cloud storage provider.
    /// </summary>
    public class AzureProviderOptions : ProviderOptions
    {
        /// <summary>
        /// The connection string used to authenticate the storage account.
        /// </summary>
        public string ConnectionString { get; set; }
    }
}
