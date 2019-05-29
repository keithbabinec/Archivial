using ArchivialLibrary.Constants;
using ArchivialLibrary.Database;
using ArchivialPowerShell.Functions.Public;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Management.Automation;

namespace ArchivialPowerShellTests.Functions.Public
{
    [TestClass]
    public class SetArchivialCloudBackupOptionsCommandTests
    {
        [TestMethod]
        public void SetArchivialCloudBackupOptionsCommand_LogFilesRetentionInDays_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialCloudBackupOptionsCommand),
                    nameof(SetArchivialCloudBackupOptionsCommand.LogFilesRetentionInDays),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialCloudBackupOptionsCommand),
                    nameof(SetArchivialCloudBackupOptionsCommand.LogFilesRetentionInDays),
                    typeof(ValidateRangeAttribute))
            );
        }

        [TestMethod]
        public void SetArchivialCloudBackupOptionsCommand_DatabaseBackupsRetentionInDays_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialCloudBackupOptionsCommand),
                    nameof(SetArchivialCloudBackupOptionsCommand.DatabaseBackupsRetentionInDays),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialCloudBackupOptionsCommand),
                    nameof(SetArchivialCloudBackupOptionsCommand.DatabaseBackupsRetentionInDays),
                    typeof(ValidateRangeAttribute))
            );
        }

        [TestMethod]
        public void SetArchivialCloudBackupOptionsCommand_BackupEngineInstancesCount_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialCloudBackupOptionsCommand),
                    nameof(SetArchivialCloudBackupOptionsCommand.BackupEngineInstancesCount),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialCloudBackupOptionsCommand),
                    nameof(SetArchivialCloudBackupOptionsCommand.BackupEngineInstancesCount),
                    typeof(ValidateRangeAttribute))
            );
        }

        [TestMethod]
        public void SetArchivialCloudBackupOptionsCommand_BackupEngineStartupDelayInSeconds_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialCloudBackupOptionsCommand),
                    nameof(SetArchivialCloudBackupOptionsCommand.BackupEngineStartupDelayInSeconds),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialCloudBackupOptionsCommand),
                    nameof(SetArchivialCloudBackupOptionsCommand.BackupEngineStartupDelayInSeconds),
                    typeof(ValidateRangeAttribute))
            );
        }

        [TestMethod]
        public void SetArchivialCloudBackupOptionsCommand_LowPriorityScanFrequencyInHours_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialCloudBackupOptionsCommand),
                    nameof(SetArchivialCloudBackupOptionsCommand.LowPriorityScanFrequencyInHours),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialCloudBackupOptionsCommand),
                    nameof(SetArchivialCloudBackupOptionsCommand.LowPriorityScanFrequencyInHours),
                    typeof(ValidateRangeAttribute))
            );
        }

        [TestMethod]
        public void SetArchivialCloudBackupOptionsCommand_MedPriorityScanFrequencyInHours_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialCloudBackupOptionsCommand),
                    nameof(SetArchivialCloudBackupOptionsCommand.MedPriorityScanFrequencyInHours),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialCloudBackupOptionsCommand),
                    nameof(SetArchivialCloudBackupOptionsCommand.MedPriorityScanFrequencyInHours),
                    typeof(ValidateRangeAttribute))
            );
        }

        [TestMethod]
        public void SetArchivialCloudBackupOptionsCommand_HighPriorityScanFrequencyInHours_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialCloudBackupOptionsCommand),
                    nameof(SetArchivialCloudBackupOptionsCommand.HighPriorityScanFrequencyInHours),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialCloudBackupOptionsCommand),
                    nameof(SetArchivialCloudBackupOptionsCommand.HighPriorityScanFrequencyInHours),
                    typeof(ValidateRangeAttribute))
            );
        }

        [TestMethod]
        public void SetArchivialCloudBackupOptionsCommand_ProtectionIV_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialCloudBackupOptionsCommand),
                    nameof(SetArchivialCloudBackupOptionsCommand.ProtectionIV),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialCloudBackupOptionsCommand),
                    nameof(SetArchivialCloudBackupOptionsCommand.ProtectionIV),
                    typeof(ValidateNotNullOrEmptyAttribute))
            );
        }

        [TestMethod]
        public void SetArchivialCloudBackupOptionsCommand_StatusUpdateSchedule_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialCloudBackupOptionsCommand),
                    nameof(SetArchivialCloudBackupOptionsCommand.StatusUpdateSchedule),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialCloudBackupOptionsCommand),
                    nameof(SetArchivialCloudBackupOptionsCommand.StatusUpdateSchedule),
                    typeof(ValidateNotNullOrEmptyAttribute))
            );
        }

        [TestMethod]
        public void SetArchivialCloudBackupOptionsCommand_MasterExclusionMatches_HasRequiredAttributes()
        {
            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialCloudBackupOptionsCommand),
                    nameof(SetArchivialCloudBackupOptionsCommand.MasterExclusionMatches),
                    typeof(ParameterAttribute))
            );

            Assert.IsTrue(
                TypeHelpers.CmdletParameterHasAttribute(
                    typeof(SetArchivialCloudBackupOptionsCommand),
                    nameof(SetArchivialCloudBackupOptionsCommand.MasterExclusionMatches),
                    typeof(ValidateNotNullOrEmptyAttribute))
            );
        }

        [TestMethod]
        public void SetArchivialCloudBackupOptionsCommand_DoesNotSetAnyOptions_WhenNoOptionsAreProvided()
        {
            // setup 

            var mockedDb = new Mock<IClientDatabase>();

            var command = new SetArchivialCloudBackupOptionsCommand(mockedDb.Object);

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();

            // verify

            mockedDb.Verify(x => x.SetApplicationOptionAsync(
                It.Is<string>(z => z == RuntimeSettingNames.LogFilesRetentionInDays), It.IsAny<string>()), Times.Never);

            mockedDb.Verify(x => x.SetApplicationOptionAsync(
                It.Is<string>(z => z == RuntimeSettingNames.DatabaseBackupsRetentionInDays), It.IsAny<string>()), Times.Never);

            mockedDb.Verify(x => x.SetApplicationOptionAsync(
                It.Is<string>(z => z == RuntimeSettingNames.BackupEngineInstancesCount), It.IsAny<string>()), Times.Never);

            mockedDb.Verify(x => x.SetApplicationOptionAsync(
                It.Is<string>(z => z == RuntimeSettingNames.BackupEngineStartupDelayInSeconds), It.IsAny<string>()), Times.Never);
            
            mockedDb.Verify(x => x.SetApplicationOptionAsync(
                It.Is<string>(z => z == RuntimeSettingNames.LowPriorityScanFrequencyInHours), It.IsAny<string>()), Times.Never);

            mockedDb.Verify(x => x.SetApplicationOptionAsync(
                It.Is<string>(z => z == RuntimeSettingNames.MedPriorityScanFrequencyInHours), It.IsAny<string>()), Times.Never);

            mockedDb.Verify(x => x.SetApplicationOptionAsync(
                It.Is<string>(z => z == RuntimeSettingNames.HighPriorityScanFrequencyInHours), It.IsAny<string>()), Times.Never);

            mockedDb.Verify(x => x.SetApplicationOptionAsync(
                It.Is<string>(z => z == RuntimeSettingNames.ProtectionIV), It.IsAny<string>()), Times.Never);

            mockedDb.Verify(x => x.SetApplicationOptionAsync(
                It.Is<string>(z => z == RuntimeSettingNames.StatusUpdateSchedule), It.IsAny<string>()), Times.Never);

            mockedDb.Verify(x => x.SetApplicationOptionAsync(
                It.Is<string>(z => z == RuntimeSettingNames.MasterExclusionMatches), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void SetArchivialCloudBackupOptionsCommand_SetsLogFileRetention_WhenOptionIsProvided()
        {
            // setup 

            var mockedDb = new Mock<IClientDatabase>();

            var command = new SetArchivialCloudBackupOptionsCommand(mockedDb.Object);
            command.LogFilesRetentionInDays = 5;

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();

            // verify

            mockedDb.Verify(x => x.SetApplicationOptionAsync(
                It.Is<string>(z => z == RuntimeSettingNames.LogFilesRetentionInDays), It.Is<string>(z => z == "5")), Times.Once);         
        }

        [TestMethod]
        public void SetArchivialCloudBackupOptionsCommand_SetsDatabaseBackupsRetention_WhenOptionIsProvided()
        {
            // setup 

            var mockedDb = new Mock<IClientDatabase>();

            var command = new SetArchivialCloudBackupOptionsCommand(mockedDb.Object);
            command.DatabaseBackupsRetentionInDays = 7;

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();

            // verify

            mockedDb.Verify(x => x.SetApplicationOptionAsync(
                It.Is<string>(z => z == RuntimeSettingNames.DatabaseBackupsRetentionInDays), It.Is<string>(z => z == "7")), Times.Once);
        }

        [TestMethod]
        public void SetArchivialCloudBackupOptionsCommand_SetsEngineInstanceCount_WhenOptionIsProvided()
        {
            // setup 

            var mockedDb = new Mock<IClientDatabase>();

            var command = new SetArchivialCloudBackupOptionsCommand(mockedDb.Object);
            command.BackupEngineInstancesCount = 8;

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();

            // verify

            mockedDb.Verify(x => x.SetApplicationOptionAsync(
                It.Is<string>(z => z == RuntimeSettingNames.BackupEngineInstancesCount), It.Is<string>(z => z == "8")), Times.Once);
        }

        [TestMethod]
        public void SetArchivialCloudBackupOptionsCommand_SetsEngineStartupDelayInSeconds_WhenOptionIsProvided()
        {
            // setup 

            var mockedDb = new Mock<IClientDatabase>();

            var command = new SetArchivialCloudBackupOptionsCommand(mockedDb.Object);
            command.BackupEngineStartupDelayInSeconds = 60;

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();

            // verify

            mockedDb.Verify(x => x.SetApplicationOptionAsync(
                It.Is<string>(z => z == RuntimeSettingNames.BackupEngineStartupDelayInSeconds), It.Is<string>(z => z == "60")), Times.Once);
        }

        [TestMethod]
        public void SetArchivialCloudBackupOptionsCommand_SetsLowPriScanFrequency_WhenOptionIsProvided()
        {
            // setup 

            var mockedDb = new Mock<IClientDatabase>();

            var command = new SetArchivialCloudBackupOptionsCommand(mockedDb.Object);
            command.LowPriorityScanFrequencyInHours = 24;

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();

            // verify

            mockedDb.Verify(x => x.SetApplicationOptionAsync(
                It.Is<string>(z => z == RuntimeSettingNames.LowPriorityScanFrequencyInHours), It.Is<string>(z => z == "24")), Times.Once);
        }

        [TestMethod]
        public void SetArchivialCloudBackupOptionsCommand_SetsMedPriScanFrequency_WhenOptionIsProvided()
        {
            // setup 

            var mockedDb = new Mock<IClientDatabase>();

            var command = new SetArchivialCloudBackupOptionsCommand(mockedDb.Object);
            command.MedPriorityScanFrequencyInHours = 12;

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();

            // verify

            mockedDb.Verify(x => x.SetApplicationOptionAsync(
                It.Is<string>(z => z == RuntimeSettingNames.MedPriorityScanFrequencyInHours), It.Is<string>(z => z == "12")), Times.Once);
        }

        [TestMethod]
        public void SetArchivialCloudBackupOptionsCommand_SetsHighPriScanFrequency_WhenOptionIsProvided()
        {
            // setup 

            var mockedDb = new Mock<IClientDatabase>();

            var command = new SetArchivialCloudBackupOptionsCommand(mockedDb.Object);
            command.HighPriorityScanFrequencyInHours = 2;

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();

            // verify

            mockedDb.Verify(x => x.SetApplicationOptionAsync(
                It.Is<string>(z => z == RuntimeSettingNames.HighPriorityScanFrequencyInHours), It.Is<string>(z => z == "2")), Times.Once);
        }

        [TestMethod]
        public void SetArchivialCloudBackupOptionsCommand_SetsProtectionIv_WhenOptionIsProvided()
        {
            // setup 

            var mockedDb = new Mock<IClientDatabase>();

            var command = new SetArchivialCloudBackupOptionsCommand(mockedDb.Object);
            command.ProtectionIV = "key";

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();

            // verify

            mockedDb.Verify(x => x.SetApplicationOptionAsync(
                It.Is<string>(z => z == RuntimeSettingNames.ProtectionIV), It.Is<string>(z => z == "key")), Times.Once);
        }

        [TestMethod]
        public void SetArchivialCloudBackupOptionsCommand_SetsStatusUpdateSchedule_WhenOptionIsProvided()
        {
            // setup 

            var mockedDb = new Mock<IClientDatabase>();

            var command = new SetArchivialCloudBackupOptionsCommand(mockedDb.Object);
            command.StatusUpdateSchedule = "0 8 * * *";

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();

            // verify

            mockedDb.Verify(x => x.SetApplicationOptionAsync(
                It.Is<string>(z => z == RuntimeSettingNames.StatusUpdateSchedule), It.Is<string>(z => z == "0 8 * * *")), Times.Once);
        }

        [TestMethod]
        public void SetArchivialCloudBackupOptionsCommand_SetsMasterExclusions_WhenOptionIsProvided()
        {
            // setup 

            var mockedDb = new Mock<IClientDatabase>();

            var command = new SetArchivialCloudBackupOptionsCommand(mockedDb.Object);
            command.MasterExclusionMatches = new string[] { "^._", ".DS_Store" };

            // execute

            var result = command.Invoke().GetEnumerator().MoveNext();

            // verify

            mockedDb.Verify(x => x.SetApplicationOptionAsync(
                It.Is<string>(z => z == RuntimeSettingNames.MasterExclusionMatches), It.Is<string>(z => z == "^._;.DS_Store")), Times.Once);
        }
    }
}
