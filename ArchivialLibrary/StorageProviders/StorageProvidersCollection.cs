using System.Collections.Generic;

namespace ArchivialLibrary.StorageProviders
{
    /// <summary>
    /// A collection of <c>Provider</c> objects.
    /// </summary>
    public class StorageProvidersCollection : List<StorageProvider>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public StorageProvidersCollection()
        {
        }

        /// <summary>
        /// Constructor that accepts a collection.
        /// </summary>
        /// <param name="collection"></param>
        public StorageProvidersCollection(IEnumerable<StorageProvider> collection) : base(collection)
        {
        }
    }
}
