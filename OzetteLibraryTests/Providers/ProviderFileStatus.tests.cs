using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OzetteLibraryTests.Providers
{
    [TestClass]
    public class ProviderFileStatusTests
    {
        [TestMethod]
        public void ProviderFileStatusResetStateDoesNotThrowIfNoProviderIsPresent()
        {
            var copyState = new OzetteLibrary.Providers.ProviderFileStatus();
            copyState.ResetState();

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void ProviderFileStatusCorrectlyAssignsProviderFromConstructor()
        {
            var copyState = new OzetteLibrary.Providers.ProviderFileStatus(OzetteLibrary.Providers.ProviderTypes.AWS);

            Assert.AreEqual(OzetteLibrary.Providers.ProviderTypes.AWS, copyState.Provider);
        }

        [TestMethod]
        public void ProviderFileStatusCorrectlyInitializesCopyStateAsUnsynced()
        {
            var copyState = new OzetteLibrary.Providers.ProviderFileStatus(OzetteLibrary.Providers.ProviderTypes.AWS);

            Assert.AreEqual(-1, copyState.LastCompletedFileBlockIndex);
            Assert.AreEqual(OzetteLibrary.Files.FileStatus.Unsynced, copyState.SyncStatus);
        }

        [TestMethod]
        public void ProviderFileStatusResetStateCorrectlyResetsProgress()
        {
            var copyState = new OzetteLibrary.Providers.ProviderFileStatus(OzetteLibrary.Providers.ProviderTypes.AWS);
            copyState.LastCompletedFileBlockIndex = 100;
            copyState.SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;
            copyState.ResetState();

            Assert.AreEqual(OzetteLibrary.Providers.ProviderTypes.AWS, copyState.Provider);
            Assert.AreEqual(-1, copyState.LastCompletedFileBlockIndex);
            Assert.AreEqual(OzetteLibrary.Files.FileStatus.Unsynced, copyState.SyncStatus);
        }

        [TestMethod]
        public void ProviderFileStatusResetStateCorrectlyResetsProgress2()
        {
            var copyState = new OzetteLibrary.Providers.ProviderFileStatus(OzetteLibrary.Providers.ProviderTypes.AWS);
            copyState.LastCompletedFileBlockIndex = 100;
            copyState.SyncStatus = OzetteLibrary.Files.FileStatus.Synced;
            copyState.ResetState();

            Assert.AreEqual(OzetteLibrary.Providers.ProviderTypes.AWS, copyState.Provider);
            Assert.AreEqual(-1, copyState.LastCompletedFileBlockIndex);
            Assert.AreEqual(OzetteLibrary.Files.FileStatus.Unsynced, copyState.SyncStatus);
        }
    }
}
