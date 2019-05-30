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
    public class RemoveArchivialLocalSourceCommandTests
    {
        [TestMethod]
        public void RemoveArchivialLocalSourceCommand_LocalSourceParameter_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(RemoveArchivialLocalSourceCommand),
                    nameof(RemoveArchivialLocalSourceCommand.LocalSource),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(RemoveArchivialLocalSourceCommand),
                    nameof(RemoveArchivialLocalSourceCommand.LocalSource),
                    typeof(ValidateNotNullAttribute))
            );
        }

        [TestMethod]
        public void RemoveArchivialLocalSourceCommand_SourceIDParameter_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(RemoveArchivialLocalSourceCommand),
                    nameof(RemoveArchivialLocalSourceCommand.SourceID),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(RemoveArchivialLocalSourceCommand),
                    nameof(RemoveArchivialLocalSourceCommand.SourceID),
                    typeof(ValidateRangeAttribute))
            );
        }

        [TestMethod]
        public void RemoveArchivialLocalSourceCommand_CanRemoveSource_FromSourceId()
        {
            var mockedDb = new Mock<IClientDatabase>();

            LocalSourceLocation databaseCommitedObject = null;

            mockedDb.Setup(x => x.GetSourceLocationsAsync()).ReturnsAsync(
                new SourceLocations()
                {
                    new LocalSourceLocation() { ID = 1 }
                }
            );

            mockedDb.Setup(x => x.RemoveSourceLocationAsync(It.IsAny<LocalSourceLocation>()))
                .Returns(Task.CompletedTask)
                .Callback<LocalSourceLocation>(x => databaseCommitedObject = x);

            var command = new RemoveArchivialLocalSourceCommand(mockedDb.Object, null, null)
            {
                SourceID = 1
            };

            var result = command.Invoke().GetEnumerator().MoveNext();

            mockedDb.Verify(x => x.RemoveSourceLocationAsync(It.IsAny<LocalSourceLocation>()), Times.Once);

            Assert.IsNotNull(databaseCommitedObject);
            Assert.AreEqual(1, databaseCommitedObject.ID);
        }

        [TestMethod]
        public void RemoveArchivialLocalSourceCommand_CanRemoveSource_FromSourceObject()
        {
            var mockedDb = new Mock<IClientDatabase>();

            LocalSourceLocation databaseCommitedObject = null;

            mockedDb.Setup(x => x.GetSourceLocationsAsync()).ReturnsAsync(
                new SourceLocations()
                {
                    new LocalSourceLocation() { ID = 1 }
                }
            );

            mockedDb.Setup(x => x.RemoveSourceLocationAsync(It.IsAny<LocalSourceLocation>()))
                .Returns(Task.CompletedTask)
                .Callback<LocalSourceLocation>(x => databaseCommitedObject = x);

            var command = new RemoveArchivialLocalSourceCommand(mockedDb.Object, null, null)
            {
                LocalSource = new LocalSourceLocation() { ID = 1 }
            };

            var result = command.Invoke().GetEnumerator().MoveNext();

            mockedDb.Verify(x => x.RemoveSourceLocationAsync(It.IsAny<LocalSourceLocation>()), Times.Once);

            Assert.IsNotNull(databaseCommitedObject);
            Assert.AreEqual(1, databaseCommitedObject.ID);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void RemoveArchivialLocalSourceCommand_ThrowsIfSourceIsNotFound()
        {
            var mockedDb = new Mock<IClientDatabase>();

            LocalSourceLocation databaseCommitedObject = null;

            mockedDb.Setup(x => x.GetSourceLocationsAsync()).ReturnsAsync(
                new SourceLocations()
                {
                    new LocalSourceLocation() { ID = 1 }
                }
            );

            mockedDb.Setup(x => x.RemoveSourceLocationAsync(It.IsAny<LocalSourceLocation>()))
                .Returns(Task.CompletedTask)
                .Callback<LocalSourceLocation>(x => databaseCommitedObject = x);

            var command = new RemoveArchivialLocalSourceCommand(mockedDb.Object, null, null)
            {
                SourceID = 2
            };

            var result = command.Invoke().GetEnumerator().MoveNext();
        }
    }
}
