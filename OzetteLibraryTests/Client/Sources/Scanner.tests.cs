using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzetteLibrary.Database.LiteDB;
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
    }
}
