using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var db = new SQLServerClientDatabase(TestConnectionString);
            var entropy = new byte[] { 123, 2, 15, 212, 174, 141, 233, 86 };
            var scope = System.Security.Cryptography.DataProtectionScope.CurrentUser;

            ProtectedDataStore pds = new ProtectedDataStore(db, scope, entropy);

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

        [TestMethod]
        public void ProtectedDataStoreSetApplicationSecretShouldSaveEncryptedSecretUserScope()
        {
            var db = new SQLServerClientDatabase(TestConnectionString);
            var entropy = new byte[] { 123, 2, 15, 212, 174, 141, 233, 86 };
            var scope = System.Security.Cryptography.DataProtectionScope.CurrentUser;

            ProtectedDataStore pds = new ProtectedDataStore(db, scope, entropy);

            pds.SetApplicationSecret(OzetteLibrary.Constants.RuntimeSettingNames.AzureStorageAccountName, "test-account");

            // check the underlying database entry
            // it should be encrypted.

            var optionValue = db.GetApplicationOption(OzetteLibrary.Constants.RuntimeSettingNames.AzureStorageAccountName);

            // the actual value of the encrypted string is going to vary by machine scope
            // for consistency in testing just verify the string has been transformed.

            Assert.IsNotNull(optionValue);
            Assert.AreNotEqual("test-account", optionValue);
            Assert.IsFalse(optionValue.Contains("test-account"));
            Assert.IsTrue(optionValue.Length > "test-account".Length);
        }

        [TestMethod]
        public void ProtectedDataStoreSetApplicationSecretShouldSaveEncryptedSecretMachineScope()
        {
            var db = new SQLServerClientDatabase(TestConnectionString);
            var entropy = new byte[] { 123, 2, 15, 212, 174, 141, 233, 86 };
            var scope = System.Security.Cryptography.DataProtectionScope.LocalMachine;

            ProtectedDataStore pds = new ProtectedDataStore(db, scope, entropy);

            pds.SetApplicationSecret(OzetteLibrary.Constants.RuntimeSettingNames.AzureStorageAccountName, "test-account");

            // check the underlying database entry
            // it should be encrypted.

            var optionValue = db.GetApplicationOption(OzetteLibrary.Constants.RuntimeSettingNames.AzureStorageAccountName);

            // the actual value of the encrypted string is going to vary by machine scope
            // for consistency in testing just verify the string has been transformed.

            Assert.IsNotNull(optionValue);
            Assert.AreNotEqual("test-account", optionValue);
            Assert.IsFalse(optionValue.Contains("test-account"));
            Assert.IsTrue(optionValue.Length > "test-account".Length);
        }
    }
}
