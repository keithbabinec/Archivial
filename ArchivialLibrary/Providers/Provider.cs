namespace ArchivialLibrary.Providers
{
    /// <summary>
    /// Describes the basic information about a provider.
    /// </summary>
    public class Provider
    {
        /// <summary>
        /// Database identifier.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Type of provider.
        /// </summary>
        public ProviderTypes Type { get; set; }

        /// <summary>
        /// Name of the provider.
        /// </summary>
        public string Name { get; set; }
    }
}
