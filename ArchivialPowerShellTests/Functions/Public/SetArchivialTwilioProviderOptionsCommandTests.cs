using ArchivialLibrary.Constants;
using ArchivialLibrary.Database;
using ArchivialLibrary.MessagingProviders;
using ArchivialLibrary.Providers;
using ArchivialLibrary.Secrets;
using ArchivialPowerShell.Functions.Public;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace ArchivialPowerShellTests.Functions.Public
{
    [TestClass]
    public class SetArchivialTwilioProviderOptionsCommandTests
    {
        /// <summary>
        /// A sample base64 encoded string (the word 'test').
        /// </summary>
        private string SharedTestEncodedString = "dGVzdA==";

        [TestMethod]
        public void SetArchivialTwilioProviderOptionsCommand_TwilioAccountID_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialTwilioProviderOptionsCommand),
                    nameof(SetArchivialTwilioProviderOptionsCommand.TwilioAccountID),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialTwilioProviderOptionsCommand),
                    nameof(SetArchivialTwilioProviderOptionsCommand.TwilioAccountID),
                    typeof(ValidateNotNullOrEmptyAttribute))
            );
        }

        [TestMethod]
        public void SetArchivialTwilioProviderOptionsCommand_TwilioAuthToken_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialTwilioProviderOptionsCommand),
                    nameof(SetArchivialTwilioProviderOptionsCommand.TwilioAuthToken),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialTwilioProviderOptionsCommand),
                    nameof(SetArchivialTwilioProviderOptionsCommand.TwilioAuthToken),
                    typeof(ValidateNotNullOrEmptyAttribute))
            );
        }

        [TestMethod]
        public void SetArchivialTwilioProviderOptionsCommand_TwilioDestinationPhones_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialTwilioProviderOptionsCommand),
                    nameof(SetArchivialTwilioProviderOptionsCommand.TwilioDestinationPhones),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialTwilioProviderOptionsCommand),
                    nameof(SetArchivialTwilioProviderOptionsCommand.TwilioDestinationPhones),
                    typeof(ValidateNotNullOrEmptyAttribute))
            );
        }

        [TestMethod]
        public void SetArchivialTwilioProviderOptionsCommand_TwilioSourcePhone_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialTwilioProviderOptionsCommand),
                    nameof(SetArchivialTwilioProviderOptionsCommand.TwilioSourcePhone),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialTwilioProviderOptionsCommand),
                    nameof(SetArchivialTwilioProviderOptionsCommand.TwilioSourcePhone),
                    typeof(ValidateNotNullOrEmptyAttribute))
            );
        }

        [TestMethod]
        public void SetArchivialTwilioProviderOptionsCommand_CanSetTwilioOptions_WhenAlreadyConfigured()
        {
            var mockedSecretStore = new Mock<ISecretStore>();
            var mockedDb = new Mock<IClientDatabase>();

            mockedDb.Setup(x => x.GetApplicationOptionAsync(It.Is<string>(z => z == RuntimeSettingNames.ProtectionIV))).ReturnsAsync(SharedTestEncodedString);

            mockedDb.Setup(x => x.GetProvidersAsync(It.Is<ProviderTypes>(z => z == ProviderTypes.Messaging)))
                .ReturnsAsync(new ProviderCollection()
                    {
                        new Provider()
                        {
                            Type = ProviderTypes.Storage,
                            Name = nameof(MessagingProviderTypes.Twilio)
                        }
                    });

            var command = new SetArchivialTwilioProviderOptionsCommand(mockedDb.Object, mockedSecretStore.Object, null);
            command.TwilioAccountID = "FakeAccountID";
            command.TwilioAuthToken = "FakeAuthToken";
            command.TwilioDestinationPhones = new string[] { "123456788", "123456789" };
            command.TwilioSourcePhone = "123456777";

            var result = command.Invoke().GetEnumerator().MoveNext();

            mockedDb.Verify(x => x.AddProviderAsync(It.IsAny<Provider>()), Times.Never);
            mockedSecretStore.Verify(x => x.SetApplicationSecretAsync(It.Is<string>(z => z == RuntimeSettingNames.TwilioAccountID), It.IsAny<string>()), Times.Once);
            mockedSecretStore.Verify(x => x.SetApplicationSecretAsync(It.Is<string>(z => z == RuntimeSettingNames.TwilioAuthToken), It.IsAny<string>()), Times.Once);
            mockedSecretStore.Verify(x => x.SetApplicationSecretAsync(It.Is<string>(z => z == RuntimeSettingNames.TwilioDestinationPhones), It.IsAny<string>()), Times.Once);
            mockedSecretStore.Verify(x => x.SetApplicationSecretAsync(It.Is<string>(z => z == RuntimeSettingNames.TwilioSourcePhone), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void SetArchivialTwilioProviderOptionsCommand_CanSetTwilioOptions_WhenNotConfigured()
        {
            var mockedSecretStore = new Mock<ISecretStore>();
            var mockedDb = new Mock<IClientDatabase>();

            mockedDb.Setup(x => x.GetApplicationOptionAsync(It.Is<string>(z => z == RuntimeSettingNames.ProtectionIV))).ReturnsAsync(SharedTestEncodedString);

            mockedDb.Setup(x => x.GetProvidersAsync(It.Is<ProviderTypes>(z => z == ProviderTypes.Messaging))).ReturnsAsync(new ProviderCollection());

            var command = new SetArchivialTwilioProviderOptionsCommand(mockedDb.Object, mockedSecretStore.Object, null);
            command.TwilioAccountID = "FakeAccountID";
            command.TwilioAuthToken = "FakeAuthToken";
            command.TwilioDestinationPhones = new string[] { "123456788", "123456789" };
            command.TwilioSourcePhone = "123456777";

            var result = command.Invoke().GetEnumerator().MoveNext();

            mockedDb.Verify(x => x.AddProviderAsync(It.IsAny<Provider>()), Times.Once);
            mockedSecretStore.Verify(x => x.SetApplicationSecretAsync(It.Is<string>(z => z == RuntimeSettingNames.TwilioAccountID), It.IsAny<string>()), Times.Once);
            mockedSecretStore.Verify(x => x.SetApplicationSecretAsync(It.Is<string>(z => z == RuntimeSettingNames.TwilioAuthToken), It.IsAny<string>()), Times.Once);
            mockedSecretStore.Verify(x => x.SetApplicationSecretAsync(It.Is<string>(z => z == RuntimeSettingNames.TwilioDestinationPhones), It.IsAny<string>()), Times.Once);
            mockedSecretStore.Verify(x => x.SetApplicationSecretAsync(It.Is<string>(z => z == RuntimeSettingNames.TwilioSourcePhone), It.IsAny<string>()), Times.Once);
        }
    }
}
