using OzettePowerShell.Exceptions;
using OzettePowerShell.Functions.Private;
using OzettePowerShell.Utility;
using System.Management.Automation;

namespace OzettePowerShell.Functions.Public
{
    [Cmdlet(VerbsLifecycle.Uninstall, "OzetteCloudBackup", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true)]
    public class UninstallOzetteCloudBackupCommand : BaseOzetteCmdlet
    {
        public UninstallOzetteCloudBackupCommand()
        {
            ActivityName = "Uninstallation";
            ActivityID = 2;
        }

        protected override void ProcessRecord()
        {
            if (!Elevation.IsRunningElevated())
            {
                throw new CmdletNotElevatedException("This cmdlet requires elevated (run-as administrator) privileges. Please re-launch the cmdlet in an elevated window.");
            }

            WriteVerbose("Removing Ozette Cloud Backup installation.");

            var db = GetDatabaseConnection();

            WriteVerboseAndProgress(10, "Stopping OzetteCloudBackup Windows Service.");
            Uninstallation.StopClientService();

            WriteVerboseAndProgress(40, "Removing Ozette client database.");
            db.DeleteClientDatabaseAsync().GetAwaiter().GetResult();

            WriteVerboseAndProgress(55, "Removing OzetteCloudBackup Windows Service.");
            Uninstallation.DeleteClientService();

            WriteVerboseAndProgress(70, "Removing installation files and folders.");
            Uninstallation.DeleteInstallationDirectories();

            WriteVerboseAndProgress(80, "Removing custom event log source.");
            Uninstallation.DeleteEventLogSource();

            WriteVerboseAndProgress(90, "Removing core settings.");
            Uninstallation.DeleteCoreSettings();

            WriteVerboseAndProgress(100, "Uninstallation completed.");
        }
    }
}
