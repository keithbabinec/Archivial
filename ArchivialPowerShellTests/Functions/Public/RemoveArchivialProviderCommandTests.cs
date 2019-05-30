using ArchivialLibrary.Database;
using ArchivialLibrary.Providers;
using ArchivialPowerShell.Functions.Public;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace ArchivialPowerShellTests.Functions.Public
{
    [TestClass]
    public class RemoveArchivialProviderCommandTests
    {
        [TestMethod]
        public void RemoveArchivialProviderCommand_ProviderParameter_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(RemoveArchivialProviderCommand),
                    nameof(RemoveArchivialProviderCommand.Provider),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(RemoveArchivialProviderCommand),
                    nameof(RemoveArchivialProviderCommand.Provider),
                    typeof(ValidateNotNullAttribute))
            );
        }

        [TestMethod]
        public void RemoveArchivialProviderCommand_ProviderNameParameter_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(RemoveArchivialProviderCommand),
                    nameof(RemoveArchivialProviderCommand.ProviderName),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(RemoveArchivialProviderCommand),
                    nameof(RemoveArchivialProviderCommand.ProviderName),
                    typeof(ValidateNotNullOrEmptyAttribute))
            );
        }

        [TestMethod]
        public void RemoveArchivialProviderCommand_CanRemoveSource_FromProviderName()
        {
            var mockedDb = new Mock<IClientDatabase>();

            string databaseSentProviderName = null;

            mockedDb.Setup(x => x.GetProvidersAsync(ProviderTypes.Any)).ReturnsAsync(
                new ProviderCollection()
                {
                    new Provider() { Name = "Azure" }
                }
            );

            mockedDb.Setup(x => x.RemoveProviderAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask)
                .Callback<string>(x => databaseSentProviderName = x);

            var command = new RemoveArchivialProviderCommand(mockedDb.Object, null, null)
            {
                ProviderName = "Azure"
            };

            var result = command.Invoke().GetEnumerator().MoveNext();

            mockedDb.Verify(x => x.RemoveProviderAsync(It.IsAny<string>()), Times.Once);
            mockedDb.Verify(x => x.RemoveApplicationOptionAsync(It.IsAny<string>()), Times.Exactly(2));

            Assert.IsNotNull(databaseSentProviderName);
            Assert.AreEqual("Azure", databaseSentProviderName);
        }

        [TestMethod]
        public void RemoveArchivialProviderCommand_CanRemoveSource_FromSourceObject()
        {
            var mockedDb = new Mock<IClientDatabase>();

            string databaseSentProviderName = null;

            mockedDb.Setup(x => x.GetProvidersAsync(ProviderTypes.Any)).ReturnsAsync(
                new ProviderCollection()
                {
                    new Provider() { Name = "Azure" }
                }
            );

            mockedDb.Setup(x => x.RemoveProviderAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask)
                .Callback<string>(x => databaseSentProviderName = x);

            var command = new RemoveArchivialProviderCommand(mockedDb.Object, null, null)
            {
                Provider = new Provider() { Name = "Azure" }
            };

            var result = command.Invoke().GetEnumerator().MoveNext();

            mockedDb.Verify(x => x.RemoveProviderAsync(It.IsAny<string>()), Times.Once);
            mockedDb.Verify(x => x.RemoveApplicationOptionAsync(It.IsAny<string>()), Times.Exactly(2));

            Assert.IsNotNull(databaseSentProviderName);
            Assert.AreEqual("Azure", databaseSentProviderName);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void RemoveArchivialProviderCommand_ThrowsIfProviderIsNotFound()
        {
            var mockedDb = new Mock<IClientDatabase>();

            string databaseSentProviderName = null;

            mockedDb.Setup(x => x.GetProvidersAsync(ProviderTypes.Any)).ReturnsAsync(
                new ProviderCollection()
                {
                    new Provider() { Name = "Azure" }
                }
            );

            mockedDb.Setup(x => x.RemoveProviderAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask)
                .Callback<string>(x => databaseSentProviderName = x);

            var command = new RemoveArchivialProviderCommand(mockedDb.Object, null, null)
            {
                ProviderName = "AWS"
            };

            var result = command.Invoke().GetEnumerator().MoveNext();
        }
    }
}
