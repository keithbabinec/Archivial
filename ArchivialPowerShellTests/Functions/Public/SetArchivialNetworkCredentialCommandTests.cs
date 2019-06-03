using ArchivialLibrary.Constants;
using ArchivialLibrary.Database;
using ArchivialLibrary.Secrets;
using ArchivialLibrary.ServiceCore;
using ArchivialPowerShell.Functions.Public;
using ArchivialPowerShell.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Management.Automation;

namespace ArchivialPowerShellTests.Functions.Public
{
    [TestClass]
    public class SetArchivialNetworkCredentialCommandTests
    {
        /// <summary>
        /// A sample base64 encoded string (the word 'test').
        /// </summary>
        private string SharedTestEncodedString = "dGVzdA==";

        [TestMethod]
        public void SetArchivialNetworkCredentialCommand_CredentialName_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialNetworkCredentialCommand),
                    nameof(SetArchivialNetworkCredentialCommand.CredentialName),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialNetworkCredentialCommand),
                    nameof(SetArchivialNetworkCredentialCommand.CredentialName),
                    typeof(ValidateNotNullOrEmptyAttribute))
            );
        }

        [TestMethod]
        public void SetArchivialNetworkCredentialCommand_ShareUsername_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialNetworkCredentialCommand),
                    nameof(SetArchivialNetworkCredentialCommand.ShareUsername),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialNetworkCredentialCommand),
                    nameof(SetArchivialNetworkCredentialCommand.ShareUsername),
                    typeof(ValidateNotNullOrEmptyAttribute))
            );
        }

        [TestMethod]
        public void SetArchivialNetworkCredentialCommand_SharePassword_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialNetworkCredentialCommand),
                    nameof(SetArchivialNetworkCredentialCommand.SharePassword),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialNetworkCredentialCommand),
                    nameof(SetArchivialNetworkCredentialCommand.SharePassword),
                    typeof(ValidateNotNullOrEmptyAttribute))
            );
        }

        [TestMethod]
        public void SetArchivialNetworkCredentialCommand_CanSetCredential_WhenAlreadyConfigured()
        {
            var mockedSecretStore = new Mock<ISecretStore>();
            var mockedDb = new Mock<IClientDatabase>();

            mockedDb.Setup(x => x.GetApplicationOptionAsync(It.Is<string>(z => z == RuntimeSettingNames.ProtectionIV))).ReturnsAsync(SharedTestEncodedString);

            mockedDb.Setup(x => x.GetNetCredentialsAsync())
                .ReturnsAsync(new NetCredentialsCollection()
                    {
                        new NetCredential()
                        {
                            CredentialName = "Credential"
                        }
                    });

            var mockedCoreSettings = new Mock<ICoreSettings>();

            var depedencies = new CmdletDependencies()
            {
                ClientDatabase = mockedDb.Object,
                SecretStore = mockedSecretStore.Object,
                CoreSettings = mockedCoreSettings.Object
            };

            var command = new SetArchivialNetworkCredentialCommand(depedencies);
            command.CredentialName = "Credential";
            command.ShareUsername = "FakeUserName";
            command.SharePassword = "FakePW";

            var result = command.Invoke().GetEnumerator().MoveNext();

            var usernameKey = string.Format(Formats.NetCredentialUserNameKeyLookup, command.CredentialName);
            var userpassKey = string.Format(Formats.NetCredentialUserPasswordKeyLookup, command.CredentialName);

            mockedDb.Verify(x => x.AddNetCredentialAsync(It.IsAny<NetCredential>()), Times.Never);
            mockedSecretStore.Verify(x => x.SetApplicationSecretAsync(It.Is<string>(z => z == usernameKey), It.IsAny<string>()), Times.Once);
            mockedSecretStore.Verify(x => x.SetApplicationSecretAsync(It.Is<string>(z => z == userpassKey), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void SetArchivialNetworkCredentialCommand_CanSetCredential_WhenNotConfigured()
        {
            var mockedSecretStore = new Mock<ISecretStore>();
            var mockedDb = new Mock<IClientDatabase>();

            mockedDb.Setup(x => x.GetApplicationOptionAsync(It.Is<string>(z => z == RuntimeSettingNames.ProtectionIV))).ReturnsAsync(SharedTestEncodedString);

            mockedDb.Setup(x => x.GetNetCredentialsAsync()).ReturnsAsync(new NetCredentialsCollection());

            var mockedCoreSettings = new Mock<ICoreSettings>();

            var depedencies = new CmdletDependencies()
            {
                ClientDatabase = mockedDb.Object,
                SecretStore = mockedSecretStore.Object,
                CoreSettings = mockedCoreSettings.Object
            };

            var command = new SetArchivialNetworkCredentialCommand(depedencies);
            command.CredentialName = "Credential";
            command.ShareUsername = "FakeUserName";
            command.SharePassword = "FakePW";

            var result = command.Invoke().GetEnumerator().MoveNext();

            var usernameKey = string.Format(Formats.NetCredentialUserNameKeyLookup, command.CredentialName);
            var userpassKey = string.Format(Formats.NetCredentialUserPasswordKeyLookup, command.CredentialName);

            mockedDb.Verify(x => x.AddNetCredentialAsync(It.IsAny<NetCredential>()), Times.Once);
            mockedSecretStore.Verify(x => x.SetApplicationSecretAsync(It.Is<string>(z => z == usernameKey), It.IsAny<string>()), Times.Once);
            mockedSecretStore.Verify(x => x.SetApplicationSecretAsync(It.Is<string>(z => z == userpassKey), It.IsAny<string>()), Times.Once);
        }
    }
}
