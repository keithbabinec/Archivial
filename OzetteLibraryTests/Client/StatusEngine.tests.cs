﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class StatusEngineTests
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
        public void StatusEngineConstructorThrowsExceptionWhenNoDatabaseIsProvided()
        {
            OzetteLibrary.Client.StatusEngine engine =
                new OzetteLibrary.Client.StatusEngine(null, new MockLogger(), 0);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StatusEngineConstructorThrowsExceptionWhenNoLoggerIsProvided()
        {
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            OzetteLibrary.Client.StatusEngine engine =
                new OzetteLibrary.Client.StatusEngine(db, null, 0);
        }

        [TestMethod]
        public void StatusEngineConstructorDoesNotThrowWhenValidArgumentsAreProvided()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            OzetteLibrary.Client.StatusEngine engine =
                new OzetteLibrary.Client.StatusEngine(db, logger, 0);

            Assert.IsNotNull(engine);
        }

        [TestMethod]
        public void StatusEngineCanStartAndStop()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            OzetteLibrary.Client.StatusEngine engine =
                new OzetteLibrary.Client.StatusEngine(db, logger, 0);

            engine.BeginStart();
            engine.BeginStop();
        }

        [TestMethod]
        public void StatusEngineTriggersStoppedEventWhenEngineHasStopped()
        {
            var logger = new MockLogger();
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());

            OzetteLibrary.Client.StatusEngine engine =
                new OzetteLibrary.Client.StatusEngine(db, logger, 0);

            var signalStoppedEvent = new AutoResetEvent(false);

            engine.Stopped += (s, e) => { signalStoppedEvent.Set(); };
            engine.BeginStart();
            engine.BeginStop();

            var engineStoppedSignaled = signalStoppedEvent.WaitOne(TimeSpan.FromSeconds(5));

            Assert.IsTrue(engineStoppedSignaled);
        }
    }
}
