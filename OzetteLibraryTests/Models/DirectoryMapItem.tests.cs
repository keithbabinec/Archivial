using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace OzetteLibraryTests.Models
{
    [TestClass()]
    public class DirectoryMapItemTests
    {
        [TestMethod()]
        [ExpectedException(typeof(NotImplementedException))]
        public void DirectoryMapItemGetRemotePathCorrectlyThrowsOnInvalidProvider()
        {
            var item = new OzetteLibrary.Models.DirectoryMapItem();
            item.ID = Guid.NewGuid();
            item.LocalPath = "c:\\bin\\programs";
            item.GetRemotePath((OzetteLibrary.Models.ProviderTypes.Azure)-1);
        }

        [TestMethod()]
        public void DirectoryMapItemGetRemotePathReturnsValidPathForAzureLowerCase()
        {
            var item = new OzetteLibrary.Models.DirectoryMapItem();
            item.ID = Guid.NewGuid();
            item.LocalPath = "C:\\Bin\\Programs";

            var path = item.GetRemotePath(OzetteLibrary.Models.ProviderTypes.Azure);

            Assert.IsNotNull(path);

            for (int i = 0; i < path.Length; i++)
            {
                if (char.IsLetter(path[i]) && !char.IsLower(path[i]))
                {
                    Assert.Fail("Expected all lowercase letters.");
                }
            }
        }

        [TestMethod()]
        public void DirectoryMapItemGetRemotePathReturnsValidPathForAzureBetween3And63Chars()
        {
            var item = new OzetteLibrary.Models.DirectoryMapItem();
            item.ID = Guid.NewGuid();
            item.LocalPath = "c:\\bin\\programs";

            var path = item.GetRemotePath(OzetteLibrary.Models.ProviderTypes.Azure);

            Assert.IsNotNull(path);

            Assert.IsTrue(path.Length >= 3);
            Assert.IsTrue(path.Length <= 63);
        }

        [TestMethod()]
        public void DirectoryMapItemGetRemotePathReturnsValidPathForAzureOnlyLettersNumbersAndDashes()
        {
            var item = new OzetteLibrary.Models.DirectoryMapItem();
            item.ID = Guid.NewGuid();
            item.LocalPath = "c:\\bin\\programs";

            var path = item.GetRemotePath(OzetteLibrary.Models.ProviderTypes.Azure);

            Assert.IsNotNull(path);

            for (int i = 0; i < path.Length; i++)
            {
                if (!char.IsLetter(path[i]) && !char.IsDigit(path[i]) && path[i] != '-')
                {
                    Assert.Fail("Expected only letters, digits, or dashes.");
                }
            }
        }

        [TestMethod()]
        public void DirectoryMapItemGetRemotePathReturnsValidPathForAzureStartsWithOnlyLetterOrNumber()
        {
            var item = new OzetteLibrary.Models.DirectoryMapItem();
            item.ID = Guid.NewGuid();
            item.LocalPath = "c:\\bin\\programs";

            var path = item.GetRemotePath(OzetteLibrary.Models.ProviderTypes.Azure);

            Assert.IsNotNull(path);
            Assert.IsTrue(path.Length > 0);

            if (!char.IsLetterOrDigit(path[0]))
            {
                Assert.Fail("Expected only letters, digits for the first character.");
            }
        }
    }
}
