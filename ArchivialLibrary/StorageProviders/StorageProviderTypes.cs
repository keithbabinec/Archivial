namespace ArchivialLibrary.StorageProviders
{
    /// <summary>
    /// Describes possible target cloud providers.
    /// </summary>
    public enum StorageProviderTypes
    {
        /// <summary>
        /// Microsoft Azure cloud storage.
        /// </summary>
        Azure = 1,

        /// <summary>
        /// Amazon Web Services cloud storage.
        /// </summary>
        AWS = 2,

        /// <summary>
        /// Google cloud storage provider.
        /// </summary>
        Google = 3
    }
}
