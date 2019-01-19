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
    public class BackupEngineTests
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
        public void BackupEngineConstructorThrowsExceptionWhenNoDatabaseIsProvided()
        {
            OzetteLibrary.Client.BackupEngine engine =
                new OzetteLibrary.Client.BackupEngine(null, new MockLogger(), GenerateMockStorageProviders(), GenerateMockMessagingProviders(), 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BackupEngineConstructorThrowsExceptionWhenNullStorageProvidersAreProvided()
        {
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            OzetteLibrary.Client.BackupEngine engine =
                new OzetteLibrary.Client.BackupEngine(db, new MockLogger(), null, GenerateMockMessagingProviders(), 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BackupEngineConstructorThrowsExceptionWhenNoStorageProvidersAreProvided()
        {
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            var providers = GenerateMockStorageProviders();
            providers.Clear(); // a valid collection, but empty

            OzetteLibrary.Client.BackupEngine engine =
                new OzetteLibrary.Client.BackupEngine(db, new MockLogger(), providers, GenerateMockMessagingProviders(), 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BackupEngineConstructorThrowsExceptionWhenNullMessagingProvidersAreProvided()
        {
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            OzetteLibrary.Client.BackupEngine engine =
                new OzetteLibrary.Client.BackupEngine(db, new MockLogger(), GenerateMockStorageProviders(), null, 0);
        }

        [TestMethod]
        public void BackupEngineConstructorDoesNotThrowExceptionWhenNoMessagingProvidersAreProvided()
        {
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            // a valid (empty) collection -- should not throw.
            var msgProviders = new MessagingProviderConnectionsCollection();
            
            OzetteLibrary.Client.BackupEngine engine =
                new OzetteLibrary.Client.BackupEngine(db, new MockLogger(), GenerateMockStorageProviders(), msgProviders, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BackupEngineConstructorThrowsExceptionWhenNoLoggerIsProvided()
        {
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            OzetteLibrary.Client.BackupEngine engine =
                new OzetteLibrary.Client.BackupEngine(db, null, GenerateMockStorageProviders(), GenerateMockMessagingProviders(), 0);
        }

        [TestMethod]
        public void BackupEngineConstructorDoesNotThrowWhenValidArgumentsAreProvided()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            OzetteLibrary.Client.BackupEngine engine =
                new OzetteLibrary.Client.BackupEngine(db, logger, GenerateMockStorageProviders(), GenerateMockMessagingProviders(), 0);

            Assert.IsNotNull(engine);
        }

        [TestMethod]
        public void BackupEngineCanStartAndStop()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            OzetteLibrary.Client.BackupEngine engine =
                new OzetteLibrary.Client.BackupEngine(db, logger, GenerateMockStorageProviders(), GenerateMockMessagingProviders(), 0);

            engine.BeginStart();
            engine.BeginStop();
        }

        [TestMethod]
        public void BackupEngineTriggersStoppedEventWhenEngineHasStopped()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            OzetteLibrary.Client.BackupEngine engine =
                new OzetteLibrary.Client.BackupEngine(db, logger, GenerateMockStorageProviders(), GenerateMockMessagingProviders(), 0);

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
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            OzetteLibrary.Client.BackupEngine engine =
                new OzetteLibrary.Client.BackupEngine(db, logger, GenerateMockStorageProviders(), GenerateMockMessagingProviders(), 0);

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
