using ArchivialLibrary.Database;
using ArchivialLibrary.ServiceCore;
using ArchivialPowerShell.Functions.Public;
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

            var command = new GetArchivialCloudBackupStatusCommand(mockedDb.Object, null, null);

            foreach (var result in command.Invoke())
            {
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(BackupProgress));
            }
        }
    }
}
