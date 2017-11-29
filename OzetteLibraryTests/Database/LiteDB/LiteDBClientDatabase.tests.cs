using LiteDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
        public void LiteDBClientDatabaseGetTargetsThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.GetAllTargets();
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
        public void LiteDBClientDatabaseGetTargetsReturnsEmptyCollectionInsteadOfNull()
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
            var t = new OzetteLibrary.Models.ClientFile(info);

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
            var t = new OzetteLibrary.Models.ClientFile(info);

            db.AddClientFile(t);

            t.LastChecked = DateTime.Now;

            db.UpdateClientFile(t);

            var liteDB = new LiteDatabase(ms);
            var clientFileCol = liteDB.GetCollection<OzetteLibrary.Models.ClientFile>(OzetteLibrary.Constants.Database.ClientsTableName);
            var result = clientFileCol.FindAll();

            int fileCount = 0;
            foreach (var file in result)
            {
                Assert.AreNotEqual(Guid.Empty, file.FileID);
                Assert.AreEqual(info.FullName, file.FullSourcePath);
                Assert.AreEqual(t.LastChecked.ToString(), file.LastChecked.ToString());

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
            var t = new OzetteLibrary.Models.ClientFile(info);

            t.FileHash = hasher.GenerateDefaultHash(info.FullName, OzetteLibrary.Models.FileBackupPriority.Medium);
            t.HashAlgorithmType = hasher.GetDefaultHashAlgorithm(OzetteLibrary.Models.FileBackupPriority.Medium);
            t.LastChecked = DateTime.Now;

            var result = db.GetClientFile(info.Name, info.DirectoryName, t.FileHash);

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
            var t = new OzetteLibrary.Models.ClientFile(info);

            t.FileHash = hasher.GenerateDefaultHash(info.FullName, OzetteLibrary.Models.FileBackupPriority.Medium);
            t.HashAlgorithmType = hasher.GetDefaultHashAlgorithm(OzetteLibrary.Models.FileBackupPriority.Medium);
            t.LastChecked = DateTime.Now;

            db.AddClientFile(t);
            var result = db.GetClientFile(info.Name, info.DirectoryName, t.FileHash);

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
            var t = new OzetteLibrary.Models.ClientFile(info);

            t.FileHash = hasher.GenerateDefaultHash(info.FullName, OzetteLibrary.Models.FileBackupPriority.Medium);
            t.HashAlgorithmType = hasher.GetDefaultHashAlgorithm(OzetteLibrary.Models.FileBackupPriority.Medium);
            t.LastChecked = DateTime.Now;

            db.AddClientFile(t);

            // update the file
            t.FileHash = hasher.GenerateDefaultHash(info.FullName, OzetteLibrary.Models.FileBackupPriority.High);
            t.HashAlgorithmType = hasher.GetDefaultHashAlgorithm(OzetteLibrary.Models.FileBackupPriority.High);
            t.LastChecked = DateTime.Now;

            var result = db.GetClientFile(info.Name, info.DirectoryName, t.FileHash);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.File);
            Assert.AreEqual(OzetteLibrary.Models.ClientFileLookupResult.Updated, result.Result);
        }
    }
}
