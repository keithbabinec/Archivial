using ArchivialLibrary.Database.SQLServer;
using ArchivialLibrary.Engine;
using ArchivialLibrary.Logging.Mock;
using ArchivialLibrary.ServiceCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;

namespace ArchivialLibraryTests.Engine
{
    [TestClass]
    public class CoreServiceEngineTests
    {
        private const string TestConnectionString = "fakedb";

        private ICoreSettings SharedMockedCoreSettings = new Mock<ICoreSettings>().Object;

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CoreServiceEngineConstructorThrowsExceptionWhenNoDatabaseIsProvided()
        {
            var engine = new CoreServiceEngine(null, new MockLogger(), 0, SharedMockedCoreSettings);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CoreServiceEngineConstructorThrowsExceptionWhenNoLoggerIsProvided()
        {
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger(), SharedMockedCoreSettings);

            var engine = new CoreServiceEngine(db, null, 0, SharedMockedCoreSettings);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CoreServiceEngineConstructorThrowsExceptionWhenNoCoreSettingsProvided()
        {
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger(), SharedMockedCoreSettings);

            var engine = new CoreServiceEngine(db, new MockLogger(), 0, null);
        }

        [TestMethod]
        public void CoreServiceEngineConstructorDoesNotThrowWhenValidArgumentsAreProvided()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger(), SharedMockedCoreSettings);

            var engine = new CoreServiceEngine(db, logger, 0, SharedMockedCoreSettings);

            Assert.IsNotNull(engine);
        }

        [TestMethod]
        public void CoreServiceEngineCanStartAndStop()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger(), SharedMockedCoreSettings);

            var engine = new CoreServiceEngine(db, logger, 0, SharedMockedCoreSettings);

            engine.BeginStart();
            engine.BeginStop();
        }

        [TestMethod]
        public void CoreServiceEngineTriggersStoppedEventWhenEngineHasStopped()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger(), SharedMockedCoreSettings);

            var engine = new CoreServiceEngine(db, logger, 0, SharedMockedCoreSettings);

            var signalStoppedEvent = new AutoResetEvent(false);

            engine.Stopped += (s, e) => { signalStoppedEvent.Set(); };
            engine.BeginStart();
            engine.BeginStop();

            var engineStoppedSignaled = signalStoppedEvent.WaitOne(TimeSpan.FromSeconds(5));

            Assert.IsTrue(engineStoppedSignaled);
        }
    }
}
