namespace ArchivialLibrary.StorageProviders
{
    /// <summary>
    /// Describes a cloud storage provider.
    /// </summary>
    public class StorageProvider
    {
        /// <summary>
        /// The internal database ID of this provider.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The provider type.
        /// </summary>
        public StorageProviderTypes Type { get; set; }
    }
}
