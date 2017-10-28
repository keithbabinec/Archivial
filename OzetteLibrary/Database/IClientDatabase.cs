using OzetteLibrary.Models;

namespace OzetteLibrary.Database
{
    /// <summary>
    /// A generic database interface for the client database.
    /// </summary>
    public interface IClientDatabase : IDatabase
    {
        /// <summary>
        /// Gets a client file from the database by full name/path. Returns null if file is not found.
        /// </summary>
        /// <param name="FullName">The full filepath.</param>
        /// <returns><c>ClientFile</c></returns>
        ClientFile GetClientFile(string FullName);

        /// <summary>
        /// Gets the targets defined in the database.
        /// </summary>
        /// <returns><c>Targets</c></returns>
        Targets GetTargets();

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
