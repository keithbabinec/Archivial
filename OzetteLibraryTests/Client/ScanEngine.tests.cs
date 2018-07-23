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
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScanEngineConstructorThrowsExceptionWhenNoDatabaseIsProvided()
        {
            OzetteLibrary.Client.ScanEngine engine =
                new OzetteLibrary.Client.ScanEngine(null, new MockLogger());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScanEngineConstructorThrowsExceptionWhenNoLoggerIsProvided()
        {
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), new MockLogger());
            
            OzetteLibrary.Client.ScanEngine engine =
                new OzetteLibrary.Client.ScanEngine(inMemoryDB, null);
        }

        [TestMethod]
        public void ScanEngineConstructorDoesNotThrowWhenValidArgumentsAreProvided()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), logger);

            OzetteLibrary.Client.ScanEngine engine =
                new OzetteLibrary.Client.ScanEngine(inMemoryDB, logger);

            Assert.IsNotNull(engine);
        }

        [TestMethod]
        public void ScanEngineCanStartAndStop()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), logger);

            inMemoryDB.PrepareDatabase();

            OzetteLibrary.Client.ScanEngine engine =
                new OzetteLibrary.Client.ScanEngine(inMemoryDB, logger);

            engine.BeginStart();
            engine.BeginStop();
        }

        [TestMethod]
        public void ScanEngineTriggersStoppedEventWhenEngineHasStopped()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), logger);

            inMemoryDB.PrepareDatabase();

            OzetteLibrary.Client.ScanEngine engine =
                new OzetteLibrary.Client.ScanEngine(inMemoryDB, logger);

            var signalStoppedEvent = new AutoResetEvent(false);

            engine.Stopped += (s, e) => { signalStoppedEvent.Set(); };
            engine.BeginStart();
            engine.BeginStop();

            var engineStoppedSignaled = signalStoppedEvent.WaitOne(TimeSpan.FromSeconds(5));

            Assert.IsTrue(engineStoppedSignaled);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ScanEngineThrowsExceptionWhenEngineIsStartedTwice()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), logger);

            inMemoryDB.PrepareDatabase();

            OzetteLibrary.Client.ScanEngine engine =
                new OzetteLibrary.Client.ScanEngine(inMemoryDB, logger);

            try
            {
                engine.BeginStart();
                engine.BeginStart();
            }
            finally
            {
                engine.BeginStop();
                Thread.Sleep(2000);
            }
        }
    }
}
