using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ArchivialLibrary.Database.SQLServer;
using ArchivialLibrary.Logging.Mock;
using ArchivialLibrary.MessagingProviders;
using ArchivialLibrary.StorageProviders;
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
            ArchivialLibrary.Client.ConnectionEngine engine =
                new ArchivialLibrary.Client.ConnectionEngine(null, new MockLogger());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConnectionEngineConstructorThrowsExceptionWhenNoLoggerIsProvided()
        {
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            ArchivialLibrary.Client.ConnectionEngine engine =
                new ArchivialLibrary.Client.ConnectionEngine(db, null);
        }

        [TestMethod]
        public void ConnectionEngineConstructorDoesNotThrowWhenValidArgumentsAreProvided()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            ArchivialLibrary.Client.ConnectionEngine engine =
                new ArchivialLibrary.Client.ConnectionEngine(db, logger);

            Assert.IsNotNull(engine);
        }

        [TestMethod]
        public void ConnectionEngineCanStartAndStop()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            ArchivialLibrary.Client.ConnectionEngine engine =
                new ArchivialLibrary.Client.ConnectionEngine(db, logger);

            engine.BeginStart();
            engine.BeginStop();
        }

        [TestMethod]
        public void ConnectionEngineTriggersStoppedEventWhenEngineHasStopped()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            ArchivialLibrary.Client.ConnectionEngine engine =
                new ArchivialLibrary.Client.ConnectionEngine(db, logger);

            var signalStoppedEvent = new AutoResetEvent(false);

            engine.Stopped += (s, e) => { signalStoppedEvent.Set(); };
            engine.BeginStart();
            engine.BeginStop();

            var engineStoppedSignaled = signalStoppedEvent.WaitOne(TimeSpan.FromSeconds(5));

            Assert.IsTrue(engineStoppedSignaled);
        }
    }
}
