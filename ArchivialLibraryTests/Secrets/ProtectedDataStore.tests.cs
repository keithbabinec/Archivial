using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ArchivialLibrary.Database;
using ArchivialLibrary.Database.SQLServer;
using ArchivialLibrary.Exceptions;
using ArchivialLibrary.Logging.Mock;
using ArchivialLibrary.Secrets;
using System;
using System.Threading.Tasks;

namespace ArchivialLibraryTests.Secrets
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
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());
            var entropy = new byte[] { };
            var scope = System.Security.Cryptography.DataProtectionScope.CurrentUser;

            ProtectedDataStore pds = new ProtectedDataStore(db, scope, entropy);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationSecretMissingException))]
        public async Task ProtectedDataStoreGetApplicationSecretThrowsWhenSecretIsNotFound()
        {
            var db = new Mock<IClientDatabase>();
            db.Setup(x => x.GetApplicationOptionAsync(It.IsAny<string>())).ReturnsAsync((string)null);

            var entropy = new byte[] { 123, 2, 15, 212, 174, 141, 233, 86 };
            var scope = System.Security.Cryptography.DataProtectionScope.CurrentUser;

            ProtectedDataStore pds = new ProtectedDataStore(db.Object, scope, entropy);

            await pds.GetApplicationSecretAsync(ArchivialLibrary.Constants.RuntimeSettingNames.AzureStorageAccountName).ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task ProtectedDataStoreSetApplicationSecretThrowsWhenNoOptionNameIsProvided()
        {
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());
            var entropy = new byte[] { 123, 2, 15, 212, 174, 141, 233, 86 };
            var scope = System.Security.Cryptography.DataProtectionScope.CurrentUser;

            ProtectedDataStore pds = new ProtectedDataStore(db, scope, entropy);

            await pds.SetApplicationSecretAsync("", "test-account").ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task ProtectedDataStoreSetApplicationSecretThrowsWhenNoOptionValueIsProvided()
        {
            var db = new SQLServerClientDatabase(TestConnectionString, new MockLogger());
            var entropy = new byte[] { 123, 2, 15, 212, 174, 141, 233, 86 };
            var scope = System.Security.Cryptography.DataProtectionScope.CurrentUser;

            ProtectedDataStore pds = new ProtectedDataStore(db, scope, entropy);

            await pds.SetApplicationSecretAsync(ArchivialLibrary.Constants.RuntimeSettingNames.AzureStorageAccountName, "").ConfigureAwait(false);
        }
    }
}
