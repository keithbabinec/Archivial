using System.Collections.Generic;

namespace OzetteLibrary.Providers
{
    /// <summary>
    /// Describes the options that for cloud storage providers.
    /// </summary>
    public class ProviderOptions
    {
        /// <summary>
        /// The type of cloud storage provider.
        /// </summary>
        public ProviderTypes Type { get; set; }

        /// <summary>
        /// A collection options in key-value pair form.
        /// </summary>
        public Dictionary<string, string> Options { get; set; }
    }
}
