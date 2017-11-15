using OzetteLibrary.Models;
using System;

namespace OzetteLibrary.Database.Mock
{
    /// <summary>
    /// A mock implementation of the client database.
    /// </summary>
    public class MockClientDatabase : IClientDatabase
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
        public ClientFileLookup GetClientFile(string FileName, string DirectoryPath, byte[] FileHash)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the targets defined in the database.
        /// </summary>
        /// <returns><c>Targets</c></returns>
        public Targets GetTargets()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a new client file to the database.
        /// </summary>
        /// <param name="File"><c>ClientFile</c></param>
        public void AddClientFile(ClientFile File)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates an existing client file in the database.
        /// </summary>
        /// <param name="File"><c>ClientFile</c></param>
        public void UpdateClientFile(ClientFile File)
        {
            throw new NotImplementedException();
        }
    }
}
