using System.Collections.Generic;

namespace OzetteLibrary.Models
{
    /// <summary>
    /// Describes requested backup source locations.
    /// </summary>
    public class SourceLocations : List<SourceLocation>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SourceLocations()
        {
        }

        /// <summary>
        /// Constructor that accepts a collection input.
        /// </summary>
        /// <param name="collection"></param>
        public SourceLocations(IEnumerable<SourceLocation> collection) : base(collection)
        {
        }
    }
}
