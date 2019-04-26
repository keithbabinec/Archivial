using ArchivialLibrary.Constants;
using ArchivialLibrary.Database;
using ArchivialLibrary.ServiceCore;
using ArchivialPowerShell.Functions.Public;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ArchivialPowerShellTests.Functions.Public
{
    [TestClass]
    public class GetArchivialCloudBackupOptionsCommandTests
    {
        [TestMethod]
        public void GetArchivialCloudBackupStatusCommand_CanReturnBackupStatus()
        {
            var mockedDb = new Mock<IClientDatabase>();

            mockedDb.Setup(x => x.GetApplicationOptionAsync(It.Is<string>(z => z == RuntimeSettingNames.LogFilesRetentionInDays))).ReturnsAsync("30");
            mockedDb.Setup(x => x.GetApplicationOptionAsync(It.Is<string>(z => z == RuntimeSettingNames.DatabaseBackupsRetentionInDays))).ReturnsAsync("7");
            mockedDb.Setup(x => x.GetApplicationOptionAsync(It.Is<string>(z => z == RuntimeSettingNames.BackupEngineInstancesCount))).ReturnsAsync("4");
            mockedDb.Setup(x => x.GetApplicationOptionAsync(It.Is<string>(z => z == RuntimeSettingNames.LowPriorityScanFrequencyInHours))).ReturnsAsync("24");
            mockedDb.Setup(x => x.GetApplicationOptionAsync(It.Is<string>(z => z == RuntimeSettingNames.MedPriorityScanFrequencyInHours))).ReturnsAsync("12");
            mockedDb.Setup(x => x.GetApplicationOptionAsync(It.Is<string>(z => z == RuntimeSettingNames.HighPriorityScanFrequencyInHours))).ReturnsAsync("1");
            mockedDb.Setup(x => x.GetApplicationOptionAsync(It.Is<string>(z => z == RuntimeSettingNames.ProtectionIV))).ReturnsAsync("protectionkey");
            mockedDb.Setup(x => x.GetApplicationOptionAsync(It.Is<string>(z => z == RuntimeSettingNames.StatusUpdateSchedule))).ReturnsAsync("*/5 * * * *");
            mockedDb.Setup(x => x.GetApplicationOptionAsync(It.Is<string>(z => z == RuntimeSettingNames.MasterExclusionMatches))).ReturnsAsync("^._;.DS_Store");

            var command = new GetArchivialCloudBackupOptionsCommand(mockedDb.Object);

            foreach (var result in command.Invoke())
            {
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(ApplicationOptionsResult));

                var options = (ApplicationOptionsResult)result;

                Assert.AreEqual(30, options.LogFilesRetentionInDays);
                Assert.AreEqual(7, options.DatabaseBackupsRetentionInDays);
                Assert.AreEqual(4, options.BackupEngineInstancesCount);
                Assert.AreEqual(24, options.LowPriorityScanFrequencyInHours);
                Assert.AreEqual(12, options.MedPriorityScanFrequencyInHours);
                Assert.AreEqual(1, options.HighPriorityScanFrequencyInHours);
                Assert.AreEqual("protectionkey", options.ProtectionIV);
                Assert.AreEqual("*/5 * * * *", options.StatusUpdateSchedule);

                Assert.AreEqual(2, options.MasterExclusionMatches.Length);
                Assert.AreEqual("^._", options.MasterExclusionMatches[0]);
                Assert.AreEqual(".DS_Store", options.MasterExclusionMatches[1]);
            }
        }
    }
}
