using LiteDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzetteLibrary.Files;
using OzetteLibrary.Folders;
using OzetteLibrary.StorageProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OzetteLibraryTests.Database.LiteDB
{
    [TestClass]
    public class LiteDBClientDatabaseTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LiteDBClientDatabaseConstructorThrowsWhenNoDatabaseStreamIsProvided()
        {
            MemoryStream ms = null;

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LiteDBClientDatabaseConstructorThrowsWhenNoDatabaseConnectionStringIsProvided()
        {
            string dbConString = null;

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(dbConString);
        }

        [TestMethod]
        public void LiteDBClientDatabaseCanBeInstantiatedWithMemoryStream()
        {
            var mem = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db = 
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(mem);

            Assert.IsNotNull(db);
        }

        [TestMethod]
        public void LiteDBClientDatabaseCanBeInstantiatedWithConnectionString()
        {
            var dbConString = "fake-connection-string";

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(dbConString);

            Assert.IsNotNull(db);
        }

        [TestMethod]
        public void LiteDBClientDatabaseCanRunPrepareDatabaseWithoutExceptions()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();
        }

        [TestMethod]
        public void LiteDBClientDatabaseCanRunPrepareDatabaseMultipleTimesWithoutExceptions()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();
            db.PrepareDatabase();
            db.PrepareDatabase();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseGetBackupFileThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.GetBackupFile(null, 0, DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseGetNextFileToBackupThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.GetNextFileToBackup();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseGetAllBackupFilesThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.GetAllBackupFiles();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseAddBackupFileThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.AddBackupFile(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseUpdateBackupFileThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.UpdateBackupFile(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseGetAllSourceLocationsThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.GetAllSourceLocations();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseSetSourceLocationsThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.SetSourceLocations(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseUpdateSourceLocationThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.UpdateSourceLocation(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseGetDirectoryMapItemThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.GetDirectoryMapItem(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseSetProvidersThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.SetProviders(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseGetProvidersListThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.GetProvidersList();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LiteDBClientDatabaseGetDirectoryMapItemThrowsIfNullDirectoryIsPassed()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            db.GetDirectoryMapItem(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LiteDBClientDatabaseGetDirectoryMapItemThrowsIfEmptyDirectoryIsPassed()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            db.GetDirectoryMapItem("");
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetDirectoryMapItemCorrectlyReturnsNewDirectory()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            var item = db.GetDirectoryMapItem("C:\\Bin\\Programs");

            Assert.IsNotNull(item);
            Assert.IsTrue(item.ID != Guid.Empty);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetDirectoryMapItemCorrectlyReturnsExistingDirectory()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            var item = db.GetDirectoryMapItem("C:\\Bin\\Programs");

            Assert.IsNotNull(item);
            Assert.IsTrue(item.ID != Guid.Empty);

            var item2 = db.GetDirectoryMapItem("C:\\Bin\\Programs");

            Assert.IsNotNull(item2);
            Assert.IsTrue(item.ID == item2.ID);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetDirectoryMapItemCorrectlyReturnsExistingDirectoryWhenCaseDoesntMatch()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            var item = db.GetDirectoryMapItem("c:\\bin\\PROGRAMS");

            Assert.IsNotNull(item);
            Assert.IsTrue(item.ID != Guid.Empty);

            var item2 = db.GetDirectoryMapItem("C:\\Bin\\Programs");

            Assert.IsNotNull(item2);
            Assert.IsTrue(item.ID == item2.ID);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetAllBackupFilesReturnsEmptyCollectionInsteadOfNull()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            var result = db.GetAllBackupFiles();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void LiteDBClientDatabaseReturnFilesFromGetAllBackupFiles()
        {
            // add a target using API AddBackupFile()
            // then manually check the stream

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // need a sample file (the calling assembly itself).
            FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var t = new OzetteLibrary.Files.BackupFile(info, OzetteLibrary.Files.FileBackupPriority.Low);

            db.AddBackupFile(t);

            var result = db.GetAllBackupFiles();

            int fileCount = 0;
            foreach (var file in result)
            {
                Assert.AreNotEqual(Guid.Empty, file.FileID);
                Assert.AreEqual(info.FullName, file.FullSourcePath);
                fileCount++;
            }

            Assert.AreEqual(1, fileCount);
        }

        [TestMethod]
        public void LiteDBClientDatabaseCanAddBackupFileSuccessfully()
        {
            // add a target using API AddBackupFile()
            // then manually check the stream

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // need a sample file (the calling assembly itself).
            FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var t = new OzetteLibrary.Files.BackupFile(info, OzetteLibrary.Files.FileBackupPriority.Low);

            db.AddBackupFile(t);

            var liteDB = new LiteDatabase(ms);
            var backupFileCol = liteDB.GetCollection<OzetteLibrary.Files.BackupFile>(OzetteLibrary.Constants.Database.FilesTableName);
            var result = backupFileCol.FindAll();

            int fileCount = 0;
            foreach (var file in result)
            {
                Assert.AreNotEqual(Guid.Empty, file.FileID);
                Assert.AreEqual(info.FullName, file.FullSourcePath);
                fileCount++;
            }

            Assert.AreEqual(1, fileCount);
        }

        [TestMethod]
        public void LiteDBClientDatabaseCanUpdateBackupFileSuccessfully()
        {
            // add a target using API AddBackupFile()
            // then manually check the stream

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // need a sample file (the calling assembly itself).
            FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var t = new OzetteLibrary.Files.BackupFile(info, OzetteLibrary.Files.FileBackupPriority.Low);

            db.AddBackupFile(t);

            t.SetLastCheckedTimeStamp();

            db.UpdateBackupFile(t);

            var liteDB = new LiteDatabase(ms);
            var backupFileCol = liteDB.GetCollection<OzetteLibrary.Files.BackupFile>(OzetteLibrary.Constants.Database.FilesTableName);
            var result = backupFileCol.FindAll();

            int fileCount = 0;
            foreach (var file in result)
            {
                Assert.AreNotEqual(Guid.Empty, file.FileID);
                Assert.AreEqual(info.FullName, file.FullSourcePath);
                Assert.AreEqual(t.GetLastCheckedTimeStamp().ToString(), file.GetLastCheckedTimeStamp().ToString());

                fileCount++;
            }

            Assert.AreEqual(1, fileCount);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupFileReturnsNewFileExample()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();
            OzetteLibrary.Crypto.Hasher hasher = new OzetteLibrary.Crypto.Hasher(logger);

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // need a sample file (the calling assembly itself).
            FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var file = new BackupFile(info, FileBackupPriority.Medium);
            file.SetLastCheckedTimeStamp();

            // file will be 'new' because no files have been added yet.
            var result = db.GetBackupFile(file.FullSourcePath, file.FileSizeBytes, file.LastModified);

            Assert.IsNotNull(result);
            Assert.IsNull(result.File);
            Assert.AreEqual(BackupFileLookupResult.New, result.Result);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupFileReturnsExistingFileExample()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();
            OzetteLibrary.Crypto.Hasher hasher = new OzetteLibrary.Crypto.Hasher(logger);

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // need a sample file (the calling assembly itself).
            FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var file = new BackupFile(info, FileBackupPriority.Medium);
            file.SetLastCheckedTimeStamp();

            // file will be 'existing'
            // it is added to the database, and then we check for it immediately afterwards.
            db.AddBackupFile(file);
            var result = db.GetBackupFile(file.FullSourcePath, file.FileSizeBytes, file.LastModified);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.File);
            Assert.AreEqual(BackupFileLookupResult.Existing, result.Result);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupFileReturnsUpdatedFileExampleUpdatedSize()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();
            OzetteLibrary.Crypto.Hasher hasher = new OzetteLibrary.Crypto.Hasher(logger);

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // need a sample file (the calling assembly itself).
            FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var file = new BackupFile(info, FileBackupPriority.Medium);
            file.SetLastCheckedTimeStamp();

            // file will be 'updated'
            // it is added to the database, and then we check for it immediately afterwards using a different file size.
            db.AddBackupFile(file);
            var updatedFileSizeBytes = 1024;
            var result = db.GetBackupFile(file.FullSourcePath, updatedFileSizeBytes, file.LastModified);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.File);
            Assert.AreEqual(BackupFileLookupResult.Updated, result.Result);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupFileReturnsUpdatedFileExampleUpdatedModifiedDate()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();
            OzetteLibrary.Crypto.Hasher hasher = new OzetteLibrary.Crypto.Hasher(logger);

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // need a sample file (the calling assembly itself).
            FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var file = new BackupFile(info, FileBackupPriority.Medium);
            file.SetLastCheckedTimeStamp();

            // file will be 'updated'
            // it is added to the database, and then we check for it immediately afterwards using a different file modified date.
            db.AddBackupFile(file);
            var updatedModifiedDate = DateTime.Now.AddMinutes(5);
            var result = db.GetBackupFile(file.FullSourcePath, file.FileSizeBytes, updatedModifiedDate);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.File);
            Assert.AreEqual(BackupFileLookupResult.Updated, result.Result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LiteDBClientDatabaseSetProvidersThrowsOnNullInput()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            db.SetProviders(null);
        }

        [TestMethod]
        public void LiteDBClientDatabaseSetProvidersDoesNotThrowOnEmptyCollection()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            db.SetProviders(new OzetteLibrary.StorageProviders.StorageProvidersCollection());
        }

        [TestMethod]
        public void LiteDBClientDatabaseSetProvidersCorrectlySavesProvidersList()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            var provList = new OzetteLibrary.StorageProviders.StorageProvidersCollection();
            provList.Add(new OzetteLibrary.StorageProviders.StorageProvider()
            {
                ID = 1,
                Type = OzetteLibrary.StorageProviders.StorageProviderTypes.Azure
            });

            db.SetProviders(provList);

            // manually check the db stream to make sure changes were applied.

            var liteDB = new LiteDatabase(ms);
            var provDBCol = liteDB.GetCollection<OzetteLibrary.StorageProviders.StorageProvider>(OzetteLibrary.Constants.Database.ProvidersTableName);

            Assert.AreEqual(1, provDBCol.FindAll().ToList().Count);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetProvidersListReturnsEmptyCollectionWhenNoProvidersPresent()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            var result = db.GetProvidersList();

            Assert.AreEqual(typeof(OzetteLibrary.StorageProviders.StorageProvidersCollection), result.GetType());
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetProvidersListCorrectlyReturnsProvidersCollection()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            var provList = new OzetteLibrary.StorageProviders.StorageProvidersCollection();
            provList.Add(new OzetteLibrary.StorageProviders.StorageProvider()
            {
                ID = 1,
                Type = OzetteLibrary.StorageProviders.StorageProviderTypes.Azure
            });

            db.SetProviders(provList);

            var result = db.GetProvidersList();

            Assert.AreEqual(typeof(OzetteLibrary.StorageProviders.StorageProvidersCollection), result.GetType());
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(OzetteLibrary.StorageProviders.StorageProviderTypes.Azure, result[0].Type);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetAllSourceLocationsReturnsEmptyCollectionInsteadOfNull()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            var result = db.GetAllSourceLocations();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void LiteDBClientDatabaseReturnSourcesFromGetAllSourceLocations()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // need a sample source

            var source = new LocalSourceLocation();
            source.ID = 1;
            source.Path = "C:\\test\\folder";

            var sources = new OzetteLibrary.Folders.SourceLocations();
            sources.Add(source);

            db.SetSourceLocations(sources);

            var result = db.GetAllSourceLocations();

            int sourceCount = 0;
            foreach (var dbSource in result)
            {
                Assert.AreEqual(source.ID, dbSource.ID);
                Assert.AreEqual(source.Path, dbSource.Path);
                sourceCount++;
            }

            Assert.AreEqual(1, sourceCount);
        }

        [TestMethod]
        public void LiteDBClientDatabaseCanUpdateSourceLocationSuccessfully()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            var source = new LocalSourceLocation();
            source.ID = 1;
            source.Path = "C:\\test\\folder";

            var sources = new OzetteLibrary.Folders.SourceLocations();
            sources.Add(source);

            db.SetSourceLocations(sources);

            source.LastCompletedScan = DateTime.Now.AddHours(-1);

            db.UpdateSourceLocation(source);

            var result = db.GetAllSourceLocations();

            int sourceCount = 0;
            foreach (var dbSource in result)
            {
                Assert.AreEqual(source.ID, dbSource.ID);
                Assert.AreEqual(source.LastCompletedScan.Value.ToString(), dbSource.LastCompletedScan.Value.ToString());
                sourceCount++;
            }

            Assert.AreEqual(1, sourceCount);
        }

        [TestMethod]
        public void LiteDBClientDatabaseCanSetSourceLocationsSuccessfullyExample1()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            var source = new LocalSourceLocation();
            source.ID = 1;
            source.Path = "C:\\test\\folder";

            var sources = new OzetteLibrary.Folders.SourceLocations();
            sources.Add(source);

            db.SetSourceLocations(sources);

            // manually check the db stream to make sure changes were applied.

            var liteDB = new LiteDatabase(ms);
            var sourcesCol = liteDB.GetCollection<OzetteLibrary.Folders.SourceLocation>(OzetteLibrary.Constants.Database.SourceLocationsTableName);
            var result = sourcesCol.FindAll();

            int sourcesCount = 0;
            foreach (var dbSource in result)
            {
                Assert.AreEqual(source.ID, dbSource.ID);
                Assert.AreEqual(source.Path, dbSource.Path);

                sourcesCount++;
            }

            Assert.AreEqual(1, sourcesCount);
        }

        [TestMethod]
        public void LiteDBClientDatabaseCanSetSourceLocationsSuccessfullyExample2()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            var source1 = new LocalSourceLocation();
            source1.ID = 1;
            source1.Path = "C:\\test\\folder1";

            var source2 = new LocalSourceLocation();
            source2.ID = 2;
            source2.Path = "C:\\test\\folder2";

            var source3 = new LocalSourceLocation();
            source3.ID = 3;
            source3.Path = "C:\\test\\folder3";

            var sources = new OzetteLibrary.Folders.SourceLocations();
            sources.Add(source1);
            sources.Add(source2);
            sources.Add(source3);

            db.SetSourceLocations(sources);

            // manually check the db stream to make sure changes were applied.

            var liteDB = new LiteDatabase(ms);
            var sourcesCol = liteDB.GetCollection<OzetteLibrary.Folders.SourceLocation>(OzetteLibrary.Constants.Database.SourceLocationsTableName);
            var result = new OzetteLibrary.Folders.SourceLocations(sourcesCol.FindAll());

            Assert.AreEqual(3, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(sources[i].ID, result[i].ID);
                Assert.AreEqual((sources[i] as LocalSourceLocation).Path, result[i].Path);
            }
        }

        [TestMethod]
        public void LiteDBClientDatabaseCanSetSourceLocationsSuccessfullyExample3()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            var source1 = new LocalSourceLocation();
            source1.ID = 1;
            source1.Path = "C:\\test\\folder1";

            var source2 = new LocalSourceLocation();
            source2.ID = 2;
            source2.Path = "C:\\test\\folder2";

            var source3 = new LocalSourceLocation();
            source3.ID = 3;
            source3.Path = "C:\\test\\folder3";

            var sources = new OzetteLibrary.Folders.SourceLocations();
            sources.Add(source1);
            sources.Add(source2);
            sources.Add(source3);

            db.SetSourceLocations(sources);

            sources.RemoveAt(2);

            db.SetSourceLocations(sources);

            // manually check the db stream to make sure changes were applied.

            var liteDB = new LiteDatabase(ms);
            var sourcesCol = liteDB.GetCollection<OzetteLibrary.Folders.SourceLocation>(OzetteLibrary.Constants.Database.SourceLocationsTableName);
            var result = new OzetteLibrary.Folders.SourceLocations(sourcesCol.FindAll());

            Assert.AreEqual(2, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(sources[i].ID, result[i].ID);
                Assert.AreEqual((sources[i] as LocalSourceLocation).Path, result[i].Path);
            }
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetNextFileToBackupReturnsNullWhenNoFilesToBackupExample1()
        {
            // no files in the database.

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            Assert.IsNull(db.GetNextFileToBackup());
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetNextFileToBackupReturnsCorrectFileToBackupExample1()
        {
            // single file (unsynced). needs backup.

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            var c1 = new OzetteLibrary.Files.BackupFile();
            c1.Priority = OzetteLibrary.Files.FileBackupPriority.Medium;
            c1.FileID = Guid.NewGuid();
            c1.Filename = "test.mp3";
            c1.Directory = "C:\\music";
            c1.OverallState = OzetteLibrary.Files.FileStatus.Unsynced;

            db.AddBackupFile(c1);

            var nextFile = db.GetNextFileToBackup();
            Assert.IsNotNull(nextFile);
            Assert.AreEqual(c1.FileID, nextFile.FileID);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetNextFileToBackupReturnsCorrectFileToBackupExample2()
        {
            // single file (outdated). needs backup.

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            var c1 = new OzetteLibrary.Files.BackupFile();
            c1.Priority = OzetteLibrary.Files.FileBackupPriority.Medium;
            c1.FileID = Guid.NewGuid();
            c1.Filename = "test.mp3";
            c1.Directory = "C:\\music";
            c1.OverallState = OzetteLibrary.Files.FileStatus.OutOfDate;

            db.AddBackupFile(c1);

            var nextFile = db.GetNextFileToBackup();
            Assert.IsNotNull(nextFile);
            Assert.AreEqual(c1.FileID, nextFile.FileID);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetNextFileToBackupReturnsCorrectFileToBackupExample3()
        {
            // if multiple files can be synced, return the more urgent one (unsynced over out-of-date).
            // assuming same file priority.

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            var c1 = new OzetteLibrary.Files.BackupFile();
            c1.FileID = Guid.NewGuid();
            c1.Priority = OzetteLibrary.Files.FileBackupPriority.Medium;
            c1.Filename = "test.mp3";
            c1.Directory = "C:\\music";
            c1.OverallState = OzetteLibrary.Files.FileStatus.OutOfDate;

            var c2 = new OzetteLibrary.Files.BackupFile();
            c2.FileID = Guid.NewGuid();
            c2.Priority = OzetteLibrary.Files.FileBackupPriority.Medium;
            c2.Filename = "test2.mp3";
            c2.Directory = "C:\\music";
            c2.OverallState = OzetteLibrary.Files.FileStatus.Unsynced;

            db.AddBackupFile(c1);
            db.AddBackupFile(c2);

            var nextFile = db.GetNextFileToBackup();
            Assert.IsNotNull(nextFile);
            Assert.AreEqual(c2.FileID, nextFile.FileID);
        }
        
        [TestMethod]
        public void LiteDBClientDatabaseGetNextFileToBackupReturnsCorrectFileToBackupExample5()
        {
            // multiple files. all need backup- (out of date), but have different priority

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            var c1 = new OzetteLibrary.Files.BackupFile();
            c1.FileID = Guid.NewGuid();
            c1.Priority = OzetteLibrary.Files.FileBackupPriority.Medium;
            c1.Filename = "test.mp3";
            c1.Directory = "C:\\music";
            c1.OverallState = OzetteLibrary.Files.FileStatus.OutOfDate;

            var c2 = new OzetteLibrary.Files.BackupFile();
            c2.FileID = Guid.NewGuid();
            c2.Priority = OzetteLibrary.Files.FileBackupPriority.Medium;
            c2.Filename = "test2.mp3";
            c2.Directory = "C:\\music";
            c2.OverallState = OzetteLibrary.Files.FileStatus.OutOfDate;

            var c3 = new OzetteLibrary.Files.BackupFile();
            c3.FileID = Guid.NewGuid();
            c3.Priority = OzetteLibrary.Files.FileBackupPriority.High;
            c3.Filename = "test3.mp3";
            c3.Directory = "C:\\music";
            c3.OverallState = OzetteLibrary.Files.FileStatus.OutOfDate;

            db.AddBackupFile(c1);
            db.AddBackupFile(c2);
            db.AddBackupFile(c3);

            var nextFile = db.GetNextFileToBackup();
            Assert.IsNotNull(nextFile);
            Assert.AreEqual(c3.FileID, nextFile.FileID);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetNextFileToBackupReturnsCorrectFileToBackupExample6()
        {
            // multiple files. only one needs backup

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            var c1 = new OzetteLibrary.Files.BackupFile();
            c1.FileID = Guid.NewGuid();
            c1.Priority = OzetteLibrary.Files.FileBackupPriority.Medium;
            c1.Filename = "test.mp3";
            c1.Directory = "C:\\music";
            c1.OverallState = OzetteLibrary.Files.FileStatus.Synced;

            var c2 = new OzetteLibrary.Files.BackupFile();
            c2.FileID = Guid.NewGuid();
            c2.Priority = OzetteLibrary.Files.FileBackupPriority.Medium;
            c2.Filename = "test2.mp3";
            c2.Directory = "C:\\music";
            c2.OverallState = OzetteLibrary.Files.FileStatus.InProgress;

            var c3 = new OzetteLibrary.Files.BackupFile();
            c3.FileID = Guid.NewGuid();
            c3.Priority = OzetteLibrary.Files.FileBackupPriority.Low;
            c3.Filename = "test3.mp3";
            c3.Directory = "C:\\music";
            c3.OverallState = OzetteLibrary.Files.FileStatus.Unsynced;

            db.AddBackupFile(c1);
            db.AddBackupFile(c2);
            db.AddBackupFile(c3);

            var nextFile = db.GetNextFileToBackup();
            Assert.IsNotNull(nextFile);
            Assert.AreEqual(c3.FileID, nextFile.FileID);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetNextFileToBackupReturnsCorrectFileToBackupExample7()
        {
            // if multiple files can be synced, return the more urgent one (priority ordering).
            // example: an high pri out-of-date takes priority over medium unsynced file.

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            var c1 = new OzetteLibrary.Files.BackupFile();
            c1.FileID = Guid.NewGuid();
            c1.Priority = OzetteLibrary.Files.FileBackupPriority.Medium;
            c1.Filename = "test.mp3";
            c1.Directory = "C:\\music";
            c1.OverallState = OzetteLibrary.Files.FileStatus.Unsynced;

            var c2 = new OzetteLibrary.Files.BackupFile();
            c2.FileID = Guid.NewGuid();
            c2.Priority = OzetteLibrary.Files.FileBackupPriority.High;
            c2.Filename = "test2.mp3";
            c2.Directory = "C:\\music";
            c2.OverallState = OzetteLibrary.Files.FileStatus.OutOfDate;

            db.AddBackupFile(c1);
            db.AddBackupFile(c2);

            var nextFile = db.GetNextFileToBackup();
            Assert.IsNotNull(nextFile);
            Assert.AreEqual(c2.FileID, nextFile.FileID);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetNextFileToBackupReturnsCorrectFileToBackupExample8()
        {
            // if multiple files can be synced, return the more urgent one (priority ordering).
            // example: an med pri out-of-date takes priority over low unsynced file.

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            var c1 = new OzetteLibrary.Files.BackupFile();
            c1.FileID = Guid.NewGuid();
            c1.Priority = OzetteLibrary.Files.FileBackupPriority.Low;
            c1.Filename = "test.mp3";
            c1.Directory = "C:\\music";
            c1.OverallState = OzetteLibrary.Files.FileStatus.Unsynced;

            var c2 = new OzetteLibrary.Files.BackupFile();
            c2.FileID = Guid.NewGuid();
            c2.Priority = OzetteLibrary.Files.FileBackupPriority.Medium;
            c2.Filename = "test2.mp3";
            c2.Directory = "C:\\music";
            c2.OverallState = OzetteLibrary.Files.FileStatus.OutOfDate;

            db.AddBackupFile(c1);
            db.AddBackupFile(c2);

            var nextFile = db.GetNextFileToBackup();
            Assert.IsNotNull(nextFile);
            Assert.AreEqual(c2.FileID, nextFile.FileID);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetNextFileToBackupReturnsCorrectFileToBackupExample9()
        {
            // exclude deleted files from backup.

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            var c1 = new OzetteLibrary.Files.BackupFile();
            c1.FileID = Guid.NewGuid();
            c1.Priority = OzetteLibrary.Files.FileBackupPriority.Low;
            c1.Filename = "test.mp3";
            c1.Directory = "C:\\music";
            c1.OverallState = OzetteLibrary.Files.FileStatus.Unsynced;
            c1.WasDeleted = DateTime.Now;

            db.AddBackupFile(c1);

            var nextFile = db.GetNextFileToBackup();
            Assert.IsNull(nextFile);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetNextFileToBackupReturnsCorrectFileToBackupExample10()
        {
            // exclude error-state files from backup.

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            var c1 = new OzetteLibrary.Files.BackupFile();
            c1.FileID = Guid.NewGuid();
            c1.Priority = OzetteLibrary.Files.FileBackupPriority.Low;
            c1.Filename = "test.mp3";
            c1.Directory = "C:\\music";
            c1.OverallState = OzetteLibrary.Files.FileStatus.Unsynced;
            c1.ErrorDetected = DateTime.Now;

            db.AddBackupFile(c1);

            var nextFile = db.GetNextFileToBackup();
            Assert.IsNull(nextFile);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseGetBackupProgressThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.GetBackupProgress();
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupProgressReturnsZeroPercentBackedUpWhenNoFilesPresent()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            var result = db.GetBackupProgress();

            // should not be null
            // should have zero % completed.
            Assert.IsNotNull(result);
            Assert.AreEqual("0.00 %", result.OverallPercentage);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupProgressReturnsZeroPercentBackedUpWhenNoFilesPresentAndProvidersPresent()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // add providers, but no files yet
            db.SetProviders(new StorageProvidersCollection() { new StorageProvider() { Enabled = true, ID = 1, Type = StorageProviderTypes.Azure } });
            db.SetProviders(new StorageProvidersCollection() { new StorageProvider() { Enabled = true, ID = 2, Type = StorageProviderTypes.AWS } });

            var result = db.GetBackupProgress();

            // should not be null
            // should have zero % completed.
            Assert.IsNotNull(result);
            Assert.AreEqual("0.00 %", result.OverallPercentage);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupProgressReturns100PercentBackedUpWhenSingleFileSingleProviderIsBackedUp()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // add provider
            db.SetProviders(new StorageProvidersCollection() { new StorageProvider() { Enabled = true, ID = 1, Type = StorageProviderTypes.Azure } });

            // create file(s)
            db.AddBackupFile(GenerateSyncedFile(1024, StorageProviderTypes.Azure));

            // query progress

            var result = db.GetBackupProgress();

            // verify results

            Assert.AreEqual("100.00 %", result.OverallPercentage);

            Assert.AreEqual("1.00 KB", result.BackedUpFileSize);
            Assert.AreEqual(1, result.BackedUpFileCount);

            Assert.AreEqual("0.00 KB", result.RemainingFileSize);
            Assert.AreEqual(0, result.RemainingFileCount);

            Assert.AreEqual("0.00 KB", result.FailedFileSize);
            Assert.AreEqual(0, result.FailedFileCount);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupProgressReturns100PercentBackedUpWhenTwoFilesSingleProviderAreBackedUp()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // add provider
            db.SetProviders(new StorageProvidersCollection() { new StorageProvider() { Enabled = true, ID = 1, Type = StorageProviderTypes.Azure } });

            // create file(s)
            db.AddBackupFile(GenerateSyncedFile(1024, StorageProviderTypes.Azure));
            db.AddBackupFile(GenerateSyncedFile(2048, StorageProviderTypes.Azure));

            // query progress

            var result = db.GetBackupProgress();

            // verify results

            Assert.AreEqual("100.00 %", result.OverallPercentage);

            Assert.AreEqual("3.00 KB", result.BackedUpFileSize);
            Assert.AreEqual(2, result.BackedUpFileCount);

            Assert.AreEqual("0.00 KB", result.RemainingFileSize);
            Assert.AreEqual(0, result.RemainingFileCount);

            Assert.AreEqual("0.00 KB", result.FailedFileSize);
            Assert.AreEqual(0, result.FailedFileCount);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupProgressReturns100PercentBackedUpWhenTwoFilesTwoProvidersAreBackedUp()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // add provider
            db.SetProviders(new StorageProvidersCollection() { new StorageProvider() { Enabled = true, ID = 1, Type = StorageProviderTypes.Azure } });
            db.SetProviders(new StorageProvidersCollection() { new StorageProvider() { Enabled = true, ID = 2, Type = StorageProviderTypes.AWS } });

            // create file(s)
            db.AddBackupFile(GenerateSyncedFile(1024, StorageProviderTypes.Azure, StorageProviderTypes.AWS));
            db.AddBackupFile(GenerateSyncedFile(2048, StorageProviderTypes.Azure, StorageProviderTypes.AWS));

            // query progress

            var result = db.GetBackupProgress();

            // verify results

            Assert.AreEqual("100.00 %", result.OverallPercentage);

            Assert.AreEqual("3.00 KB", result.BackedUpFileSize);
            Assert.AreEqual(2, result.BackedUpFileCount);

            Assert.AreEqual("0.00 KB", result.RemainingFileSize);
            Assert.AreEqual(0, result.RemainingFileCount);

            Assert.AreEqual("0.00 KB", result.FailedFileSize);
            Assert.AreEqual(0, result.FailedFileCount);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupProgressReturnsPartialPercentBackedUpFileAndRemainingFileSingleProvider()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // add provider
            db.SetProviders(new StorageProvidersCollection() { new StorageProvider() { Enabled = true, ID = 1, Type = StorageProviderTypes.Azure } });

            // create file(s)
            db.AddBackupFile(GenerateSyncedFile(1024, StorageProviderTypes.Azure));
            db.AddBackupFile(GenerateUnsyncedFile(2048, StorageProviderTypes.Azure));

            // query progress

            var result = db.GetBackupProgress();

            // verify results

            Assert.AreEqual("33.33 %", result.OverallPercentage);

            Assert.AreEqual("1.00 KB", result.BackedUpFileSize);
            Assert.AreEqual(1, result.BackedUpFileCount);

            Assert.AreEqual("2.00 KB", result.RemainingFileSize);
            Assert.AreEqual(1, result.RemainingFileCount);

            Assert.AreEqual("0.00 KB", result.FailedFileSize);
            Assert.AreEqual(0, result.FailedFileCount);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupProgressReturnsPartialPercentBackedUpFileAndFailedFileSingleProvider()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // add provider
            db.SetProviders(new StorageProvidersCollection() { new StorageProvider() { Enabled = true, ID = 1, Type = StorageProviderTypes.Azure } });

            // create file(s)
            db.AddBackupFile(GenerateSyncedFile(1024, StorageProviderTypes.Azure));
            db.AddBackupFile(GenerateFailedFile(2048, StorageProviderTypes.Azure));

            // query progress

            var result = db.GetBackupProgress();

            // verify results

            Assert.AreEqual("33.33 %", result.OverallPercentage);

            Assert.AreEqual("1.00 KB", result.BackedUpFileSize);
            Assert.AreEqual(1, result.BackedUpFileCount);

            Assert.AreEqual("0.00 KB", result.RemainingFileSize);
            Assert.AreEqual(0, result.RemainingFileCount);

            Assert.AreEqual("2.00 KB", result.FailedFileSize);
            Assert.AreEqual(1, result.FailedFileCount);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupProgressReturnsPartialPercentWithAllFileStatesSingleProvider()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // add provider
            db.SetProviders(new StorageProvidersCollection() { new StorageProvider() { Enabled = true, ID = 1, Type = StorageProviderTypes.Azure } });

            // create file(s)
            db.AddBackupFile(GenerateSyncedFile(1024, StorageProviderTypes.Azure));
            db.AddBackupFile(GenerateFailedFile(2048, StorageProviderTypes.Azure));
            db.AddBackupFile(GenerateUnsyncedFile(2048, StorageProviderTypes.Azure));

            // query progress

            var result = db.GetBackupProgress();

            // verify results

            Assert.AreEqual("20.00 %", result.OverallPercentage);

            Assert.AreEqual("1.00 KB", result.BackedUpFileSize);
            Assert.AreEqual(1, result.BackedUpFileCount);

            Assert.AreEqual("2.00 KB", result.RemainingFileSize);
            Assert.AreEqual(1, result.RemainingFileCount);

            Assert.AreEqual("2.00 KB", result.FailedFileSize);
            Assert.AreEqual(1, result.FailedFileCount);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupProgressReturnsPartialPercentWithSeveralFilesWithAllFileStatesSingleProvider()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // add provider
            db.SetProviders(new StorageProvidersCollection() { new StorageProvider() { Enabled = true, ID = 1, Type = StorageProviderTypes.Azure } });

            // create file(s)
            db.AddBackupFile(GenerateSyncedFile(1024, StorageProviderTypes.Azure));
            db.AddBackupFile(GenerateSyncedFile(2048, StorageProviderTypes.Azure));
            db.AddBackupFile(GenerateSyncedFile(4096, StorageProviderTypes.Azure));

            db.AddBackupFile(GenerateFailedFile(4096, StorageProviderTypes.Azure));
            db.AddBackupFile(GenerateFailedFile(4096, StorageProviderTypes.Azure));

            db.AddBackupFile(GenerateUnsyncedFile(1024, StorageProviderTypes.Azure));
            db.AddBackupFile(GenerateUnsyncedFile(2048, StorageProviderTypes.Azure));
            db.AddBackupFile(GenerateUnsyncedFile(2048, StorageProviderTypes.Azure));
            db.AddBackupFile(GenerateUnsyncedFile(4096, StorageProviderTypes.Azure));
            db.AddBackupFile(GenerateUnsyncedFile(1024, StorageProviderTypes.Azure));
            db.AddBackupFile(GenerateUnsyncedFile(2048, StorageProviderTypes.Azure));
            db.AddBackupFile(GenerateUnsyncedFile(4096, StorageProviderTypes.Azure));

            // query progress

            var result = db.GetBackupProgress();

            // verify results

            Assert.AreEqual("22.58 %", result.OverallPercentage);

            Assert.AreEqual("7.00 KB", result.BackedUpFileSize);
            Assert.AreEqual(3, result.BackedUpFileCount);

            Assert.AreEqual("8.00 KB", result.FailedFileSize);
            Assert.AreEqual(2, result.FailedFileCount);

            Assert.AreEqual("16.00 KB", result.RemainingFileSize);
            Assert.AreEqual(7, result.RemainingFileCount);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupProgressReturnsCorrectBackedUpSizeStringForKilobytesEx1()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // add provider
            db.SetProviders(new StorageProvidersCollection() { new StorageProvider() { Enabled = true, ID = 1, Type = StorageProviderTypes.Azure } });

            // create file(s)
            db.AddBackupFile(GenerateSyncedFile(800, StorageProviderTypes.Azure));

            var result = db.GetBackupProgress();

            // under 1kb
            Assert.AreEqual("0.78 KB", result.BackedUpFileSize);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupProgressReturnsCorrectBackedUpSizeStringForKilobytesEx2()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // add provider
            db.SetProviders(new StorageProvidersCollection() { new StorageProvider() { Enabled = true, ID = 1, Type = StorageProviderTypes.Azure } });

            // create file(s)
            db.AddBackupFile(GenerateSyncedFile(1024, StorageProviderTypes.Azure));

            var result = db.GetBackupProgress();

            // exact value (1kb)
            Assert.AreEqual("1.00 KB", result.BackedUpFileSize);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupProgressReturnsCorrectBackedUpSizeStringForKilobytesEx3()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // add provider
            db.SetProviders(new StorageProvidersCollection() { new StorageProvider() { Enabled = true, ID = 1, Type = StorageProviderTypes.Azure } });

            // create file(s)
            db.AddBackupFile(GenerateSyncedFile(1300, StorageProviderTypes.Azure));

            var result = db.GetBackupProgress();

            // slightly over 1kb
            Assert.AreEqual("1.27 KB", result.BackedUpFileSize);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupProgressReturnsCorrectBackedUpSizeStringForKilobytesEx4()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // add provider
            db.SetProviders(new StorageProvidersCollection() { new StorageProvider() { Enabled = true, ID = 1, Type = StorageProviderTypes.Azure } });

            // create file(s)
            db.AddBackupFile(GenerateSyncedFile(1014421, StorageProviderTypes.Azure));

            var result = db.GetBackupProgress();

            // just under 1mb
            Assert.AreEqual("990.65 KB", result.BackedUpFileSize);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupProgressReturnsCorrectBackedUpSizeStringForMegabytesEx1()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // add provider
            db.SetProviders(new StorageProvidersCollection() { new StorageProvider() { Enabled = true, ID = 1, Type = StorageProviderTypes.Azure } });

            // create file(s)
            db.AddBackupFile(GenerateSyncedFile(1048576, StorageProviderTypes.Azure));

            var result = db.GetBackupProgress();

            // exact value
            Assert.AreEqual("1.00 MB", result.BackedUpFileSize);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupProgressReturnsCorrectBackedUpSizeStringForMegabytesEx2()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // add provider
            db.SetProviders(new StorageProvidersCollection() { new StorageProvider() { Enabled = true, ID = 1, Type = StorageProviderTypes.Azure } });

            // create file(s)
            db.AddBackupFile(GenerateSyncedFile(1108576, StorageProviderTypes.Azure));

            var result = db.GetBackupProgress();

            // slightly over
            Assert.AreEqual("1.06 MB", result.BackedUpFileSize);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupProgressReturnsCorrectBackedUpSizeStringForMegabytesEx3()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // add provider
            db.SetProviders(new StorageProvidersCollection() { new StorageProvider() { Enabled = true, ID = 1, Type = StorageProviderTypes.Azure } });

            // create file(s)
            db.AddBackupFile(GenerateSyncedFile(1013741824, StorageProviderTypes.Azure));

            var result = db.GetBackupProgress();

            // just under next increment
            Assert.AreEqual("966.78 MB", result.BackedUpFileSize);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupProgressReturnsCorrectBackedUpSizeStringForGigabytesEx1()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // add provider
            db.SetProviders(new StorageProvidersCollection() { new StorageProvider() { Enabled = true, ID = 1, Type = StorageProviderTypes.Azure } });

            // create file(s)
            db.AddBackupFile(GenerateSyncedFile(1073741824, StorageProviderTypes.Azure));

            var result = db.GetBackupProgress();

            // exact value
            Assert.AreEqual("1.00 GB", result.BackedUpFileSize);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupProgressReturnsCorrectBackedUpSizeStringForGigabytesEx2()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // add provider
            db.SetProviders(new StorageProvidersCollection() { new StorageProvider() { Enabled = true, ID = 1, Type = StorageProviderTypes.Azure } });

            // create file(s)
            db.AddBackupFile(GenerateSyncedFile(1123741824, StorageProviderTypes.Azure));

            var result = db.GetBackupProgress();

            // slightly over
            Assert.AreEqual("1.05 GB", result.BackedUpFileSize);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupProgressReturnsCorrectBackedUpSizeStringForGigabytesEx3()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // add provider
            db.SetProviders(new StorageProvidersCollection() { new StorageProvider() { Enabled = true, ID = 1, Type = StorageProviderTypes.Azure } });

            // create file(s)
            db.AddBackupFile(GenerateSyncedFile(1033511627776, StorageProviderTypes.Azure));

            var result = db.GetBackupProgress();

            // just under next increment
            Assert.AreEqual("962.53 GB", result.BackedUpFileSize);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupProgressReturnsCorrectBackedUpSizeStringForTerabytesEx1()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // add provider
            db.SetProviders(new StorageProvidersCollection() { new StorageProvider() { Enabled = true, ID = 1, Type = StorageProviderTypes.Azure } });

            // create file(s)
            db.AddBackupFile(GenerateSyncedFile(1099511627776, StorageProviderTypes.Azure));

            var result = db.GetBackupProgress();

            // exact value
            Assert.AreEqual("1.00 TB", result.BackedUpFileSize);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupProgressReturnsCorrectBackedUpSizeStringForTerabytesEx2()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // add provider
            db.SetProviders(new StorageProvidersCollection() { new StorageProvider() { Enabled = true, ID = 1, Type = StorageProviderTypes.Azure } });

            // create file(s)
            db.AddBackupFile(GenerateSyncedFile(1208511627776, StorageProviderTypes.Azure));

            var result = db.GetBackupProgress();

            // slightly over
            Assert.AreEqual("1.10 TB", result.BackedUpFileSize);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupProgressReturnsCorrectBackedUpSizeStringForTerabytesEx3()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // add provider
            db.SetProviders(new StorageProvidersCollection() { new StorageProvider() { Enabled = true, ID = 1, Type = StorageProviderTypes.Azure } });

            // create file(s)
            db.AddBackupFile(GenerateSyncedFile(1025199906842624, StorageProviderTypes.Azure));

            var result = db.GetBackupProgress();

            // just under next increment
            Assert.AreEqual("932.41 TB", result.BackedUpFileSize);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupProgressReturnsCorrectBackedUpSizeStringForPetabytesEx1()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // add provider
            db.SetProviders(new StorageProvidersCollection() { new StorageProvider() { Enabled = true, ID = 1, Type = StorageProviderTypes.Azure } });

            // create file(s)
            db.AddBackupFile(GenerateSyncedFile(1125899906842624, StorageProviderTypes.Azure));

            var result = db.GetBackupProgress();

            // exact value
            Assert.AreEqual("1.00 PB", result.BackedUpFileSize);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupProgressReturnsCorrectBackedUpSizeStringForPetabytesEx2()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // add provider
            db.SetProviders(new StorageProvidersCollection() { new StorageProvider() { Enabled = true, ID = 1, Type = StorageProviderTypes.Azure } });

            // create file(s)
            db.AddBackupFile(GenerateSyncedFile(1225899906842624, StorageProviderTypes.Azure));

            var result = db.GetBackupProgress();

            // slightly over
            Assert.AreEqual("1.09 PB", result.BackedUpFileSize);
        }

        /// <summary>
        /// Helper method to quickly create a testable BackupFile object with a predetermined size.
        /// </summary>
        /// <param name="sizeInBytes"></param>
        /// <returns></returns>
        private BackupFile GenerateFile(long sizeInBytes)
        {
            return new BackupFile()
            {
                FileID = Guid.NewGuid(),
                FileSizeBytes = sizeInBytes,
                CopyState = new Dictionary<StorageProviderTypes, StorageProviderFileStatus>()
            };
        }

        /// <summary>
        /// Helper method to quickly create a testable BackupFile object with a predetermined size and unsynced state.
        /// </summary>
        /// <param name="sizeInBytes"></param>
        /// <param name="backedUpProviders"></param>
        /// <returns></returns>
        private BackupFile GenerateUnsyncedFile(long sizeInBytes, params StorageProviderTypes[] backedUpProviders)
        {
            var file = GenerateFile(sizeInBytes);

            foreach (var item in backedUpProviders)
            {
                file.OverallState = FileStatus.Unsynced;
            }

            return file;
        }

        /// <summary>
        /// Helper method to quickly create a testable BackupFile object with a predetermined size and synced state.
        /// </summary>
        /// <param name="sizeInBytes"></param>
        /// <param name="backedUpProviders"></param>
        /// <returns></returns>
        private BackupFile GenerateSyncedFile(long sizeInBytes, params StorageProviderTypes[] backedUpProviders)
        {
            var file = GenerateFile(sizeInBytes);

            foreach (var item in backedUpProviders)
            {
                file.OverallState = FileStatus.Synced;
            }

            return file;
        }

        /// <summary>
        /// Helper method to quickly create a testable BackupFile object with a predetermined size and failed state.
        /// </summary>
        /// <param name="sizeInBytes"></param>
        /// <param name="backedUpProviders"></param>
        /// <returns></returns>
        private BackupFile GenerateFailedFile(long sizeInBytes, params StorageProviderTypes[] backedUpProviders)
        {
            var file = GenerateFile(sizeInBytes);

            foreach (var item in backedUpProviders)
            {
                file.OverallState = FileStatus.ProviderError;
            }

            return file;
        }
    }
}
