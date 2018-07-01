using LiteDB;
using OzetteLibrary.Files;
using OzetteLibrary.Folders;
using OzetteLibrary.Logging;
using OzetteLibrary.Providers;
using OzetteLibrary.ServiceCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            ConfigureDatabaseIdentityMappings();
            ConfigureDatabaseCollections();

            DatabaseHasBeenPrepared = true;
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
            var map = BsonMapper.Global;

            map.Entity<BackupFile>().Id(x => x.FileID);
            map.Entity<SourceLocation>().Id(x => x.ID);
        }

        /// <summary>
        /// Configures database collections (tables) for use.
        /// </summary>
        /// <remarks>
        /// This involves creating tables if they are missing, and ensuring indexes are present.
        /// </remarks>
        private void ConfigureDatabaseCollections()
        {
            using (var db = GetLiteDBInstance())
            {
                // the action of 'getting' the collection will create it if missing.
                // EnsureIndex() will also only create the indexes if they are missing.

                var optionsCol = db.GetCollection<ServiceOption>(Constants.Database.ServiceOptionsTableName);
                optionsCol.EnsureIndex(x => x.ID);
                optionsCol.EnsureIndex(x => x.Name);

                var backupFilesCol = db.GetCollection<BackupFile>(Constants.Database.FilesTableName);
                backupFilesCol.EnsureIndex(x => x.FileID);
                backupFilesCol.EnsureIndex(x => x.Filename);
                backupFilesCol.EnsureIndex(x => x.Directory);
                backupFilesCol.EnsureIndex(x => x.FileHash);

                var dirMapCol = db.GetCollection<DirectoryMapItem>(Constants.Database.DirectoryMapTableName);
                dirMapCol.EnsureIndex(x => x.LocalPath);
                dirMapCol.EnsureIndex(x => x.ID);

                var sourcesCol = db.GetCollection<SourceLocation>(Constants.Database.SourceLocationsTableName);
                sourcesCol.EnsureIndex(x => x.ID);

                // collections without indexes:

                var providersCol = db.GetCollection<ProviderOptions>(Constants.Database.ProvidersTableName);
            }
        }
        
        /// <summary>
        /// Returns a LiteDB instance.
        /// </summary>
        /// <remarks>
        /// This class supports both in-memory streamed database, and file-on-disk database.
        /// Return an instance using whichever one was supplied to the constructor.
        /// </remarks>
        /// <returns></returns>
        private LiteDatabase GetLiteDBInstance()
        {
            if (DatabaseConnectionString == null && DatabaseMemoryStream != null)
            {
                return new LiteDatabase(DatabaseMemoryStream);
            }
            else if (DatabaseConnectionString != null && DatabaseMemoryStream == null)
            {
                return new LiteDatabase(DatabaseConnectionString);
            }
            else
            {
                throw new InvalidOperationException("Unable to return a LiteDB instance. No memory stream or connection string was provided.");
            }
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
        /// Saves an application setting to the database.
        /// </summary>
        /// <param name="option">ServiceOption</param>
        public void SetApplicationOption(ServiceOption option)
        {
            if (DatabaseHasBeenPrepared == false)
            {
                throw new InvalidOperationException("Database has not been prepared.");
            }
            if (option == null)
            {
                throw new ArgumentNullException(nameof(option));
            }
            if (option.ID > 0)
            {
                throw new ArgumentException(nameof(option.ID) + " must be provided.");
            }
            if (string.IsNullOrWhiteSpace(option.Name))
            {
                throw new ArgumentException(nameof(option.Name) + " must be provided.");
            }
            if (string.IsNullOrWhiteSpace(option.Value))
            {
                throw new ArgumentException(nameof(option.Value) + " must be provided.");
            }

            using (var db = GetLiteDBInstance())
            {
                var providersCol = db.GetCollection<ServiceOption>(Constants.Database.ServiceOptionsTableName);

                providersCol.Upsert(option);
            }
        }

        /// <summary>
        /// Retrieves an application setting value from the database.
        /// </summary>
        /// <remarks>
        /// Returns null if the setting is not found.
        /// </remarks>
        /// <param name="SettingID">The setting ID number.</param>
        /// <returns>The setting value.</returns>
        public string GetApplicationOption(int SettingID)
        {
            if (DatabaseHasBeenPrepared == false)
            {
                throw new InvalidOperationException("Database has not been prepared.");
            }

            using (var db = GetLiteDBInstance())
            {
                var providersCol = db.GetCollection<ServiceOption>(Constants.Database.ServiceOptionsTableName);

                var setting = providersCol.FindOne(x => x.ID == SettingID);

                if (setting != null)
                {
                    return setting.Value;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Commits the provider options to the database.
        /// </summary>
        /// <param name="Provider">A list of Provider options</param>
        public void SetProviders(List<ProviderOptions> Providers)
        {
            if (DatabaseHasBeenPrepared == false)
            {
                throw new InvalidOperationException("Database has not been prepared.");
            }
            if (Providers == null)
            {
                throw new ArgumentNullException(nameof(Providers));
            }

            using (var db = GetLiteDBInstance())
            {
                var providersCol = db.GetCollection<ProviderOptions>(Constants.Database.ProvidersTableName);

                // remove all documents in this collection
                providersCol.Delete(x => 1 == 1);

                // insert providers (if any)
                // note: empty (0 count) is valid input from Locations.
                foreach (var provider in Providers)
                {
                    providersCol.Insert(provider);
                }
            }
        }

        /// <summary>
        /// Returns all of the providers defined in the database.
        /// </summary>
        /// <returns>An array of Provider types</returns>
        public ProviderTypes[] GetProvidersList()
        {
            if (DatabaseHasBeenPrepared == false)
            {
                throw new InvalidOperationException("Database has not been prepared.");
            }

            using (var db = GetLiteDBInstance())
            {
                var providersCol = db.GetCollection<ProviderOptions>(Constants.Database.ProvidersTableName);

                if (providersCol.Count() > 0)
                {
                    return providersCol.FindAll().Select(x => x.Type).ToArray();
                }
                else
                {
                    return new ProviderTypes[0];
                }
            }
        }

        /// <summary>
        /// Returns the provider options for the specified provider.
        /// </summary>
        /// <param name="ProviderType">A single provider type.</param>
        /// <returns><c>ProviderOptions</c></returns>
        public ProviderOptions GetProviderOptions(ProviderTypes ProviderType)
        {
            if (DatabaseHasBeenPrepared == false)
            {
                throw new InvalidOperationException("Database has not been prepared.");
            }

            using (var db = GetLiteDBInstance())
            {
                var providersCol = db.GetCollection<ProviderOptions>(Constants.Database.ProvidersTableName);

                if (providersCol.Count() > 0)
                {
                    return providersCol.Find(x => x.Type == ProviderType).FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
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
        /// <returns><c>BackupFileLookup</c></returns>
        public BackupFileLookup GetBackupFile(string FileName, string DirectoryPath, byte[] FileHash)
        {
            if (DatabaseHasBeenPrepared == false)
            {
                throw new InvalidOperationException("Database has not been prepared.");
            }

            var existingFile = FindFullMatchOnNameDirectoryAndHash(FileName, DirectoryPath, FileHash);
            if (existingFile != null)
            {
                return new BackupFileLookup() { File = existingFile, Result = BackupFileLookupResult.Existing };
            }

            var updatedFile = FindFilesWithExactNameAndPathButWrongHash(FileName, DirectoryPath, FileHash);
            if (updatedFile != null)
            {
                return new BackupFileLookup() { File = updatedFile, Result = BackupFileLookupResult.Updated };
            }

            return new BackupFileLookup() { Result = BackupFileLookupResult.New };
        }

        /// <summary>
        /// Checks the index for a full file exact match.
        /// </summary>
        /// <param name="FileName">Name of the file (ex: document.doc)</param>
        /// <param name="DirectoryPath">Full directory path (ex: C:\folder\documents)</param>
        /// <param name="FileHash">File hash expressed as a byte array.</param>
        /// <returns><c>BackupFileLookup</c></returns>
        private BackupFile FindFullMatchOnNameDirectoryAndHash(string FileName, string DirectoryPath, byte[] FileHash)
        {
            using (var db = GetLiteDBInstance())
            {
                BackupFileLookup result = new BackupFileLookup();

                var backupFilesCol = db.GetCollection<BackupFile>(Constants.Database.FilesTableName);
                var matchesOnHash = backupFilesCol.Find(x => x.Filename == FileName && x.Directory == DirectoryPath && x.FileHash == FileHash);

                foreach (var file in matchesOnHash)
                {
                    // exact file hash, name, location.
                    // only possible to have one.
                    return file;
                }

                return null;
            }
        }

        /// <summary>
        /// Checks the index for a file name/path match, but hash mismatch.
        /// </summary>
        /// <param name="FileName">Name of the file (ex: document.doc)</param>
        /// <param name="DirectoryPath">Full directory path (ex: C:\folder\documents)</param>
        /// <param name="FileHash">File hash expressed as a byte array.</param>
        /// <returns><c>BackupFileLookup</c></returns>
        private BackupFile FindFilesWithExactNameAndPathButWrongHash(string FileName, string DirectoryPath, byte[] FileHash)
        {
            using (var db = GetLiteDBInstance())
            {
                var backupFilesCol = db.GetCollection<BackupFile>(Constants.Database.FilesTableName);
                var matchesOnHash = backupFilesCol.Find(x => x.Filename == FileName && x.Directory == DirectoryPath && x.FileHash != FileHash);

                foreach (var file in matchesOnHash)
                {
                    // exact name, location.
                    // only possible to have one.
                    return file;
                }

                return null;
            }
        }

        /// <summary>
        /// Returns all of the client files in the database.
        /// </summary>
        /// <returns><c>BackupFiles</c></returns>
        public BackupFiles GetAllBackupFiles()
        {
            if (DatabaseHasBeenPrepared == false)
            {
                throw new InvalidOperationException("Database has not been prepared.");
            }

            using (var db = GetLiteDBInstance())
            {
                var backupCol = db.GetCollection<BackupFile>(Constants.Database.FilesTableName);

                if (backupCol.Count() > 0)
                {
                    return new BackupFiles(backupCol.FindAll());
                }
                else
                {
                    return new BackupFiles();
                }
            }
        }

        /// <summary>
        /// Returns the directory map item for the specified local directory.
        /// </summary>
        /// <remarks>
        /// A new directory map item will be created if none currently exists for the specified folder.
        /// </remarks>
        /// <param name="DirectoryPath">Local directory path. Ex: 'C:\bin\programs'</param>
        /// <returns><c>DirectoryMapItem</c></returns>
        public DirectoryMapItem GetDirectoryMapItem(string DirectoryPath)
        {
            if (DatabaseHasBeenPrepared == false)
            {
                throw new InvalidOperationException("Database has not been prepared.");
            }
            if (string.IsNullOrWhiteSpace(DirectoryPath))
            {
                throw new ArgumentException("Must provide a valid directory path.");
            }

            // folder matching is case-insensitive in Windows. 
            // ensure the folder lookup is performed in this way to ensure consistent results.
            
            // do this by storing/searching all results in lowercase.
            // this is faster then doing case-insensitive lookup matches.

            string lowerDirPath = DirectoryPath.ToLower();

            using (var db = GetLiteDBInstance())
            {
                var mapItemsCol = db.GetCollection<DirectoryMapItem>(Constants.Database.DirectoryMapTableName);
                var foundDir = mapItemsCol.FindOne(x => x.LocalPath == lowerDirPath);

                if (foundDir != null)
                {
                    // directory mapping already exists.
                    // return it.

                    return foundDir;
                }
                else
                {
                    // directory mapping is not found.
                    // make a new one and save it.

                    DirectoryMapItem mappedItem = new DirectoryMapItem();
                    mappedItem.ID = Guid.NewGuid();
                    mappedItem.LocalPath = lowerDirPath;

                    mapItemsCol.Insert(mappedItem);

                    return mappedItem;
                }
            }
        }

        /// <summary>
        /// Returns all source locations defined in the database.
        /// </summary>
        /// <returns><c>SourceLocations</c></returns>
        public SourceLocations GetAllSourceLocations()
        {
            if (DatabaseHasBeenPrepared == false)
            {
                throw new InvalidOperationException("Database has not been prepared.");
            }

            using (var db = GetLiteDBInstance())
            {
                var sourcesCol = db.GetCollection<SourceLocation>(Constants.Database.SourceLocationsTableName);

                if (sourcesCol.Count() > 0)
                {
                    return new SourceLocations(sourcesCol.FindAll());
                }
                else
                {
                    return new SourceLocations();
                }
            }
        }

        /// <summary>
        /// Sets a new source locations collection in the database (this will wipe out existing sources).
        /// </summary>
        /// <param name="Locations"><c>SourceLocations</c></param>
        public void SetSourceLocations(SourceLocations Locations)
        {
            if (DatabaseHasBeenPrepared == false)
            {
                throw new InvalidOperationException("Database has not been prepared.");
            }
            if (Locations == null) 
            {
                throw new ArgumentNullException(nameof(Locations));
            }

            using (var db = GetLiteDBInstance())
            {
                var sourcesCol = db.GetCollection<SourceLocation>(Constants.Database.SourceLocationsTableName);

                // this effectively deletes all documents, since there source ID of 0 or less is not valid.
                sourcesCol.Delete(x => x.ID > 0);

                // insert locations (if any)
                // note: empty (0 count) is valid input from Locations.
                foreach (var source in Locations)
                {
                    sourcesCol.Insert(source);
                }
            }
        }

        /// <summary>
        /// Updates a single source location with the specified source.
        /// </summary>
        /// <param name="Location"><c>SourceLocation</c></param>
        public void UpdateSourceLocation(SourceLocation Location)
        {
            if (DatabaseHasBeenPrepared == false)
            {
                throw new InvalidOperationException("Database has not been prepared.");
            }
            if (Location == null)
            {
                throw new ArgumentNullException(nameof(Location));
            }

            using (var db = GetLiteDBInstance())
            {
                var sourcesCol = db.GetCollection<SourceLocation>(Constants.Database.SourceLocationsTableName);
                sourcesCol.Update(Location);
            }
        }

        /// <summary>
        /// Adds a new client file to the database.
        /// </summary>
        /// <param name="File"><c>BackupFile</c></param>
        public void AddBackupFile(BackupFile File)
        {
            if (DatabaseHasBeenPrepared == false)
            {
                throw new InvalidOperationException("Database has not been prepared.");
            }
            if (File == null)
            {
                throw new ArgumentNullException(nameof(File));
            }

            using (var db = GetLiteDBInstance())
            {
                var targetCol = db.GetCollection<BackupFile>(Constants.Database.FilesTableName);
                targetCol.Insert(File);
            }
        }

        /// <summary>
        /// Updates an existing client file in the database.
        /// </summary>
        /// <param name="File"><c>BackupFile</c></param>
        public void UpdateBackupFile(BackupFile File)
        {
            if (DatabaseHasBeenPrepared == false)
            {
                throw new InvalidOperationException("Database has not been prepared.");
            }
            if (File == null)
            {
                throw new ArgumentNullException(nameof(File));
            }

            using (var db = GetLiteDBInstance())
            {
                var targetCol = db.GetCollection<BackupFile>(Constants.Database.FilesTableName);
                targetCol.Update(File);
            }
        }

        /// <summary>
        /// Gets the next file that needs to be backed up.
        /// </summary>
        /// <remarks>
        /// If no files need to be backed up, return null.
        /// 
        /// The ordering in which files are returned is defined as follows: (ascending)
        /// 1. High priority unsynced files.
        /// 2. High priority out-of-sync files.
        /// 3. Medium priority unsynced files.
        /// 4. Medium priority out-of-sync files.
        /// 5. Low priority unsynced files.
        /// 6. Low priority out-of-sync files.
        /// </remarks>
        /// <returns><c>BackupFile</c></returns>
        public BackupFile GetNextFileToBackup()
        {
            if (DatabaseHasBeenPrepared == false)
            {
                throw new InvalidOperationException("Database has not been prepared.");
            }

            FileBackupPriority[] priorities = new FileBackupPriority[]
            {
                FileBackupPriority.High,
                FileBackupPriority.Medium,
                FileBackupPriority.Low
            };

            FileStatus[] states = new FileStatus[]
            {
                FileStatus.Unsynced,
                FileStatus.OutOfDate
            };

            for (int i = 0; i < priorities.Length; i++)
            {
                for (int j = 0; j < states.Length; j++)
                {
                    var matchedFile = FindNextBackupFileByPriorityAndStatus(priorities[i], states[j]);

                    if (matchedFile != null)
                    {
                        return matchedFile;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the next file that matches the priority and status criteria.
        /// </summary>
        /// <param name="Priority"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        private BackupFile FindNextBackupFileByPriorityAndStatus(FileBackupPriority Priority, FileStatus Status)
        {
            using (var db = GetLiteDBInstance())
            {
                var backupFilesCol = db.GetCollection<BackupFile>(Constants.Database.FilesTableName);
                return backupFilesCol.FindOne(x => x.Priority == Priority && x.OverallState == Status);
            }
        }
    }
}
