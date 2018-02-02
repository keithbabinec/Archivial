using LiteDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace OzetteLibraryTests.Database.LiteDB
{
    [TestClass()]
    public class LiteDBClientDatabaseTests
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LiteDBClientDatabaseConstructorThrowsWhenNoLoggerIsProvided()
        {
            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db = 
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(new MemoryStream(), null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LiteDBClientDatabaseConstructorThrowsWhenNoDatabaseStreamIsProvided()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            MemoryStream ms = null;

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void LiteDBClientDatabaseConstructorThrowsWhenNoDatabaseConnectionStringIsProvided()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            string dbConString = null;

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(dbConString, logger);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseCanBeInstantiatedWithMemoryStream()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();
            var mem = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db = 
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(mem, logger);

            Assert.IsNotNull(db);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseCanBeInstantiatedWithConnectionString()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();
            var dbConString = "fake-connection-string";

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(dbConString, logger);

            Assert.IsNotNull(db);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseCanRunPrepareDatabaseWithoutExceptions()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();
        }

        [TestMethod()]
        public void LiteDBClientDatabaseCanRunPrepareDatabaseMultipleTimesWithoutExceptions()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();
            db.PrepareDatabase();
            db.PrepareDatabase();
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseGetClientFileThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.GetClientFile(null, null, null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseGetAllTargetsThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.GetAllTargets();
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseGetNextFileToBackupThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.GetNextFileToBackup();
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseGetAllClientFilesThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.GetAllClientFiles();
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseAddClientFileThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.AddClientFile(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseUpdateClientFileThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.UpdateClientFile(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseAddTargetThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.AddTarget(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseGetAllSourceLocationsThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.GetAllSourceLocations();
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseSetSourceLocationsThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.SetSourceLocations(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseUpdateSourceLocationThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.UpdateSourceLocation(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LiteDBClientDatabaseAddTargetThrowsIfNullTargetIsPassed()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();
            db.AddTarget(null);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseGetAllTargetsReturnsEmptyCollectionInsteadOfNull()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            var result = db.GetAllTargets();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseGetAllClientFilesReturnsEmptyCollectionInsteadOfNull()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            var result = db.GetAllClientFiles();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseCanAddTargetSuccessfully()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            // add a target using API AddTarget()
            // then manually check the stream
            
            var ms = new MemoryStream();
            
            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            var t = new OzetteLibrary.Models.Target() { ID = 1, Name = "test" };
            db.AddTarget(t);

            var liteDB = new LiteDatabase(ms);
            var targetCol = liteDB.GetCollection<OzetteLibrary.Models.Target>(OzetteLibrary.Constants.Database.TargetsTableName);
            var result = new OzetteLibrary.Models.Targets(targetCol.FindAll());

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);

            Assert.AreEqual(1, result[0].ID);
            Assert.AreEqual("test", result[0].Name);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseCanGetTargetSuccessfully()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            // manually add a target to the database stream.
            // then check API GetTargets()

            var t = new OzetteLibrary.Models.Target() { ID = 1, Name = "test" };
            var ms = new MemoryStream();
            var liteDB = new LiteDatabase(ms);
            var targetCol = liteDB.GetCollection<OzetteLibrary.Models.Target>(OzetteLibrary.Constants.Database.TargetsTableName);
            targetCol.Insert(t);

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            var result = db.GetAllTargets();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);

            Assert.AreEqual(1, result[0].ID);
            Assert.AreEqual("test", result[0].Name);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseCanAddAndGetSingleTargetSuccessfully()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            OzetteLibrary.Models.Target t = new OzetteLibrary.Models.Target();
            t.ID = 1;
            t.Name = "test";
            t.Port = 80;
            t.Url = "http://address.url.company.com";
            t.RootDirectory = "C:\\backuptarget\\test";

            db.AddTarget(t);
            var result = db.GetAllTargets();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);

            Assert.AreEqual(1, result[0].ID);
            Assert.AreEqual("test", result[0].Name);
            Assert.AreEqual(80, result[0].Port);
            Assert.AreEqual("http://address.url.company.com", result[0].Url);
            Assert.AreEqual("C:\\backuptarget\\test", result[0].RootDirectory);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseCanAddAndGetMultipleTargetsSuccessfully()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            OzetteLibrary.Models.Target t = new OzetteLibrary.Models.Target();
            t.ID = 1;
            t.Name = "test";

            OzetteLibrary.Models.Target t2 = new OzetteLibrary.Models.Target();
            t2.ID = 2;
            t2.Name = "test2";

            OzetteLibrary.Models.Target t3 = new OzetteLibrary.Models.Target();
            t3.ID = 3;
            t3.Name = "test3";

            db.AddTarget(t);
            db.AddTarget(t2);
            db.AddTarget(t3);

            var result = db.GetAllTargets();

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);

            Assert.AreEqual(1, result[0].ID);
            Assert.AreEqual("test", result[0].Name);

            Assert.AreEqual(2, result[1].ID);
            Assert.AreEqual("test2", result[1].Name);

            Assert.AreEqual(3, result[2].ID);
            Assert.AreEqual("test3", result[2].Name);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseReturnFilesFromGetAllClientFiles()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            // add a target using API AddClientFile()
            // then manually check the stream

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            // need a sample file (the calling assembly itself).
            FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var t = new OzetteLibrary.Models.ClientFile(info, OzetteLibrary.Models.FileBackupPriority.Low);

            db.AddClientFile(t);

            var result = db.GetAllClientFiles();

            int fileCount = 0;
            foreach (var file in result)
            {
                Assert.AreNotEqual(Guid.Empty, file.FileID);
                Assert.AreEqual(info.FullName, file.FullSourcePath);
                fileCount++;
            }

            Assert.AreEqual(1, fileCount);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseCanAddClientFileSuccessfully()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            // add a target using API AddClientFile()
            // then manually check the stream

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            // need a sample file (the calling assembly itself).
            FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var t = new OzetteLibrary.Models.ClientFile(info, OzetteLibrary.Models.FileBackupPriority.Low);

            db.AddClientFile(t);

            var liteDB = new LiteDatabase(ms);
            var clientFileCol = liteDB.GetCollection<OzetteLibrary.Models.ClientFile>(OzetteLibrary.Constants.Database.ClientsTableName);
            var result = clientFileCol.FindAll();

            int fileCount = 0;
            foreach (var file in result)
            {
                Assert.AreNotEqual(Guid.Empty, file.FileID);
                Assert.AreEqual(info.FullName, file.FullSourcePath);
                fileCount++;
            }

            Assert.AreEqual(1, fileCount);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseCanUpdateClientFileSuccessfully()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            // add a target using API AddClientFile()
            // then manually check the stream

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            // need a sample file (the calling assembly itself).
            FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var t = new OzetteLibrary.Models.ClientFile(info, OzetteLibrary.Models.FileBackupPriority.Low);

            db.AddClientFile(t);

            t.SetLastCheckedTimeStamp();

            db.UpdateClientFile(t);

            var liteDB = new LiteDatabase(ms);
            var clientFileCol = liteDB.GetCollection<OzetteLibrary.Models.ClientFile>(OzetteLibrary.Constants.Database.ClientsTableName);
            var result = clientFileCol.FindAll();

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

        [TestMethod()]
        public void LiteDBClientDatabaseGetClientFileReturnsNewFileExample()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();
            OzetteLibrary.Crypto.Hasher hasher = new OzetteLibrary.Crypto.Hasher(logger);

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            // need a sample file (the calling assembly itself).
            FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var t = new OzetteLibrary.Models.ClientFile(info, OzetteLibrary.Models.FileBackupPriority.Medium);

            t.SetFileHashWithAlgorithm(hasher.GenerateDefaultHash(info.FullName, OzetteLibrary.Models.FileBackupPriority.Medium),
                          hasher.GetDefaultHashAlgorithm(OzetteLibrary.Models.FileBackupPriority.Medium));

            t.SetLastCheckedTimeStamp();

            var result = db.GetClientFile(info.Name, info.DirectoryName, t.GetFileHash());

            Assert.IsNotNull(result);
            Assert.IsNull(result.File);
            Assert.AreEqual(OzetteLibrary.Models.ClientFileLookupResult.New, result.Result);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseGetClientFileReturnsExistingFileExample()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();
            OzetteLibrary.Crypto.Hasher hasher = new OzetteLibrary.Crypto.Hasher(logger);

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            // need a sample file (the calling assembly itself).
            FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var t = new OzetteLibrary.Models.ClientFile(info, OzetteLibrary.Models.FileBackupPriority.Medium);

            t.SetFileHashWithAlgorithm(hasher.GenerateDefaultHash(info.FullName, OzetteLibrary.Models.FileBackupPriority.Medium),
                          hasher.GetDefaultHashAlgorithm(OzetteLibrary.Models.FileBackupPriority.Medium));

            t.SetLastCheckedTimeStamp();

            db.AddClientFile(t);
            var result = db.GetClientFile(info.Name, info.DirectoryName, t.GetFileHash());

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.File);
            Assert.AreEqual(OzetteLibrary.Models.ClientFileLookupResult.Existing, result.Result);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseGetClientFileReturnsUpdatedFileExample()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();
            OzetteLibrary.Crypto.Hasher hasher = new OzetteLibrary.Crypto.Hasher(logger);

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            // need a sample file (the calling assembly itself).
            FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var t = new OzetteLibrary.Models.ClientFile(info, OzetteLibrary.Models.FileBackupPriority.Medium);

            t.SetFileHashWithAlgorithm(hasher.GenerateDefaultHash(info.FullName, OzetteLibrary.Models.FileBackupPriority.Medium),
                          hasher.GetDefaultHashAlgorithm(OzetteLibrary.Models.FileBackupPriority.Medium));

            t.SetLastCheckedTimeStamp();

            db.AddClientFile(t);

            // update the file
            t.SetFileHashWithAlgorithm(hasher.GenerateDefaultHash(info.FullName, OzetteLibrary.Models.FileBackupPriority.High),
                          hasher.GetDefaultHashAlgorithm(OzetteLibrary.Models.FileBackupPriority.High));

            t.SetLastCheckedTimeStamp();

            var result = db.GetClientFile(info.Name, info.DirectoryName, t.GetFileHash());

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.File);
            Assert.AreEqual(OzetteLibrary.Models.ClientFileLookupResult.Updated, result.Result);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseGetAllSourceLocationsReturnsEmptyCollectionInsteadOfNull()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            var result = db.GetAllSourceLocations();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseReturnSourcesFromGetAllSourceLocations()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            // need a sample source

            var source = new OzetteLibrary.Models.SourceLocation();
            source.ID = 1;
            source.FolderPath = "C:\\test\\folder";

            var sources = new OzetteLibrary.Models.SourceLocations();
            sources.Add(source);

            db.SetSourceLocations(sources);

            var result = db.GetAllSourceLocations();

            int sourceCount = 0;
            foreach (var dbSource in result)
            {
                Assert.AreEqual(source.ID, dbSource.ID);
                Assert.AreEqual(source.FolderPath, dbSource.FolderPath);
                sourceCount++;
            }

            Assert.AreEqual(1, sourceCount);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseCanUpdateSourceLocationSuccessfully()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            var source = new OzetteLibrary.Models.SourceLocation();
            source.ID = 1;
            source.FolderPath = "C:\\test\\folder";

            var sources = new OzetteLibrary.Models.SourceLocations();
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

        [TestMethod()]
        public void LiteDBClientDatabaseCanSetSourceLocationsSuccessfullyExample1()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            var source = new OzetteLibrary.Models.SourceLocation();
            source.ID = 1;
            source.FolderPath = "C:\\test\\folder";

            var sources = new OzetteLibrary.Models.SourceLocations();
            sources.Add(source);

            db.SetSourceLocations(sources);

            // manually check the db stream to make sure changes were applied.

            var liteDB = new LiteDatabase(ms);
            var sourcesCol = liteDB.GetCollection<OzetteLibrary.Models.SourceLocation>(OzetteLibrary.Constants.Database.SourceLocationsTableName);
            var result = sourcesCol.FindAll();

            int sourcesCount = 0;
            foreach (var dbSource in result)
            {
                Assert.AreEqual(source.ID, dbSource.ID);
                Assert.AreEqual(source.FolderPath, dbSource.FolderPath);

                sourcesCount++;
            }

            Assert.AreEqual(1, sourcesCount);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseCanSetSourceLocationsSuccessfullyExample2()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            var source1 = new OzetteLibrary.Models.SourceLocation();
            source1.ID = 1;
            source1.FolderPath = "C:\\test\\folder1";

            var source2 = new OzetteLibrary.Models.SourceLocation();
            source2.ID = 2;
            source2.FolderPath = "C:\\test\\folder2";

            var source3 = new OzetteLibrary.Models.SourceLocation();
            source3.ID = 3;
            source3.FolderPath = "C:\\test\\folder3";

            var sources = new OzetteLibrary.Models.SourceLocations();
            sources.Add(source1);
            sources.Add(source2);
            sources.Add(source3);

            db.SetSourceLocations(sources);

            // manually check the db stream to make sure changes were applied.

            var liteDB = new LiteDatabase(ms);
            var sourcesCol = liteDB.GetCollection<OzetteLibrary.Models.SourceLocation>(OzetteLibrary.Constants.Database.SourceLocationsTableName);
            var result = new OzetteLibrary.Models.SourceLocations(sourcesCol.FindAll());

            Assert.AreEqual(3, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(sources[i].ID, result[i].ID);
                Assert.AreEqual(sources[i].FolderPath, result[i].FolderPath);
            }
        }

        [TestMethod()]
        public void LiteDBClientDatabaseCanSetSourceLocationsSuccessfullyExample3()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            var source1 = new OzetteLibrary.Models.SourceLocation();
            source1.ID = 1;
            source1.FolderPath = "C:\\test\\folder1";

            var source2 = new OzetteLibrary.Models.SourceLocation();
            source2.ID = 2;
            source2.FolderPath = "C:\\test\\folder2";

            var source3 = new OzetteLibrary.Models.SourceLocation();
            source3.ID = 3;
            source3.FolderPath = "C:\\test\\folder3";

            var sources = new OzetteLibrary.Models.SourceLocations();
            sources.Add(source1);
            sources.Add(source2);
            sources.Add(source3);

            db.SetSourceLocations(sources);

            sources.RemoveAt(2);

            db.SetSourceLocations(sources);

            // manually check the db stream to make sure changes were applied.

            var liteDB = new LiteDatabase(ms);
            var sourcesCol = liteDB.GetCollection<OzetteLibrary.Models.SourceLocation>(OzetteLibrary.Constants.Database.SourceLocationsTableName);
            var result = new OzetteLibrary.Models.SourceLocations(sourcesCol.FindAll());

            Assert.AreEqual(2, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(sources[i].ID, result[i].ID);
                Assert.AreEqual(sources[i].FolderPath, result[i].FolderPath);
            }
        }

        [TestMethod()]
        public void LiteDBClientDatabaseGetNextFileToBackupReturnsNullWhenNoFilesToBackupExample1()
        {
            // no files in the database.

            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            Assert.IsNull(db.GetNextFileToBackup());
        }

        [TestMethod()]
        public void LiteDBClientDatabaseGetNextFileToBackupReturnsNullWhenNoFilesToBackupExample2()
        {
            // one file in the database, already backed up.

            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            var c1 = new OzetteLibrary.Models.ClientFile();
            c1.FileID = Guid.NewGuid();
            c1.Filename = "test.mp3";
            c1.Directory = "C:\\music";
            c1.CopyState = new Dictionary<int, OzetteLibrary.Models.TargetCopyState>();
            c1.CopyState.Add(1, new OzetteLibrary.Models.TargetCopyState() { TargetID = 1, TargetStatus = OzetteLibrary.Models.FileStatus.Synced });

            db.AddClientFile(c1);

            Assert.IsNull(db.GetNextFileToBackup());
        }

        [TestMethod()]
        public void LiteDBClientDatabaseGetNextFileToBackupReturnsNullWhenNoFilesToBackupExample3()
        {
            // single file in the database, file is currently copying.

            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            var c1 = new OzetteLibrary.Models.ClientFile();
            c1.FileID = Guid.NewGuid();
            c1.Filename = "test.mp3";
            c1.Directory = "C:\\music";
            c1.CopyState = new Dictionary<int, OzetteLibrary.Models.TargetCopyState>();
            c1.CopyState.Add(1, new OzetteLibrary.Models.TargetCopyState() { TargetID = 1, TargetStatus = OzetteLibrary.Models.FileStatus.InProgress });

            db.AddClientFile(c1);

            Assert.IsNull(db.GetNextFileToBackup());
        }

        [TestMethod()]
        public void LiteDBClientDatabaseGetNextFileToBackupReturnsNullWhenNoFilesToBackupExample4()
        {
            // multiple files in the database, all are already backed up.

            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            var c1 = new OzetteLibrary.Models.ClientFile();
            c1.FileID = Guid.NewGuid();
            c1.Filename = "test.mp3";
            c1.Directory = "C:\\music";
            c1.CopyState = new Dictionary<int, OzetteLibrary.Models.TargetCopyState>();
            c1.CopyState.Add(1, new OzetteLibrary.Models.TargetCopyState() { TargetID = 1, TargetStatus = OzetteLibrary.Models.FileStatus.Synced });

            var c2 = new OzetteLibrary.Models.ClientFile();
            c2.FileID = Guid.NewGuid();
            c2.Filename = "test2.mp3";
            c2.Directory = "C:\\music";
            c2.CopyState = new Dictionary<int, OzetteLibrary.Models.TargetCopyState>();
            c2.CopyState.Add(1, new OzetteLibrary.Models.TargetCopyState() { TargetID = 1, TargetStatus = OzetteLibrary.Models.FileStatus.Synced });

            db.AddClientFile(c1);
            db.AddClientFile(c2);

            Assert.IsNull(db.GetNextFileToBackup());
        }

        [TestMethod()]
        public void LiteDBClientDatabaseGetNextFileToBackupReturnsNullWhenNoFilesToBackupExample5()
        {
            // multiple files in the database, all are already backed up or copying.

            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            var c1 = new OzetteLibrary.Models.ClientFile();
            c1.FileID = Guid.NewGuid();
            c1.Filename = "test.mp3";
            c1.Directory = "C:\\music";
            c1.CopyState = new Dictionary<int, OzetteLibrary.Models.TargetCopyState>();
            c1.CopyState.Add(1, new OzetteLibrary.Models.TargetCopyState() { TargetID = 1, TargetStatus = OzetteLibrary.Models.FileStatus.Synced });

            var c2 = new OzetteLibrary.Models.ClientFile();
            c2.FileID = Guid.NewGuid();
            c2.Filename = "test2.mp3";
            c2.Directory = "C:\\music";
            c2.CopyState = new Dictionary<int, OzetteLibrary.Models.TargetCopyState>();
            c2.CopyState.Add(1, new OzetteLibrary.Models.TargetCopyState() { TargetID = 1, TargetStatus = OzetteLibrary.Models.FileStatus.InProgress });

            var c3 = new OzetteLibrary.Models.ClientFile();
            c3.FileID = Guid.NewGuid();
            c3.Filename = "test3.mp3";
            c3.Directory = "C:\\music";
            c3.CopyState = new Dictionary<int, OzetteLibrary.Models.TargetCopyState>();
            c3.CopyState.Add(1, new OzetteLibrary.Models.TargetCopyState() { TargetID = 1, TargetStatus = OzetteLibrary.Models.FileStatus.Synced });

            db.AddClientFile(c1);
            db.AddClientFile(c2);
            db.AddClientFile(c3);

            Assert.IsNull(db.GetNextFileToBackup());
        }

        [TestMethod()]
        public void LiteDBClientDatabaseGetNextFileToBackupReturnsNullWhenNoFilesToBackupExample6()
        {
            // multiple files in the database (with multiple sources), all are already backed up.

            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            var c1 = new OzetteLibrary.Models.ClientFile();
            c1.FileID = Guid.NewGuid();
            c1.Filename = "test.mp3";
            c1.Directory = "C:\\music";
            c1.CopyState = new Dictionary<int, OzetteLibrary.Models.TargetCopyState>();
            c1.CopyState.Add(1, new OzetteLibrary.Models.TargetCopyState() { TargetID = 1, TargetStatus = OzetteLibrary.Models.FileStatus.Synced });
            c1.CopyState.Add(2, new OzetteLibrary.Models.TargetCopyState() { TargetID = 2, TargetStatus = OzetteLibrary.Models.FileStatus.Synced });
            c1.CopyState.Add(3, new OzetteLibrary.Models.TargetCopyState() { TargetID = 3, TargetStatus = OzetteLibrary.Models.FileStatus.Synced });

            var c2 = new OzetteLibrary.Models.ClientFile();
            c2.FileID = Guid.NewGuid();
            c2.Filename = "test2.mp3";
            c2.Directory = "C:\\music";
            c2.CopyState = new Dictionary<int, OzetteLibrary.Models.TargetCopyState>();
            c2.CopyState.Add(1, new OzetteLibrary.Models.TargetCopyState() { TargetID = 1, TargetStatus = OzetteLibrary.Models.FileStatus.Synced });
            c2.CopyState.Add(2, new OzetteLibrary.Models.TargetCopyState() { TargetID = 2, TargetStatus = OzetteLibrary.Models.FileStatus.Synced });

            db.AddClientFile(c1);
            db.AddClientFile(c2);

            Assert.IsNull(db.GetNextFileToBackup());
        }

        [TestMethod()]
        public void LiteDBClientDatabaseGetNextFileToBackupReturnsCorrectFileToBackupExample1()
        {
            // single file (unsynced). needs backup.

            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            var c1 = new OzetteLibrary.Models.ClientFile();
            c1.Priority = OzetteLibrary.Models.FileBackupPriority.Medium;
            c1.FileID = Guid.NewGuid();
            c1.Filename = "test.mp3";
            c1.Directory = "C:\\music";
            c1.OverallState = OzetteLibrary.Models.FileStatus.Unsynced;

            db.AddClientFile(c1);

            var nextFile = db.GetNextFileToBackup();
            Assert.IsNotNull(nextFile);
            Assert.AreEqual(c1.FileID, nextFile.FileID);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseGetNextFileToBackupReturnsCorrectFileToBackupExample2()
        {
            // single file (outdated). needs backup.

            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            var c1 = new OzetteLibrary.Models.ClientFile();
            c1.Priority = OzetteLibrary.Models.FileBackupPriority.Medium;
            c1.FileID = Guid.NewGuid();
            c1.Filename = "test.mp3";
            c1.Directory = "C:\\music";
            c1.OverallState = OzetteLibrary.Models.FileStatus.OutOfDate;

            db.AddClientFile(c1);

            var nextFile = db.GetNextFileToBackup();
            Assert.IsNotNull(nextFile);
            Assert.AreEqual(c1.FileID, nextFile.FileID);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseGetNextFileToBackupReturnsCorrectFileToBackupExample3()
        {
            // if multiple files can be synced, return the more urgent one (unsynced over out-of-date).
            // assuming same file priority.

            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            var c1 = new OzetteLibrary.Models.ClientFile();
            c1.FileID = Guid.NewGuid();
            c1.Priority = OzetteLibrary.Models.FileBackupPriority.Medium;
            c1.Filename = "test.mp3";
            c1.Directory = "C:\\music";
            c1.OverallState = OzetteLibrary.Models.FileStatus.OutOfDate;

            var c2 = new OzetteLibrary.Models.ClientFile();
            c2.FileID = Guid.NewGuid();
            c2.Priority = OzetteLibrary.Models.FileBackupPriority.Medium;
            c2.Filename = "test2.mp3";
            c2.Directory = "C:\\music";
            c2.OverallState = OzetteLibrary.Models.FileStatus.Unsynced;

            db.AddClientFile(c1);
            db.AddClientFile(c2);

            var nextFile = db.GetNextFileToBackup();
            Assert.IsNotNull(nextFile);
            Assert.AreEqual(c2.FileID, nextFile.FileID);
        }
        
        [TestMethod()]
        public void LiteDBClientDatabaseGetNextFileToBackupReturnsCorrectFileToBackupExample5()
        {
            // multiple files. all need backup- (out of date), but have different priority

            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            var c1 = new OzetteLibrary.Models.ClientFile();
            c1.FileID = Guid.NewGuid();
            c1.Priority = OzetteLibrary.Models.FileBackupPriority.Medium;
            c1.Filename = "test.mp3";
            c1.Directory = "C:\\music";
            c1.OverallState = OzetteLibrary.Models.FileStatus.OutOfDate;

            var c2 = new OzetteLibrary.Models.ClientFile();
            c2.FileID = Guid.NewGuid();
            c2.Priority = OzetteLibrary.Models.FileBackupPriority.Medium;
            c2.Filename = "test2.mp3";
            c2.Directory = "C:\\music";
            c2.OverallState = OzetteLibrary.Models.FileStatus.OutOfDate;

            var c3 = new OzetteLibrary.Models.ClientFile();
            c3.FileID = Guid.NewGuid();
            c3.Priority = OzetteLibrary.Models.FileBackupPriority.High;
            c3.Filename = "test3.mp3";
            c3.Directory = "C:\\music";
            c3.OverallState = OzetteLibrary.Models.FileStatus.OutOfDate;

            db.AddClientFile(c1);
            db.AddClientFile(c2);
            db.AddClientFile(c3);

            var nextFile = db.GetNextFileToBackup();
            Assert.IsNotNull(nextFile);
            Assert.AreEqual(c3.FileID, nextFile.FileID);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseGetNextFileToBackupReturnsCorrectFileToBackupExample6()
        {
            // multiple files. only one needs backup

            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            var c1 = new OzetteLibrary.Models.ClientFile();
            c1.FileID = Guid.NewGuid();
            c1.Priority = OzetteLibrary.Models.FileBackupPriority.Medium;
            c1.Filename = "test.mp3";
            c1.Directory = "C:\\music";
            c1.OverallState = OzetteLibrary.Models.FileStatus.Synced;

            var c2 = new OzetteLibrary.Models.ClientFile();
            c2.FileID = Guid.NewGuid();
            c2.Priority = OzetteLibrary.Models.FileBackupPriority.Medium;
            c2.Filename = "test2.mp3";
            c2.Directory = "C:\\music";
            c2.OverallState = OzetteLibrary.Models.FileStatus.InProgress;

            var c3 = new OzetteLibrary.Models.ClientFile();
            c3.FileID = Guid.NewGuid();
            c3.Priority = OzetteLibrary.Models.FileBackupPriority.Low;
            c3.Filename = "test3.mp3";
            c3.Directory = "C:\\music";
            c3.OverallState = OzetteLibrary.Models.FileStatus.Unsynced;

            db.AddClientFile(c1);
            db.AddClientFile(c2);
            db.AddClientFile(c3);

            var nextFile = db.GetNextFileToBackup();
            Assert.IsNotNull(nextFile);
            Assert.AreEqual(c3.FileID, nextFile.FileID);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseGetNextFileToBackupReturnsCorrectFileToBackupExample7()
        {
            // if multiple files can be synced, return the more urgent one (priority ordering).
            // example: an high pri out-of-date takes priority over medium unsynced file.

            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            var c1 = new OzetteLibrary.Models.ClientFile();
            c1.FileID = Guid.NewGuid();
            c1.Priority = OzetteLibrary.Models.FileBackupPriority.Medium;
            c1.Filename = "test.mp3";
            c1.Directory = "C:\\music";
            c1.OverallState = OzetteLibrary.Models.FileStatus.Unsynced;

            var c2 = new OzetteLibrary.Models.ClientFile();
            c2.FileID = Guid.NewGuid();
            c2.Priority = OzetteLibrary.Models.FileBackupPriority.High;
            c2.Filename = "test2.mp3";
            c2.Directory = "C:\\music";
            c2.OverallState = OzetteLibrary.Models.FileStatus.OutOfDate;

            db.AddClientFile(c1);
            db.AddClientFile(c2);

            var nextFile = db.GetNextFileToBackup();
            Assert.IsNotNull(nextFile);
            Assert.AreEqual(c2.FileID, nextFile.FileID);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseGetNextFileToBackupReturnsCorrectFileToBackupExample8()
        {
            // if multiple files can be synced, return the more urgent one (priority ordering).
            // example: an med pri out-of-date takes priority over low unsynced file.

            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            var c1 = new OzetteLibrary.Models.ClientFile();
            c1.FileID = Guid.NewGuid();
            c1.Priority = OzetteLibrary.Models.FileBackupPriority.Low;
            c1.Filename = "test.mp3";
            c1.Directory = "C:\\music";
            c1.OverallState = OzetteLibrary.Models.FileStatus.Unsynced;

            var c2 = new OzetteLibrary.Models.ClientFile();
            c2.FileID = Guid.NewGuid();
            c2.Priority = OzetteLibrary.Models.FileBackupPriority.Medium;
            c2.Filename = "test2.mp3";
            c2.Directory = "C:\\music";
            c2.OverallState = OzetteLibrary.Models.FileStatus.OutOfDate;

            db.AddClientFile(c1);
            db.AddClientFile(c2);

            var nextFile = db.GetNextFileToBackup();
            Assert.IsNotNull(nextFile);
            Assert.AreEqual(c2.FileID, nextFile.FileID);
        }
    }
}
