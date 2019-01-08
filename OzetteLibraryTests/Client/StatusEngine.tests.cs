using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OzetteLibrary.Database.LiteDB;
using OzetteLibrary.Logging.Mock;
using OzetteLibrary.MessagingProviders;
using OzetteLibrary.StorageProviders;
using System;
using System.IO;
using System.Threading;

namespace OzetteLibraryTests.Client
{
    [TestClass]
    public class StatusEngineTests
    {
        private StorageProviderConnectionsCollection GenerateMockStorageProviders()
        {
            var providers = new StorageProviderConnectionsCollection();
            var mockedProvider = new Mock<IStorageProviderFileOperations>();
            providers.Add(StorageProviderTypes.Azure, mockedProvider.Object);

            return providers;
        }

        private MessagingProviderConnectionsCollection GenerateMockMessagingProviders()
        {
            var providers = new MessagingProviderConnectionsCollection();
            var mockedProvider = new Mock<IMessagingProviderOperations>();
            providers.Add(MessagingProviderTypes.Twilio, mockedProvider.Object);

            return providers;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StatusEngineConstructorThrowsExceptionWhenNoDatabaseIsProvided()
        {
            OzetteLibrary.Client.StatusEngine engine =
                new OzetteLibrary.Client.StatusEngine(null, new MockLogger(), GenerateMockStorageProviders(), GenerateMockMessagingProviders());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StatusEngineConstructorThrowsExceptionWhenNullStorageProvidersAreProvided()
        {
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream());

            OzetteLibrary.Client.StatusEngine engine =
                new OzetteLibrary.Client.StatusEngine(inMemoryDB, new MockLogger(), null, GenerateMockMessagingProviders());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void StatusEngineConstructorThrowsExceptionWhenNoStorageProvidersAreProvided()
        {
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream());

            var providers = GenerateMockStorageProviders();
            providers.Clear(); // a valid collection, but empty

            OzetteLibrary.Client.StatusEngine engine =
                new OzetteLibrary.Client.StatusEngine(inMemoryDB, new MockLogger(), providers, GenerateMockMessagingProviders());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StatusEngineConstructorThrowsExceptionWhenNullMessagingProvidersAreProvided()
        {
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream());

            OzetteLibrary.Client.StatusEngine engine =
                new OzetteLibrary.Client.StatusEngine(inMemoryDB, new MockLogger(), GenerateMockStorageProviders(), null);
        }

        [TestMethod]
        public void StatusEngineConstructorDoesNotThrowExceptionWhenNoMessagingProvidersAreProvided()
        {
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream());

            // a valid (empty) collection -- should not throw.
            var msgProviders = new MessagingProviderConnectionsCollection();

            OzetteLibrary.Client.StatusEngine engine =
                new OzetteLibrary.Client.StatusEngine(inMemoryDB, new MockLogger(), GenerateMockStorageProviders(), msgProviders);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StatusEngineConstructorThrowsExceptionWhenNoLoggerIsProvided()
        {
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream());

            OzetteLibrary.Client.StatusEngine engine =
                new OzetteLibrary.Client.StatusEngine(inMemoryDB, null, GenerateMockStorageProviders(), GenerateMockMessagingProviders());
        }

        [TestMethod]
        public void StatusEngineConstructorDoesNotThrowWhenValidArgumentsAreProvided()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream());

            OzetteLibrary.Client.StatusEngine engine =
                new OzetteLibrary.Client.StatusEngine(inMemoryDB, logger, GenerateMockStorageProviders(), GenerateMockMessagingProviders());

            Assert.IsNotNull(engine);
        }

        [TestMethod]
        public void StatusEngineCanStartAndStop()
        {
            var logger = new MockLogger();
            var inMemoryDB = new LiteDBClientDatabase(new MemoryStream());

            inMemoryDB.PrepareDatabase();

            OzetteLibrary.Client.StatusEngine engine =
                new OzetteLibrary.Client.StatusEngine(inMemoryDB, logger, GenerateMockStorageProviders(), GenerateMockMessagingProviders());

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
                new OzetteLibrary.Client.StatusEngine(inMemoryDB, logger, GenerateMockStorageProviders(), GenerateMockMessagingProviders());

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
                new OzetteLibrary.Client.StatusEngine(inMemoryDB, logger, GenerateMockStorageProviders(), GenerateMockMessagingProviders());

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
