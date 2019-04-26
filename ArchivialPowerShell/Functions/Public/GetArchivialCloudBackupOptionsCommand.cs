using ArchivialLibrary.Database;
using ArchivialLibrary.ServiceCore;
using ArchivialPowerShell.Utility;
using System;
using System.Management.Automation;

namespace ArchivialPowerShell.Functions.Public
{
    /// <summary>
    ///   <para type="synopsis">Gets the application options for Archivial Cloud Backup.</para>
    ///   <para type="description">Gets the application options for Archivial Cloud Backup. To change existing options, run Set-ArchivialCloudBackupOptions</para>
    /// </summary>
    /// <example>
    ///   <code>C:\> Get-ArchivialCloudBackupOptions</code>
    ///   <para>Returns the application options for Archivial Cloud Backup.</para>
    ///   <para></para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "ArchivialCloudBackupOptions")]
    public class GetArchivialCloudBackupOptionsCommand : BaseArchivialCmdlet
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public GetArchivialCloudBackupOptionsCommand() : base() { }

        /// <summary>
        /// A secondary constructor for dependency injection.
        /// </summary>
        /// <param name="database"></param>
        public GetArchivialCloudBackupOptionsCommand(IClientDatabase database) : base(database) { }

        protected override void ProcessRecord()
        {
            var db = GetDatabaseConnection();

            var result = new ApplicationOptionsResult();

            WriteVerbose("Querying application setting in the database: LogFilesRetentionInDays");
            var logRetention = ArchivialLibrary.Constants.RuntimeSettingNames.LogFilesRetentionInDays;
            result.LogFilesRetentionInDays = Convert.ToInt32(db.GetApplicationOptionAsync(logRetention).GetAwaiter().GetResult());
            
            WriteVerbose("Querying application setting in the database: DatabaseBackupsRetentionInDays");
            var dbRetention = ArchivialLibrary.Constants.RuntimeSettingNames.DatabaseBackupsRetentionInDays;
            result.DatabaseBackupsRetentionInDays = Convert.ToInt32(db.GetApplicationOptionAsync(dbRetention).GetAwaiter().GetResult());

            WriteVerbose("Querying application setting in the database: BackupEngineInstancesCount");
            var engineCount = ArchivialLibrary.Constants.RuntimeSettingNames.BackupEngineInstancesCount;
            result.BackupEngineInstancesCount = Convert.ToInt32(db.GetApplicationOptionAsync(engineCount).GetAwaiter().GetResult());

            WriteVerbose("Querying application setting in the database: LowPriorityScanFrequencyInHours");
            var lowPri = ArchivialLibrary.Constants.RuntimeSettingNames.LowPriorityScanFrequencyInHours;
            result.LowPriorityScanFrequencyInHours = Convert.ToInt32(db.GetApplicationOptionAsync(lowPri).GetAwaiter().GetResult());

            WriteVerbose("Querying application setting in the database: MedPriorityScanFrequencyInHours");
            var medPri = ArchivialLibrary.Constants.RuntimeSettingNames.MedPriorityScanFrequencyInHours;
            result.MedPriorityScanFrequencyInHours = Convert.ToInt32(db.GetApplicationOptionAsync(medPri).GetAwaiter().GetResult());

            WriteVerbose("Querying application setting in the database: HighPriorityScanFrequencyInHours");
            var highPri = ArchivialLibrary.Constants.RuntimeSettingNames.HighPriorityScanFrequencyInHours;
            result.HighPriorityScanFrequencyInHours = Convert.ToInt32(db.GetApplicationOptionAsync(highPri).GetAwaiter().GetResult());

            WriteVerbose("Querying application setting in the database: ProtectionIV");
            var iv = ArchivialLibrary.Constants.RuntimeSettingNames.ProtectionIV;
            result.ProtectionIV = db.GetApplicationOptionAsync(iv).GetAwaiter().GetResult();

            WriteVerbose("Querying application setting in the database: StatusUpdateSchedule");
            var statusUpdate = ArchivialLibrary.Constants.RuntimeSettingNames.StatusUpdateSchedule;
            result.StatusUpdateSchedule = db.GetApplicationOptionAsync(statusUpdate).GetAwaiter().GetResult();

            WriteVerbose("Querying application setting in the database: MasterExclusionMatches");
            var exclusionSettingName = ArchivialLibrary.Constants.RuntimeSettingNames.MasterExclusionMatches;
            var exclusionValue = db.GetApplicationOptionAsync(exclusionSettingName).GetAwaiter().GetResult();

            if (exclusionValue.Length == 0)
            {
                result.MasterExclusionMatches = null;
            }
            else
            {
                result.MasterExclusionMatches = exclusionValue.Split(';');
            }

            WriteObject(result);
        }
    }
}
