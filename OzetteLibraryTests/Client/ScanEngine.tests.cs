using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OzetteLibrary.Database.LiteDB;
using OzetteLibrary.Logging.Mock;
using OzetteLibrary.Providers;
using System;
using System.IO;
using System.Threading;

namespace OzetteLibraryTests.Client
{
    [TestClass]
    public class ScanEngineTests
    {
        private ProviderConnectionsCollection GenerateMockProviders()
        {
            var providers = new ProviderConnectionsCollection();
            var mockedProvider = new Mock<IProviderFileOperations>();
            providers.Add(ProviderTypes.Azure, mockedProvider.Object);

            return providers;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScanEngineConstructorThrowsExceptionWhenNoDatabaseIsProvided()
        {
            OzetteLibrary.Client.ScanEngine engine =
                new OzetteLibrary.Client.ScanEngine(null, new MockLogger(), GenerateMockProviders());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScanEngineConstructorThrowsExceptionWhenNoLoggerIsProvided()
        {
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), new MockLogger());
            
            OzetteLibrary.Client.ScanEngine engine =
                new OzetteLibrary.Client.ScanEngine(inMemoryDB, null, GenerateMockProviders());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScanEngineConstructorThrowsExceptionWhenNullProvidersAreProvided()
        {
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), new MockLogger());

            OzetteLibrary.Client.ScanEngine engine =
                new OzetteLibrary.Client.ScanEngine(inMemoryDB, new MockLogger(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ScanEngineConstructorThrowsExceptionWhenNoProvidersAreProvided()
        {
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), new MockLogger());

            var providers = GenerateMockProviders();
            providers.Clear(); // a valid collection, but empty

            OzetteLibrary.Client.ScanEngine engine =
                new OzetteLibrary.Client.ScanEngine(inMemoryDB, new MockLogger(), providers);
        }

        [TestMethod]
        public void ScanEngineConstructorDoesNotThrowWhenValidArgumentsAreProvided()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), logger);

            OzetteLibrary.Client.ScanEngine engine =
                new OzetteLibrary.Client.ScanEngine(inMemoryDB, logger, GenerateMockProviders());

            Assert.IsNotNull(engine);
        }

        [TestMethod]
        public void ScanEngineCanStartAndStop()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream(), logger);

            inMemoryDB.PrepareDatabase();

            OzetteLibrary.Client.ScanEngine engine =
                new OzetteLibrary.Client.ScanEngine(inMemoryDB, logger, GenerateMockProviders());

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
                new OzetteLibrary.Client.ScanEngine(inMemoryDB, logger, GenerateMockProviders());

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
                new OzetteLibrary.Client.ScanEngine(inMemoryDB, logger, GenerateMockProviders());

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
