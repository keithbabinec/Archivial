using System.Collections.Generic;

namespace OzetteLibrary.Models
{
    /// <summary>
    /// A collection of <c>Target</c> objects.
    /// </summary>
    public class Targets : List<Target>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Targets()
        {
        }

        /// <summary>
        /// Constructor that accepts a collection of objects.
        /// </summary>
        /// <param name="collection"></param>
        public Targets(IEnumerable<Target> collection) : base(collection)
        {
        }
    }
}
