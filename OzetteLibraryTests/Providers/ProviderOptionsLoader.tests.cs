using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace OzetteLibraryTests.Providers
{
    [TestClass()]
    public class ProviderOptionsLoaderTests
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ProviderOptionsLoaderThrowsExceptionWhenNoFileIsProvided()
        {
            OzetteLibrary.Providers.ProviderOptionsLoader load = new OzetteLibrary.Providers.ProviderOptionsLoader();
            load.LoadOptionsFile(null);
        }

        [TestMethod()]
        public void ProviderOptionsLoaderSafelyReturnsEmptyProviderOptionsCollectionFromEmptySourceFile()
        {
            OzetteLibrary.Providers.ProviderOptionsLoader load = new OzetteLibrary.Providers.ProviderOptionsLoader();

            var loaded = load.LoadOptionsFile(".\\TestFiles\\ProviderOptions\\EmptyProvidersFile.json");

            Assert.IsNotNull(loaded);
            Assert.AreEqual(0, loaded.Count);
            Assert.AreEqual(typeof(List<OzetteLibrary.Providers.ProviderOptions>), loaded.GetType());
        }

        [TestMethod()]
        public void ProviderOptionsLoaderCanLoadAzureExample1()
        {
            OzetteLibrary.Providers.ProviderOptionsLoader load = new OzetteLibrary.Providers.ProviderOptionsLoader();

            var providers = load.LoadOptionsFile(".\\TestFiles\\ProviderOptions\\SingleProvider-Azure-Example1.json");

            Assert.IsNotNull(providers);
            Assert.IsTrue(providers.Count == 1);

            Assert.IsNotNull(providers[0].Options);
            Assert.IsTrue(providers[0].Options.Count == 1);

            Assert.AreEqual(OzetteLibrary.Providers.ProviderTypes.Azure, providers[0].Type);
            Assert.IsTrue(providers[0].Options.ContainsKey("ConnectionString"));

            Assert.AreEqual(
                "DefaultEndpointsProtocol=https;AccountName=MyStorageAccountName;AccountKey=MyStorageAccountKey;EndpointSuffix=core.windows.net", 
                providers[0].Options["ConnectionString"]);
        }
    }
}
