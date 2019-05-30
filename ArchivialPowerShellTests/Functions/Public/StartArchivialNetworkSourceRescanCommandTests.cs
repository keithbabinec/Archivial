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
    public class StartArchivialNetworkSourceRescanCommandTests
    {
        [TestMethod]
        public void StartArchivialNetworkSourceRescanCommand_NetworkSourceParameter_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(StartArchivialNetworkSourceRescanCommand),
                    nameof(StartArchivialNetworkSourceRescanCommand.NetworkSource),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(StartArchivialNetworkSourceRescanCommand),
                    nameof(StartArchivialNetworkSourceRescanCommand.NetworkSource),
                    typeof(ValidateNotNullAttribute))
            );
        }

        [TestMethod]
        public void StartArchivialNetworkSourceRescanCommand_SourceIDParameter_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(StartArchivialNetworkSourceRescanCommand),
                    nameof(StartArchivialNetworkSourceRescanCommand.SourceID),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(StartArchivialNetworkSourceRescanCommand),
                    nameof(StartArchivialNetworkSourceRescanCommand.SourceID),
                    typeof(ValidateRangeAttribute))
            );
        }

        [TestMethod]
        public void StartArchivialNetworkSourceRescanCommand_CanQueueRescan_FromSourceId()
        {
            var mockedDb = new Mock<IClientDatabase>();

            NetworkSourceLocation databaseCommitedObject = null;

            mockedDb.Setup(x => x.GetSourceLocationsAsync()).ReturnsAsync(
                new SourceLocations()
                {
                    new NetworkSourceLocation() { ID = 1 }
                }
            );

            mockedDb.Setup(x => x.RescanSourceLocationAsync(It.IsAny<NetworkSourceLocation>()))
                .Returns(Task.CompletedTask)
                .Callback<NetworkSourceLocation>(x => databaseCommitedObject = x);

            var command = new StartArchivialNetworkSourceRescanCommand(mockedDb.Object, null, null)
            {
                SourceID = 1
            };

            var result = command.Invoke().GetEnumerator().MoveNext();

            mockedDb.Verify(x => x.RescanSourceLocationAsync(It.IsAny<NetworkSourceLocation>()), Times.Once);

            Assert.IsNotNull(databaseCommitedObject);
            Assert.AreEqual(1, databaseCommitedObject.ID);
        }

        [TestMethod]
        public void StartArchivialNetworkSourceRescanCommand_CanQueueRescan_FromSourceObject()
        {
            var mockedDb = new Mock<IClientDatabase>();

            NetworkSourceLocation databaseCommitedObject = null;

            mockedDb.Setup(x => x.GetSourceLocationsAsync()).ReturnsAsync(
                new SourceLocations()
                {
                    new NetworkSourceLocation() { ID = 1 }
                }
            );

            mockedDb.Setup(x => x.RescanSourceLocationAsync(It.IsAny<NetworkSourceLocation>()))
                .Returns(Task.CompletedTask)
                .Callback<NetworkSourceLocation>(x => databaseCommitedObject = x);

            var command = new StartArchivialNetworkSourceRescanCommand(mockedDb.Object, null, null)
            {
                NetworkSource = new NetworkSourceLocation() { ID = 1 }
            };

            var result = command.Invoke().GetEnumerator().MoveNext();

            mockedDb.Verify(x => x.RescanSourceLocationAsync(It.IsAny<NetworkSourceLocation>()), Times.Once);

            Assert.IsNotNull(databaseCommitedObject);
            Assert.AreEqual(1, databaseCommitedObject.ID);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void StartArchivialNetworkSourceRescanCommand_ThrowsIfSourceIsNotFound()
        {
            var mockedDb = new Mock<IClientDatabase>();

            NetworkSourceLocation databaseCommitedObject = null;

            mockedDb.Setup(x => x.GetSourceLocationsAsync()).ReturnsAsync(
                new SourceLocations()
                {
                    new NetworkSourceLocation() { ID = 1 }
                }
            );

            mockedDb.Setup(x => x.RescanSourceLocationAsync(It.IsAny<NetworkSourceLocation>()))
                .Returns(Task.CompletedTask)
                .Callback<NetworkSourceLocation>(x => databaseCommitedObject = x);

            var command = new StartArchivialNetworkSourceRescanCommand(mockedDb.Object, null, null)
            {
                SourceID = 2
            };

            var result = command.Invoke().GetEnumerator().MoveNext();
        }
    }
}
