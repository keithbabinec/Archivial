using OzetteLibrary.Logging;
using OzetteLibrary.Models;
using System;
using System.IO;

namespace OzetteLibrary.Database.LiteDB
{
    /// <summary>
    /// A LiteDB implementation of the client database.
    /// </summary>
    public class LiteDBClientDatabase : IClientDatabase
    {
        /// <summary>
        /// Instantiates a client DB from memory stream.
        /// </summary>
        /// <remarks>
        /// The memory stream constructor is typically used for unit testing.
        /// </remarks>
        /// <param name="databaseStream"></param>
        /// <param name="logger"></param>
        public LiteDBClientDatabase(MemoryStream databaseStream, ILogger logger)
        {
            if (databaseStream == null)
            {
                throw new ArgumentNullException(nameof(databaseStream));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            DatabaseMemoryStream = databaseStream;
            Logger = logger;
        }

        /// <summary>
        /// Instantiates a client DB from database connection string.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="logger"></param>
        public LiteDBClientDatabase(string connectionString, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(nameof(connectionString));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            DatabaseConnectionString = connectionString;
            Logger = logger;
        }

        /// <summary>
        /// Runs database preparation steps.
        /// </summary>
        /// <remarks>
        /// This step prepares any tables, indexes, identity mappings, etc. for use.
        /// This action is idempotent (safe to run over and over), but should be run once at each application startup.
        /// </remarks>
        public void PrepareDatabase()
        {
            ConfigureDatabaseTables();
            ConfigureDatabaseIndexes();
            ConfigureDatabaseIdentityMappings();

            DatabaseHasBeenPrepared = true;
        }

        /// <summary>
        /// Ensures database tables are present (create if missing).
        /// </summary>
        private void ConfigureDatabaseTables()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Ensures database indexes are enabled (create if missing).
        /// </summary>
        private void ConfigureDatabaseIndexes()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates class-object identity mappings.
        /// </summary>
        /// <remarks>
        /// This allows us to make mapping changes to specific properties without having to stick external references
        /// inside of the plain class objects (keeps the library models clean). Since this normally done by attribute 
        /// tags inside the model classes.
        /// </remarks>
        private void ConfigureDatabaseIdentityMappings()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// A reference to the logger.
        /// </summary>
        private ILogger Logger;

        /// <summary>
        /// The database connection string.
        /// </summary>
        /// <remarks>
        /// A memory stream or database file is used, but not both.
        /// </remarks>
        private string DatabaseConnectionString;

        /// <summary>
        /// The database memory stream.
        /// </summary>
        /// <remarks>
        /// A memory stream or database file is used, but not both.
        /// </remarks>
        private MemoryStream DatabaseMemoryStream;

        /// <summary>
        /// A flag to indicate if the database has been prepared.
        /// </summary>
        private bool DatabaseHasBeenPrepared;

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
