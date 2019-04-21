using Microsoft.VisualStudio.TestTools.UnitTesting;
using ArchivialLibrary.Logging.Mock;
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
            var provider = new ArchivialLibrary.StorageProviders.Azure.AzureStorageProviderFileOperations(null, "storage account", "token");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AzureProviderFileOperationsConstructorThrowsOnMissingStorageAccount()
        {
            var provider = new ArchivialLibrary.StorageProviders.Azure.AzureStorageProviderFileOperations(new MockLogger(), "  ", "token");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AzureProviderFileOperationsConstructorThrowsOnMissingSASToken()
        {
            var provider = new ArchivialLibrary.StorageProviders.Azure.AzureStorageProviderFileOperations(new MockLogger(), "storage account", "   ");
        }
    }
}
