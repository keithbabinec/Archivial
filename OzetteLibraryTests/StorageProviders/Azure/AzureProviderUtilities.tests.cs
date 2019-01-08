using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace OzetteLibraryTests.StorageProviders.Azure
{
    [TestClass]
    public class AzureProviderUtilitiesTests
    {
        [TestMethod]
        public void AzureProviderUtilitiesGetFileUriReturnsHttpsUri()
        {
            var provider = new OzetteLibrary.StorageProviders.Azure.AzureStorageProviderUtilities();

            var storageAccount = "storage-account";
            var folderID = Guid.NewGuid().ToString();
            var fileID = Guid.NewGuid().ToString();
            var containerName = string.Format("{0}-directory-{1}", OzetteLibrary.Constants.Logging.AppName, folderID).ToLower();

            var uri = provider.GetFileUri(storageAccount, containerName, fileID);

            Assert.IsNotNull(uri);
            Assert.IsTrue(uri.StartsWith("https://"));
        }

        [TestMethod]
        public void AzureProviderUtilitiesGetFileUriReturnsCorrectlyFormattedFullURI()
        {
            var provider = new OzetteLibrary.StorageProviders.Azure.AzureStorageProviderUtilities();

            var storageAccount = "storage-account";
            var folderID = "a4b78664-90c9-4957-90a1-66d9c70c0492";
            var fileID = "29164dbc-f17b-4643-8f40-868e13b98141";

            var containerName = string.Format("{0}-directory-{1}", OzetteLibrary.Constants.Logging.AppName, folderID).ToLower();
            var fileName = string.Format("{0}-file-{1}", OzetteLibrary.Constants.Logging.AppName, fileID).ToLower();

            var computedUri = provider.GetFileUri(storageAccount, containerName, fileName);

            var expected = "https://storage-account.blob.core.windows.net/ozette-directory-a4b78664-90c9-4957-90a1-66d9c70c0492/ozette-file-29164dbc-f17b-4643-8f40-868e13b98141";

            Assert.IsNotNull(computedUri);
            Assert.AreEqual(expected, computedUri);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AzureProviderUtilitiesGetFileUriThrowsArgumentExceptionOnMissingStorageAccount()
        {
            var provider = new OzetteLibrary.StorageProviders.Azure.AzureStorageProviderUtilities();

            var storageAccount = string.Empty; // should throw
            var folderID = "a4b78664-90c9-4957-90a1-66d9c70c0492";
            var fileID = "29164dbc-f17b-4643-8f40-868e13b98141";

            var containerName = string.Format("{0}-directory-{1}", OzetteLibrary.Constants.Logging.AppName, folderID).ToLower();
            var fileName = string.Format("{0}-file-{1}", OzetteLibrary.Constants.Logging.AppName, fileID).ToLower();

            var computedUri = provider.GetFileUri(storageAccount, containerName, fileName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AzureProviderUtilitiesGetFileUriThrowsArgumentExceptionOnMissingContainerName()
        {
            var provider = new OzetteLibrary.StorageProviders.Azure.AzureStorageProviderUtilities();

            var storageAccount = "storage-account";
            var fileID = "29164dbc-f17b-4643-8f40-868e13b98141";

            var containerName = string.Empty; // should throw
            var fileName = string.Format("{0}-file-{1}", OzetteLibrary.Constants.Logging.AppName, fileID).ToLower();

            var computedUri = provider.GetFileUri(storageAccount, containerName, fileName);
        }

        [TestMethod]
        public void AzureProviderUtilitiesGetContainerUriReturnsHttpsUri()
        {
            var provider = new OzetteLibrary.StorageProviders.Azure.AzureStorageProviderUtilities();

            var storageAccount = "storage-account";
            var folderID = Guid.NewGuid().ToString();
            var containerName = string.Format("{0}-directory-{1}", OzetteLibrary.Constants.Logging.AppName, folderID).ToLower();

            var uri = provider.GetContainerUri(storageAccount, containerName);

            Assert.IsNotNull(uri);
            Assert.IsTrue(uri.StartsWith("https://"));
        }

        [TestMethod]
        public void AzureProviderUtilitiesGetContainerUriReturnsCorrectlyFormattedFullURI()
        {
            var provider = new OzetteLibrary.StorageProviders.Azure.AzureStorageProviderUtilities();

            var storageAccount = "storage-account";
            var folderID = "a4b78664-90c9-4957-90a1-66d9c70c0492";
            var containerName = string.Format("{0}-directory-{1}", OzetteLibrary.Constants.Logging.AppName, folderID).ToLower();

            var computedUri = provider.GetContainerUri(storageAccount, containerName);

            var expected = "https://storage-account.blob.core.windows.net/ozette-directory-a4b78664-90c9-4957-90a1-66d9c70c0492";

            Assert.IsNotNull(computedUri);
            Assert.AreEqual(expected, computedUri);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AzureProviderUtilitiesGetContainerUriThrowsArgumentExceptionOnMissingStorageAccount()
        {
            var provider = new OzetteLibrary.StorageProviders.Azure.AzureStorageProviderUtilities();

            var storageAccount = string.Empty; // should throw
            var folderID = "a4b78664-90c9-4957-90a1-66d9c70c0492";
            var containerName = string.Format("{0}-directory-{1}", OzetteLibrary.Constants.Logging.AppName, folderID).ToLower();

            var computedUri = provider.GetContainerUri(storageAccount, containerName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AzureProviderUtilitiesGetContainerUriThrowsArgumentExceptionOnMissingContainerName()
        {
            var provider = new OzetteLibrary.StorageProviders.Azure.AzureStorageProviderUtilities();

            var storageAccount = "storage-account";
            var containerName = string.Empty; // should throw

            var computedUri = provider.GetContainerUri(storageAccount, containerName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AzureProviderUtilitiesGetFileUriThrowsArgumentExceptionOnMissingFileName()
        {
            var provider = new OzetteLibrary.StorageProviders.Azure.AzureStorageProviderUtilities();

            var storageAccount = "storage-account";
            var folderID = "a4b78664-90c9-4957-90a1-66d9c70c0492";

            var containerName = string.Format("{0}-directory-{1}", OzetteLibrary.Constants.Logging.AppName, folderID).ToLower();
            var fileName = string.Empty; // should throw

            var computedUri = provider.GetFileUri(storageAccount, containerName, fileName);
        }

        [TestMethod]
        public void AzureProviderUtilitiesGenerateBlockIdentifierBase64StringReturnsValidOutput()
        {
            var provider = new OzetteLibrary.StorageProviders.Azure.AzureStorageProviderUtilities();

            Guid fileID = new Guid("51690d6e-f42a-4581-8e45-660b859bb432");
            int blockID = 1;

            var result = provider.GenerateBlockIdentifierBase64String(fileID, blockID);
            var expected = "NTE2OTBkNmUtZjQyYS00NTgxLThlNDUtNjYwYjg1OWJiNDMyLTAwMDAwMDAx";

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AzureProviderUtilitiesGenerateBlockIdentifierBase64StringThrowsOnMissingFileID()
        {
            var provider = new OzetteLibrary.StorageProviders.Azure.AzureStorageProviderUtilities();

            Guid fileID = Guid.Empty; // should throw
            int blockID = 1;

            var result = provider.GenerateBlockIdentifierBase64String(fileID, blockID);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AzureProviderUtilitiesGenerateBlockIdentifierBase64StringThrowsOnNegativeBlockNumber()
        {
            var provider = new OzetteLibrary.StorageProviders.Azure.AzureStorageProviderUtilities();

            Guid fileID = new Guid("51690d6e-f42a-4581-8e45-660b859bb432");
            int blockID = -1; // should throw

            var result = provider.GenerateBlockIdentifierBase64String(fileID, blockID);
        }

        [TestMethod]
        public void AzureProviderUtilitiesGenerateListOfBlocksToCommitReturnsValidOutputForSingleBlock()
        {
            var provider = new OzetteLibrary.StorageProviders.Azure.AzureStorageProviderUtilities();

            Guid fileID = new Guid("51690d6e-f42a-4581-8e45-660b859bb432");
            int blockID = 0;

            var result = provider.GenerateListOfBlocksToCommit(fileID, blockID);

            var expected = new List<string>()
            {
                "NTE2OTBkNmUtZjQyYS00NTgxLThlNDUtNjYwYjg1OWJiNDMyLTAwMDAwMDAw"
            };

            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Count, result.Count);
            Assert.AreEqual(expected[0], result[0]);
        }

        [TestMethod]
        public void AzureProviderUtilitiesGenerateListOfBlocksToCommitReturnsValidOutputForMultipleBlocks()
        {
            var provider = new OzetteLibrary.StorageProviders.Azure.AzureStorageProviderUtilities();

            Guid fileID = new Guid("51690d6e-f42a-4581-8e45-660b859bb432");
            int blockID = 4;

            var result = provider.GenerateListOfBlocksToCommit(fileID, blockID);

            var expected = new List<string>()
            {
                "NTE2OTBkNmUtZjQyYS00NTgxLThlNDUtNjYwYjg1OWJiNDMyLTAwMDAwMDAw",
                "NTE2OTBkNmUtZjQyYS00NTgxLThlNDUtNjYwYjg1OWJiNDMyLTAwMDAwMDAx",
                "NTE2OTBkNmUtZjQyYS00NTgxLThlNDUtNjYwYjg1OWJiNDMyLTAwMDAwMDAy",
                "NTE2OTBkNmUtZjQyYS00NTgxLThlNDUtNjYwYjg1OWJiNDMyLTAwMDAwMDAz",
                "NTE2OTBkNmUtZjQyYS00NTgxLThlNDUtNjYwYjg1OWJiNDMyLTAwMDAwMDA0"
            };

            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Count, result.Count);
            Assert.AreEqual(expected[0], result[0]);
            Assert.AreEqual(expected[1], result[1]);
            Assert.AreEqual(expected[2], result[2]);
            Assert.AreEqual(expected[3], result[3]);
            Assert.AreEqual(expected[4], result[4]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AzureProviderUtilitiesGenerateListOfBlocksToCommitThrowsOnMissingFileID()
        {
            var provider = new OzetteLibrary.StorageProviders.Azure.AzureStorageProviderUtilities();

            Guid fileID = Guid.Empty; // should throw
            int blockID = 1;

            var result = provider.GenerateListOfBlocksToCommit(fileID, blockID);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AzureProviderUtilitiesGenerateListOfBlocksToCommitThrowsOnMissingBlockNumber()
        {
            var provider = new OzetteLibrary.StorageProviders.Azure.AzureStorageProviderUtilities();

            Guid fileID = new Guid("51690d6e-f42a-4581-8e45-660b859bb432");
            int blockID = -1; // should throw

            var result = provider.GenerateListOfBlocksToCommit(fileID, blockID);
        }

        [TestMethod]
        public void AzureProviderUtilitiesGetHydrationStatusFromAzureStateCorrectlyParsesNull()
        {
            var provider = new OzetteLibrary.StorageProviders.Azure.AzureStorageProviderUtilities();

            var result = provider.GetHydrationStatusFromAzureState(null);
            var expected = OzetteLibrary.StorageProviders.StorageProviderHydrationStatus.None.ToString();

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void AzureProviderUtilitiesGetHydrationStatusFromAzureStateCorrectlyParsesUnknown()
        {
            var provider = new OzetteLibrary.StorageProviders.Azure.AzureStorageProviderUtilities();

            var result = provider.GetHydrationStatusFromAzureState(Microsoft.WindowsAzure.Storage.Blob.RehydrationStatus.Unknown);
            var expected = OzetteLibrary.StorageProviders.StorageProviderHydrationStatus.None.ToString();

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void AzureProviderUtilitiesGetHydrationStatusFromAzureStateCorrectlyParsesPendingHot()
        {
            var provider = new OzetteLibrary.StorageProviders.Azure.AzureStorageProviderUtilities();

            var result = provider.GetHydrationStatusFromAzureState(Microsoft.WindowsAzure.Storage.Blob.RehydrationStatus.PendingToHot);
            var expected = OzetteLibrary.StorageProviders.StorageProviderHydrationStatus.MovingToActiveTier.ToString();

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void AzureProviderUtilitiesGetHydrationStatusFromAzureStateCorrectlyParsesPendingCool()
        {
            var provider = new OzetteLibrary.StorageProviders.Azure.AzureStorageProviderUtilities();

            var result = provider.GetHydrationStatusFromAzureState(Microsoft.WindowsAzure.Storage.Blob.RehydrationStatus.PendingToCool);
            var expected = OzetteLibrary.StorageProviders.StorageProviderHydrationStatus.MovingToActiveTier.ToString();

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void AzureProviderUtilitiesGetHydrationStatusFromAzureStateCorrectlyThrowsOnUnexpectedValue()
        {
            var provider = new OzetteLibrary.StorageProviders.Azure.AzureStorageProviderUtilities();

            var result = provider.GetHydrationStatusFromAzureState((Microsoft.WindowsAzure.Storage.Blob.RehydrationStatus)500); // not a real enum value
        }
    }
}
