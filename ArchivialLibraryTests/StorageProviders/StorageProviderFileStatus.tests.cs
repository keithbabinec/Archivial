using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ArchivialLibraryTests.StorageProviders
{
    [TestClass]
    public class StorageProviderFileStatusTests
    {
        [TestMethod]
        public void ProviderFileStatusResetStateDoesNotThrowIfNoProviderIsPresent()
        {
            var copyState = new ArchivialLibrary.StorageProviders.StorageProviderFileStatus();
            copyState.ResetState();

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void ProviderFileStatusCorrectlyAssignsProviderFromConstructor()
        {
            var copyState = new ArchivialLibrary.StorageProviders.StorageProviderFileStatus(ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS);

            Assert.AreEqual(ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS, copyState.Provider);
        }

        [TestMethod]
        public void ProviderFileStatusCorrectlyInitializesCopyStateAsUnsynced()
        {
            var copyState = new ArchivialLibrary.StorageProviders.StorageProviderFileStatus(ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS);

            Assert.AreEqual(-1, copyState.LastCompletedFileBlockIndex);
            Assert.AreEqual(ArchivialLibrary.Files.FileStatus.Unsynced, copyState.SyncStatus);
        }

        [TestMethod]
        public void ProviderFileStatusResetStateCorrectlyResetsProgress()
        {
            var copyState = new ArchivialLibrary.StorageProviders.StorageProviderFileStatus(ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS);
            copyState.LastCompletedFileBlockIndex = 100;
            copyState.SyncStatus = ArchivialLibrary.Files.FileStatus.InProgress;
            copyState.ResetState();

            Assert.AreEqual(ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS, copyState.Provider);
            Assert.AreEqual(-1, copyState.LastCompletedFileBlockIndex);
            Assert.AreEqual(ArchivialLibrary.Files.FileStatus.Unsynced, copyState.SyncStatus);
        }

        [TestMethod]
        public void ProviderFileStatusResetStateCorrectlyResetsProgress2()
        {
            var copyState = new ArchivialLibrary.StorageProviders.StorageProviderFileStatus(ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS);
            copyState.LastCompletedFileBlockIndex = 100;
            copyState.SyncStatus = ArchivialLibrary.Files.FileStatus.Synced;
            copyState.ResetState();

            Assert.AreEqual(ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS, copyState.Provider);
            Assert.AreEqual(-1, copyState.LastCompletedFileBlockIndex);
            Assert.AreEqual(ArchivialLibrary.Files.FileStatus.Unsynced, copyState.SyncStatus);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProviderFileStatusApplyMetadataToStateThrowsOnNullMetadata()
        {
            var copyState = new ArchivialLibrary.StorageProviders.StorageProviderFileStatus(ArchivialLibrary.StorageProviders.StorageProviderTypes.Azure);
            copyState.ResetState();
            copyState.ApplyMetadataToState(null);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ProviderFileStatusApplyMetadataToStateThrowsOnNonImplementedProvider()
        {
            var copyState = new ArchivialLibrary.StorageProviders.StorageProviderFileStatus(ArchivialLibrary.StorageProviders.StorageProviderTypes.Google);
            copyState.ResetState();

            var metadata = new Dictionary<string, string>();

            copyState.ApplyMetadataToState(metadata);
        }

        [TestMethod]
        public void ProviderFileStatusApplyAzureMetadataToStateCorrectlyParsesRequiredProperties()
        {
            var copyState = new ArchivialLibrary.StorageProviders.StorageProviderFileStatus(ArchivialLibrary.StorageProviders.StorageProviderTypes.Azure);
            copyState.ResetState();

            var metadata = new Dictionary<string, string>();
            metadata.Add(ArchivialLibrary.Constants.ProviderMetadata.ProviderSyncStatusKeyName, ArchivialLibrary.Files.FileStatus.InProgress.ToString());
            metadata.Add(ArchivialLibrary.Constants.ProviderMetadata.ProviderLastCompletedFileBlockIndexKeyName, "4");
            metadata.Add(ArchivialLibrary.Constants.ProviderMetadata.HydrationStateKeyName, ArchivialLibrary.StorageProviders.StorageProviderHydrationStatus.MovingToActiveTier.ToString());

            copyState.ApplyMetadataToState(metadata);

            Assert.IsNotNull(copyState.Metadata);
            Assert.AreEqual(metadata.Count, copyState.Metadata.Count);
            Assert.AreEqual(4, copyState.LastCompletedFileBlockIndex);
            Assert.AreEqual(ArchivialLibrary.Files.FileStatus.InProgress, copyState.SyncStatus);
            Assert.AreEqual(ArchivialLibrary.StorageProviders.StorageProviderHydrationStatus.MovingToActiveTier, copyState.HydrationStatus);
        }

        [TestMethod]
        [ExpectedException(typeof(ArchivialLibrary.Exceptions.ProviderMetadataMissingException))]
        public void ProviderFileStatusApplyAzureMetadataToStateThrowsOnMissingSyncStatus()
        {
            var copyState = new ArchivialLibrary.StorageProviders.StorageProviderFileStatus(ArchivialLibrary.StorageProviders.StorageProviderTypes.Azure);
            copyState.ResetState();

            var metadata = new Dictionary<string, string>();
            metadata.Add(ArchivialLibrary.Constants.ProviderMetadata.ProviderLastCompletedFileBlockIndexKeyName, "4");

            copyState.ApplyMetadataToState(metadata);
        }

        [TestMethod]
        [ExpectedException(typeof(ArchivialLibrary.Exceptions.ProviderMetadataMissingException))]
        public void ProviderFileStatusApplyAzureMetadataToStateThrowsOnMissingLastCompletedBlock()
        {
            var copyState = new ArchivialLibrary.StorageProviders.StorageProviderFileStatus(ArchivialLibrary.StorageProviders.StorageProviderTypes.Azure);
            copyState.ResetState();

            var metadata = new Dictionary<string, string>();
            metadata.Add(ArchivialLibrary.Constants.ProviderMetadata.ProviderSyncStatusKeyName, ArchivialLibrary.Files.FileStatus.InProgress.ToString());

            copyState.ApplyMetadataToState(metadata);
        }

        [TestMethod]
        [ExpectedException(typeof(ArchivialLibrary.Exceptions.ProviderMetadataMissingException))]
        public void ProviderFileStatusApplyAzureMetadataToStateThrowsOnMissingHydrationStatus()
        {
            var copyState = new ArchivialLibrary.StorageProviders.StorageProviderFileStatus(ArchivialLibrary.StorageProviders.StorageProviderTypes.Azure);
            copyState.ResetState();

            var metadata = new Dictionary<string, string>();
            metadata.Add(ArchivialLibrary.Constants.ProviderMetadata.ProviderSyncStatusKeyName, ArchivialLibrary.Files.FileStatus.InProgress.ToString());
            metadata.Add(ArchivialLibrary.Constants.ProviderMetadata.ProviderLastCompletedFileBlockIndexKeyName, "4");

            copyState.ApplyMetadataToState(metadata);
        }

        [TestMethod]
        [ExpectedException(typeof(ArchivialLibrary.Exceptions.ProviderMetadataMalformedException))]
        public void ProviderFileStatusApplyAzureMetadataToStateThrowsOnMalformedSyncStatus()
        {
            var copyState = new ArchivialLibrary.StorageProviders.StorageProviderFileStatus(ArchivialLibrary.StorageProviders.StorageProviderTypes.Azure);
            copyState.ResetState();

            var metadata = new Dictionary<string, string>();
            metadata.Add(ArchivialLibrary.Constants.ProviderMetadata.ProviderSyncStatusKeyName, "InPogress"); // misspelled, should not parse
            metadata.Add(ArchivialLibrary.Constants.ProviderMetadata.ProviderLastCompletedFileBlockIndexKeyName, "4");
            metadata.Add(ArchivialLibrary.Constants.ProviderMetadata.HydrationStateKeyName, ArchivialLibrary.StorageProviders.StorageProviderHydrationStatus.None.ToString());

            copyState.ApplyMetadataToState(metadata);
        }

        [TestMethod]
        [ExpectedException(typeof(ArchivialLibrary.Exceptions.ProviderMetadataMalformedException))]
        public void ProviderFileStatusApplyAzureMetadataToStateThrowsOnMalformedLastCompletedBlock()
        {
            var copyState = new ArchivialLibrary.StorageProviders.StorageProviderFileStatus(ArchivialLibrary.StorageProviders.StorageProviderTypes.Azure);
            copyState.ResetState();

            var metadata = new Dictionary<string, string>();
            metadata.Add(ArchivialLibrary.Constants.ProviderMetadata.ProviderSyncStatusKeyName, ArchivialLibrary.Files.FileStatus.InProgress.ToString());
            metadata.Add(ArchivialLibrary.Constants.ProviderMetadata.ProviderLastCompletedFileBlockIndexKeyName, "what?"); // not a number, should not parse.
            metadata.Add(ArchivialLibrary.Constants.ProviderMetadata.HydrationStateKeyName, ArchivialLibrary.StorageProviders.StorageProviderHydrationStatus.None.ToString());

            copyState.ApplyMetadataToState(metadata);
        }
    }
}
