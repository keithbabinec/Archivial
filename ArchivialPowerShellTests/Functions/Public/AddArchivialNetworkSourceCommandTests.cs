using ArchivialLibrary.Database;
using ArchivialLibrary.Exceptions;
using ArchivialLibrary.Files;
using ArchivialLibrary.Folders;
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
    public class AddArchivialNetworkSourceCommandTests
    {
        [TestMethod]
        public void AddArchivialNetworkSourceCommand_UncPathParameter_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(AddArchivialNetworkSourceCommand),
                    nameof(AddArchivialNetworkSourceCommand.UncPath),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(AddArchivialNetworkSourceCommand),
                    nameof(AddArchivialNetworkSourceCommand.UncPath),
                    typeof(ValidateNotNullOrEmptyAttribute))
            );
        }

        [TestMethod]
        public void AddArchivialNetworkSourceCommand_PriorityParameter_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(AddArchivialNetworkSourceCommand),
                    nameof(AddArchivialNetworkSourceCommand.Priority),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(AddArchivialNetworkSourceCommand),
                    nameof(AddArchivialNetworkSourceCommand.Priority),
                    typeof(ValidateSetAttribute))
            );
        }

        [TestMethod]
        public void AddArchivialNetworkSourceCommand_RevisionsParameter_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(AddArchivialNetworkSourceCommand),
                    nameof(AddArchivialNetworkSourceCommand.Revisions),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(AddArchivialNetworkSourceCommand),
                    nameof(AddArchivialNetworkSourceCommand.Revisions),
                    typeof(ValidateRangeAttribute))
            );
        }

        [TestMethod]
        public void AddArchivialNetworkSourceCommand_CredentialName_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(AddArchivialNetworkSourceCommand),
                    nameof(AddArchivialNetworkSourceCommand.CredentialName),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(AddArchivialNetworkSourceCommand),
                    nameof(AddArchivialNetworkSourceCommand.CredentialName),
                    typeof(ValidateNotNullOrEmptyAttribute))
            );
        }

        [TestMethod]
        public void AddArchivialNetworkSourceCommand_MatchFilterParameter_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(AddArchivialNetworkSourceCommand),
                    nameof(AddArchivialNetworkSourceCommand.MatchFilter),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(AddArchivialNetworkSourceCommand),
                    nameof(AddArchivialNetworkSourceCommand.MatchFilter),
                    typeof(ValidateNotNullOrEmptyAttribute))
            );
        }

        [TestMethod]
        public void AddArchivialNetworkSourceCommand_CanSaveNewNetworkSource()
        {
            var mockedDb = new Mock<IClientDatabase>();

            NetworkSourceLocation databaseCommitedObject = null;

            mockedDb.Setup(x => x.GetSourceLocationsAsync()).ReturnsAsync(new SourceLocations());

            mockedDb.Setup(x => x.SetSourceLocationAsync(It.IsAny<NetworkSourceLocation>()))
                .Returns(Task.CompletedTask)
                .Callback<NetworkSourceLocation>(x => databaseCommitedObject = x);

            var command = new AddArchivialNetworkSourceCommand(mockedDb.Object)
            {
                UncPath = "\\\\network\\path\\to\\folder",
                Priority = "Low",
                Revisions = 1,
                MatchFilter = "*",
                CredentialName = "creds"
            };

            var result = command.Invoke().GetEnumerator().MoveNext();

            mockedDb.Verify(x => x.SetSourceLocationAsync(It.IsAny<NetworkSourceLocation>()), Times.Once);

            Assert.IsNotNull(databaseCommitedObject);
            Assert.AreEqual("\\\\network\\path\\to\\folder", databaseCommitedObject.Path);
            Assert.AreEqual(FileBackupPriority.Low, databaseCommitedObject.Priority);
            Assert.AreEqual(1, databaseCommitedObject.RevisionCount);
            Assert.AreEqual("*", databaseCommitedObject.FileMatchFilter);
        }

        [TestMethod]
        [ExpectedException(typeof(SourceLocationException))]
        public void AddArchivialNetworkSourceCommand_ThrowsErrorIfSourceFolderAndFileMatchExists()
        {
            var mockedDb = new Mock<IClientDatabase>();

            mockedDb.Setup(x => x.GetSourceLocationsAsync()).ReturnsAsync(
                new SourceLocations()
                {
                    new NetworkSourceLocation()
                    {
                        Path = "\\\\network\\path\\to\\folder",
                        FileMatchFilter = "*"
                    }
                }
            );

            var command = new AddArchivialNetworkSourceCommand(mockedDb.Object)
            {
                UncPath = "\\\\network\\path\\to\\folder",
                MatchFilter = "*",
                Priority = "Low",
                Revisions = 1,
                CredentialName = "creds"
            };

            var result = command.Invoke().GetEnumerator().MoveNext();
        }
    }
}
