using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzetteLibrary.Database.SQLServer;
using OzetteLibrary.Files;
using OzetteLibrary.Folders;
using OzetteLibrary.Logging.Mock;
using System;

namespace OzetteLibraryTests.Client.Sources
{
    [TestClass]
    public class ScannerTests
    {
        private const string TestConnectionString = "fakedb";

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScannerConstructorThrowsExceptionWhenNoDatabaseIsProvided()
        {
            OzetteLibrary.Client.Sources.SourceScanner scanner = 
                new OzetteLibrary.Client.Sources.SourceScanner(null, new MockLogger());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScannerConstructorThrowsExceptionWhenNoLoggerIsProvided()
        {
            var db = new SQLServerClientDatabase(TestConnectionString);

            OzetteLibrary.Client.Sources.SourceScanner scanner =
                new OzetteLibrary.Client.Sources.SourceScanner(db, null);
        }

        [TestMethod]
        public void ScannerConstructorDoesNotThrowWhenValidArgumentsAreProvided()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString);

            OzetteLibrary.Client.Sources.SourceScanner scanner =
                new OzetteLibrary.Client.Sources.SourceScanner(db, logger);

            Assert.IsNotNull(scanner);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ScannerThrowsWhenBeginScanIsCalledAfterScanHasAlreadyStarted()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString);

            OzetteLibrary.Client.Sources.SourceScanner scanner =
                new OzetteLibrary.Client.Sources.SourceScanner(db, logger);

            var source = new LocalSourceLocation()
            {
                Path = Environment.CurrentDirectory,
                FileMatchFilter = "*.*",
                Priority = OzetteLibrary.Files.FileBackupPriority.Low,
                RevisionCount = 1
            };

            scanner.BeginScan(source);
            scanner.BeginScan(source);
        }

        [TestMethod]
        public void ScannerSignalsCompleteAfterScanHasCompleted()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString);

            OzetteLibrary.Client.Sources.SourceScanner scanner =
                new OzetteLibrary.Client.Sources.SourceScanner(db, logger);

            var source = new LocalSourceLocation()
            {
                Path = Environment.CurrentDirectory,
                FileMatchFilter = "*.*",
                Priority = FileBackupPriority.Low,
                RevisionCount = 1
            };

            var iasync = scanner.BeginScan(source);

            bool completeSignaled = iasync.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5));

            Assert.IsTrue(completeSignaled);
            Assert.IsTrue(iasync.IsCompleted);
        }

        [TestMethod]
        public void ScannerCanScanSuccessfullyAfterCompletingAnEarlierScan()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString);

            OzetteLibrary.Client.Sources.SourceScanner scanner =
                new OzetteLibrary.Client.Sources.SourceScanner(db, logger);

            var source = new LocalSourceLocation()
            {
                Path = Environment.CurrentDirectory,
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

        [TestMethod]
        public void ScannerCanAddClientFilesToDatabaseWithCorrectMetadata()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString);

            OzetteLibrary.Client.Sources.SourceScanner scanner =
                new OzetteLibrary.Client.Sources.SourceScanner(db, logger);

            var source = new LocalSourceLocation()
            {
                Path = Environment.CurrentDirectory,
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

            var clients = db.GetAllBackupFiles();

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

        [TestMethod]
        public void TraceMessagesAreWrittenToTheTraceLogDuringScanning()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString);

            OzetteLibrary.Client.Sources.SourceScanner scanner =
                new OzetteLibrary.Client.Sources.SourceScanner(db, logger);

            var source = new LocalSourceLocation()
            {
                Path = Environment.CurrentDirectory,
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
