using OzetteLibrary.Models;

namespace OzetteLibrary.Database
{
    /// <summary>
    /// A generic database interface for the client database.
    /// </summary>
    public interface IClientDatabase : IDatabase
    {
        /// <summary>
        /// Checks the index for a file matching the provided name, path, and hash.
        /// </summary>
        /// <remarks>
        /// The lookup result object is returned that contains:
        /// 1. A reference to the indexed file, if present.
        /// 2. An enumeration that describes the file state (new, updated, moved, renamed, etc).
        /// </remarks>
        /// <param name="FileName">Name of the file (ex: document.doc)</param>
        /// <param name="DirectoryPath">Full directory path (ex: C:\folder\documents)</param>
        /// <param name="FileHash">File hash expressed as a byte array.</param>
        /// <returns><c>ClientFileLookup</c></returns>
        ClientFileLookup GetClientFile(string FileName, string DirectoryPath, byte[] FileHash);

        /// <summary>
        /// Returns all of the client files in the database.
        /// </summary>
        /// <returns><c>ClientFiles</c></returns>
        ClientFiles GetAllClientFiles();

        /// <summary>
        /// Returns all of the targets defined in the database.
        /// </summary>
        /// <returns><c>Targets</c></returns>
        Targets GetAllTargets();

        /// <summary>
        /// Adds a single target to the database.
        /// </summary>
        /// <param name="Target"></param>
        void AddTarget(Target Target);

        /// <summary>
        /// Returns all source locations defined in the database.
        /// </summary>
        /// <returns><c>SourceLocations</c></returns>
        SourceLocations GetAllSourceLocations();

        /// <summary>
        /// Sets a new source locations collection in the database (this will wipe out existing sources).
        /// </summary>
        /// <param name="Locations"><c>SourceLocations</c></param>
        void SetSourceLocations(SourceLocations Locations);

        /// <summary>
        /// Updates a single source location with the specified source.
        /// </summary>
        /// <param name="Location"><c>SourceLocation</c></param>
        void UpdateSourceLocation(SourceLocation Location);

        /// <summary>
        /// Adds a new client file to the database.
        /// </summary>
        /// <param name="File"><c>ClientFile</c></param>
        void AddClientFile(ClientFile File);

        /// <summary>
        /// Updates an existing client file in the database.
        /// </summary>
        /// <param name="File"><c>ClientFile</c></param>
        void UpdateClientFile(ClientFile File);
    }
}
