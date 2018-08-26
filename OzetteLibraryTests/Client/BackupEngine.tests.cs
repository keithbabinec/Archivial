using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OzetteLibrary.Database.LiteDB;
using OzetteLibrary.Logging.Mock;
using OzetteLibrary.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace OzetteLibraryTests.Client
{
    [TestClass]
    public class BackupEngineTests
    {
        private Dictionary<ProviderTypes, IProviderFileOperations> GenerateMockProviders()
        {
            Dictionary<ProviderTypes, IProviderFileOperations> providers = new Dictionary<ProviderTypes, IProviderFileOperations>();
            var mockedProvider = new Mock<IProviderFileOperations>();
            providers.Add(ProviderTypes.Azure, mockedProvider.Object);

            return providers;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BackupEngineConstructorThrowsExceptionWhenNoDatabaseIsProvided()
        {
            OzetteLibrary.Client.BackupEngine engine =
                new OzetteLibrary.Client.BackupEngine(null, new MockLogger(), GenerateMockProviders());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BackupEngineConstructorThrowsExceptionWhenNullProvidersAreProvided()
        {
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), new MockLogger());

            OzetteLibrary.Client.BackupEngine engine =
                new OzetteLibrary.Client.BackupEngine(inMemoryDB, new MockLogger(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BackupEngineConstructorThrowsExceptionWhenNoProvidersAreProvided()
        {
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), new MockLogger());

            var providers = GenerateMockProviders();
            providers.Clear(); // a valid collection, but empty

            OzetteLibrary.Client.BackupEngine engine =
                new OzetteLibrary.Client.BackupEngine(inMemoryDB, new MockLogger(), providers);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BackupEngineConstructorThrowsExceptionWhenNoLoggerIsProvided()
        {
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), new MockLogger());

            OzetteLibrary.Client.BackupEngine engine =
                new OzetteLibrary.Client.BackupEngine(inMemoryDB, null, GenerateMockProviders());
        }

        [TestMethod]
        public void BackupEngineConstructorDoesNotThrowWhenValidArgumentsAreProvided()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), logger);

            OzetteLibrary.Client.BackupEngine engine =
                new OzetteLibrary.Client.BackupEngine(inMemoryDB, logger, GenerateMockProviders());

            Assert.IsNotNull(engine);
        }

        [TestMethod]
        public void BackupEngineCanStartAndStop()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), logger);

            inMemoryDB.PrepareDatabase();

            OzetteLibrary.Client.BackupEngine engine =
                new OzetteLibrary.Client.BackupEngine(inMemoryDB, logger, GenerateMockProviders());

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
                new OzetteLibrary.Client.BackupEngine(inMemoryDB, logger, GenerateMockProviders());

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
                new OzetteLibrary.Client.BackupEngine(inMemoryDB, logger, GenerateMockProviders());

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
