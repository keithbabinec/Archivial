using System.Collections.Generic;

namespace OzetteLibrary.Models
{
    /// <summary>
    /// A collection of <c>Target</c> objects.
    /// </summary>
    public class ClientFiles : List<ClientFile>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ClientFiles()
        {
        }

        /// <summary>
        /// Constructor that accepts a collection of objects.
        /// </summary>
        /// <param name="collection"></param>
        public ClientFiles(IEnumerable<ClientFile> collection) : base(collection)
        {
        }
    }
}
