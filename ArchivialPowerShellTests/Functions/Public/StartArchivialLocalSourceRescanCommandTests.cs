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
    public class StartArchivialLocalSourceRescanCommandTests
    {
        [TestMethod]
        public void StartArchivialLocalSourceRescanCommand_LocalSourceParameter_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(StartArchivialLocalSourceRescanCommand),
                    nameof(StartArchivialLocalSourceRescanCommand.LocalSource),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(StartArchivialLocalSourceRescanCommand),
                    nameof(StartArchivialLocalSourceRescanCommand.LocalSource),
                    typeof(ValidateNotNullAttribute))
            );
        }

        [TestMethod]
        public void StartArchivialLocalSourceRescanCommand_SourceIDParameter_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(StartArchivialLocalSourceRescanCommand),
                    nameof(StartArchivialLocalSourceRescanCommand.SourceID),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(StartArchivialLocalSourceRescanCommand),
                    nameof(StartArchivialLocalSourceRescanCommand.SourceID),
                    typeof(ValidateRangeAttribute))
            );
        }

        [TestMethod]
        public void StartArchivialLocalSourceRescanCommand_CanQueueRescan_FromSourceId()
        {
            var mockedDb = new Mock<IClientDatabase>();

            LocalSourceLocation databaseCommitedObject = null;

            mockedDb.Setup(x => x.GetSourceLocationsAsync()).ReturnsAsync(
                new SourceLocations()
                {
                    new LocalSourceLocation() { ID = 1 }
                }
            );

            mockedDb.Setup(x => x.RescanSourceLocationAsync(It.IsAny<LocalSourceLocation>()))
                .Returns(Task.CompletedTask)
                .Callback<LocalSourceLocation>(x => databaseCommitedObject = x);

            var command = new StartArchivialLocalSourceRescanCommand(mockedDb.Object, null, null)
            {
                SourceID = 1
            };

            var result = command.Invoke().GetEnumerator().MoveNext();

            mockedDb.Verify(x => x.RescanSourceLocationAsync(It.IsAny<LocalSourceLocation>()), Times.Once);

            Assert.IsNotNull(databaseCommitedObject);
            Assert.AreEqual(1, databaseCommitedObject.ID);
        }

        [TestMethod]
        public void StartArchivialLocalSourceRescanCommand_CanQueueRescan_FromSourceObject()
        {
            var mockedDb = new Mock<IClientDatabase>();

            LocalSourceLocation databaseCommitedObject = null;

            mockedDb.Setup(x => x.GetSourceLocationsAsync()).ReturnsAsync(
                new SourceLocations()
                {
                    new LocalSourceLocation() { ID = 1 }
                }
            );

            mockedDb.Setup(x => x.RescanSourceLocationAsync(It.IsAny<LocalSourceLocation>()))
                .Returns(Task.CompletedTask)
                .Callback<LocalSourceLocation>(x => databaseCommitedObject = x);

            var command = new StartArchivialLocalSourceRescanCommand(mockedDb.Object, null, null)
            {
                LocalSource = new LocalSourceLocation() { ID = 1 }
            };

            var result = command.Invoke().GetEnumerator().MoveNext();

            mockedDb.Verify(x => x.RescanSourceLocationAsync(It.IsAny<LocalSourceLocation>()), Times.Once);

            Assert.IsNotNull(databaseCommitedObject);
            Assert.AreEqual(1, databaseCommitedObject.ID);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void StartArchivialLocalSourceRescanCommand_ThrowsIfSourceIsNotFound()
        {
            var mockedDb = new Mock<IClientDatabase>();

            LocalSourceLocation databaseCommitedObject = null;

            mockedDb.Setup(x => x.GetSourceLocationsAsync()).ReturnsAsync(
                new SourceLocations()
                {
                    new LocalSourceLocation() { ID = 1 }
                }
            );

            mockedDb.Setup(x => x.RescanSourceLocationAsync(It.IsAny<LocalSourceLocation>()))
                .Returns(Task.CompletedTask)
                .Callback<LocalSourceLocation>(x => databaseCommitedObject = x);

            var command = new StartArchivialLocalSourceRescanCommand(mockedDb.Object, null, null)
            {
                SourceID = 2
            };

            var result = command.Invoke().GetEnumerator().MoveNext();
        }
    }
}
