using OzettePowerShell.Exceptions;
using OzettePowerShell.Utility;
using System.Management.Automation;

namespace OzettePowerShell.Functions.Public
{
    /// <summary>
    ///   <para type="synopsis">Uninstalls the Ozette Cloud Backup software from this computer.</para>
    ///   <para type="description">Uninstalls the Ozette Cloud Backup software from this computer. This will permenantly delete the installation folder, state database, and log files. This action is not reversable.</para>
    ///   <para type="description">Although all local installation data is deleted, any of the data already backed up to a cloud provider will not be removed. You must remove that manually if you wish to delete that data.</para>
    ///   <para type="description">This command requires an elevated (run-as administrator) PowerShell prompt to complete. It will also prompt for comfirmation unless the -Force switch is applied.</para>
    /// </summary>
    /// <example>
    ///   <code>C:\> Uninstall-OzetteCloudBackup</code>
    ///   <para>Starts the uninstallation process. The user will be prompted for confirmation.</para>
    ///   <para></para>
    /// </example>
    /// <example>
    ///   <code>C:\> Uninstall-OzetteCloudBackup -Force</code>
    ///   <para>Starts the uninstallation and suppresses the confirmation prompt.</para>
    ///   <para></para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Uninstall, "OzetteCloudBackup", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true)]
    public class UninstallOzetteCloudBackupCommand : BaseOzetteCmdlet
    {
        /// <summary>
        ///   <para type="description">Suppresses the confirmation prompt.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Force = false;

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
            if (!Force && !ShouldProcess("Uninstalling Ozette Cloud Backup Software", "Are you sure that you would like to uninstall this software? It will permenantly delete the installation folder, state database, and log files. This action is not reversable.", "Uninstall Ozette Cloud Backup"))
            {
                throw new CmdletExecutionNotApprovedException("This action must be approved (or provide the -force switch) to run.");
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
            Uninstallation.DeleteEventLogContents();

            WriteVerboseAndProgress(90, "Removing core settings.");
            Uninstallation.DeleteCoreSettings();

            WriteVerboseAndProgress(100, "Uninstallation completed.");
        }
    }
}
