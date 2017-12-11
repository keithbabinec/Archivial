using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzetteLibrary.Database.LiteDB;
using OzetteLibrary.Logging.Mock;
using OzetteLibrary.Models;
using System;
using System.IO;
using System.Threading;

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

            var source = new SourceLocation()
            {
                FolderPath = Environment.CurrentDirectory,
                FileMatchFilter = "*.*",
                Priority = FileBackupPriority.Low,
                RevisionCount = 1
            };

            scanner.BeginScan(source);
            scanner.BeginScan(source);
        }

        [TestMethod()]
        public void ScannerTriggersScanCompletedEventAfterScanHasCompleted()
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

            var signalScanCompleteEvent = new AutoResetEvent(false);

            scanner.ScanCompleted += (s, e) => { signalScanCompleteEvent.Set(); };
            scanner.BeginScan(source);

            var scanCompletedSignaled = signalScanCompleteEvent.WaitOne(TimeSpan.FromSeconds(5));

            Assert.IsTrue(scanCompletedSignaled);
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

            var signalScanCompleteEvent = new AutoResetEvent(false);

            scanner.ScanCompleted += (s, e) => { signalScanCompleteEvent.Set(); };
            scanner.BeginScan(source);

            var scanCompletedSignaled = signalScanCompleteEvent.WaitOne(TimeSpan.FromSeconds(5));
            Assert.IsTrue(scanCompletedSignaled);

            // reset the signal
            // initiate a second scan of the same source.

            signalScanCompleteEvent.Reset();

            scanner.BeginScan(source);

            var scan2CompletedSignaled = signalScanCompleteEvent.WaitOne(TimeSpan.FromSeconds(5));
            Assert.IsTrue(scan2CompletedSignaled);
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

            var signalScanCompleteEvent = new AutoResetEvent(false);

            scanner.ScanCompleted += (s, e) => { signalScanCompleteEvent.Set(); };
            scanner.BeginScan(source);

            var scanCompletedSignaled = signalScanCompleteEvent.WaitOne(TimeSpan.FromSeconds(5));
            Assert.IsTrue(scanCompletedSignaled);

            // now check the database. 
            // do we have client objects correctly populated?

            var clients = inMemoryDB.GetAllClientFiles();

            Assert.IsTrue(clients != null);
            Assert.IsTrue(clients.Count > 0);

            foreach (var client in clients)
            {
                Assert.IsFalse(string.IsNullOrEmpty(client.Directory));
                Assert.IsFalse(string.IsNullOrEmpty(client.Filename));
                Assert.IsFalse(string.IsNullOrEmpty(client.FullSourcePath));
                Assert.IsNotNull(client.LastChecked);
                Assert.IsNotNull(client.CopyState);
                Assert.IsFalse(client.FileID == Guid.Empty);
            }
        }

        [TestMethod()]
        public void ScannerReturnsPopulatedScanResults()
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

            var signalScanCompleteEvent = new AutoResetEvent(false);

            ScanResults results = null; 

            scanner.ScanCompleted += (s, e) => {
                signalScanCompleteEvent.Set();
                results = e;
            };

            scanner.BeginScan(source);

            var scanCompletedSignaled = signalScanCompleteEvent.WaitOne(TimeSpan.FromSeconds(5));

            Assert.IsTrue(scanCompletedSignaled);
            Assert.IsNotNull(results);

            Assert.IsTrue(results.NewBytesFound > 0);
            Assert.IsTrue(results.NewFilesFound > 0);
            Assert.IsTrue(results.TotalBytesFound > 0);
            Assert.IsTrue(results.TotalFilesFound > 0);
            Assert.IsTrue(results.ScannedDirectoriesCount > 0);
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

            var signalScanCompleteEvent = new AutoResetEvent(false);

            scanner.ScanCompleted += (s, e) => { signalScanCompleteEvent.Set(); };
            scanner.BeginScan(source);

            var scanCompletedSignaled = signalScanCompleteEvent.WaitOne(TimeSpan.FromSeconds(5));

            Assert.IsTrue(scanCompletedSignaled);
            Assert.IsTrue(logger.WriteTraceMessageHasBeenCalled);
        }
    }
}
