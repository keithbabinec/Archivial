using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzetteLibrary.Database.Mock;
using OzetteLibrary.Logging.Mock;
using OzetteLibrary.Models;
using System;
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
            OzetteLibrary.Client.Sources.Scanner scanner = 
                new OzetteLibrary.Client.Sources.Scanner(null, new MockLogger());
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScannerConstructorThrowsExceptionWhenNoLoggerIsProvided()
        {
            OzetteLibrary.Client.Sources.Scanner scanner =
                new OzetteLibrary.Client.Sources.Scanner(new MockClientDatabase(), null);
        }

        [TestMethod()]
        public void ScannerConstructorDoesNotThrowWhenValidArgumentsAreProvided()
        {
            OzetteLibrary.Client.Sources.Scanner scanner =
                new OzetteLibrary.Client.Sources.Scanner(new MockClientDatabase(), new MockLogger());

            Assert.IsTrue(true);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ScannerThrowsWhenBeginScanIsCalledAfterScanHasAlreadyStarted()
        {
            OzetteLibrary.Client.Sources.Scanner scanner =
                new OzetteLibrary.Client.Sources.Scanner(
                    new MockClientDatabase(),
                    new MockLogger());

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
            OzetteLibrary.Client.Sources.Scanner scanner =
                new OzetteLibrary.Client.Sources.Scanner(
                    new MockClientDatabase(),
                    new MockLogger());

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
        public void TraceMessagesAreWrittenToTheTraceLogDuringScanning()
        {
            var logger = new MockLogger();

            OzetteLibrary.Client.Sources.Scanner scanner =
                new OzetteLibrary.Client.Sources.Scanner(
                    new MockClientDatabase(),
                    logger);

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
