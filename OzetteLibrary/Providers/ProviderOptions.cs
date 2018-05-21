using Microsoft.WindowsAzure.Storage;
using OzetteLibrary.Models.Exceptions;
using System;
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

        /// <summary>
        /// Validates that the provider options are usable.
        /// </summary>
        public void Validate()
        {
            if (Options == null || Options.Count == 0)
            {
                throw new ProviderOptionsException("No provider-specific options were specified.");
            }

            if (Type == ProviderTypes.Azure)
            {
                ValidateAzureOptions();
            }
            else
            {
                throw new NotImplementedException("Unexpected provider option type.");
            }
        }

        /// <summary>
        /// Validates Azure-specific provider options.
        /// </summary>
        private void ValidateAzureOptions()
        {
            if (Options.ContainsKey("ConnectionString") == false || string.IsNullOrWhiteSpace(Options["ConnectionString"]))
            {
                throw new ProviderOptionsException("Azure Provider: Expected option was missing: ConnectionString");
            }

            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Options["ConnectionString"]);
            }
            catch (Exception)
            {
                throw new ProviderOptionsException("Azure Provider: ConnectionString argument was not a valid connection string.");
            }
        }
    }
}
