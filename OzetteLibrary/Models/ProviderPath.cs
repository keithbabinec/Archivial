namespace OzetteLibrary.Models
{
    /// <summary>
    /// Describes a single cloud storage provider's container/storage path.
    /// </summary>
    public class ProviderPath
    {
        /// <summary>
        /// The remote directory path.
        /// </summary>
        /// <example>
        /// Ex: 'storage-container-c7839365-1c1a-4f41-bc90-2daa26c832cf'
        /// </example>
        public string RemotePath { get; set; }

        /// <summary>
        /// The type of cloud storage provider this entry maps to.
        /// </summary>
        /// <example>
        /// Ex: 'ProviderTypes.Azure'
        /// </example>
        public ProviderTypes Provider { get; set; }
    }
}
