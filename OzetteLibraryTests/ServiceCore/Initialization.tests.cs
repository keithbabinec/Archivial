using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzetteLibrary.Logging.Mock;
using System;
using System.Configuration;
using System.Threading;

namespace OzetteLibraryTests.ServiceCore
{
    [TestClass]
    public class InitializationTests
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceCoreInitializationConstructorThrowsWhenNullLoggerIsSent()
        {
            OzetteLibrary.ServiceCore.Initialization i = new OzetteLibrary.ServiceCore.Initialization(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceCoreInitializationBeginStartThrowsWhenNullPropertiesAreSent()
        {
            OzetteLibrary.ServiceCore.Initialization i = new OzetteLibrary.ServiceCore.Initialization(new MockLogger());
            i.BeginStart(null);
        }

        [TestMethod()]
        public void ServiceCoreInitializationCompletedEventFires()
        {
            OzetteLibrary.ServiceCore.Initialization i = new OzetteLibrary.ServiceCore.Initialization(new MockLogger());

            var signalCompleteEvent = new AutoResetEvent(false);

            i.Completed += (s, e) => { signalCompleteEvent.Set(); };

            i.BeginStart(new SettingsPropertyCollection());

            var completeSignaled = signalCompleteEvent.WaitOne(TimeSpan.FromSeconds(10));
            Assert.IsTrue(completeSignaled);
        }

        [TestMethod()]
        public void ServiceCoreInitializationCorrectlyParsesOptionsIntoObject()
        {
            OzetteLibrary.ServiceCore.Initialization i = new OzetteLibrary.ServiceCore.Initialization(new MockLogger());

            var signalCompleteEvent = new AutoResetEvent(false);

            i.Completed += (s, e) => { signalCompleteEvent.Set(); };

            var props = new SettingsPropertyCollection();
            props.Add(new SettingsProperty("LogFilesDirectory") { DefaultValue = "abc" });
            props.Add(new SettingsProperty("EventlogName") { DefaultValue = "def" });
            props.Add(new SettingsProperty("DatabaseFileName") { DefaultValue = "xyz" });
            
            i.BeginStart(props);

            var completeSignaled = signalCompleteEvent.WaitOne(TimeSpan.FromSeconds(10));

            Assert.IsNotNull(i.Options);
            Assert.AreEqual("abc", i.Options.LogFilesDirectory);
            Assert.AreEqual("def", i.Options.EventlogName);
            Assert.AreEqual("xyz", i.Options.DatabaseFileName);
        }

        [TestMethod()]
        public void ServiceCoreInitializationCorrectlyParsesPartialOptionsIntoObject()
        {
            OzetteLibrary.ServiceCore.Initialization i = new OzetteLibrary.ServiceCore.Initialization(new MockLogger());

            var signalCompleteEvent = new AutoResetEvent(false);

            i.Completed += (s, e) => { signalCompleteEvent.Set(); };

            var props = new SettingsPropertyCollection();
            props.Add(new SettingsProperty("EventlogName") { DefaultValue = "def" });
            props.Add(new SettingsProperty("DatabaseFileName") { DefaultValue = "xyz" });

            i.BeginStart(props);

            var completeSignaled = signalCompleteEvent.WaitOne(TimeSpan.FromSeconds(10));

            Assert.IsNotNull(i.Options);
            Assert.IsNull(i.Options.LogFilesDirectory);
            Assert.AreEqual("def", i.Options.EventlogName);
            Assert.AreEqual("xyz", i.Options.DatabaseFileName);
        }

        [TestMethod()]
        public void ServiceCoreInitializationHandlesEmptySettingsPropertyCollection()
        {
            OzetteLibrary.ServiceCore.Initialization i = new OzetteLibrary.ServiceCore.Initialization(new MockLogger());

            var signalCompleteEvent = new AutoResetEvent(false);

            i.Completed += (s, e) => { signalCompleteEvent.Set(); };

            var props = new SettingsPropertyCollection();

            i.BeginStart(props);

            var completeSignaled = signalCompleteEvent.WaitOne(TimeSpan.FromSeconds(10));

            Assert.IsNotNull(i.Options);
            Assert.IsNull(i.Options.LogFilesDirectory);
            Assert.IsNull(i.Options.EventlogName);
            Assert.IsNull(i.Options.DatabaseFileName);
        }

        [TestMethod()]
        public void ServiceCoreInitializationCallsEventLogCreationMock()
        {
            var mock = new MockLogger();
            OzetteLibrary.ServiceCore.Initialization i = new OzetteLibrary.ServiceCore.Initialization(mock);

            var signalCompleteEvent = new AutoResetEvent(false);

            i.Completed += (s, e) => { signalCompleteEvent.Set(); };

            var props = new SettingsPropertyCollection();

            i.BeginStart(props);

            var completeSignaled = signalCompleteEvent.WaitOne(TimeSpan.FromSeconds(10));

            Assert.IsTrue(mock.SetupCustomWindowsEventLogIfNotPresentHasBeenCalled);
        }

        [TestMethod()]
        public void ServiceCoreInitializationCallsLogFolderOnDiskCreationMock()
        {
            var mock = new MockLogger();
            OzetteLibrary.ServiceCore.Initialization i = new OzetteLibrary.ServiceCore.Initialization(mock);

            var signalCompleteEvent = new AutoResetEvent(false);

            i.Completed += (s, e) => { signalCompleteEvent.Set(); };

            var props = new SettingsPropertyCollection();

            i.BeginStart(props);

            var completeSignaled = signalCompleteEvent.WaitOne(TimeSpan.FromSeconds(10));

            Assert.IsTrue(mock.SetupLogsFolderIfNotPresentHasBeenCalled);
        }
    }
}
