using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzetteLibrary.Database.LiteDB;
using OzetteLibrary.Files;
using OzetteLibrary.Folders;
using OzetteLibrary.Logging.Mock;
using System;
using System.IO;

namespace OzetteLibraryTests.Client.Sources
{
    [TestClass()]
    public class ScannerTests
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScannerConstructorThrowsExceptionWhenNoDatabaseIsProvided()
        {
            OzetteLibrary.Client.Sources.SourceScanner scanner = 
                new OzetteLibrary.Client.Sources.SourceScanner(null, new MockLogger());
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScannerConstructorThrowsExceptionWhenNoLoggerIsProvided()
        {
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), new MockLogger());

            OzetteLibrary.Client.Sources.SourceScanner scanner =
                new OzetteLibrary.Client.Sources.SourceScanner(inMemoryDB, null);
        }

        [TestMethod()]
        public void ScannerConstructorDoesNotThrowWhenValidArgumentsAreProvided()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), logger);

            OzetteLibrary.Client.Sources.SourceScanner scanner =
                new OzetteLibrary.Client.Sources.SourceScanner(inMemoryDB, logger);

            Assert.IsNotNull(scanner);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ScannerThrowsWhenBeginScanIsCalledAfterScanHasAlreadyStarted()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), logger);

            inMemoryDB.PrepareDatabase();

            OzetteLibrary.Client.Sources.SourceScanner scanner =
                new OzetteLibrary.Client.Sources.SourceScanner(inMemoryDB, logger);

            var source = new OzetteLibrary.Folders.SourceLocation()
            {
                FolderPath = Environment.CurrentDirectory,
                FileMatchFilter = "*.*",
                Priority = OzetteLibrary.Files.FileBackupPriority.Low,
                RevisionCount = 1
            };

            scanner.BeginScan(source);
            scanner.BeginScan(source);
        }

        [TestMethod()]
        public void ScannerSignalsCompleteAfterScanHasCompleted()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), logger);

            inMemoryDB.PrepareDatabase();

            OzetteLibrary.Client.Sources.SourceScanner scanner =
                new OzetteLibrary.Client.Sources.SourceScanner(inMemoryDB, logger);

            var source = new SourceLocation()
            {
                FolderPath = Environment.CurrentDirectory,
                FileMatchFilter = "*.*",
                Priority = FileBackupPriority.Low,
                RevisionCount = 1
            };

            var iasync = scanner.BeginScan(source);

            bool completeSignaled = iasync.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5));

            Assert.IsTrue(completeSignaled);
            Assert.IsTrue(iasync.IsCompleted);
        }

        [TestMethod()]
        public void ScannerCanScanSuccessfullyAfterCompletingAnEarlierScan()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), logger);

            inMemoryDB.PrepareDatabase();

            OzetteLibrary.Client.Sources.SourceScanner scanner =
                new OzetteLibrary.Client.Sources.SourceScanner(inMemoryDB, logger);

            var source = new SourceLocation()
            {
                FolderPath = Environment.CurrentDirectory,
                FileMatchFilter = "*.*",
                Priority = FileBackupPriority.Low,
                RevisionCount = 1
            };

            var iasync = scanner.BeginScan(source);

            bool completeSignaled = iasync.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5));

            Assert.IsTrue(completeSignaled);
            Assert.IsTrue(iasync.IsCompleted);

            var iasync2 = scanner.BeginScan(source);

            bool completeSignaled2 = iasync2.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5));

            Assert.IsTrue(completeSignaled2);
            Assert.IsTrue(iasync2.IsCompleted);
        }

        [TestMethod()]
        public void ScannerCanAddClientFilesToDatabaseWithCorrectMetadata()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), logger);

            inMemoryDB.PrepareDatabase();

            OzetteLibrary.Client.Sources.SourceScanner scanner =
                new OzetteLibrary.Client.Sources.SourceScanner(inMemoryDB, logger);

            var source = new SourceLocation()
            {
                FolderPath = Environment.CurrentDirectory,
                FileMatchFilter = "*.*",
                Priority = FileBackupPriority.Low,
                RevisionCount = 1
            };

            var iasync = scanner.BeginScan(source);

            bool completeSignaled = iasync.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5));

            Assert.IsTrue(completeSignaled);
            Assert.IsTrue(iasync.IsCompleted);

            // now check the database. 
            // do we have client objects correctly populated?

            var clients = inMemoryDB.GetAllBackupFiles();

            Assert.IsTrue(clients != null);
            Assert.IsTrue(clients.Count > 0);

            foreach (var client in clients)
            {
                Assert.IsFalse(string.IsNullOrEmpty(client.Directory));
                Assert.IsFalse(string.IsNullOrEmpty(client.Filename));
                Assert.IsFalse(string.IsNullOrEmpty(client.FullSourcePath));
                Assert.IsNotNull(client.GetLastCheckedTimeStamp());
                Assert.IsNotNull(client.CopyState);
                Assert.IsFalse(client.FileID == Guid.Empty);
            }
        }

        [TestMethod()]
        public void TraceMessagesAreWrittenToTheTraceLogDuringScanning()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), logger);

            inMemoryDB.PrepareDatabase();

            OzetteLibrary.Client.Sources.SourceScanner scanner =
                new OzetteLibrary.Client.Sources.SourceScanner(inMemoryDB, logger);

            var source = new SourceLocation()
            {
                FolderPath = Environment.CurrentDirectory,
                FileMatchFilter = "*.*",
                Priority = FileBackupPriority.Low,
                RevisionCount = 1
            };

            var iasync = scanner.BeginScan(source);

            bool completeSignaled = iasync.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5));

            Assert.IsTrue(completeSignaled);
            Assert.IsTrue(iasync.IsCompleted);

            Assert.IsTrue(logger.WriteTraceMessageHasBeenCalled);
        }
    }
}
