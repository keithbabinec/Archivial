using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzetteLibrary.Database.LiteDB;
using OzetteLibrary.Exceptions;
using OzetteLibrary.Logging.Mock;
using OzetteLibrary.Secrets;
using OzetteLibrary.ServiceCore;
using System;
using System.IO;

namespace OzetteLibraryTests.Secrets
{
    [TestClass]
    public class ProtectedDataStoreTests
    {
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
            var ms = new MemoryStream();
            var db = new LiteDBClientDatabase(ms);
            db.PrepareDatabase();
            var entropy = new byte[] { };
            var scope = System.Security.Cryptography.DataProtectionScope.CurrentUser;

            ProtectedDataStore pds = new ProtectedDataStore(db, scope, entropy);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationSecretMissingException))]
        public void ProtectedDataStoreGetApplicationSecretThrowsWhenSecretIsNotFound()
        {
            var ms = new MemoryStream();
            var db = new LiteDBClientDatabase(ms);
            db.PrepareDatabase();
            var entropy = new byte[] { 123, 2, 15, 212, 174, 141, 233, 86 };
            var scope = System.Security.Cryptography.DataProtectionScope.CurrentUser;

            ProtectedDataStore pds = new ProtectedDataStore(db, scope, entropy);

            pds.GetApplicationSecret(OzetteLibrary.Constants.OptionIDs.AzureStorageAccountName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProtectedDataStoreSetApplicationSecretThrowsWhenNoOptionIsProvided()
        {
            var ms = new MemoryStream();
            var db = new LiteDBClientDatabase(ms);
            db.PrepareDatabase();
            var entropy = new byte[] { 123, 2, 15, 212, 174, 141, 233, 86 };
            var scope = System.Security.Cryptography.DataProtectionScope.CurrentUser;

            ProtectedDataStore pds = new ProtectedDataStore(db, scope, entropy);

            pds.SetApplicationSecret(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProtectedDataStoreSetApplicationSecretThrowsWhenNoOptionIdIsProvided()
        {
            var ms = new MemoryStream();
            var db = new LiteDBClientDatabase(ms);
            db.PrepareDatabase();
            var entropy = new byte[] { 123, 2, 15, 212, 174, 141, 233, 86 };
            var scope = System.Security.Cryptography.DataProtectionScope.CurrentUser;

            ProtectedDataStore pds = new ProtectedDataStore(db, scope, entropy);

            var option = new ServiceOption();
            option.ID = 0; // should throw
            option.IsEncryptedOption = true;
            option.Name = nameof(OzetteLibrary.Constants.OptionIDs.AzureStorageAccountName);
            option.Value = "test-account";

            pds.SetApplicationSecret(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProtectedDataStoreSetApplicationSecretThrowsWhenNoOptionNameIsProvided()
        {
            var ms = new MemoryStream();
            var db = new LiteDBClientDatabase(ms);
            db.PrepareDatabase();
            var entropy = new byte[] { 123, 2, 15, 212, 174, 141, 233, 86 };
            var scope = System.Security.Cryptography.DataProtectionScope.CurrentUser;

            ProtectedDataStore pds = new ProtectedDataStore(db, scope, entropy);

            var option = new ServiceOption();
            option.ID = OzetteLibrary.Constants.OptionIDs.AzureStorageAccountName;
            option.IsEncryptedOption = true;
            option.Name = ""; // should throw
            option.Value = "test-account";

            pds.SetApplicationSecret(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProtectedDataStoreSetApplicationSecretThrowsWhenNoOptionValueIsProvided()
        {
            var ms = new MemoryStream();
            var db = new LiteDBClientDatabase(ms);
            db.PrepareDatabase();
            var entropy = new byte[] { 123, 2, 15, 212, 174, 141, 233, 86 };
            var scope = System.Security.Cryptography.DataProtectionScope.CurrentUser;

            ProtectedDataStore pds = new ProtectedDataStore(db, scope, entropy);

            var option = new ServiceOption();
            option.ID = OzetteLibrary.Constants.OptionIDs.AzureStorageAccountName;
            option.IsEncryptedOption = true;
            option.Name = nameof(OzetteLibrary.Constants.OptionIDs.AzureStorageAccountName);
            option.Value = ""; // should throw

            pds.SetApplicationSecret(option);
        }

        [TestMethod]
        public void ProtectedDataStoreSetApplicationSecretShouldSaveEncryptedSecretUserScope()
        {
            var ms = new MemoryStream();
            var db = new LiteDBClientDatabase(ms);
            db.PrepareDatabase();
            var entropy = new byte[] { 123, 2, 15, 212, 174, 141, 233, 86 };
            var scope = System.Security.Cryptography.DataProtectionScope.CurrentUser;

            ProtectedDataStore pds = new ProtectedDataStore(db, scope, entropy);

            var option = new ServiceOption();
            option.ID = OzetteLibrary.Constants.OptionIDs.AzureStorageAccountName;
            option.IsEncryptedOption = true;
            option.Name = nameof(OzetteLibrary.Constants.OptionIDs.AzureStorageAccountName);
            option.Value = "test-account";

            pds.SetApplicationSecret(option);

            // check the underlying database entry
            // it should be encrypted.

            var optionValue = db.GetApplicationOption(OzetteLibrary.Constants.OptionIDs.AzureStorageAccountName);

            // the actual value of the encrypted string is going to vary by machine scope
            // for consistency in testing just verify the string has been transformed.

            Assert.IsNotNull(optionValue);
            Assert.AreNotEqual("test-account", optionValue);
            Assert.IsFalse(optionValue.Contains("test-account"));
            Assert.IsTrue(optionValue.Length > "test-account".Length);
        }

        [TestMethod]
        public void ProtectedDataStoreGetApplicationSecretShouldReturnDecryptedSecretUserScope()
        {
            var ms = new MemoryStream();
            var db = new LiteDBClientDatabase(ms);
            db.PrepareDatabase();
            var entropy = new byte[] { 123, 2, 15, 212, 174, 141, 233, 86 };
            var scope = System.Security.Cryptography.DataProtectionScope.CurrentUser;

            ProtectedDataStore pds = new ProtectedDataStore(db, scope, entropy);

            var option = new ServiceOption();
            option.ID = OzetteLibrary.Constants.OptionIDs.AzureStorageAccountName;
            option.IsEncryptedOption = true;
            option.Name = nameof(OzetteLibrary.Constants.OptionIDs.AzureStorageAccountName);
            option.Value = "test-account";

            pds.SetApplicationSecret(option);

            Assert.AreEqual("test-account", pds.GetApplicationSecret(option.ID));
        }

        [TestMethod]
        public void ProtectedDataStoreSetApplicationSecretShouldSaveEncryptedSecretMachineScope()
        {
            var ms = new MemoryStream();
            var db = new LiteDBClientDatabase(ms);
            db.PrepareDatabase();
            var entropy = new byte[] { 123, 2, 15, 212, 174, 141, 233, 86 };
            var scope = System.Security.Cryptography.DataProtectionScope.LocalMachine;

            ProtectedDataStore pds = new ProtectedDataStore(db, scope, entropy);

            var option = new ServiceOption();
            option.ID = OzetteLibrary.Constants.OptionIDs.AzureStorageAccountName;
            option.IsEncryptedOption = true;
            option.Name = nameof(OzetteLibrary.Constants.OptionIDs.AzureStorageAccountName);
            option.Value = "test-account";

            pds.SetApplicationSecret(option);

            // check the underlying database entry
            // it should be encrypted.

            var optionValue = db.GetApplicationOption(OzetteLibrary.Constants.OptionIDs.AzureStorageAccountName);

            // the actual value of the encrypted string is going to vary by machine scope
            // for consistency in testing just verify the string has been transformed.

            Assert.IsNotNull(optionValue);
            Assert.AreNotEqual("test-account", optionValue);
            Assert.IsFalse(optionValue.Contains("test-account"));
            Assert.IsTrue(optionValue.Length > "test-account".Length);
        }

        [TestMethod]
        public void ProtectedDataStoreGetApplicationSecretShouldReturnDecryptedSecretMachineScope()
        {
            var ms = new MemoryStream();
            var db = new LiteDBClientDatabase(ms);
            db.PrepareDatabase();
            var entropy = new byte[] { 123, 2, 15, 212, 174, 141, 233, 86 };
            var scope = System.Security.Cryptography.DataProtectionScope.LocalMachine;

            ProtectedDataStore pds = new ProtectedDataStore(db, scope, entropy);

            var option = new ServiceOption();
            option.ID = OzetteLibrary.Constants.OptionIDs.AzureStorageAccountName;
            option.IsEncryptedOption = true;
            option.Name = nameof(OzetteLibrary.Constants.OptionIDs.AzureStorageAccountName);
            option.Value = "test-account";

            pds.SetApplicationSecret(option);

            Assert.AreEqual("test-account", pds.GetApplicationSecret(option.ID));
        }
    }
}
