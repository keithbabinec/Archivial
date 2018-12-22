using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzetteLibrary.Logging.Mock;
using System;

namespace OzetteLibraryTests.StorageProviders.Azure
{
    [TestClass]
    public class AzureProviderFileOperationsTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AzureProviderFileOperationsConstructorThrowsOnMissingLogger()
        {
            var provider = new OzetteLibrary.StorageProviders.Azure.AzureProviderFileOperations(null, "storage account", "token");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AzureProviderFileOperationsConstructorThrowsOnMissingStorageAccount()
        {
            var provider = new OzetteLibrary.StorageProviders.Azure.AzureProviderFileOperations(new MockLogger(), "  ", "token");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AzureProviderFileOperationsConstructorThrowsOnMissingSASToken()
        {
            var provider = new OzetteLibrary.StorageProviders.Azure.AzureProviderFileOperations(new MockLogger(), "storage account", "   ");
        }
    }
}
