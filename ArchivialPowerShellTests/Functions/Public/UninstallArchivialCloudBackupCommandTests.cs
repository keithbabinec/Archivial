using ArchivialLibrary.Database;
using ArchivialPowerShell.Exceptions;
using ArchivialPowerShell.Functions.Public;
using ArchivialPowerShell.Setup;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace ArchivialPowerShellTests.Functions.Public
{
    [TestClass]
    public class UninstallArchivialCloudBackupCommandTests
    {
        [TestMethod]
        public void UninstallArchivialCloudBackupCommand_RunsCompleteUninstall()
        {
            // setup 

            var mockedDb = new Mock<IClientDatabase>();
            mockedDb.Setup(x => x.DeleteClientDatabaseAsync()).Returns(Task.CompletedTask);

            var mockedSetup = new Mock<ISetup>();
            mockedSetup.Setup(x => x.IsRunningElevated()).Returns(true);

            var command = new UninstallArchivialCloudBackupCommand(mockedDb.Object, null, mockedSetup.Object);
            command.Force = true;

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();

            // verify

            mockedSetup.Verify(x => x.StopClientService(), Times.Once);
            mockedDb.Verify(x => x.DeleteClientDatabaseAsync(), Times.Once);
            mockedSetup.Verify(x => x.DeleteClientService(), Times.Once);
            mockedSetup.Verify(x => x.DeleteInstallationDirectories(), Times.Once);
            mockedSetup.Verify(x => x.DeleteEventLogContents(), Times.Once);
            mockedSetup.Verify(x => x.DeleteCoreSettings(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(CmdletNotElevatedException))]
        public void UninstallArchivialCloudBackupCommand_Throws_IfUserIsNotElevated()
        {
            // setup 

            var mockedSetup = new Mock<ISetup>();

            mockedSetup.Setup(x => x.IsRunningElevated()).Returns(false);

            var command = new UninstallArchivialCloudBackupCommand(null, null, mockedSetup.Object);

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();
        }
    }
}
