using System.Collections.Generic;

namespace OzetteLibrary.Secrets
{
    /// <summary>
    /// A collection of <c>Provider</c> objects.
    /// </summary>
    public class NetCredentialsCollection : List<NetCredential>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public NetCredentialsCollection()
        {
        }

        /// <summary>
        /// Constructor that accepts a collection.
        /// </summary>
        /// <param name="collection"></param>
        public NetCredentialsCollection(IEnumerable<NetCredential> collection) : base(collection)
        {
        }
    }
}
