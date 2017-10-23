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
                new OzetteLibrary.Client.Sources.Scanner(null, new OzetteLibrary.Database.MockClientDB());
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
                    new OzetteLibrary.Database.MockClientDB());

            Assert.IsTrue(true);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ScannerThrowsWhenBeginScanIsCalledAfterScanHasAlreadyStarted()
        {
            OzetteLibrary.Client.Sources.Scanner scanner =
                new OzetteLibrary.Client.Sources.Scanner(
                    new OzetteLibrary.Models.SourceLocation(),
                    new OzetteLibrary.Database.MockClientDB());

            scanner.BeginScan();
            scanner.BeginScan();
        }

        [TestMethod()]
        public void ScannerTriggersScanCompletedEventAfterScanHasCompleted()
        {
            OzetteLibrary.Client.Sources.Scanner scanner =
                new OzetteLibrary.Client.Sources.Scanner(
                    new OzetteLibrary.Models.SourceLocation(),
                    new OzetteLibrary.Database.MockClientDB());

            var signalScanCompleteEvent = new AutoResetEvent(false);

            scanner.ScanCompleted += (s, e) => { signalScanCompleteEvent.Set(); };
            scanner.BeginScan();

            var scanCompletedSignaled = signalScanCompleteEvent.WaitOne(TimeSpan.FromSeconds(10));

            Assert.IsTrue(scanCompletedSignaled);
        }
    }
}
