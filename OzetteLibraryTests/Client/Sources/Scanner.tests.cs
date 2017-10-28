using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                new OzetteLibrary.Client.Sources.Scanner(null, new OzetteLibrary.Database.Mock.MockClientDatabase());
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScannerConstructorThrowsExceptionWhenNoDatabaseIsProvided()
        {
            OzetteLibrary.Client.Sources.Scanner scanner = 
                new OzetteLibrary.Client.Sources.Scanner(new OzetteLibrary.Models.SourceLocation(), null);
        }

        [TestMethod()]
        public void ScannerConstructorDoesNotThrowWhenValidArgumentsAreProvided()
        {
            OzetteLibrary.Client.Sources.Scanner scanner =
                new OzetteLibrary.Client.Sources.Scanner(
                    new OzetteLibrary.Models.SourceLocation(), 
                    new OzetteLibrary.Database.Mock.MockClientDatabase());

            Assert.IsTrue(true);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ScannerThrowsWhenBeginScanIsCalledAfterScanHasAlreadyStarted()
        {
            OzetteLibrary.Client.Sources.Scanner scanner =
                new OzetteLibrary.Client.Sources.Scanner(
                    new OzetteLibrary.Models.SourceLocation()
                    {
                        FolderPath = Environment.CurrentDirectory,
                        FileMatchFilter = "*.*",
                        Priority = OzetteLibrary.Models.FileBackupPriority.Low,
                        RevisionCount = 1
                    },
                    new OzetteLibrary.Database.Mock.MockClientDatabase());

            scanner.BeginScan();
            scanner.BeginScan();
        }

        [TestMethod()]
        public void ScannerTriggersScanCompletedEventAfterScanHasCompleted()
        {
            OzetteLibrary.Client.Sources.Scanner scanner =
                new OzetteLibrary.Client.Sources.Scanner(
                    new OzetteLibrary.Models.SourceLocation()
                    {
                        FolderPath = Environment.CurrentDirectory,
                        FileMatchFilter = "*.*",
                        Priority = OzetteLibrary.Models.FileBackupPriority.Low,
                        RevisionCount = 1
                    },
                    new OzetteLibrary.Database.Mock.MockClientDatabase());

            var signalScanCompleteEvent = new AutoResetEvent(false);

            scanner.ScanCompleted += (s, e) => { signalScanCompleteEvent.Set(); };
            scanner.BeginScan();

            var scanCompletedSignaled = signalScanCompleteEvent.WaitOne(TimeSpan.FromSeconds(10));

            Assert.IsTrue(scanCompletedSignaled);
        }
    }
}
