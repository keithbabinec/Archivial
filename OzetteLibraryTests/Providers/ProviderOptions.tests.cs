using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace OzetteLibraryTests.Providers
{
    [TestClass()]
    public class ProviderOptionsTests
    {
        [TestMethod()]
        [ExpectedException(typeof(OzetteLibrary.Exceptions.ProviderOptionsException))]
        public void ProviderOptionsValidateThrowsWhenNoOptionsArePresent()
        {
            OzetteLibrary.Providers.ProviderOptions optionSet = new OzetteLibrary.Providers.ProviderOptions();
            optionSet.Type = OzetteLibrary.Providers.ProviderTypes.Azure;

            optionSet.Validate();
        }

        [TestMethod()]
        [ExpectedException(typeof(NotImplementedException))]
        public void ProviderOptionsValidateThrowsWhenInvalidTypeIsSet()
        {
            OzetteLibrary.Providers.ProviderOptions optionSet = new OzetteLibrary.Providers.ProviderOptions();
            optionSet.Options = new Dictionary<string, string>();
            optionSet.Options.Add("key", "value");
            optionSet.Type = (OzetteLibrary.Providers.ProviderTypes)(-1);

            optionSet.Validate();
        }

        [TestMethod()]
        [ExpectedException(typeof(OzetteLibrary.Exceptions.ProviderOptionsException))]
        public void ProviderOptionsForAzureThrowWhenConnectionStringArgIsMissing()
        {
            OzetteLibrary.Providers.ProviderOptions optionSet = new OzetteLibrary.Providers.ProviderOptions();
            optionSet.Options = new Dictionary<string, string>();
            optionSet.Options.Add("key", "value");
            optionSet.Type = OzetteLibrary.Providers.ProviderTypes.Azure;

            optionSet.Validate();
        }

        [TestMethod()]
        [ExpectedException(typeof(OzetteLibrary.Exceptions.ProviderOptionsException))]
        public void ProviderOptionsForAzureThrowWhenConnectionStringIsInvalid()
        {
            OzetteLibrary.Providers.ProviderOptions optionSet = new OzetteLibrary.Providers.ProviderOptions();
            optionSet.Options = new Dictionary<string, string>();
            optionSet.Options.Add("ConnectionString", "not a real connection string");
            optionSet.Type = OzetteLibrary.Providers.ProviderTypes.Azure;

            optionSet.Validate();
        }

        [TestMethod()]
        public void ProviderOptionsForAzurePassValidateWithKnownGoodConfiguration()
        {
            OzetteLibrary.Providers.ProviderOptions optionSet = new OzetteLibrary.Providers.ProviderOptions();
            optionSet.Options = new Dictionary<string, string>();
            optionSet.Options.Add("ConnectionString", "DefaultEndpointsProtocol=https;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;EndpointSuffix=core.windows.net");
            optionSet.Type = OzetteLibrary.Providers.ProviderTypes.Azure;

            optionSet.Validate();
        }
    }
}
