using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzetteLibrary.Database.LiteDB;
using OzetteLibrary.Logging.Mock;
using System;
using System.IO;
using System.Threading;

namespace OzetteLibraryTests.Client
{
    [TestClass]
    public class ScanEngineTests
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScanEngineConstructorThrowsExceptionWhenNoDatabaseIsProvided()
        {
            OzetteLibrary.Client.ScanEngine engine =
                new OzetteLibrary.Client.ScanEngine(null, new MockLogger(), new OzetteLibrary.ServiceCore.ServiceOptions());
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScanEngineConstructorThrowsExceptionWhenNoLoggerIsProvided()
        {
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), new MockLogger());
            
            OzetteLibrary.Client.ScanEngine engine =
                new OzetteLibrary.Client.ScanEngine(inMemoryDB, null, new OzetteLibrary.ServiceCore.ServiceOptions());
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScanEngineConstructorThrowsExceptionWhenNoOptionsAreProvided()
        {
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), new MockLogger());

            OzetteLibrary.Client.ScanEngine engine =
                new OzetteLibrary.Client.ScanEngine(inMemoryDB, new MockLogger(), null);
        }

        [TestMethod()]
        public void ScanEngineConstructorDoesNotThrowWhenValidArgumentsAreProvided()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), logger);

            OzetteLibrary.Client.ScanEngine engine =
                new OzetteLibrary.Client.ScanEngine(inMemoryDB, logger, new OzetteLibrary.ServiceCore.ServiceOptions());

            Assert.IsNotNull(engine);
        }

        [TestMethod()]
        public void ScanEngineCanStartAndStop()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), logger);

            inMemoryDB.PrepareDatabase();

            OzetteLibrary.Client.ScanEngine engine =
                new OzetteLibrary.Client.ScanEngine(inMemoryDB, logger, new OzetteLibrary.ServiceCore.ServiceOptions());

            engine.BeginStart();
            engine.BeginStop();
        }

        [TestMethod()]
        public void ScanEngineTriggersStoppedEventWhenEngineHasStopped()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), logger);

            inMemoryDB.PrepareDatabase();

            OzetteLibrary.Client.ScanEngine engine =
                new OzetteLibrary.Client.ScanEngine(inMemoryDB, logger, new OzetteLibrary.ServiceCore.ServiceOptions());

            var signalStoppedEvent = new AutoResetEvent(false);

            engine.Stopped += (s, e) => { signalStoppedEvent.Set(); };
            engine.BeginStart();
            engine.BeginStop();

            var engineStoppedSignaled = signalStoppedEvent.WaitOne(TimeSpan.FromSeconds(5));

            Assert.IsTrue(engineStoppedSignaled);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ScanEngineThrowsExceptionWhenEngineIsStartedTwice()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), logger);

            inMemoryDB.PrepareDatabase();

            OzetteLibrary.Client.ScanEngine engine =
                new OzetteLibrary.Client.ScanEngine(inMemoryDB, logger, new OzetteLibrary.ServiceCore.ServiceOptions());

            engine.BeginStart();
            engine.BeginStart();
        }
    }
}
