using ArchivialLibrary.Constants;
using ArchivialLibrary.Database;
using ArchivialLibrary.Providers;
using ArchivialLibrary.Secrets;
using ArchivialLibrary.StorageProviders;
using ArchivialPowerShell.Functions.Public;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Management.Automation;

namespace ArchivialPowerShellTests.Functions.Public
{
    [TestClass]
    public class SetArchivialAzureProviderOptionsCommandTests
    {
        /// <summary>
        /// A sample base64 encoded string (the word 'test').
        /// </summary>
        private string SharedTestEncodedString = "dGVzdA==";

        [TestMethod]
        public void SetArchivialAzureProviderOptionsCommand_AzureStorageAccountName_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialAzureProviderOptionsCommand),
                    nameof(SetArchivialAzureProviderOptionsCommand.AzureStorageAccountName),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialAzureProviderOptionsCommand),
                    nameof(SetArchivialAzureProviderOptionsCommand.AzureStorageAccountName),
                    typeof(ValidateNotNullOrEmptyAttribute))
            );
        }

        [TestMethod]
        public void SetArchivialAzureProviderOptionsCommand_AzureStorageAccountToken_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialAzureProviderOptionsCommand),
                    nameof(SetArchivialAzureProviderOptionsCommand.AzureStorageAccountToken),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialAzureProviderOptionsCommand),
                    nameof(SetArchivialAzureProviderOptionsCommand.AzureStorageAccountToken),
                    typeof(ValidateNotNullOrEmptyAttribute))
            );
        }

        [TestMethod]
        public void SetArchivialAzureProviderOptionsCommand_CanSetAzureOptions_WhenAlreadyConfigured()
        {
            var mockedSecretStore = new Mock<ISecretStore>();
            var mockedDb = new Mock<IClientDatabase>();

            mockedDb.Setup(x => x.GetApplicationOptionAsync(It.Is<string>(z => z == RuntimeSettingNames.ProtectionIV))).ReturnsAsync(SharedTestEncodedString);

            mockedDb.Setup(x => x.GetProvidersAsync(It.Is<ProviderTypes>(z => z == ProviderTypes.Storage)))
                .ReturnsAsync(new ProviderCollection()
                    {
                        new Provider()
                        {
                            Type = ProviderTypes.Storage,
                            Name = nameof(StorageProviderTypes.Azure)
                        }
                    });

            var command = new SetArchivialAzureProviderOptionsCommand(mockedDb.Object, mockedSecretStore.Object, null);
            command.AzureStorageAccountName = "FakeStorageAccount";
            command.AzureStorageAccountToken = "FakeStorageAccountToken";

            var result = command.Invoke().GetEnumerator().MoveNext();

            mockedDb.Verify(x => x.AddProviderAsync(It.IsAny<Provider>()), Times.Never);
            mockedSecretStore.Verify(x => x.SetApplicationSecretAsync(It.Is<string>(z => z == RuntimeSettingNames.AzureStorageAccountName), It.IsAny<string>()), Times.Once);
            mockedSecretStore.Verify(x => x.SetApplicationSecretAsync(It.Is<string>(z => z == RuntimeSettingNames.AzureStorageAccountToken), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void SetArchivialAzureProviderOptionsCommand_CanSetAzureOptions_WhenNotConfigured()
        {
            var mockedSecretStore = new Mock<ISecretStore>();
            var mockedDb = new Mock<IClientDatabase>();

            mockedDb.Setup(x => x.GetApplicationOptionAsync(It.Is<string>(z => z == RuntimeSettingNames.ProtectionIV))).ReturnsAsync(SharedTestEncodedString);

            mockedDb.Setup(x => x.GetProvidersAsync(It.Is<ProviderTypes>(z => z == ProviderTypes.Storage))).ReturnsAsync(new ProviderCollection());

            var command = new SetArchivialAzureProviderOptionsCommand(mockedDb.Object, mockedSecretStore.Object, null);
            command.AzureStorageAccountName = "FakeStorageAccount";
            command.AzureStorageAccountToken = "FakeStorageAccountToken";

            var result = command.Invoke().GetEnumerator().MoveNext();

            mockedDb.Verify(x => x.AddProviderAsync(It.IsAny<Provider>()), Times.Once);
            mockedSecretStore.Verify(x => x.SetApplicationSecretAsync(It.Is<string>(z => z == RuntimeSettingNames.AzureStorageAccountName), It.IsAny<string>()), Times.Once);
            mockedSecretStore.Verify(x => x.SetApplicationSecretAsync(It.Is<string>(z => z == RuntimeSettingNames.AzureStorageAccountToken), It.IsAny<string>()), Times.Once);
        }
    }
}
