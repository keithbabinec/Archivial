using ArchivialLibrary.Database;
using ArchivialLibrary.Folders;
using ArchivialPowerShell.Functions.Public;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace ArchivialPowerShellTests.Functions.Public
{
    [TestClass]
    public class RemoveArchivialNetworkSourceCommandTests
    {
        [TestMethod]
        public void RemoveArchivialNetworkSourceCommand_NetworkSourceParameter_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(RemoveArchivialNetworkSourceCommand),
                    nameof(RemoveArchivialNetworkSourceCommand.NetworkSource),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(RemoveArchivialNetworkSourceCommand),
                    nameof(RemoveArchivialNetworkSourceCommand.NetworkSource),
                    typeof(ValidateNotNullAttribute))
            );
        }

        [TestMethod]
        public void RemoveArchivialNetworkSourceCommand_SourceIDParameter_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(RemoveArchivialNetworkSourceCommand),
                    nameof(RemoveArchivialNetworkSourceCommand.SourceID),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(RemoveArchivialNetworkSourceCommand),
                    nameof(RemoveArchivialNetworkSourceCommand.SourceID),
                    typeof(ValidateRangeAttribute))
            );
        }

        [TestMethod]
        public void RemoveArchivialNetworkSourceCommand_CanRemoveSource_FromSourceId()
        {
            var mockedDb = new Mock<IClientDatabase>();

            NetworkSourceLocation databaseCommitedObject = null;

            mockedDb.Setup(x => x.GetSourceLocationsAsync()).ReturnsAsync(
                new SourceLocations()
                {
                    new NetworkSourceLocation() { ID = 1 }
                }
            );

            mockedDb.Setup(x => x.RemoveSourceLocationAsync(It.IsAny<NetworkSourceLocation>()))
                .Returns(Task.CompletedTask)
                .Callback<NetworkSourceLocation>(x => databaseCommitedObject = x);

            var command = new RemoveArchivialNetworkSourceCommand(mockedDb.Object)
            {
                SourceID = 1
            };

            var result = command.Invoke().GetEnumerator().MoveNext();

            mockedDb.Verify(x => x.RemoveSourceLocationAsync(It.IsAny<NetworkSourceLocation>()), Times.Once);

            Assert.IsNotNull(databaseCommitedObject);
            Assert.AreEqual(1, databaseCommitedObject.ID);
        }

        [TestMethod]
        public void RemoveArchivialNetworkSourceCommand_CanRemoveSource_FromSourceObject()
        {
            var mockedDb = new Mock<IClientDatabase>();

            NetworkSourceLocation databaseCommitedObject = null;

            mockedDb.Setup(x => x.GetSourceLocationsAsync()).ReturnsAsync(
                new SourceLocations()
                {
                    new NetworkSourceLocation() { ID = 1 }
                }
            );

            mockedDb.Setup(x => x.RemoveSourceLocationAsync(It.IsAny<NetworkSourceLocation>()))
                .Returns(Task.CompletedTask)
                .Callback<NetworkSourceLocation>(x => databaseCommitedObject = x);

            var command = new RemoveArchivialNetworkSourceCommand(mockedDb.Object)
            {
                NetworkSource = new NetworkSourceLocation() { ID = 1 }
            };

            var result = command.Invoke().GetEnumerator().MoveNext();

            mockedDb.Verify(x => x.RemoveSourceLocationAsync(It.IsAny<NetworkSourceLocation>()), Times.Once);

            Assert.IsNotNull(databaseCommitedObject);
            Assert.AreEqual(1, databaseCommitedObject.ID);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void RemoveArchivialNetworkSourceCommand_ThrowsIfSourceIsNotFound()
        {
            var mockedDb = new Mock<IClientDatabase>();

            NetworkSourceLocation databaseCommitedObject = null;

            mockedDb.Setup(x => x.GetSourceLocationsAsync()).ReturnsAsync(
                new SourceLocations()
                {
                    new NetworkSourceLocation() { ID = 1 }
                }
            );

            mockedDb.Setup(x => x.RemoveSourceLocationAsync(It.IsAny<NetworkSourceLocation>()))
                .Returns(Task.CompletedTask)
                .Callback<NetworkSourceLocation>(x => databaseCommitedObject = x);

            var command = new RemoveArchivialNetworkSourceCommand(mockedDb.Object)
            {
                SourceID = 2
            };

            var result = command.Invoke().GetEnumerator().MoveNext();
        }
    }
}
