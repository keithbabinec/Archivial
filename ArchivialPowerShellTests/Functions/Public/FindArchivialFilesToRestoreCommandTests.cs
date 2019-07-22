using ArchivialLibrary.Database;
using ArchivialLibrary.Files;
using ArchivialLibrary.Folders;
using ArchivialLibrary.ServiceCore;
using ArchivialPowerShell.Functions.Public;
using ArchivialPowerShell.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Management.Automation;

namespace ArchivialPowerShellTests.Functions.Public
{
    [TestClass]
    public class FindArchivialFilesToRestoreCommandTests
    {
        [TestMethod]
        public void FindArchivialFilesToRestoreCommand_SourceParameter_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(FindArchivialFilesToRestoreCommand),
                    nameof(FindArchivialFilesToRestoreCommand.Source),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(FindArchivialFilesToRestoreCommand),
                    nameof(FindArchivialFilesToRestoreCommand.Source),
                    typeof(ValidateNotNullAttribute))
            );
        }

        [TestMethod]
        public void FindArchivialFilesToRestoreCommand_FileHashParameter_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(FindArchivialFilesToRestoreCommand),
                    nameof(FindArchivialFilesToRestoreCommand.FileHash),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(FindArchivialFilesToRestoreCommand),
                    nameof(FindArchivialFilesToRestoreCommand.FileHash),
                    typeof(ValidateNotNullOrEmptyAttribute))
            );
        }

        [TestMethod]
        public void FindArchivialFilesToRestoreCommand_MatchFilterParameter_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(FindArchivialFilesToRestoreCommand),
                    nameof(FindArchivialFilesToRestoreCommand.MatchFilter),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(FindArchivialFilesToRestoreCommand),
                    nameof(FindArchivialFilesToRestoreCommand.MatchFilter),
                    typeof(ValidateNotNullOrEmptyAttribute))
            );
        }

        [TestMethod]
        public void FindArchivialFilesToRestoreCommand_LimitResultsParameter_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(FindArchivialFilesToRestoreCommand),
                    nameof(FindArchivialFilesToRestoreCommand.LimitResults),
                    typeof(ParameterAttribute))
            );
        }

        [TestMethod]
        public void FindArchivialFilesToRestoreCommand_CanQueryByMatchFilter()
        {
            var mockedDb = new Mock<IClientDatabase>();

            string dbSubmittedMatch = null;
            int dbSubmittedLimit = 0;

            mockedDb.Setup(x => x.FindArchivialFilesToRestoreByFilter(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(new BackupFileSearchResults())
                .Callback<string, int>((x, y) =>
                    {
                        dbSubmittedMatch = x;
                        dbSubmittedLimit = y;
                    });

            var mockedCoreSettings = new Mock<ICoreSettings>();

            var depedencies = new CmdletDependencies()
            {
                ClientDatabase = mockedDb.Object,
                CoreSettings = mockedCoreSettings.Object
            };

            var command = new FindArchivialFilesToRestoreCommand(depedencies)
            {
                MatchFilter = "*.docx",
                LimitResults = 10
            };

            var result = command.Invoke().GetEnumerator().MoveNext();

            mockedDb.Verify(x => x.FindArchivialFilesToRestoreByFilter(It.IsAny<string>(), It.IsAny<int>()), Times.Once);

            Assert.AreEqual(10, dbSubmittedLimit);
            Assert.AreEqual("\"*.docx\"", dbSubmittedMatch); // wrapped in quotes is expected for SQL full-text search support
        }

        [TestMethod]
        public void FindArchivialFilesToRestoreCommand_CanQueryByFileHash()
        {
            var mockedDb = new Mock<IClientDatabase>();

            string dbSubmittedHash = null;
            int dbSubmittedLimit = 0;

            mockedDb.Setup(x => x.FindArchivialFilesToRestoreByHash(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(new BackupFileSearchResults())
                .Callback<string, int>((x, y) =>
                {
                    dbSubmittedHash = x;
                    dbSubmittedLimit = y;
                });

            var mockedCoreSettings = new Mock<ICoreSettings>();

            var depedencies = new CmdletDependencies()
            {
                ClientDatabase = mockedDb.Object,
                CoreSettings = mockedCoreSettings.Object
            };

            var command = new FindArchivialFilesToRestoreCommand(depedencies)
            {
                FileHash = "A37CC82F2876DB6CF59BA29B4EB148C7BF5CC920",
                LimitResults = 10
            };

            var result = command.Invoke().GetEnumerator().MoveNext();

            mockedDb.Verify(x => x.FindArchivialFilesToRestoreByHash(It.IsAny<string>(), It.IsAny<int>()), Times.Once);

            Assert.AreEqual(10, dbSubmittedLimit);
            Assert.AreEqual("\"A37CC82F2876DB6CF59BA29B4EB148C7BF5CC920\"", dbSubmittedHash); // wrapped in quotes is expected for SQL full-text search support
        }

        [TestMethod]
        public void FindArchivialFilesToRestoreCommand_CanQueryByLocalSource()
        {
            var mockedDb = new Mock<IClientDatabase>();

            SourceLocation dbSubmittedSource = null;
            int dbSubmittedLimit = 0;

            mockedDb.Setup(x => x.FindArchivialFilesToRestoreBySource(It.IsAny<SourceLocation>(), It.IsAny<int>()))
                .ReturnsAsync(new BackupFileSearchResults())
                .Callback<SourceLocation, int>((x, y) =>
                {
                    dbSubmittedSource = x;
                    dbSubmittedLimit = y;
                });

            var mockedCoreSettings = new Mock<ICoreSettings>();

            var depedencies = new CmdletDependencies()
            {
                ClientDatabase = mockedDb.Object,
                CoreSettings = mockedCoreSettings.Object
            };

            var command = new FindArchivialFilesToRestoreCommand(depedencies)
            {
                Source = new LocalSourceLocation() { ID = 3 },
                LimitResults = 10
            };

            var result = command.Invoke().GetEnumerator().MoveNext();

            mockedDb.Verify(x => x.FindArchivialFilesToRestoreBySource(It.IsAny<SourceLocation>(), It.IsAny<int>()), Times.Once);

            Assert.IsNotNull(dbSubmittedSource);
            Assert.AreEqual(3, dbSubmittedSource.ID);
            Assert.AreEqual(10, dbSubmittedLimit);
        }

        [TestMethod]
        public void FindArchivialFilesToRestoreCommand_CanQueryByNetworkSource()
        {
            var mockedDb = new Mock<IClientDatabase>();

            SourceLocation dbSubmittedSource = null;
            int dbSubmittedLimit = 0;

            mockedDb.Setup(x => x.FindArchivialFilesToRestoreBySource(It.IsAny<SourceLocation>(), It.IsAny<int>()))
                .ReturnsAsync(new BackupFileSearchResults())
                .Callback<SourceLocation, int>((x, y) =>
                {
                    dbSubmittedSource = x;
                    dbSubmittedLimit = y;
                });

            var mockedCoreSettings = new Mock<ICoreSettings>();

            var depedencies = new CmdletDependencies()
            {
                ClientDatabase = mockedDb.Object,
                CoreSettings = mockedCoreSettings.Object
            };

            var command = new FindArchivialFilesToRestoreCommand(depedencies)
            {
                Source = new NetworkSourceLocation() { ID = 5 },
                LimitResults = 10
            };

            var result = command.Invoke().GetEnumerator().MoveNext();

            mockedDb.Verify(x => x.FindArchivialFilesToRestoreBySource(It.IsAny<SourceLocation>(), It.IsAny<int>()), Times.Once);

            Assert.IsNotNull(dbSubmittedSource);
            Assert.AreEqual(5, dbSubmittedSource.ID);
            Assert.AreEqual(10, dbSubmittedLimit);
        }

        [TestMethod]
        public void FindArchivialFilesToRestoreCommand_CanQueryAllFiles()
        {
            var mockedDb = new Mock<IClientDatabase>();

            mockedDb.Setup(x => x.FindAllArchivialFilesToRestore()).ReturnsAsync(new BackupFileSearchResults());

            var mockedCoreSettings = new Mock<ICoreSettings>();

            var depedencies = new CmdletDependencies()
            {
                ClientDatabase = mockedDb.Object,
                CoreSettings = mockedCoreSettings.Object
            };

            var command = new FindArchivialFilesToRestoreCommand(depedencies)
            {
                All = true
            };

            var result = command.Invoke().GetEnumerator().MoveNext();

            mockedDb.Verify(x => x.FindAllArchivialFilesToRestore(), Times.Once);
        }
    }
}
