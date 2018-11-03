using LiteDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzetteLibrary.Folders;
using System;
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

            db.GetBackupFile(null, null, null);
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
            var t = new OzetteLibrary.Files.BackupFile(info, OzetteLibrary.Files.FileBackupPriority.Medium);

            t.SetFileHashWithAlgorithm(hasher.GenerateDefaultHash(info.FullName, OzetteLibrary.Files.FileBackupPriority.Medium),
                          hasher.GetDefaultHashAlgorithm(OzetteLibrary.Files.FileBackupPriority.Medium));

            t.SetLastCheckedTimeStamp();

            var result = db.GetBackupFile(info.Name, info.DirectoryName, t.GetFileHashString());

            Assert.IsNotNull(result);
            Assert.IsNull(result.File);
            Assert.AreEqual(OzetteLibrary.Files.BackupFileLookupResult.New, result.Result);
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
            var t = new OzetteLibrary.Files.BackupFile(info, OzetteLibrary.Files.FileBackupPriority.Medium);

            t.SetFileHashWithAlgorithm(hasher.GenerateDefaultHash(info.FullName, OzetteLibrary.Files.FileBackupPriority.Medium),
                          hasher.GetDefaultHashAlgorithm(OzetteLibrary.Files.FileBackupPriority.Medium));

            t.SetLastCheckedTimeStamp();

            db.AddBackupFile(t);
            var result = db.GetBackupFile(info.Name, info.DirectoryName, t.GetFileHashString());

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.File);
            Assert.AreEqual(OzetteLibrary.Files.BackupFileLookupResult.Existing, result.Result);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetBackupFileReturnsUpdatedFileExample()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();
            OzetteLibrary.Crypto.Hasher hasher = new OzetteLibrary.Crypto.Hasher(logger);

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            // need a sample file (the calling assembly itself).
            FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var t = new OzetteLibrary.Files.BackupFile(info, OzetteLibrary.Files.FileBackupPriority.Medium);

            t.SetFileHashWithAlgorithm(hasher.GenerateDefaultHash(info.FullName, OzetteLibrary.Files.FileBackupPriority.Medium),
                          hasher.GetDefaultHashAlgorithm(OzetteLibrary.Files.FileBackupPriority.Medium));

            t.SetLastCheckedTimeStamp();

            db.AddBackupFile(t);

            // update the file
            t.SetFileHashWithAlgorithm(hasher.GenerateDefaultHash(info.FullName, OzetteLibrary.Files.FileBackupPriority.High),
                          hasher.GetDefaultHashAlgorithm(OzetteLibrary.Files.FileBackupPriority.High));

            t.SetLastCheckedTimeStamp();

            var result = db.GetBackupFile(info.Name, info.DirectoryName, t.GetFileHashString());

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.File);
            Assert.AreEqual(OzetteLibrary.Files.BackupFileLookupResult.Updated, result.Result);
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

            db.SetProviders(new OzetteLibrary.Providers.ProvidersCollection());
        }

        [TestMethod]
        public void LiteDBClientDatabaseSetProvidersCorrectlySavesProvidersList()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            var provList = new OzetteLibrary.Providers.ProvidersCollection();
            provList.Add(new OzetteLibrary.Providers.Provider()
            {
                ID = 1,
                Type = OzetteLibrary.Providers.ProviderTypes.Azure
            });

            db.SetProviders(provList);

            // manually check the db stream to make sure changes were applied.

            var liteDB = new LiteDatabase(ms);
            var provDBCol = liteDB.GetCollection<OzetteLibrary.Providers.Provider>(OzetteLibrary.Constants.Database.ProvidersTableName);

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

            Assert.AreEqual(typeof(OzetteLibrary.Providers.ProvidersCollection), result.GetType());
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void LiteDBClientDatabaseGetProvidersListCorrectlyReturnsProvidersCollection()
        {
            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms);

            db.PrepareDatabase();

            var provList = new OzetteLibrary.Providers.ProvidersCollection();
            provList.Add(new OzetteLibrary.Providers.Provider()
            {
                ID = 1,
                Type = OzetteLibrary.Providers.ProviderTypes.Azure
            });

            db.SetProviders(provList);

            var result = db.GetProvidersList();

            Assert.AreEqual(typeof(OzetteLibrary.Providers.ProvidersCollection), result.GetType());
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(OzetteLibrary.Providers.ProviderTypes.Azure, result[0].Type);
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
            source.FolderPath = "C:\\test\\folder";

            var sources = new OzetteLibrary.Folders.SourceLocations();
            sources.Add(source);

            db.SetSourceLocations(sources);

            var result = db.GetAllSourceLocations();

            int sourceCount = 0;
            foreach (var dbSource in result)
            {
                Assert.AreEqual(source.ID, dbSource.ID);
                Assert.AreEqual(source.FolderPath, (dbSource as LocalSourceLocation).FolderPath);
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
            source.FolderPath = "C:\\test\\folder";

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
            source.FolderPath = "C:\\test\\folder";

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
                Assert.AreEqual(source.FolderPath, (dbSource as LocalSourceLocation).FolderPath);

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
            source1.FolderPath = "C:\\test\\folder1";

            var source2 = new LocalSourceLocation();
            source2.ID = 2;
            source2.FolderPath = "C:\\test\\folder2";

            var source3 = new LocalSourceLocation();
            source3.ID = 3;
            source3.FolderPath = "C:\\test\\folder3";

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
                Assert.AreEqual((sources[i] as LocalSourceLocation).FolderPath, (result[i] as LocalSourceLocation).FolderPath);
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
            source1.FolderPath = "C:\\test\\folder1";

            var source2 = new LocalSourceLocation();
            source2.ID = 2;
            source2.FolderPath = "C:\\test\\folder2";

            var source3 = new LocalSourceLocation();
            source3.ID = 3;
            source3.FolderPath = "C:\\test\\folder3";

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
                Assert.AreEqual((sources[i] as LocalSourceLocation).FolderPath, (result[i] as LocalSourceLocation).FolderPath);
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
    }
}
