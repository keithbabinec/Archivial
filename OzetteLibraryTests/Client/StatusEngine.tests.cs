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
    public class StatusEngineTests
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
        public void StatusEngineConstructorThrowsExceptionWhenNoDatabaseIsProvided()
        {
            OzetteLibrary.Client.StatusEngine engine =
                new OzetteLibrary.Client.StatusEngine(null, new MockLogger(), GenerateMockProviders());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StatusEngineConstructorThrowsExceptionWhenNullProvidersAreProvided()
        {
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream());

            OzetteLibrary.Client.StatusEngine engine =
                new OzetteLibrary.Client.StatusEngine(inMemoryDB, new MockLogger(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void StatusEngineConstructorThrowsExceptionWhenNoProvidersAreProvided()
        {
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream());

            var providers = GenerateMockProviders();
            providers.Clear(); // a valid collection, but empty

            OzetteLibrary.Client.StatusEngine engine =
                new OzetteLibrary.Client.StatusEngine(inMemoryDB, new MockLogger(), providers);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StatusEngineConstructorThrowsExceptionWhenNoLoggerIsProvided()
        {
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream());

            OzetteLibrary.Client.StatusEngine engine =
                new OzetteLibrary.Client.StatusEngine(inMemoryDB, null, GenerateMockProviders());
        }

        [TestMethod]
        public void StatusEngineConstructorDoesNotThrowWhenValidArgumentsAreProvided()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream());

            OzetteLibrary.Client.StatusEngine engine =
                new OzetteLibrary.Client.StatusEngine(inMemoryDB, logger, GenerateMockProviders());

            Assert.IsNotNull(engine);
        }

        [TestMethod]
        public void StatusEngineCanStartAndStop()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream());

            inMemoryDB.PrepareDatabase();

            OzetteLibrary.Client.StatusEngine engine =
                new OzetteLibrary.Client.StatusEngine(inMemoryDB, logger, GenerateMockProviders());

            engine.BeginStart();
            engine.BeginStop();
        }

        [TestMethod]
        public void StatusEngineTriggersStoppedEventWhenEngineHasStopped()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream());

            inMemoryDB.PrepareDatabase();

            OzetteLibrary.Client.StatusEngine engine =
                new OzetteLibrary.Client.StatusEngine(inMemoryDB, logger, GenerateMockProviders());

            var signalStoppedEvent = new AutoResetEvent(false);

            engine.Stopped += (s, e) => { signalStoppedEvent.Set(); };
            engine.BeginStart();
            engine.BeginStop();

            var engineStoppedSignaled = signalStoppedEvent.WaitOne(TimeSpan.FromSeconds(5));

            Assert.IsTrue(engineStoppedSignaled);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void StatusEngineThrowsExceptionWhenEngineIsStartedTwice()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream());

            inMemoryDB.PrepareDatabase();

            OzetteLibrary.Client.StatusEngine engine =
                new OzetteLibrary.Client.StatusEngine(inMemoryDB, logger, GenerateMockProviders());

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
