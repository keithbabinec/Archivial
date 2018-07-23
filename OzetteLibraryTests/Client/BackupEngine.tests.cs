using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzetteLibrary.Database.LiteDB;
using OzetteLibrary.Logging.Mock;
using System;
using System.IO;
using System.Threading;

namespace OzetteLibraryTests.Client
{
    [TestClass]
    public class BackupEngineTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BackupEngineConstructorThrowsExceptionWhenNoDatabaseIsProvided()
        {
            OzetteLibrary.Client.BackupEngine engine =
                new OzetteLibrary.Client.BackupEngine(null, new MockLogger());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BackupEngineConstructorThrowsExceptionWhenNoLoggerIsProvided()
        {
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), new MockLogger());

            OzetteLibrary.Client.BackupEngine engine =
                new OzetteLibrary.Client.BackupEngine(inMemoryDB, null);
        }

        [TestMethod]
        public void BackupEngineConstructorDoesNotThrowWhenValidArgumentsAreProvided()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), logger);

            OzetteLibrary.Client.BackupEngine engine =
                new OzetteLibrary.Client.BackupEngine(inMemoryDB, logger);

            Assert.IsNotNull(engine);
        }

        [TestMethod]
        public void BackupEngineCanStartAndStop()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), logger);

            inMemoryDB.PrepareDatabase();

            OzetteLibrary.Client.BackupEngine engine =
                new OzetteLibrary.Client.BackupEngine(inMemoryDB, logger);

            engine.BeginStart();
            engine.BeginStop();
        }

        [TestMethod]
        public void BackupEngineTriggersStoppedEventWhenEngineHasStopped()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), logger);

            inMemoryDB.PrepareDatabase();

            OzetteLibrary.Client.BackupEngine engine =
                new OzetteLibrary.Client.BackupEngine(inMemoryDB, logger);

            var signalStoppedEvent = new AutoResetEvent(false);

            engine.Stopped += (s, e) => { signalStoppedEvent.Set(); };
            engine.BeginStart();
            engine.BeginStop();

            var engineStoppedSignaled = signalStoppedEvent.WaitOne(TimeSpan.FromSeconds(5));

            Assert.IsTrue(engineStoppedSignaled);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BackupEngineThrowsExceptionWhenEngineIsStartedTwice()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), logger);

            inMemoryDB.PrepareDatabase();

            OzetteLibrary.Client.BackupEngine engine =
                new OzetteLibrary.Client.BackupEngine(inMemoryDB, logger);

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
