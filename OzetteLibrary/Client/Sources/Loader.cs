using Newtonsoft.Json;
using OzetteLibrary.Models;
using System;
using System.IO;

namespace OzetteLibrary.Client.Sources
{
    /// <summary>
    /// Contains functionality for loading SourceLocations.
    /// </summary>
    public class Loader
    {
        /// <summary>
        /// Loads a sources file from disk.
        /// </summary>
        /// <param name="sourcesFilePath"></param>
        /// <returns></returns>
        public SourceLocations LoadSources(string sourcesFilePath)
        {
            if (string.IsNullOrEmpty(sourcesFilePath))
            {
                throw new ArgumentException(sourcesFilePath);
            }

            if (File.Exists(sourcesFilePath) == false)
            {
                throw new FileNotFoundException(sourcesFilePath);
            }

            return JsonConvert.DeserializeObject<SourceLocations>(File.ReadAllText(sourcesFilePath));
        }
    }
}
