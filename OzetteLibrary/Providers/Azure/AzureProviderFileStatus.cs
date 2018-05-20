using System.Collections.Generic;

namespace OzetteLibrary.Providers.Azure
{
    /// <summary>
    /// Contains Azure-specific file state information.
    /// </summary>
    public class AzureProviderFileStatus : ProviderFileStatus
    {
        /// <summary>
        /// A collection of metadata properties assigned to the file in Azure.
        /// </summary>
        public IDictionary<string, string> Metadata { get; set; }
    }
}
