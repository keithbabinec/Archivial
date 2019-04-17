using OzettePowerShell.Utility;
using System.Management.Automation;
using System.Text;

namespace OzettePowerShell.Functions.Public
{
    /// <summary>
    ///   <para type="synopsis">Sets one or more application options for Ozette Cloud Backup.</para>
    ///   <para type="description">Specify at least one option to change. To see existing options, run Get-OzetteCloudBackupOptions</para>
    /// </summary>
    /// <example>
    ///   <code>C:\> Set-OzetteCloudBackupOptions -LogFilesRetentionInDays 60</code>
    ///   <para>Sets the local log file retention to 60 days.</para>
    ///   <para></para>
    /// </example>
    /// <example>
    ///   <code>C:\> Set-OzetteCloudBackupOptions -LowPriorityScanFrequencyInHours 4 -MedPriorityScanFrequencyInHours 2</code>
    ///   <para>Sets low priority and medium priority scanning frequencies.</para>
    ///   <para></para>
    /// </example>
    /// <example>
    ///   <code>C:\> Set-OzetteCloudBackupOptions -StatusUpdateSchedule "0 8 * * *"</code>
    ///   <para>Sets the status update notification schedule to once per day at 8am.</para>
    ///   <para></para>
    /// </example>
    /// <example>
    ///   <code>C:\> Set-OzetteCloudBackupOptions -MasterExclusionMatches "^._",".DS_Store"</code>
    ///   <para>Sets the master exclusions to exlude Mac OS related file system metadata files.</para>
    ///   <para></para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "OzetteCloudBackupOptions")]
    public class SetOzetteCloudBackupOptionsCommand : BaseOzetteCmdlet
    {
        /// <summary>
        ///   <para type="description">Optionally specify the length of time (in days) that locally stored log files should be retained.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int LogFilesRetentionInDays { get; set; }

        /// <summary>
        ///   <para type="description">Optionally specify the length of time (in days) that locally stored database backups should be retrained.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int DatabaseBackupsRetentionInDays { get; set; }

        /// <summary>
        ///   <para type="description">Optionally specify the number of concurrent backup engine instances to run.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int BackupEngineInstancesCount { get; set; }

        /// <summary>
        ///   <para type="description">Optionally specify how often (in hours) that low-priority folder scans should be performed.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int LowPriorityScanFrequencyInHours { get; set; }

        /// <summary>
        ///   <para type="description">Optionally specify how often (in hours) that medium-priority folder scans should be performed.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int MedPriorityScanFrequencyInHours { get; set; }

        /// <summary>
        ///   <para type="description">Optionally specify how often (in hours) that high-priority folder scans should be performed.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int HighPriorityScanFrequencyInHours { get; set; }

        /// <summary>
        ///   <para type="description">Optionally specify the local encryption initialization vector key. Key should be a crypto-random 16-byte array, expressed as a base64 encoded string.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string ProtectionIV { get; set; }

        /// <summary>
        ///   <para type="description">Optionally specify the schedule for when status notification messages should be sent. Expressed as a cron schedule.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string StatusUpdateSchedule { get; set; }

        /// <summary>
        ///   <para type="description">Optionally specify a set of regex match exclusions that should apply to all scanned folders.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string[] MasterExclusionMatches { get; set; }

        protected override void ProcessRecord()
        {
            var db = GetDatabaseConnection();

            if (LogFilesRetentionInDays > 0)
            {
                WriteVerbose("Updating application setting in the database: LogFilesRetentionInDays");

                var settingName = OzetteLibrary.Constants.RuntimeSettingNames.LogFilesRetentionInDays;
                db.SetApplicationOptionAsync(settingName, LogFilesRetentionInDays.ToString()).GetAwaiter().GetResult();
            }

            if (DatabaseBackupsRetentionInDays > 0)
            {
                WriteVerbose("Updating application setting in the database: DatabaseBackupsRetentionInDays");

                var settingName = OzetteLibrary.Constants.RuntimeSettingNames.DatabaseBackupsRetentionInDays;
                db.SetApplicationOptionAsync(settingName, DatabaseBackupsRetentionInDays.ToString()).GetAwaiter().GetResult();
            }

            if (BackupEngineInstancesCount > 0)
            {
                WriteVerbose("Updating application setting in the database: BackupEngineInstancesCount");

                var settingName = OzetteLibrary.Constants.RuntimeSettingNames.BackupEngineInstancesCount;
                db.SetApplicationOptionAsync(settingName, BackupEngineInstancesCount.ToString()).GetAwaiter().GetResult();
            }

            if (LowPriorityScanFrequencyInHours > 0)
            {
                WriteVerbose("Updating application setting in the database: LowPriorityScanFrequencyInHours");

                var settingName = OzetteLibrary.Constants.RuntimeSettingNames.LowPriorityScanFrequencyInHours;
                db.SetApplicationOptionAsync(settingName, LowPriorityScanFrequencyInHours.ToString()).GetAwaiter().GetResult();
            }

            if (MedPriorityScanFrequencyInHours > 0)
            {
                WriteVerbose("Updating application setting in the database: MedPriorityScanFrequencyInHours");

                var settingName = OzetteLibrary.Constants.RuntimeSettingNames.MedPriorityScanFrequencyInHours;
                db.SetApplicationOptionAsync(settingName, MedPriorityScanFrequencyInHours.ToString()).GetAwaiter().GetResult();
            }

            if (HighPriorityScanFrequencyInHours > 0)
            {
                WriteVerbose("Updating application setting in the database: HighPriorityScanFrequencyInHours");

                var settingName = OzetteLibrary.Constants.RuntimeSettingNames.HighPriorityScanFrequencyInHours;
                db.SetApplicationOptionAsync(settingName, HighPriorityScanFrequencyInHours.ToString()).GetAwaiter().GetResult();
            }

            if (!string.IsNullOrEmpty(ProtectionIV))
            {
                WriteVerbose("Updating application setting in the database: ProtectionIV");

                var settingName = OzetteLibrary.Constants.RuntimeSettingNames.ProtectionIV;
                db.SetApplicationOptionAsync(settingName, ProtectionIV).GetAwaiter().GetResult();
            }

            if (!string.IsNullOrEmpty(StatusUpdateSchedule))
            {
                WriteVerbose("Updating application setting in the database: StatusUpdateSchedule");

                var settingName = OzetteLibrary.Constants.RuntimeSettingNames.StatusUpdateSchedule;
                db.SetApplicationOptionAsync(settingName, StatusUpdateSchedule).GetAwaiter().GetResult();
            }

            if (MasterExclusionMatches.Length > 0)
            {
                WriteVerbose("Updating application setting in the database: MasterExclusionMatches");

                var settingName = OzetteLibrary.Constants.RuntimeSettingNames.MasterExclusionMatches;

                StringBuilder sb = new StringBuilder();

                foreach (var match in MasterExclusionMatches)
                {
                    if (!string.IsNullOrEmpty(match))
                    {
                        sb.Append(string.Format("{0};", match));
                    }
                }

                db.SetApplicationOptionAsync(settingName, StatusUpdateSchedule).GetAwaiter().GetResult();
            }
        }
    }
}
