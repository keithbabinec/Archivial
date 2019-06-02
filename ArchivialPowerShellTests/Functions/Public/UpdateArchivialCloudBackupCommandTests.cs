using ArchivialPowerShell.Exceptions;
using ArchivialPowerShell.Functions.Public;
using ArchivialPowerShell.Setup;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace ArchivialPowerShellTests.Functions.Public
{
    [TestClass]
    public class UpdateArchivialCloudBackupCommandTests
    {
        [TestMethod]
        public void UpdateArchivialCloudBackupCommand_RunsCompleteUpdate()
        {
            // setup 

            var mockedSetup = new Mock<ISetup>();

            mockedSetup.Setup(x => x.IsRunningElevated()).Returns(true);
            mockedSetup.Setup(x => x.GetInstalledVersionAsync()).ReturnsAsync(new Version(1,0,0,0));
            mockedSetup.Setup(x => x.GetPowerShellModuleVersion()).Returns(new Version(1,0,0,1));

            var command = new UpdateArchivialCloudBackupCommand(null, null, mockedSetup.Object);
            command.Force = true;

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();

            // verify

            mockedSetup.Verify(x => x.StopClientService(), Times.Once);
            mockedSetup.Verify(x => x.CopyProgramFiles(), Times.Once);
            mockedSetup.Verify(x => x.SetDatabasePublishRequiredCoreOption(), Times.Once);
            mockedSetup.Verify(x => x.StartClientService(), Times.Once);
            mockedSetup.Verify(x => x.WaitForFirstTimeSetup(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(CmdletNotElevatedException))]
        public void UpdateArchivialCloudBackupCommand_Throws_IfUserIsNotElevated()
        {
            // setup 

            var mockedSetup = new Mock<ISetup>();

            mockedSetup.Setup(x => x.IsRunningElevated()).Returns(false);

            var command = new UpdateArchivialCloudBackupCommand(null, null, mockedSetup.Object);

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();
        }

        [TestMethod]
        [ExpectedException(typeof(CmdletExecutionFailedProductNotInstalledException))]
        public void UpdateArchivialCloudBackupCommand_Throws_IfProductIsNotInstalled()
        {
            // setup 

            var mockedSetup = new Mock<ISetup>();

            mockedSetup.Setup(x => x.IsRunningElevated()).Returns(true);
            mockedSetup.Setup(x => x.GetInstalledVersionAsync()).ReturnsAsync((Version)null);

            var command = new UpdateArchivialCloudBackupCommand(null, null, mockedSetup.Object);

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();
        }

        [TestMethod]
        [ExpectedException(typeof(CmdletExecutionFailedCannotDowngradeSoftwareException))]
        public void UpdateArchivialCloudBackupCommand_Throws_IfProductWillBeDowngraded()
        {
            // setup 

            var mockedSetup = new Mock<ISetup>();

            mockedSetup.Setup(x => x.IsRunningElevated()).Returns(true);
            mockedSetup.Setup(x => x.GetInstalledVersionAsync()).ReturnsAsync(new Version(1,0,0,1));
            mockedSetup.Setup(x => x.GetPowerShellModuleVersion()).Returns(new Version(1,0,0,0));

            var command = new UpdateArchivialCloudBackupCommand(null, null, mockedSetup.Object);

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();
        }

        [TestMethod]
        public void UpdateArchivialCloudBackupCommand_Returns_IfSoftwareIsAlreadyUpToDate()
        {
            // setup 

            var mockedSetup = new Mock<ISetup>();

            mockedSetup.Setup(x => x.IsRunningElevated()).Returns(true);
            mockedSetup.Setup(x => x.GetInstalledVersionAsync()).ReturnsAsync(new Version(1, 0, 0, 0));
            mockedSetup.Setup(x => x.GetPowerShellModuleVersion()).Returns(new Version(1, 0, 0, 0));

            var command = new UpdateArchivialCloudBackupCommand(null, null, mockedSetup.Object);

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();

            // verify that none of our update steps were executed.

            mockedSetup.Verify(x => x.StopClientService(), Times.Never);
            mockedSetup.Verify(x => x.CopyProgramFiles(), Times.Never);
            mockedSetup.Verify(x => x.SetDatabasePublishRequiredCoreOption(), Times.Never);
            mockedSetup.Verify(x => x.StartClientService(), Times.Never);
            mockedSetup.Verify(x => x.WaitForFirstTimeSetup(), Times.Never);
        }
    }
}
