using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzetteLibrary.Logging.Mock;
using System;

namespace OzetteLibraryTests.Providers.Azure
{
    [TestClass]
    public class AzureProviderFileOperationsTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AzureProviderFileOperationsConstructorThrowsOnMissingLogger()
        {
            var provider = new OzetteLibrary.Providers.Azure.AzureProviderFileOperations(null, "storage account", "token");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AzureProviderFileOperationsConstructorThrowsOnMissingStorageAccount()
        {
            var provider = new OzetteLibrary.Providers.Azure.AzureProviderFileOperations(new MockLogger(), "  ", "token");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AzureProviderFileOperationsConstructorThrowsOnMissingSASToken()
        {
            var provider = new OzetteLibrary.Providers.Azure.AzureProviderFileOperations(new MockLogger(), "storage account", "   ");
        }

        [TestMethod]
        public void AzureProviderFileOperationsGetFileUriReturnsHttpsUri()
        {
            var provider = new OzetteLibrary.Providers.Azure.AzureProviderFileOperations(new MockLogger(), "storage-account", "token");

            var folderID = Guid.NewGuid().ToString();
            var fileID = Guid.NewGuid().ToString();
            var containerName = string.Format("{0}-directory-{1}", OzetteLibrary.Constants.Logging.AppName, folderID).ToLower();

            var uri = provider.GetFileUri(containerName, fileID);

            Assert.IsNotNull(uri);
            Assert.IsTrue(uri.StartsWith("https://"));
        }

        [TestMethod]
        public void AzureProviderFileOperationsGetFileUriReturnsCorrectlyFormattedFullURI()
        {
            var provider = new OzetteLibrary.Providers.Azure.AzureProviderFileOperations(new MockLogger(), "storage-account", "token");

            var folderID = "a4b78664-90c9-4957-90a1-66d9c70c0492";
            var fileID = "29164dbc-f17b-4643-8f40-868e13b98141";

            var containerName = string.Format("{0}-directory-{1}", OzetteLibrary.Constants.Logging.AppName, folderID).ToLower();
            var fileName = string.Format("{0}-file-{1}", OzetteLibrary.Constants.Logging.AppName, fileID).ToLower();

            var computedUri = provider.GetFileUri(containerName, fileName);

            var expected = "https://storage-account.blob.core.windows.net/ozette-directory-a4b78664-90c9-4957-90a1-66d9c70c0492/ozette-file-29164dbc-f17b-4643-8f40-868e13b98141?token";

            Assert.IsNotNull(computedUri);
            Assert.AreEqual(expected, computedUri);
        }
    }
}
