using System.Collections.Generic;

namespace OzetteLibrary.Providers
{
    /// <summary>
    /// A collection object that contains <c>Provider</c> objects.
    /// </summary>
    public class ProviderCollection : List<Provider>
    {
        /// <summary>
        /// Base constructor.
        /// </summary>
        public ProviderCollection()
        {
        }

        /// <summary>
        /// A constructor that accepts a collection.
        /// </summary>
        /// <param name="collection"></param>
        public ProviderCollection(IEnumerable<Provider> collection) : base(collection)
        {
        }
    }
}
