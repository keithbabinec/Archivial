using OzetteLibrary.Models.Exceptions;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// A diff/compare operation that checks if two SourceLocations collections are the same (ignoring LastCompletedScan).
        /// </summary>
        /// <param name="Locations"></param>
        /// <returns></returns>
        public bool CollectionHasSameContent(SourceLocations Locations)
        {
            var source1 = this;
            var source2 = Locations;

            if (source2 == null)
            {
                return false;
            }
            if (source1.Count != source2.Count)
            {
                return false;
            }

            // sort collections by ID.
            // the source ID property must be unique, so this can be used as sort key to compare objects.

            var sorted1 = source1.OrderBy(x => x.ID).ToList();
            var sorted2 = source2.OrderBy(x => x.ID).ToList();

            for (int i = 0; i < sorted1.Count; i++)
            {
                var a = sorted1[i];
                var b = sorted2[i];

                // diff each property that we care about.
                // we dont want to diff every property, since there are some we don't care about (like LastCompletedScan).

                if (a.ID != b.ID)
                {
                    return false;
                }
                if (a.FolderPath != b.FolderPath)
                {
                    return false;
                }
                if (a.FileMatchFilter != b.FileMatchFilter)
                {
                    return false;
                }
                if (a.Priority != b.Priority)
                {
                    return false;
                }
                if (a.RevisionCount != b.RevisionCount)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
