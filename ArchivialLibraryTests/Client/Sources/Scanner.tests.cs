using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ArchivialLibrary.Database;
using ArchivialLibrary.Database.SQLServer;
using ArchivialLibrary.Files;
using ArchivialLibrary.Folders;
using ArchivialLibrary.Logging.Mock;
using ArchivialLibrary.Providers;
using System;
using System.Threading;
using System.Threading.Tasks;
using ArchivialLibrary.ServiceCore;

namespace ArchivialLibraryTests.Client.Sources
{
    [TestClass]
    public class ScannerTests
    {
        private const string TestConnectionString = "fakedb";

        private string[] TestMatchPatterns = new string[] { "^._", ".DS_Store" };

        private ICoreSettings SharedMockedCoreSettings = new Mock<ICoreSettings>().Object;

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScannerConstructorThrowsExceptionWhenNoDatabaseIsProvided()
        {
            ArchivialLibrary.Client.Sources.SourceScanner scanner = 
                new ArchivialLibrary.Client.Sources.SourceScanner(null, new MockLogger(), TestMatchPatterns);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScannerConstructorThrowsExceptionWhenNoLoggerIsProvided()
        {
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger(), SharedMockedCoreSettings);

            ArchivialLibrary.Client.Sources.SourceScanner scanner =
                new ArchivialLibrary.Client.Sources.SourceScanner(db, null, TestMatchPatterns);
        }

        [TestMethod]
        public void ScannerConstructorDoesNotThrowWhenValidArgumentsAreProvided()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger(), SharedMockedCoreSettings);

            ArchivialLibrary.Client.Sources.SourceScanner scanner =
                new ArchivialLibrary.Client.Sources.SourceScanner(db, logger, TestMatchPatterns);

            Assert.IsNotNull(scanner);
        }

        [TestMethod]
        public void ScannerConstructorDoesNotThrowMatchPatternsAreNotProvided()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger(), SharedMockedCoreSettings);

            ArchivialLibrary.Client.Sources.SourceScanner scanner =
                new ArchivialLibrary.Client.Sources.SourceScanner(db, logger, null);

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

            ArchivialLibrary.Client.Sources.SourceScanner scanner =
                new ArchivialLibrary.Client.Sources.SourceScanner(db.Object, logger, TestMatchPatterns);

            var source = new LocalSourceLocation()
            {
                Path = Environment.CurrentDirectory,
                FileMatchFilter = "*.*",
                Priority = FileBackupPriority.Low,
                RevisionCount = 1
            };

            var cancelSource = new CancellationTokenSource();
            await scanner.ScanAsync(source, cancelSource.Token).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ScannerCanScanSuccessfullyAfterCompletingAnEarlierScan()
        {
            var logger = new MockLogger();

            var db = new Mock<IClientDatabase>();

            db.Setup(x => x.FindBackupFileAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new BackupFileLookup() { Result = BackupFileLookupResult.New });

            db.Setup(x => x.GetProvidersAsync(ProviderTypes.Storage)).ReturnsAsync(new ProviderCollection());

            ArchivialLibrary.Client.Sources.SourceScanner scanner =
                new ArchivialLibrary.Client.Sources.SourceScanner(db.Object, logger, TestMatchPatterns);

            var source = new LocalSourceLocation()
            {
                Path = Environment.CurrentDirectory,
                FileMatchFilter = "*.*",
                Priority = FileBackupPriority.Low,
                RevisionCount = 1
            };

            var cancelSource = new CancellationTokenSource();
            await scanner.ScanAsync(source, cancelSource.Token).ConfigureAwait(false);
            await scanner.ScanAsync(source, cancelSource.Token).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ScannerCanAddClientFilesToDatabase()
        {
            var logger = new MockLogger();

            var db = new Mock<IClientDatabase>();

            db.Setup(x => x.FindBackupFileAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new BackupFileLookup() { Result = BackupFileLookupResult.New });

            db.Setup(x => x.GetProvidersAsync(ProviderTypes.Storage)).ReturnsAsync(new ProviderCollection());

            ArchivialLibrary.Client.Sources.SourceScanner scanner =
                new ArchivialLibrary.Client.Sources.SourceScanner(db.Object, logger, TestMatchPatterns);

            var source = new LocalSourceLocation()
            {
                Path = Environment.CurrentDirectory,
                FileMatchFilter = "*.*",
                Priority = FileBackupPriority.Low,
                RevisionCount = 1
            };

            var cancelSource = new CancellationTokenSource();
            await scanner.ScanAsync(source, cancelSource.Token).ConfigureAwait(false);

            db.Verify(x => x.AddBackupFileAsync(It.IsAny<BackupFile>()), Times.AtLeast(10));
        }

        [TestMethod]
        public async Task ScannerCanIgnoreClientFilesThatMatchTheExclusionPattern()
        {
            var logger = new MockLogger();

            var db = new Mock<IClientDatabase>();

            db.Setup(x => x.FindBackupFileAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new BackupFileLookup() { Result = BackupFileLookupResult.New });

            db.Setup(x => x.GetProvidersAsync(ProviderTypes.Storage)).ReturnsAsync(new ProviderCollection());

            // testing the match + exclusion behavior together here.
            // we want files that end with .dll
            // but exclude files that match the regex pattern for *Archivial*.

            var fileMatch = "*.dll";
            var exclusionPattern = new string[] { "(Archivial)" };

            ArchivialLibrary.Client.Sources.SourceScanner scanner =
                new ArchivialLibrary.Client.Sources.SourceScanner(db.Object, logger, exclusionPattern);

            var source = new LocalSourceLocation()
            {
                Path = Environment.CurrentDirectory,
                FileMatchFilter = fileMatch,
                Priority = FileBackupPriority.Low,
                RevisionCount = 1
            };

            var cancelSource = new CancellationTokenSource();
            await scanner.ScanAsync(source, cancelSource.Token).ConfigureAwait(false);

            // ensure we back up some dll files
            // but ensure we don't back up any dll files that match the Archivial name.

            db.Verify(x => x.AddBackupFileAsync(It.IsAny<BackupFile>()), Times.AtLeastOnce);
            db.Verify(x => x.AddBackupFileAsync(It.Is<BackupFile>(z => z.Filename.Contains("Archivial"))), Times.Never);
        }

        [TestMethod]
        public async Task TraceMessagesAreWrittenToTheTraceLogDuringScanning()
        {
            var logger = new MockLogger();

            var db = new Mock<IClientDatabase>();

            db.Setup(x => x.FindBackupFileAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new BackupFileLookup() { Result = BackupFileLookupResult.New });

            db.Setup(x => x.GetProvidersAsync(ProviderTypes.Storage)).ReturnsAsync(new ProviderCollection());

            ArchivialLibrary.Client.Sources.SourceScanner scanner =
                new ArchivialLibrary.Client.Sources.SourceScanner(db.Object, logger, TestMatchPatterns);

            var source = new LocalSourceLocation()
            {
                Path = Environment.CurrentDirectory,
                FileMatchFilter = "*.*",
                Priority = FileBackupPriority.Low,
                RevisionCount = 1
            };

            var cancelSource = new CancellationTokenSource();
            await scanner.ScanAsync(source, cancelSource.Token).ConfigureAwait(false);

            Assert.IsTrue(logger.WriteTraceMessageHasBeenCalled);
        }
    }
}
