using System.Collections.Generic;

namespace OzetteLibrary.Providers
{
    /// <summary>
    /// A collection of <c>Provider</c> objects.
    /// </summary>
    public class ProvidersCollection : List<Provider>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProvidersCollection()
        {
        }

        /// <summary>
        /// Constructor that accepts a collection.
        /// </summary>
        /// <param name="collection"></param>
        public ProvidersCollection(IEnumerable<Provider> collection) : base(collection)
        {
        }
    }
}
