using Newtonsoft.Json;
using OzetteLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        public SourceLocations LoadSourcesFile(string sourcesFilePath)
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
        
        /// <summary>
        /// Sorts a collection of source locations by priority.
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        public SourceLocations SortSources(SourceLocations sources)
        {
            return new SourceLocations(sources.OrderByDescending(x => (int)x.Priority).ToList());
        }
    }
}
