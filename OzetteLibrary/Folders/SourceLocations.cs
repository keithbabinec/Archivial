using OzetteLibrary.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace OzetteLibrary.Folders
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

        /// <summary>
        /// Validates all of the items in this collection.
        /// </summary>
        public void Validate()
        {
            if (Count > 0)
            {
                HashSet<int> definedIDs = new HashSet<int>();

                foreach (var item in this)
                {
                    item.Validate();

                    if (definedIDs.Contains(item.ID) == false)
                    {
                        definedIDs.Add(item.ID);
                    }
                    else
                    {
                        throw new SourceLocationsDuplicateIDException(item.ToString());
                    }
                }
            }
        }
    }
}
