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
        public void ScannerConstructorThrowsExceptionWhenNoSourceLocationIsProvided()
        {
            OzetteLibrary.Client.Sources.Scanner scanner = 
                new OzetteLibrary.Client.Sources.Scanner(null, new MockClientDatabase(), new MockLogger());
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScannerConstructorThrowsExceptionWhenNoDatabaseIsProvided()
        {
            OzetteLibrary.Client.Sources.Scanner scanner = 
                new OzetteLibrary.Client.Sources.Scanner(new SourceLocation(), null, new MockLogger());
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScannerConstructorThrowsExceptionWhenNoLoggerIsProvided()
        {
            OzetteLibrary.Client.Sources.Scanner scanner =
                new OzetteLibrary.Client.Sources.Scanner(new SourceLocation(), new MockClientDatabase(), null);
        }

        [TestMethod()]
        public void ScannerConstructorDoesNotThrowWhenValidArgumentsAreProvided()
        {
            OzetteLibrary.Client.Sources.Scanner scanner =
                new OzetteLibrary.Client.Sources.Scanner(
                    new SourceLocation(), 
                    new MockClientDatabase(),
                    new MockLogger());

            Assert.IsTrue(true);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ScannerThrowsWhenBeginScanIsCalledAfterScanHasAlreadyStarted()
        {
            OzetteLibrary.Client.Sources.Scanner scanner =
                new OzetteLibrary.Client.Sources.Scanner(
                    new SourceLocation()
                    {
                        FolderPath = Environment.CurrentDirectory,
                        FileMatchFilter = "*.*",
                        Priority = FileBackupPriority.Low,
                        RevisionCount = 1
                    },
                    new MockClientDatabase(),
                    new MockLogger());

            scanner.BeginScan();
            scanner.BeginScan();
        }

        [TestMethod()]
        public void ScannerTriggersScanCompletedEventAfterScanHasCompleted()
        {
            OzetteLibrary.Client.Sources.Scanner scanner =
                new OzetteLibrary.Client.Sources.Scanner(
                    new SourceLocation()
                    {
                        FolderPath = Environment.CurrentDirectory,
                        FileMatchFilter = "*.*",
                        Priority = FileBackupPriority.Low,
                        RevisionCount = 1
                    },
                    new MockClientDatabase(),
                    new MockLogger());

            var signalScanCompleteEvent = new AutoResetEvent(false);

            scanner.ScanCompleted += (s, e) => { signalScanCompleteEvent.Set(); };
            scanner.BeginScan();

            var scanCompletedSignaled = signalScanCompleteEvent.WaitOne(TimeSpan.FromSeconds(10));

            Assert.IsTrue(scanCompletedSignaled);
        }
    }
}
