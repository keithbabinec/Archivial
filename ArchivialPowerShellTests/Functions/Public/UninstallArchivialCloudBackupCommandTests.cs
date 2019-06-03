using ArchivialLibrary.Database;
using ArchivialLibrary.ServiceCore;
using ArchivialPowerShell.Exceptions;
using ArchivialPowerShell.Functions.Public;
using ArchivialPowerShell.Setup;
using ArchivialPowerShell.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
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
            mockedSetup.Setup(x => x.GetInstalledVersionAsync()).ReturnsAsync(new Version(1, 0, 0, 0));

            var mockedCoreSettings = new Mock<ICoreSettings>();

            var depedencies = new CmdletDependencies()
            {
                ClientDatabase = mockedDb.Object,
                Setup = mockedSetup.Object,
                CoreSettings = mockedCoreSettings.Object
            };

            var command = new UninstallArchivialCloudBackupCommand(depedencies);
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
        public void UninstallArchivialCloudBackupCommand_DoesNotUninstall_IfProductIsntInstalled()
        {
            // setup 

            var mockedDb = new Mock<IClientDatabase>();
            mockedDb.Setup(x => x.DeleteClientDatabaseAsync()).Returns(Task.CompletedTask);

            var mockedSetup = new Mock<ISetup>();
            mockedSetup.Setup(x => x.IsRunningElevated()).Returns(true);
            mockedSetup.Setup(x => x.GetInstalledVersionAsync()).ReturnsAsync((Version)null);

            var mockedCoreSettings = new Mock<ICoreSettings>();

            var depedencies = new CmdletDependencies()
            {
                ClientDatabase = mockedDb.Object,
                Setup = mockedSetup.Object,
                CoreSettings = mockedCoreSettings.Object
            };

            var command = new UninstallArchivialCloudBackupCommand(depedencies);
            command.Force = true;

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();

            // verify

            mockedSetup.Verify(x => x.StopClientService(), Times.Never);
            mockedDb.Verify(x => x.DeleteClientDatabaseAsync(), Times.Never);
            mockedSetup.Verify(x => x.DeleteClientService(), Times.Never);
            mockedSetup.Verify(x => x.DeleteInstallationDirectories(), Times.Never);
            mockedSetup.Verify(x => x.DeleteEventLogContents(), Times.Never);
            mockedSetup.Verify(x => x.DeleteCoreSettings(), Times.Never);
        }

        [TestMethod]
        [ExpectedException(typeof(CmdletExecutionFailedDamagedProductInstallationException))]
        public void UninstallArchivialCloudBackupCommand_Throws_IfProductIsDamaged()
        {
            // setup 

            var mockedDb = new Mock<IClientDatabase>();
            mockedDb.Setup(x => x.DeleteClientDatabaseAsync()).Returns(Task.CompletedTask);

            var mockedSetup = new Mock<ISetup>();
            mockedSetup.Setup(x => x.IsRunningElevated()).Returns(true);
            mockedSetup.Setup(x => x.GetInstalledVersionAsync()).ThrowsAsync(new CmdletExecutionFailedDamagedProductInstallationException());

            var mockedCoreSettings = new Mock<ICoreSettings>();

            var depedencies = new CmdletDependencies()
            {
                ClientDatabase = mockedDb.Object,
                Setup = mockedSetup.Object,
                CoreSettings = mockedCoreSettings.Object
            };

            var command = new UninstallArchivialCloudBackupCommand(depedencies);
            command.Force = true;

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();
        }

        [TestMethod]
        [ExpectedException(typeof(CmdletNotElevatedException))]
        public void UninstallArchivialCloudBackupCommand_Throws_IfUserIsNotElevated()
        {
            // setup 

            var mockedSetup = new Mock<ISetup>();

            mockedSetup.Setup(x => x.IsRunningElevated()).Returns(false);
            mockedSetup.Setup(x => x.GetInstalledVersionAsync()).ReturnsAsync(new Version(1,0,0,0));

            var mockedCoreSettings = new Mock<ICoreSettings>();

            var depedencies = new CmdletDependencies()
            {
                Setup = mockedSetup.Object,
                CoreSettings = mockedCoreSettings.Object
            };

            var command = new UninstallArchivialCloudBackupCommand(depedencies);

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();
        }
    }
}
