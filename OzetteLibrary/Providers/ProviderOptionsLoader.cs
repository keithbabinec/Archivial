using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace OzetteLibrary.Providers
{
    /// <summary>
    /// Contains functionality for loading Provider Options from disk.
    /// </summary>
    public class ProviderOptionsLoader
    {
        /// <summary>
        /// Loads a provider options file from disk.
        /// </summary>
        /// <param name="providerOptionsFilePath">The file path</param>
        /// <returns>A list of provider options</returns>
        public List<ProviderOptions> LoadOptionsFile(string providerOptionsFilePath)
        {
            if (string.IsNullOrEmpty(providerOptionsFilePath))
            {
                throw new ArgumentException(providerOptionsFilePath);
            }

            if (File.Exists(providerOptionsFilePath) == false)
            {
                throw new FileNotFoundException(providerOptionsFilePath);
            }

            return JsonConvert.DeserializeObject<List<ProviderOptions>>(File.ReadAllText(providerOptionsFilePath));
        }
    }
}
