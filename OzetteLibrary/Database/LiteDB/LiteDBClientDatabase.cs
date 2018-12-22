using LiteDB;
using OzetteLibrary.Files;
using OzetteLibrary.Folders;
using OzetteLibrary.StorageProviders;
using OzetteLibrary.Secrets;
using OzetteLibrary.ServiceCore;
using System;
using System.Globalization;
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
        public LiteDBClientDatabase(MemoryStream databaseStream)
        {
            if (databaseStream == null)
            {
                throw new ArgumentNullException(nameof(databaseStream));
            }

            DatabaseMemoryStream = databaseStream;
        }

        /// <summary>
        /// Instantiates a client DB from database connection string.
        /// </summary>
        /// <param name="connectionString"></param>
        public LiteDBClientDatabase(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(nameof(connectionString));
            }

            DatabaseConnectionString = connectionString;
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
            ConfigureDefaultApplicationSettingsIfMissing();

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

            map.Entity<ApplicationOption>().Id(x => x.ID);
            map.Entity<BackupFile>().Id(x => x.FileID);
            map.Entity<DirectoryMapItem>().Id(x => x.ID);
            map.Entity<SourceLocation>().Id(x => x.ID);
            map.Entity<StorageProvider>().Id(x => x.ID);
            map.Entity<NetCredential>().Id(x => x.ID);
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

                var optionsCol = db.GetCollection<ApplicationOption>(Constants.Database.ApplicationOptionsTableName);
                optionsCol.EnsureIndex(x => x.Name);

                var backupFilesCol = db.GetCollection<BackupFile>(Constants.Database.FilesTableName);
                backupFilesCol.EnsureIndex(x => x.FileID);
                backupFilesCol.EnsureIndex(x => x.Filename);
                backupFilesCol.EnsureIndex(x => x.Directory);
                backupFilesCol.EnsureIndex(x => x.FullSourcePath);
                backupFilesCol.EnsureIndex(x => x.FileHashString);

                var dirMapCol = db.GetCollection<DirectoryMapItem>(Constants.Database.DirectoryMapTableName);
                dirMapCol.EnsureIndex(x => x.LocalPath);
                dirMapCol.EnsureIndex(x => x.ID);

                var sourcesCol = db.GetCollection<SourceLocation>(Constants.Database.SourceLocationsTableName);
                sourcesCol.EnsureIndex(x => x.ID);

                var providersCol = db.GetCollection<StorageProvider>(Constants.Database.ProvidersTableName);
                providersCol.EnsureIndex(x => x.ID);

                var credentialsCol = db.GetCollection<NetCredential>(Constants.Database.NetCredentialsTableName);
                credentialsCol.EnsureIndex(x => x.CredentialName);
            }
        }

        /// <summary>
        /// Set the default setting options if they are missing.
        /// </summary>
        private void ConfigureDefaultApplicationSettingsIfMissing()
        {
            using (var db = GetLiteDBInstance())
            {
                var optionsCol = db.GetCollection<ApplicationOption>(Constants.Database.ApplicationOptionsTableName);

                if (optionsCol.FindOne(x => x.Name == Constants.RuntimeSettingNames.LowPriorityScanFrequencyInHours) == null)
                {
                    optionsCol.Insert(
                        new ApplicationOption()
                        {
                            Name = Constants.RuntimeSettingNames.LowPriorityScanFrequencyInHours,
                            Value = "72"
                        });
                }

                if (optionsCol.FindOne(x => x.Name == Constants.RuntimeSettingNames.MedPriorityScanFrequencyInHours) == null)
                {
                    optionsCol.Insert(
                        new ApplicationOption()
                        {
                            Name = Constants.RuntimeSettingNames.MedPriorityScanFrequencyInHours,
                            Value = "24"
                        });
                }

                if (optionsCol.FindOne(x => x.Name == Constants.RuntimeSettingNames.HighPriorityScanFrequencyInHours) == null)
                {
                    optionsCol.Insert(
                        new ApplicationOption()
                        {
                            Name = Constants.RuntimeSettingNames.HighPriorityScanFrequencyInHours,
                            Value = "1"
                        });
                }

                if (optionsCol.FindOne(x => x.Name == Constants.RuntimeSettingNames.StatusUpdateSchedule) == null)
                {
                    optionsCol.Insert(
                        new ApplicationOption()
                        {
                            Name = Constants.RuntimeSettingNames.StatusUpdateSchedule,
                            Value = "0 8 * * *"
                        });
                }
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
        /// Calculates and returns the overall backup progress.
        /// </summary>
        /// <returns></returns>
        public BackupProgress GetBackupProgress()
        {
            if (DatabaseHasBeenPrepared == false)
            {
                throw new InvalidOperationException("Database has not been prepared.");
            }

            using (var db = GetLiteDBInstance())
            {
                BackupProgress progress = new BackupProgress();

                var backupFilesCol = db.GetCollection<BackupFile>(Constants.Database.FilesTableName);

                var allFiles = backupFilesCol.FindAll();

                foreach (var file in allFiles)
                {
                    progress.TotalFileCount++;
                    progress.TotalFileSizeBytes += file.FileSizeBytes;

                    switch (file.OverallState)
                    {
                        case FileStatus.Unsynced:
                        case FileStatus.OutOfDate:
                        case FileStatus.InProgress:
                        {
                            progress.RemainingFileCount++;
                            progress.RemainingFileSizeBytes += file.FileSizeBytes;
                            break;
                        }
                        case FileStatus.Synced:
                        {
                            progress.BackedUpFileCount++;
                            progress.BackedUpFileSizeBytes += file.FileSizeBytes;
                            break;
                        }
                        case FileStatus.ProviderError:
                        {
                            progress.FailedFileCount++;
                            progress.FailedFileSizeBytes += file.FileSizeBytes;
                            break;
                        }
                    }
                }

                // set the overall completion rate
                if (progress.TotalFileSizeBytes != 0)
                {
                    progress.OverallPercentage = ((double)progress.BackedUpFileSizeBytes / progress.TotalFileSizeBytes).ToString("P2", CultureInfo.CreateSpecificCulture("US"));
                }
                else
                {
                    progress.OverallPercentage = "0.00 %";
                }

                return progress;
            }
        }

        /// <summary>
        /// Saves an application setting to the database.
        /// </summary>
        /// <param name="OptionName">Option name</param>
        /// <param name="OptionValue">Option value</param>
        public void SetApplicationOption(string OptionName, string OptionValue)
        {
            if (DatabaseHasBeenPrepared == false)
            {
                throw new InvalidOperationException("Database has not been prepared.");
            }
            if (string.IsNullOrWhiteSpace(OptionName))
            {
                throw new ArgumentException(nameof(OptionName) + " must be provided.");
            }
            if (string.IsNullOrWhiteSpace(OptionValue))
            {
                throw new ArgumentException(nameof(OptionValue) + " must be provided.");
            }

            using (var db = GetLiteDBInstance())
            {
                var providersCol = db.GetCollection<ApplicationOption>(Constants.Database.ApplicationOptionsTableName);

                providersCol.Upsert(new ApplicationOption(OptionName, OptionValue));
            }
        }

        /// <summary>
        /// Retrieves an application setting value from the database.
        /// </summary>
        /// <remarks>
        /// Returns null if the setting is not found.
        /// </remarks>
        /// <param name="OptionName">Option name</param>
        /// <returns>The setting value.</returns>
        public string GetApplicationOption(string OptionName)
        {
            if (DatabaseHasBeenPrepared == false)
            {
                throw new InvalidOperationException("Database has not been prepared.");
            }

            using (var db = GetLiteDBInstance())
            {
                var providersCol = db.GetCollection<ApplicationOption>(Constants.Database.ApplicationOptionsTableName);

                var setting = providersCol.FindOne(x => x.Name == OptionName);

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
        /// Removes an application setting value from the database.
        /// </summary>
        /// <param name="OptionName">Option name</param>
        public void RemoveApplicationOption(string OptionName)
        {
            if (DatabaseHasBeenPrepared == false)
            {
                throw new InvalidOperationException("Database has not been prepared.");
            }

            using (var db = GetLiteDBInstance())
            {
                var providersCol = db.GetCollection<ApplicationOption>(Constants.Database.ApplicationOptionsTableName);

                var setting = providersCol.Delete(x => x.Name == OptionName);
            }
        }

        /// <summary>
        /// Commits the providers collection to the database.
        /// </summary>
        /// <param name="Providers">A collection of providers.</param>
        public void SetProviders(StorageProvidersCollection Providers)
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
                var providersCol = db.GetCollection<StorageProvider>(Constants.Database.ProvidersTableName);

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
        /// <returns>A collection of providers.</returns>
        public StorageProvidersCollection GetProvidersList()
        {
            if (DatabaseHasBeenPrepared == false)
            {
                throw new InvalidOperationException("Database has not been prepared.");
            }

            using (var db = GetLiteDBInstance())
            {
                var providersCol = db.GetCollection<StorageProvider>(Constants.Database.ProvidersTableName);

                if (providersCol.Count() > 0)
                {
                    return new StorageProvidersCollection(providersCol.FindAll());
                }
                else
                {
                    return new StorageProvidersCollection();
                }
            }
        }

        /// <summary>
        /// Commits the net credentials collection to the database.
        /// </summary>
        /// <param name="Credentials">A collection of net credentials.</param>
        public void SetNetCredentialsList(NetCredentialsCollection Credentials)
        {
            if (DatabaseHasBeenPrepared == false)
            {
                throw new InvalidOperationException("Database has not been prepared.");
            }
            if (Credentials == null)
            {
                throw new ArgumentNullException(nameof(Credentials));
            }

            using (var db = GetLiteDBInstance())
            {
                var credentialsCol = db.GetCollection<NetCredential>(Constants.Database.NetCredentialsTableName);

                // remove all documents in this collection
                credentialsCol.Delete(x => 1 == 1);

                // insert credentials (if any)
                // note: empty (0 count) is valid input.
                foreach (var provider in Credentials)
                {
                    credentialsCol.Insert(provider);
                }
            }
        }

        /// <summary>
        /// Returns all of the net credentials defined in the database.
        /// </summary>
        /// <returns>A collection of net credentials.</returns>
        public NetCredentialsCollection GetNetCredentialsList()
        {
            if (DatabaseHasBeenPrepared == false)
            {
                throw new InvalidOperationException("Database has not been prepared.");
            }

            using (var db = GetLiteDBInstance())
            {
                var credentialsCol = db.GetCollection<NetCredential>(Constants.Database.NetCredentialsTableName);

                if (credentialsCol.Count() > 0)
                {
                    return new NetCredentialsCollection(credentialsCol.FindAll());
                }
                else
                {
                    return new NetCredentialsCollection();
                }
            }
        }

        /// <summary>
        /// Checks the index for a file matching the provided name, path, filesize, and lastmodified date.
        /// </summary>
        /// <param name="FullFilePath">Full file path (file name and path)</param>
        /// <param name="FileSizeBytes">File size in bytes</param>
        /// <param name="FileLastModified">File last modified timestamp</param>
        /// <returns></returns>
        public BackupFileLookup GetBackupFile(string FullFilePath, long FileSizeBytes, DateTime FileLastModified)
        {
            if (DatabaseHasBeenPrepared == false)
            {
                throw new InvalidOperationException("Database has not been prepared.");
            }

            var existingFile = FindFullMatchOnNameDirectorySizeAndModified(FullFilePath, FileSizeBytes, FileLastModified);
            if (existingFile != null)
            {
                return new BackupFileLookup() { File = existingFile, Result = BackupFileLookupResult.Existing };
            }

            var updatedFile = FindFilesWithExactNameAndPathButWrongSizeOrLastModified(FullFilePath, FileSizeBytes, FileLastModified);
            if (updatedFile != null)
            {
                return new BackupFileLookup() { File = updatedFile, Result = BackupFileLookupResult.Updated };
            }

            return new BackupFileLookup() { Result = BackupFileLookupResult.New };
        }

        /// <summary>
        /// Checks the index for a full file exact match.
        /// </summary>
        /// <param name="FullFilePath">Full file path (file name and path)</param>
        /// <param name="FileSizeBytes">File size in bytes</param>
        /// <param name="FileLastModified">File last modified timestamp</param>
        /// <returns><c>BackupFileLookup</c></returns>
        private BackupFile FindFullMatchOnNameDirectorySizeAndModified(string FullFilePath, long FileSizeBytes, DateTime FileLastModified)
        {
            using (var db = GetLiteDBInstance())
            {
                var backupFilesCol = db.GetCollection<BackupFile>(Constants.Database.FilesTableName);

                var exactMatches = backupFilesCol.Find(
                    x => x.FullSourcePath == FullFilePath
                      && x.FileSizeBytes == FileSizeBytes
                      && x.LastModified == FileLastModified
                );

                foreach (var file in exactMatches)
                {
                    // exact file hash, name, location.
                    // only possible to have one.
                    return file;
                }

                return null;
            }
        }

        /// <summary>
        /// Checks the index for a partial file match (wrong size or modified date).
        /// </summary>
        /// <param name="FullFilePath">Full file path (file name and path)</param>
        /// <param name="FileSizeBytes">File size in bytes</param>
        /// <param name="FileLastModified">File last modified timestamp</param>
        /// <returns><c>BackupFileLookup</c></returns>
        private BackupFile FindFilesWithExactNameAndPathButWrongSizeOrLastModified(string FullFilePath, long FileSizeBytes, DateTime FileLastModified)
        {
            using (var db = GetLiteDBInstance())
            {
                var backupFilesCol = db.GetCollection<BackupFile>(Constants.Database.FilesTableName);

                var partialMatches = backupFilesCol.Find(
                    x => x.FullSourcePath == FullFilePath
                      && (x.FileSizeBytes != FileSizeBytes || x.LastModified != FileLastModified)
                );

                foreach (var file in partialMatches)
                {
                    // grab the match (should only be one, since filename and path must be unique)
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
                // set the updated timestamp.
                File.LastUpdated = DateTime.Now;

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
                return backupFilesCol.FindOne(
                    x => x.Priority == Priority 
                      && x.OverallState == Status
                      && x.WasDeleted == null
                      && x.ErrorDetected == null
                );
            }
        }
    }
}
