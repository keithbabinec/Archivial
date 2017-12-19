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
            SourceLocations sorted = new SourceLocations();

            var high = new SourceLocations();
            var med = new SourceLocations();
            var low = new SourceLocations();

            for (int i = 0; i < sources.Count; i++)
            {
                if (sources[i].Priority == FileBackupPriority.High)
                {
                    high.Add(sources[i]);
                }
                else if (sources[i].Priority == FileBackupPriority.Medium)
                {
                    med.Add(sources[i]);
                }
                else if (sources[i].Priority == FileBackupPriority.Low)
                {
                    low.Add(sources[i]);
                }
                else
                {
                    throw new InvalidOperationException("Unexpected source priority specified: " + sources[i].Priority);
                }
            }

            for (int i = 0; i < high.Count; i++)
            {
                sorted.Add(high[i]);
            }
            for (int i = 0; i < med.Count; i++)
            {
                sorted.Add(med[i]);
            }
            for (int i = 0; i < low.Count; i++)
            {
                sorted.Add(low[i]);
            }

            return sorted;
        }
    }
}
