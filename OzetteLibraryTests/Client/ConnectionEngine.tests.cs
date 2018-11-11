using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OzetteLibrary.Database.LiteDB;
using OzetteLibrary.Logging.Mock;
using OzetteLibrary.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OzetteLibraryTests.Client
{
    [TestClass]
    public class ConnectionEngineTests
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
        public void ConnectionEngineConstructorThrowsExceptionWhenNoDatabaseIsProvided()
        {
            OzetteLibrary.Client.ConnectionEngine engine =
                new OzetteLibrary.Client.ConnectionEngine(null, new MockLogger(), GenerateMockProviders());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConnectionEngineConstructorThrowsExceptionWhenNullProvidersAreProvided()
        {
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream());

            OzetteLibrary.Client.ConnectionEngine engine =
                new OzetteLibrary.Client.ConnectionEngine(inMemoryDB, new MockLogger(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConnectionEngineConstructorThrowsExceptionWhenNoProvidersAreProvided()
        {
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream());

            var providers = GenerateMockProviders();
            providers.Clear(); // a valid collection, but empty

            OzetteLibrary.Client.ConnectionEngine engine =
                new OzetteLibrary.Client.ConnectionEngine(inMemoryDB, new MockLogger(), providers);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConnectionEngineConstructorThrowsExceptionWhenNoLoggerIsProvided()
        {
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream());

            OzetteLibrary.Client.ConnectionEngine engine =
                new OzetteLibrary.Client.ConnectionEngine(inMemoryDB, null, GenerateMockProviders());
        }

        [TestMethod]
        public void ConnectionEngineConstructorDoesNotThrowWhenValidArgumentsAreProvided()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream());

            OzetteLibrary.Client.ConnectionEngine engine =
                new OzetteLibrary.Client.ConnectionEngine(inMemoryDB, logger, GenerateMockProviders());

            Assert.IsNotNull(engine);
        }

        [TestMethod]
        public void ConnectionEngineCanStartAndStop()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream());

            inMemoryDB.PrepareDatabase();

            OzetteLibrary.Client.ConnectionEngine engine =
                new OzetteLibrary.Client.ConnectionEngine(inMemoryDB, logger, GenerateMockProviders());

            engine.BeginStart();
            engine.BeginStop();
        }

        [TestMethod]
        public void ConnectionEngineTriggersStoppedEventWhenEngineHasStopped()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream());

            inMemoryDB.PrepareDatabase();

            OzetteLibrary.Client.ConnectionEngine engine =
                new OzetteLibrary.Client.ConnectionEngine(inMemoryDB, logger, GenerateMockProviders());

            var signalStoppedEvent = new AutoResetEvent(false);

            engine.Stopped += (s, e) => { signalStoppedEvent.Set(); };
            engine.BeginStart();
            engine.BeginStop();

            var engineStoppedSignaled = signalStoppedEvent.WaitOne(TimeSpan.FromSeconds(5));

            Assert.IsTrue(engineStoppedSignaled);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConnectionEngineThrowsExceptionWhenEngineIsStartedTwice()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream());

            inMemoryDB.PrepareDatabase();

            OzetteLibrary.Client.ConnectionEngine engine =
                new OzetteLibrary.Client.ConnectionEngine(inMemoryDB, logger, GenerateMockProviders());

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
