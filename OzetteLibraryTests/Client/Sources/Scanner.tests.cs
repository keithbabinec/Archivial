using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OzetteLibrary.Database;
using OzetteLibrary.Database.SQLServer;
using OzetteLibrary.Files;
using OzetteLibrary.Folders;
using OzetteLibrary.Logging.Mock;
using OzetteLibrary.Providers;
using System;
using System.Threading.Tasks;

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
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            OzetteLibrary.Client.Sources.SourceScanner scanner =
                new OzetteLibrary.Client.Sources.SourceScanner(db, null);
        }

        [TestMethod]
        public void ScannerConstructorDoesNotThrowWhenValidArgumentsAreProvided()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            OzetteLibrary.Client.Sources.SourceScanner scanner =
                new OzetteLibrary.Client.Sources.SourceScanner(db, logger);

            Assert.IsNotNull(scanner);
        }

        [TestMethod]
        public async Task ScannerCanCallScanAsync()
        {
            var logger = new MockLogger();

            var db = new Mock<IClientDatabase>();

            db.Setup(x => x.FindBackupFileAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new BackupFileLookup() { Result = BackupFileLookupResult.New });

            db.Setup(x => x.GetProvidersAsync(ProviderTypes.Storage)).ReturnsAsync(new ProviderCollection());

            OzetteLibrary.Client.Sources.SourceScanner scanner =
                new OzetteLibrary.Client.Sources.SourceScanner(db.Object, logger);

            var source = new LocalSourceLocation()
            {
                Path = Environment.CurrentDirectory,
                FileMatchFilter = "*.*",
                Priority = FileBackupPriority.Low,
                RevisionCount = 1
            };

            await scanner.ScanAsync(source);
        }

        [TestMethod]
        public async Task ScannerCanScanSuccessfullyAfterCompletingAnEarlierScan()
        {
            var logger = new MockLogger();

            var db = new Mock<IClientDatabase>();

            db.Setup(x => x.FindBackupFileAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new BackupFileLookup() { Result = BackupFileLookupResult.New });

            db.Setup(x => x.GetProvidersAsync(ProviderTypes.Storage)).ReturnsAsync(new ProviderCollection());

            OzetteLibrary.Client.Sources.SourceScanner scanner =
                new OzetteLibrary.Client.Sources.SourceScanner(db.Object, logger);

            var source = new LocalSourceLocation()
            {
                Path = Environment.CurrentDirectory,
                FileMatchFilter = "*.*",
                Priority = FileBackupPriority.Low,
                RevisionCount = 1
            };

            await scanner.ScanAsync(source);
            await scanner.ScanAsync(source);
        }

        [TestMethod]
        public async Task ScannerCanAddClientFilesToDatabase()
        {
            var logger = new MockLogger();

            var db = new Mock<IClientDatabase>();

            db.Setup(x => x.FindBackupFileAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new BackupFileLookup() { Result = BackupFileLookupResult.New });

            db.Setup(x => x.GetProvidersAsync(ProviderTypes.Storage)).ReturnsAsync(new ProviderCollection());

            OzetteLibrary.Client.Sources.SourceScanner scanner =
                new OzetteLibrary.Client.Sources.SourceScanner(db.Object, logger);

            var source = new LocalSourceLocation()
            {
                Path = Environment.CurrentDirectory,
                FileMatchFilter = "*.*",
                Priority = FileBackupPriority.Low,
                RevisionCount = 1
            };

            await scanner.ScanAsync(source);

            db.Verify(x => x.AddBackupFileAsync(It.IsAny<BackupFile>()), Times.AtLeast(10));
        }

        [TestMethod]
        public async Task TraceMessagesAreWrittenToTheTraceLogDuringScanning()
        {
            var logger = new MockLogger();

            var db = new Mock<IClientDatabase>();

            db.Setup(x => x.FindBackupFileAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new BackupFileLookup() { Result = BackupFileLookupResult.New });

            db.Setup(x => x.GetProvidersAsync(ProviderTypes.Storage)).ReturnsAsync(new ProviderCollection());

            OzetteLibrary.Client.Sources.SourceScanner scanner =
                new OzetteLibrary.Client.Sources.SourceScanner(db.Object, logger);

            var source = new LocalSourceLocation()
            {
                Path = Environment.CurrentDirectory,
                FileMatchFilter = "*.*",
                Priority = FileBackupPriority.Low,
                RevisionCount = 1
            };

            await scanner.ScanAsync(source);

            Assert.IsTrue(logger.WriteTraceMessageHasBeenCalled);
        }
    }
}
