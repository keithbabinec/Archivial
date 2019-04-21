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
    public class ScanEngineTests
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
        public void ScanEngineConstructorThrowsExceptionWhenNoDatabaseIsProvided()
        {
            ArchivialLibrary.Client.ScanEngine engine =
                new ArchivialLibrary.Client.ScanEngine(null, new MockLogger(), 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScanEngineConstructorThrowsExceptionWhenNoLoggerIsProvided()
        {
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());
            
            ArchivialLibrary.Client.ScanEngine engine =
                new ArchivialLibrary.Client.ScanEngine(db, null, 0);
        }
        
        [TestMethod]
        public void ScanEngineConstructorDoesNotThrowWhenValidArgumentsAreProvided()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            ArchivialLibrary.Client.ScanEngine engine =
                new ArchivialLibrary.Client.ScanEngine(db, logger, 0);

            Assert.IsNotNull(engine);
        }

        [TestMethod]
        public void ScanEngineCanStartAndStop()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            ArchivialLibrary.Client.ScanEngine engine =
                new ArchivialLibrary.Client.ScanEngine(db, logger, 0);

            engine.BeginStart();
            engine.BeginStop();
        }

        [TestMethod]
        public void ScanEngineTriggersStoppedEventWhenEngineHasStopped()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            ArchivialLibrary.Client.ScanEngine engine =
                new ArchivialLibrary.Client.ScanEngine(db, logger, 0);

            var signalStoppedEvent = new AutoResetEvent(false);

            engine.Stopped += (s, e) => { signalStoppedEvent.Set(); };
            engine.BeginStart();
            engine.BeginStop();

            var engineStoppedSignaled = signalStoppedEvent.WaitOne(TimeSpan.FromSeconds(5));

            Assert.IsTrue(engineStoppedSignaled);
        }
    }
}
