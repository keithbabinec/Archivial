using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OzetteLibrary.Database;
using OzetteLibrary.Database.SQLServer;
using OzetteLibrary.Exceptions;
using OzetteLibrary.Secrets;
using System;
using System.IO;

namespace OzetteLibraryTests.Secrets
{
    [TestClass]
    public class ProtectedDataStoreTests
    {
        private const string TestConnectionString = "fakedb";

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProtectedDataStoreConstructorThrowsOnNullDatabase()
        {
            var entropy = new byte[] { 123, 2, 15, 212, 174, 141, 233, 86 };
            var scope = System.Security.Cryptography.DataProtectionScope.CurrentUser;

            ProtectedDataStore pds = new ProtectedDataStore(null, scope, entropy);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProtectedDataStoreConstructorThrowsOnNoEntropy()
        {
            var db = new SQLServerClientDatabase(TestConnectionString);
            var entropy = new byte[] { };
            var scope = System.Security.Cryptography.DataProtectionScope.CurrentUser;

            ProtectedDataStore pds = new ProtectedDataStore(db, scope, entropy);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationSecretMissingException))]
        public void ProtectedDataStoreGetApplicationSecretThrowsWhenSecretIsNotFound()
        {
            var db = new Mock<IClientDatabase>();
            db.Setup(x => x.GetApplicationOptionAsync(It.IsAny<string>())).Returns((string)null);

            var entropy = new byte[] { 123, 2, 15, 212, 174, 141, 233, 86 };
            var scope = System.Security.Cryptography.DataProtectionScope.CurrentUser;

            ProtectedDataStore pds = new ProtectedDataStore(db.Object, scope, entropy);

            pds.GetApplicationSecret(OzetteLibrary.Constants.RuntimeSettingNames.AzureStorageAccountName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProtectedDataStoreSetApplicationSecretThrowsWhenNoOptionNameIsProvided()
        {
            var db = new SQLServerClientDatabase(TestConnectionString);
            var entropy = new byte[] { 123, 2, 15, 212, 174, 141, 233, 86 };
            var scope = System.Security.Cryptography.DataProtectionScope.CurrentUser;

            ProtectedDataStore pds = new ProtectedDataStore(db, scope, entropy);

            pds.SetApplicationSecret("", "test-account");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProtectedDataStoreSetApplicationSecretThrowsWhenNoOptionValueIsProvided()
        {
            var db = new SQLServerClientDatabase(TestConnectionString);
            var entropy = new byte[] { 123, 2, 15, 212, 174, 141, 233, 86 };
            var scope = System.Security.Cryptography.DataProtectionScope.CurrentUser;

            ProtectedDataStore pds = new ProtectedDataStore(db, scope, entropy);

            pds.SetApplicationSecret(OzetteLibrary.Constants.RuntimeSettingNames.AzureStorageAccountName, "");
        }
    }
}
