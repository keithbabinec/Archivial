namespace OzetteLibrary.Models
{
    /// <summary>
    /// Describes the possible results of a file lookup attempt.
    /// </summary>
    /// <remarks>
    /// This result is returned when looking up a file in the database index.
    /// The lookup included 3-factors: file name, file location (directory), and file hash.
    /// Depending on how these factors have changed or not changed, results in one of the listed states below.
    /// </remarks>
    public enum ClientFileLookupResult
    {
        /// <summary>
        /// The file is new to the database.
        /// </summary>
        /// <remarks>
        /// Lookup combination:
        /// > { Filename mismatch, Location mismatch, Hash mismatch }
        /// </remarks>
        New = 0,

        /// <summary>
        /// This file is already indexed.
        /// </summary>
        /// <remarks>
        /// Lookup combination:
        /// > { Filename matched, Location matched, Hash matched }
        /// </remarks>
        Existing = 1,

        /// <summary>
        /// The file is in the same place, but has been updated.
        /// </summary>
        /// <remarks>
        /// Lookup combination:
        /// > { Filename matched, Location matched, Hash mismatch }
        /// </remarks>
        Updated = 2,

        /// <summary>
        /// File contents match another filename or different location.
        /// </summary>
        /// <remarks>
        /// Lookup combination:
        /// > { Filename mismatch, Location matched, Hash matched }
        /// OR
        /// > { Filename matched, Location mismatch, Hash matched }
        /// </remarks>
        Duplicate = 3,
    }
}
