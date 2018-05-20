using OzetteLibrary.Models;

namespace OzetteLibrary.Providers
{
    /// <summary>
    /// Describes the status for a file under any cloud provider.
    /// </summary>
    public class ProviderFileStatus
    {
        /// <summary>
        /// A flag to indicate if the file exists in the cloud provider.
        /// </summary>
        public bool FileExistsInCloudProvider { get; set; }

        /// <summary>
        /// A status enumeration to describe sync state.
        /// </summary>
        public FileStatus SyncStatus { get; set; }
    }
}
