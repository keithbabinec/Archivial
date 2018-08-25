using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

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

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProviderFileStatusApplyMetadataToStateThrowsOnNullMetadata()
        {
            var copyState = new OzetteLibrary.Providers.ProviderFileStatus(OzetteLibrary.Providers.ProviderTypes.Azure);
            copyState.ResetState();
            copyState.ApplyMetadataToState(null);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ProviderFileStatusApplyMetadataToStateThrowsOnNonImplementedProvider()
        {
            var copyState = new OzetteLibrary.Providers.ProviderFileStatus(OzetteLibrary.Providers.ProviderTypes.Google);
            copyState.ResetState();

            var metadata = new Dictionary<string, string>();

            copyState.ApplyMetadataToState(metadata);
        }

        [TestMethod]
        public void ProviderFileStatusApplyAzureMetadataToStateCorrectlyParsesRequiredProperties()
        {
            var copyState = new OzetteLibrary.Providers.ProviderFileStatus(OzetteLibrary.Providers.ProviderTypes.Azure);
            copyState.ResetState();

            var metadata = new Dictionary<string, string>();
            metadata.Add(OzetteLibrary.Constants.ProviderMetadata.ProviderSyncStatusKeyName, OzetteLibrary.Files.FileStatus.InProgress.ToString());
            metadata.Add(OzetteLibrary.Constants.ProviderMetadata.ProviderLastCompletedFileBlockIndexKeyName, "4");

            copyState.ApplyMetadataToState(metadata);

            Assert.IsNotNull(copyState.Metadata);
            Assert.AreEqual(metadata.Count, copyState.Metadata.Count);
            Assert.AreEqual(4, copyState.LastCompletedFileBlockIndex);
            Assert.AreEqual(OzetteLibrary.Files.FileStatus.InProgress, copyState.SyncStatus);
        }

        [TestMethod]
        [ExpectedException(typeof(OzetteLibrary.Exceptions.ProviderMetadataMissingException))]
        public void ProviderFileStatusApplyAzureMetadataToStateThrowsOnMissingSyncStatus()
        {
            var copyState = new OzetteLibrary.Providers.ProviderFileStatus(OzetteLibrary.Providers.ProviderTypes.Azure);
            copyState.ResetState();

            var metadata = new Dictionary<string, string>();
            metadata.Add(OzetteLibrary.Constants.ProviderMetadata.ProviderLastCompletedFileBlockIndexKeyName, "4");

            copyState.ApplyMetadataToState(metadata);
        }

        [TestMethod]
        [ExpectedException(typeof(OzetteLibrary.Exceptions.ProviderMetadataMissingException))]
        public void ProviderFileStatusApplyAzureMetadataToStateThrowsOnMissingLastCompletedBlock()
        {
            var copyState = new OzetteLibrary.Providers.ProviderFileStatus(OzetteLibrary.Providers.ProviderTypes.Azure);
            copyState.ResetState();

            var metadata = new Dictionary<string, string>();
            metadata.Add(OzetteLibrary.Constants.ProviderMetadata.ProviderSyncStatusKeyName, OzetteLibrary.Files.FileStatus.InProgress.ToString());

            copyState.ApplyMetadataToState(metadata);
        }

        [TestMethod]
        [ExpectedException(typeof(OzetteLibrary.Exceptions.ProviderMetadataMalformedException))]
        public void ProviderFileStatusApplyAzureMetadataToStateThrowsOnMalformedSyncStatus()
        {
            var copyState = new OzetteLibrary.Providers.ProviderFileStatus(OzetteLibrary.Providers.ProviderTypes.Azure);
            copyState.ResetState();

            var metadata = new Dictionary<string, string>();
            metadata.Add(OzetteLibrary.Constants.ProviderMetadata.ProviderSyncStatusKeyName, "InPogress"); // misspelled, should not parse
            metadata.Add(OzetteLibrary.Constants.ProviderMetadata.ProviderLastCompletedFileBlockIndexKeyName, "4");

            copyState.ApplyMetadataToState(metadata);
        }

        [TestMethod]
        [ExpectedException(typeof(OzetteLibrary.Exceptions.ProviderMetadataMalformedException))]
        public void ProviderFileStatusApplyAzureMetadataToStateThrowsOnMalformedLastCompletedBlock()
        {
            var copyState = new OzetteLibrary.Providers.ProviderFileStatus(OzetteLibrary.Providers.ProviderTypes.Azure);
            copyState.ResetState();

            var metadata = new Dictionary<string, string>();
            metadata.Add(OzetteLibrary.Constants.ProviderMetadata.ProviderSyncStatusKeyName, OzetteLibrary.Files.FileStatus.InProgress.ToString());
            metadata.Add(OzetteLibrary.Constants.ProviderMetadata.ProviderLastCompletedFileBlockIndexKeyName, "what?"); // not a number, should not parse.

            copyState.ApplyMetadataToState(metadata);
        }
    }
}
