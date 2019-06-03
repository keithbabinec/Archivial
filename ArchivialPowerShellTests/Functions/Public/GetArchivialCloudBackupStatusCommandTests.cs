using ArchivialLibrary.Database;
using ArchivialLibrary.ServiceCore;
using ArchivialPowerShell.Functions.Public;
using ArchivialPowerShell.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ArchivialPowerShellTests.Functions.Public
{
    [TestClass]
    public class GetArchivialCloudBackupStatusCommandTests
    {
        [TestMethod]
        public void GetArchivialCloudBackupStatusCommand_CanReturnBackupStatus()
        {
            var mockedDb = new Mock<IClientDatabase>();

            mockedDb.Setup(x => x.GetBackupProgressAsync())
                .ReturnsAsync(new BackupProgress());

            var mockedCoreSettings = new Mock<ICoreSettings>();

            var depedencies = new CmdletDependencies()
            {
                ClientDatabase = mockedDb.Object,
                CoreSettings = mockedCoreSettings.Object
            };

            var command = new GetArchivialCloudBackupStatusCommand(depedencies);

            foreach (var result in command.Invoke())
            {
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(BackupProgress));
            }
        }
    }
}
