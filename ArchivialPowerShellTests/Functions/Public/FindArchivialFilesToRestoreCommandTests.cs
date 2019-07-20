using ArchivialPowerShell.Functions.Public;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}
