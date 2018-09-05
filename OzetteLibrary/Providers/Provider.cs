namespace OzetteLibrary.Providers
{
    /// <summary>
    /// Describes a cloud storage provider.
    /// </summary>
    public class Provider
    {
        /// <summary>
        /// The internal database ID of this provider.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The provider type.
        /// </summary>
        public ProviderTypes Type { get; set; }

        /// <summary>
        /// A flag to indicate if the provider is enabled.
        /// </summary>
        public bool Enabled { get; set; }
    }
}
