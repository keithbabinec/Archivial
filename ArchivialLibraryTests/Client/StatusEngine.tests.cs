using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ArchivialLibrary.Database.SQLServer;
using ArchivialLibrary.Logging.Mock;
using System;
using System.Threading;
using ArchivialLibrary.ServiceCore;

namespace ArchivialLibraryTests.Client
{
    [TestClass]
    public class StatusEngineTests
    {
        private const string TestConnectionString = "fakedb";

        private ICoreSettings SharedMockedCoreSettings = new Mock<ICoreSettings>().Object;

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StatusEngineConstructorThrowsExceptionWhenNoDatabaseIsProvided()
        {
            ArchivialLibrary.Client.StatusEngine engine =
                new ArchivialLibrary.Client.StatusEngine(null, new MockLogger(), 0, SharedMockedCoreSettings);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StatusEngineConstructorThrowsExceptionWhenNoLoggerIsProvided()
        {
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger(), SharedMockedCoreSettings);

            ArchivialLibrary.Client.StatusEngine engine =
                new ArchivialLibrary.Client.StatusEngine(db, null, 0, SharedMockedCoreSettings);
        }

        [TestMethod]
        public void StatusEngineConstructorDoesNotThrowWhenValidArgumentsAreProvided()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger(), SharedMockedCoreSettings);

            ArchivialLibrary.Client.StatusEngine engine =
                new ArchivialLibrary.Client.StatusEngine(db, logger, 0, SharedMockedCoreSettings);

            Assert.IsNotNull(engine);
        }

        [TestMethod]
        public void StatusEngineCanStartAndStop()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger(), SharedMockedCoreSettings);

            ArchivialLibrary.Client.StatusEngine engine =
                new ArchivialLibrary.Client.StatusEngine(db, logger, 0, SharedMockedCoreSettings);

            engine.BeginStart();
            engine.BeginStop();
        }

        [TestMethod]
        public void StatusEngineTriggersStoppedEventWhenEngineHasStopped()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger(), SharedMockedCoreSettings);

            ArchivialLibrary.Client.StatusEngine engine =
                new ArchivialLibrary.Client.StatusEngine(db, logger, 0, SharedMockedCoreSettings);

            var signalStoppedEvent = new AutoResetEvent(false);

            engine.Stopped += (s, e) => { signalStoppedEvent.Set(); };
            engine.BeginStart();
            engine.BeginStop();

            var engineStoppedSignaled = signalStoppedEvent.WaitOne(TimeSpan.FromSeconds(5));

            Assert.IsTrue(engineStoppedSignaled);
        }
    }
}
