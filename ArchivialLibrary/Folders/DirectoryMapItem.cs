using ArchivialLibrary.StorageProviders;
using System;

namespace ArchivialLibrary.Folders
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
        /// The unique identifier for this directory.
        /// </summary>
        /// <remarks>
        /// This ID is used to calculate the remote path for each cloud storage provider.
        /// For example: 'container-c7839365-1c1a-4f41-bc90-2daa26c832cf'.
        /// </remarks>
        public Guid ID { get; set; }

        /// <summary>
        /// Calculates the provider-specific remote path. 
        /// </summary>
        /// <param name="Provider"></param>
        /// <returns></returns>
        public string GetRemoteContainerName(StorageProviderTypes Provider)
        {
            // Different cloud providers may have different naming rules for URIs.
            // Azure for example is all lowercase required.

            if (ID == Guid.Empty)
            {
                throw new InvalidOperationException("Cannot generate container name. Directory ID has not been set.");
            }

            if (Provider == StorageProviderTypes.Azure)
            {
                return string.Format("{0}-directory-{1}", Constants.Logging.AppName, ID.ToString()).ToLower();
            }
            else
            {
                throw new NotImplementedException("unexpected provider type: " + Provider.ToString());
            }
        }
    }
}
