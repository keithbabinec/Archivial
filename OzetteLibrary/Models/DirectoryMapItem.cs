using System.Collections.Generic;

namespace OzetteLibrary.Models
{
    /// <summary>
    /// A class that describes the local to remote path mapping for a directory.
    /// </summary>
    public class DirectoryMapItem
    {
        /// <summary>
        /// The local directory path.
        /// </summary>
        /// <example>
        /// Ex: 'C:\bin\programs'
        /// </example>
        public string LocalPath { get; set; }

        /// <summary>
        /// A collection of provider paths for where this folder maps to.
        /// </summary>
        /// <example>
        /// Ex: [{ Provider: "ProviderTypes.Azure", RemotePath: "storage-container-c7839365-1c1a-4f41-bc90-2daa26c832cf" }]
        /// </example>
        public List<ProviderPath> ProviderPaths { get; set; }
    }
}
