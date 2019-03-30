using OzetteLibrary.ServiceCore;
using OzettePowerShell.Exceptions;
using OzettePowerShell.Functions.Private;
using OzettePowerShell.Utility;
using System.Management.Automation;

namespace OzettePowerShell.Functions.Public
{
    [Cmdlet(VerbsLifecycle.Install, "OzetteCloudBackup")]
    public class InstallOzetteCloudBackupCommand : BaseOzetteCmdlet
    {
        [Parameter(Mandatory = false)]
        [ValidateNotNullOrEmpty]
        public string InstallDirectory = OzetteLibrary.Constants.CommandLine.DefaultInstallLocation;

        public InstallOzetteCloudBackupCommand()
        {
            ActivityName = "Installation";
            ActivityID = 1;
        }

        protected override void ProcessRecord()
        {
            if (!Elevation.IsRunningElevated())
            {
                throw new CmdletNotElevatedException("This cmdlet requires elevated (run-as administrator) privileges. Please re-launch the cmdlet in an elevated window.");
            }

            WriteVerbose("Starting Ozette Cloud Backup installation.");

            WriteVerboseAndProgress(10, "Applying core settings.");
            Installation.CreateCoreSettings(InstallDirectory);

            WriteVerbose("InstallationDirectory=" + CoreSettings.InstallationDirectory);
            WriteVerbose("LogFilesDirectory=" + CoreSettings.LogFilesDirectory);
            WriteVerbose("LogFilesArchiveDirectory=" + CoreSettings.LogFilesArchiveDirectory);
            WriteVerbose("LogFilesRetentionInDays=" + CoreSettings.LogFilesRetentionInDays);
            WriteVerbose("DatabaseDirectory=" + CoreSettings.DatabaseDirectory);
            WriteVerbose("DatabaseBackupsDirectory=" + CoreSettings.DatabaseBackupsDirectory);
            WriteVerbose("DatabaseBackupsRetentionInDays=" + CoreSettings.DatabaseBackupsRetentionInDays);
            WriteVerbose("EventlogName=" + CoreSettings.EventlogName);
            WriteVerbose("DatabaseConnectionString=" + CoreSettings.DatabaseConnectionString);
            WriteVerbose("BackupEngineInstancesCount=" + CoreSettings.BackupEngineInstanceCount);

            WriteVerboseAndProgress(25, "Registering custom event log source.");
            Installation.CreateEventLogSource();

            WriteVerboseAndProgress(40, "Creating installation directories.");
            Installation.CreateInstallationDirectories();

            WriteVerboseAndProgress(55, "Copying program files to the installation directory.");
            Installation.CopyProgramFiles();

            WriteVerboseAndProgress(70, "Configuring OzetteCloudBackup Windows Service.");
            Installation.CreateClientService();

            WriteVerboseAndProgress(85, "Starting OzetteCloudBackup Windows Service.");
            Installation.StartClientService();

            WriteVerboseAndProgress(100, "Installation completed.");
        }
    }
}
