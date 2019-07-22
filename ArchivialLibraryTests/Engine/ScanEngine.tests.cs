using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ArchivialLibrary.Database.SQLServer;
using ArchivialLibrary.Logging.Mock;
using System;
using System.Threading;
using ArchivialLibrary.ServiceCore;
using ArchivialLibrary.Engine;

namespace ArchivialLibraryTests.Engine
{
    [TestClass]
    public class ScanEngineTests
    {
        private const string TestConnectionString = "fakedb";

        private ICoreSettings SharedMockedCoreSettings = new Mock<ICoreSettings>().Object;

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScanEngineConstructorThrowsExceptionWhenNoDatabaseIsProvided()
        {
            var engine = new ScanEngine(null, new MockLogger(), 0, SharedMockedCoreSettings);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScanEngineConstructorThrowsExceptionWhenNoLoggerIsProvided()
        {
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger(), SharedMockedCoreSettings);

            var engine = new ScanEngine(db, null, 0, SharedMockedCoreSettings);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScanEngineConstructorThrowsExceptionWhenNoCoreSettingsProvided()
        {
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger(), SharedMockedCoreSettings);

            var engine = new ScanEngine(db, new MockLogger(), 0, null);
        }

        [TestMethod]
        public void ScanEngineConstructorDoesNotThrowWhenValidArgumentsAreProvided()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger(), SharedMockedCoreSettings);

            var engine = new ScanEngine(db, logger, 0, SharedMockedCoreSettings);

            Assert.IsNotNull(engine);
        }

        [TestMethod]
        public void ScanEngineCanStartAndStop()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger(), SharedMockedCoreSettings);

            var engine = new ScanEngine(db, logger, 0, SharedMockedCoreSettings);

            engine.BeginStart();
            engine.BeginStop();
        }

        [TestMethod]
        public void ScanEngineTriggersStoppedEventWhenEngineHasStopped()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger(), SharedMockedCoreSettings);

            var engine = new ScanEngine(db, logger, 0, SharedMockedCoreSettings);

            var signalStoppedEvent = new AutoResetEvent(false);

            engine.Stopped += (s, e) => { signalStoppedEvent.Set(); };
            engine.BeginStart();
            engine.BeginStop();

            var engineStoppedSignaled = signalStoppedEvent.WaitOne(TimeSpan.FromSeconds(5));

            Assert.IsTrue(engineStoppedSignaled);
        }
    }
}
