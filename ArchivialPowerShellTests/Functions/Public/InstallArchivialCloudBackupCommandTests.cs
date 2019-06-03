using ArchivialLibrary.ServiceCore;
using ArchivialPowerShell.Exceptions;
using ArchivialPowerShell.Functions.Public;
using ArchivialPowerShell.Setup;
using ArchivialPowerShell.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using System.Management.Automation;

namespace ArchivialPowerShellTests.Functions.Public
{
    [TestClass]
    public class InstallArchivialCloudBackupCommandTests
    {
        [TestMethod]
        public void InstallArchivialCloudBackupCommand_InstallDirectory_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(InstallArchivialCloudBackupCommand),
                    nameof(InstallArchivialCloudBackupCommand.InstallDirectory),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(InstallArchivialCloudBackupCommand),
                    nameof(InstallArchivialCloudBackupCommand.InstallDirectory),
                    typeof(ValidateNotNullOrEmptyAttribute))
            );
        }

        [TestMethod]
        public void InstallArchivialCloudBackupCommand_RunsCompleteInstall()
        {
            // setup 

            var mockedSetup = new Mock<ISetup>();

            mockedSetup.Setup(x => x.IsRunningElevated()).Returns(true);
            mockedSetup.Setup(x => x.SqlServerPrerequisiteIsAvailable()).Returns(true);

            var mockedCoreSettings = new Mock<ICoreSettings>();

            var depedencies = new CmdletDependencies()
            {
                Setup = mockedSetup.Object,
                CoreSettings = mockedCoreSettings.Object
            };

            var command = new InstallArchivialCloudBackupCommand(depedencies);
            command.Force = true;

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();

            // verify

            var defaultWindowsInstallFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Archivial Cloud Backup");

            mockedSetup.Verify(x => x.CreateCoreSettings(It.Is<string>(z => z == defaultWindowsInstallFolder)), Times.Once);
            mockedSetup.Verify(x => x.CreateEventLogSource(), Times.Once);
            mockedSetup.Verify(x => x.CreateInstallationDirectories(), Times.Once);
            mockedSetup.Verify(x => x.CopyProgramFiles(), Times.Once);
            mockedSetup.Verify(x => x.CreateClientService(), Times.Once);
            mockedSetup.Verify(x => x.StartClientService(), Times.Once);
            mockedSetup.Verify(x => x.WaitForFirstTimeSetup(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(CmdletNotElevatedException))]
        public void InstallArchivialCloudBackupCommand_Throws_IfUserIsNotElevated()
        {
            // setup 

            var mockedSetup = new Mock<ISetup>();

            mockedSetup.Setup(x => x.IsRunningElevated()).Returns(false);
            mockedSetup.Setup(x => x.SqlServerPrerequisiteIsAvailable()).Returns(true);

            var mockedCoreSettings = new Mock<ICoreSettings>();

            var depedencies = new CmdletDependencies()
            {
                Setup = mockedSetup.Object,
                CoreSettings = mockedCoreSettings.Object
            };

            var command = new InstallArchivialCloudBackupCommand(depedencies);

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();
        }

        [TestMethod]
        [ExpectedException(typeof(CmdletPrerequisiteNotFoundException))]
        public void InstallArchivialCloudBackupCommand_Throws_IfSqlPrerequisiteIsMissing()
        {
            // setup 

            var mockedSetup = new Mock<ISetup>();

            mockedSetup.Setup(x => x.IsRunningElevated()).Returns(true);
            mockedSetup.Setup(x => x.SqlServerPrerequisiteIsAvailable()).Returns(false);

            var mockedCoreSettings = new Mock<ICoreSettings>();

            var depedencies = new CmdletDependencies()
            {
                Setup = mockedSetup.Object,
                CoreSettings = mockedCoreSettings.Object
            };

            var command = new InstallArchivialCloudBackupCommand(depedencies);

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();
        }

        [TestMethod]
        [ExpectedException(typeof(CmdletExecutionFailedDamagedProductInstallationException))]
        public void InstallArchivialCloudBackupCommand_Throws_IfProductIsDamaged()
        {
            // setup 

            var mockedSetup = new Mock<ISetup>();
            mockedSetup.Setup(x => x.IsRunningElevated()).Returns(true);
            mockedSetup.Setup(x => x.SqlServerPrerequisiteIsAvailable()).Returns(true);
            mockedSetup.Setup(x => x.GetInstalledVersionAsync()).ThrowsAsync(new CmdletExecutionFailedDamagedProductInstallationException());

            var mockedCoreSettings = new Mock<ICoreSettings>();

            var depedencies = new CmdletDependencies()
            {
                Setup = mockedSetup.Object,
                CoreSettings = mockedCoreSettings.Object
            };

            var command = new InstallArchivialCloudBackupCommand(depedencies);
            command.Force = true;

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();
        }

        [TestMethod]
        [ExpectedException(typeof(CmdletExecutionFailedProductAlreadyInstalledException))]
        public void InstallArchivialCloudBackupCommand_Throws_IfProductIsAlreadyInstalled()
        {
            // setup 

            var mockedSetup = new Mock<ISetup>();
            mockedSetup.Setup(x => x.IsRunningElevated()).Returns(true);
            mockedSetup.Setup(x => x.SqlServerPrerequisiteIsAvailable()).Returns(true);
            mockedSetup.Setup(x => x.GetInstalledVersionAsync()).ReturnsAsync(new Version(1,0,0,0));

            var mockedCoreSettings = new Mock<ICoreSettings>();

            var depedencies = new CmdletDependencies()
            {
                Setup = mockedSetup.Object,
                CoreSettings = mockedCoreSettings.Object
            };

            var command = new InstallArchivialCloudBackupCommand(depedencies);
            command.Force = true;

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();
        }
    }
}
