using ArchivialLibrary.Database;
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
    public class RemoveArchivialNetworkCredentialCommandTests
    {
        [TestMethod]
        public void RemoveArchivialNetworkCredentialCommand_NetCredentialParameter_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(RemoveArchivialNetworkCredentialCommand),
                    nameof(RemoveArchivialNetworkCredentialCommand.NetCredential),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(RemoveArchivialNetworkCredentialCommand),
                    nameof(RemoveArchivialNetworkCredentialCommand.NetCredential),
                    typeof(ValidateNotNullAttribute))
            );
        }

        [TestMethod]
        public void RemoveArchivialNetworkCredentialCommand_CredentialNameParameter_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(RemoveArchivialNetworkCredentialCommand),
                    nameof(RemoveArchivialNetworkCredentialCommand.CredentialName),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(RemoveArchivialNetworkCredentialCommand),
                    nameof(RemoveArchivialNetworkCredentialCommand.CredentialName),
                    typeof(ValidateNotNullOrEmptyAttribute))
            );
        }

        [TestMethod]
        public void RemoveArchivialNetworkCredentialCommand_CanRemoveCredential_FromCredentialName()
        {
            var mockedDb = new Mock<IClientDatabase>();

            string databaseSentCredentialName = null;

            mockedDb.Setup(x => x.GetNetCredentialsAsync()).ReturnsAsync(
                new NetCredentialsCollection()
                {
                    new NetCredential() { CredentialName = "Test" }
                }
            );

            mockedDb.Setup(x => x.RemoveNetCredentialAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask)
                .Callback<string>(x => databaseSentCredentialName = x);

            var command = new RemoveArchivialNetworkCredentialCommand(mockedDb.Object, null, null)
            {
                CredentialName = "Test"
            };

            var result = command.Invoke().GetEnumerator().MoveNext();

            mockedDb.Verify(x => x.RemoveNetCredentialAsync(It.IsAny<string>()), Times.Once);
            mockedDb.Verify(x => x.RemoveApplicationOptionAsync(It.IsAny<string>()), Times.Exactly(2));

            Assert.IsNotNull(databaseSentCredentialName);
            Assert.AreEqual("Test", databaseSentCredentialName);
        }

        [TestMethod]
        public void RemoveArchivialNetworkCredentialCommand_CanRemoveSource_FromSourceObject()
        {
            var mockedDb = new Mock<IClientDatabase>();

            string databaseSentCredentialName = null;

            mockedDb.Setup(x => x.GetNetCredentialsAsync()).ReturnsAsync(
                new NetCredentialsCollection()
                {
                    new NetCredential() { CredentialName = "Test" }
                }
            );

            mockedDb.Setup(x => x.RemoveNetCredentialAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask)
                .Callback<string>(x => databaseSentCredentialName = x);

            var command = new RemoveArchivialNetworkCredentialCommand(mockedDb.Object, null, null)
            {
                NetCredential = new NetCredential() { CredentialName = "Test" }
            };

            var result = command.Invoke().GetEnumerator().MoveNext();

            mockedDb.Verify(x => x.RemoveNetCredentialAsync(It.IsAny<string>()), Times.Once);
            mockedDb.Verify(x => x.RemoveApplicationOptionAsync(It.IsAny<string>()), Times.Exactly(2));

            Assert.IsNotNull(databaseSentCredentialName);
            Assert.AreEqual("Test", databaseSentCredentialName);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void RemoveArchivialNetworkCredentialCommand_ThrowsIfProviderIsNotFound()
        {
            var mockedDb = new Mock<IClientDatabase>();

            string databaseSentCredentialName = null;

            mockedDb.Setup(x => x.GetNetCredentialsAsync()).ReturnsAsync(
                new NetCredentialsCollection()
                {
                    new NetCredential() { CredentialName = "Test" }
                }
            );

            mockedDb.Setup(x => x.RemoveNetCredentialAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask)
                .Callback<string>(x => databaseSentCredentialName = x);

            var command = new RemoveArchivialNetworkCredentialCommand(mockedDb.Object, null, null)
            {
                CredentialName = "TestTest"
            };

            var result = command.Invoke().GetEnumerator().MoveNext();
        }
    }
}
