using ArchivialLibrary.Database;
using ArchivialLibrary.Exceptions;
using ArchivialLibrary.Files;
using ArchivialLibrary.Folders;
using ArchivialPowerShell.Functions.Public;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace ArchivialPowerShellTests.Functions.Public
{
    [TestClass]
    public class AddArchivialLocalSourceCommandTests
    {
        [TestMethod]
        public void AddArchivialLocalSourceCommand_FolderPathParameter_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(AddArchivialLocalSourceCommand),
                    nameof(AddArchivialLocalSourceCommand.FolderPath),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(AddArchivialLocalSourceCommand),
                    nameof(AddArchivialLocalSourceCommand.FolderPath),
                    typeof(ValidateNotNullOrEmptyAttribute))
            );
        }

        [TestMethod]
        public void AddArchivialLocalSourceCommand_PriorityParameter_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(AddArchivialLocalSourceCommand),
                    nameof(AddArchivialLocalSourceCommand.Priority),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(AddArchivialLocalSourceCommand),
                    nameof(AddArchivialLocalSourceCommand.Priority),
                    typeof(ValidateSetAttribute))
            );
        }

        [TestMethod]
        public void AddArchivialLocalSourceCommand_RevisionsParameter_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(AddArchivialLocalSourceCommand),
                    nameof(AddArchivialLocalSourceCommand.Revisions),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(AddArchivialLocalSourceCommand),
                    nameof(AddArchivialLocalSourceCommand.Revisions),
                    typeof(ValidateRangeAttribute))
            );
        }

        [TestMethod]
        public void AddArchivialLocalSourceCommand_MatchFilterParameter_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(AddArchivialLocalSourceCommand),
                    nameof(AddArchivialLocalSourceCommand.MatchFilter),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(AddArchivialLocalSourceCommand),
                    nameof(AddArchivialLocalSourceCommand.MatchFilter),
                    typeof(ValidateNotNullOrEmptyAttribute))
            );
        }

        [TestMethod]
        public void AddArchivialLocalSourceCommand_CanSaveNewLocalSource()
        {
            var mockedDb = new Mock<IClientDatabase>();

            LocalSourceLocation databaseCommitedObject = null;

            mockedDb.Setup(x => x.GetSourceLocationsAsync()).ReturnsAsync(new SourceLocations());

            mockedDb.Setup(x => x.SetSourceLocationAsync(It.IsAny<LocalSourceLocation>()))
                .Returns(Task.CompletedTask)
                .Callback<LocalSourceLocation>(x => databaseCommitedObject = x);
            
            var command = new AddArchivialLocalSourceCommand(mockedDb.Object, null, null)
            {
                FolderPath = "C:\\folder\\path",
                Priority = "Low",
                Revisions = 1,
                MatchFilter = "*"
            };

            var result = command.Invoke().GetEnumerator().MoveNext();

            mockedDb.Verify(x => x.SetSourceLocationAsync(It.IsAny<LocalSourceLocation>()), Times.Once);

            Assert.IsNotNull(databaseCommitedObject);
            Assert.AreEqual("C:\\folder\\path", databaseCommitedObject.Path);
            Assert.AreEqual(FileBackupPriority.Low, databaseCommitedObject.Priority);
            Assert.AreEqual(1, databaseCommitedObject.RevisionCount);
            Assert.AreEqual("*", databaseCommitedObject.FileMatchFilter);
        }

        [TestMethod]
        [ExpectedException(typeof(SourceLocationException))]
        public void AddArchivialLocalSourceCommand_ThrowsErrorIfSourceFolderAndFileMatchExists()
        {
            var mockedDb = new Mock<IClientDatabase>();

            mockedDb.Setup(x => x.GetSourceLocationsAsync()).ReturnsAsync(
                new SourceLocations()
                {
                    new LocalSourceLocation()
                    {
                        Path = "C:\\folder\\path",
                        FileMatchFilter = "*"
                    }
                }
            );

            var command = new AddArchivialLocalSourceCommand(mockedDb.Object, null, null)
            {
                FolderPath = "C:\\folder\\path",
                MatchFilter = "*",
                Priority = "Low",
                Revisions = 1
            };

            var result = command.Invoke().GetEnumerator().MoveNext();
        }
    }
}
