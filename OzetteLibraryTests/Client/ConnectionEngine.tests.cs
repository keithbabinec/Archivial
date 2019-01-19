using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OzetteLibrary.Database.SQLServer;
using OzetteLibrary.Logging.Mock;
using OzetteLibrary.MessagingProviders;
using OzetteLibrary.StorageProviders;
using System;
using System.Threading;

namespace OzetteLibraryTests.Client
{
    [TestClass]
    public class ConnectionEngineTests
    {
        private const string TestConnectionString = "fakedb";

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
        public void ConnectionEngineConstructorThrowsExceptionWhenNoDatabaseIsProvided()
        {
            OzetteLibrary.Client.ConnectionEngine engine =
                new OzetteLibrary.Client.ConnectionEngine(null, new MockLogger(), GenerateMockStorageProviders(), GenerateMockMessagingProviders());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConnectionEngineConstructorThrowsExceptionWhenNullStorageProvidersAreProvided()
        {
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            OzetteLibrary.Client.ConnectionEngine engine =
                new OzetteLibrary.Client.ConnectionEngine(db, new MockLogger(), null, GenerateMockMessagingProviders());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConnectionEngineConstructorThrowsExceptionWhenNoStorageProvidersAreProvided()
        {
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            var providers = GenerateMockStorageProviders();
            providers.Clear(); // a valid collection, but empty

            OzetteLibrary.Client.ConnectionEngine engine =
                new OzetteLibrary.Client.ConnectionEngine(db, new MockLogger(), providers, GenerateMockMessagingProviders());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConnectionEngineConstructorThrowsExceptionWhenNullMessagingProvidersAreProvided()
        {
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            OzetteLibrary.Client.ConnectionEngine engine =
                new OzetteLibrary.Client.ConnectionEngine(db, new MockLogger(), GenerateMockStorageProviders(), null);
        }

        [TestMethod]
        public void ConnectionEngineConstructorDoesNotThrowExceptionWhenNoMessagingProvidersAreProvided()
        {
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            // a valid (empty) collection -- should not throw.
            var msgProviders = new MessagingProviderConnectionsCollection();

            OzetteLibrary.Client.ConnectionEngine engine =
                new OzetteLibrary.Client.ConnectionEngine(db, new MockLogger(), GenerateMockStorageProviders(), msgProviders);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConnectionEngineConstructorThrowsExceptionWhenNoLoggerIsProvided()
        {
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            OzetteLibrary.Client.ConnectionEngine engine =
                new OzetteLibrary.Client.ConnectionEngine(db, null, GenerateMockStorageProviders(), GenerateMockMessagingProviders());
        }

        [TestMethod]
        public void ConnectionEngineConstructorDoesNotThrowWhenValidArgumentsAreProvided()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            OzetteLibrary.Client.ConnectionEngine engine =
                new OzetteLibrary.Client.ConnectionEngine(db, logger, GenerateMockStorageProviders(), GenerateMockMessagingProviders());

            Assert.IsNotNull(engine);
        }

        [TestMethod]
        public void ConnectionEngineCanStartAndStop()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            OzetteLibrary.Client.ConnectionEngine engine =
                new OzetteLibrary.Client.ConnectionEngine(db, logger, GenerateMockStorageProviders(), GenerateMockMessagingProviders());

            engine.BeginStart();
            engine.BeginStop();
        }

        [TestMethod]
        public void ConnectionEngineTriggersStoppedEventWhenEngineHasStopped()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            OzetteLibrary.Client.ConnectionEngine engine =
                new OzetteLibrary.Client.ConnectionEngine(db, logger, GenerateMockStorageProviders(), GenerateMockMessagingProviders());

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
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            OzetteLibrary.Client.ConnectionEngine engine =
                new OzetteLibrary.Client.ConnectionEngine(db, logger, GenerateMockStorageProviders(), GenerateMockMessagingProviders());

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
