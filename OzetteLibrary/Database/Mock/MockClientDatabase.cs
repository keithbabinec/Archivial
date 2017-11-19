using OzetteLibrary.Models;
using System.Collections.Generic;

namespace OzetteLibrary.Database.Mock
{
    /// <summary>
    /// A mock implementation of the client database.
    /// </summary>
    public class MockClientDatabase : IClientDatabase
    {
        /// <summary>
        /// A mock database table for client files.
        /// </summary>
        private Dictionary<string, ClientFile> ClientFiles;

        /// <summary>
        /// A mock database table for targets.
        /// </summary>
        private Targets Targets;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MockClientDatabase()
        {
            ClientFiles = new Dictionary<string, ClientFile>();
            Targets = new Targets();

            var target = new Target();
            target.ID = 1;
            target.Name = "remotehost";
            target.Port = 20513;
            target.RootDirectory = "D:\\data\backuptarget";
            target.Url = "remotehost.backups.com";

            Targets.Add(target);
        }

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
            return new ClientFileLookup() { File = null, Result = ClientFileLookupResult.New };
        }

        /// <summary>
        /// Gets the targets defined in the database.
        /// </summary>
        /// <returns><c>Targets</c></returns>
        public Targets GetTargets()
        {
            return this.Targets;
        }

        /// <summary>
        /// Adds a new client file to the database.
        /// </summary>
        /// <param name="File"><c>ClientFile</c></param>
        public void AddClientFile(ClientFile File)
        {
            ClientFiles.Add(File.FullSourcePath, File);
        }

        /// <summary>
        /// Updates an existing client file in the database.
        /// </summary>
        /// <param name="File"><c>ClientFile</c></param>
        public void UpdateClientFile(ClientFile File)
        {
            ClientFiles[File.FullSourcePath] = File;
        }
    }
}
