using ArchivialLibrary.Database;
using ArchivialLibrary.Secrets;
using ArchivialPowerShell.Exceptions;
using ArchivialPowerShell.Setup;
using ArchivialPowerShell.Utility;
using System.Management.Automation;

namespace ArchivialPowerShell.Functions.Public
{
    /// <summary>
    ///   <para type="synopsis">Uninstalls the Archivial Cloud Backup software from this computer.</para>
    ///   <para type="description">Uninstalls the Archivial Cloud Backup software from this computer. This will permenantly delete the installation folder, state database, and log files. This action is not reversable.</para>
    ///   <para type="description">Although all local installation data is deleted, any of the data already backed up to a cloud provider will not be removed. You must remove that manually if you wish to delete that data.</para>
    ///   <para type="description">This command requires an elevated (run-as administrator) PowerShell prompt to complete. It will also prompt for comfirmation unless the -Force switch is applied.</para>
    /// </summary>
    /// <example>
    ///   <code>C:\> Uninstall-ArchivialCloudBackup</code>
    ///   <para>Starts the uninstallation process. The user will be prompted for confirmation.</para>
    ///   <para></para>
    /// </example>
    /// <example>
    ///   <code>C:\> Uninstall-ArchivialCloudBackup -Force</code>
    ///   <para>Starts the uninstallation and suppresses the confirmation prompt.</para>
    ///   <para></para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Uninstall, "ArchivialCloudBackup", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true)]
    public class UninstallArchivialCloudBackupCommand : BaseArchivialCmdlet
    {
        /// <summary>
        ///   <para type="description">Suppresses the confirmation prompt.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Force = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UninstallArchivialCloudBackupCommand() : base()
        {
            ActivityName = "Uninstallation";
            ActivityID = 2;
        }

        /// <summary>
        /// A secondary constructor for dependency injection.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="setup"></param>
        /// <param name="secretStore"></param>
        public UninstallArchivialCloudBackupCommand(IClientDatabase database, ISecretStore secretStore, ISetup setup) : base(database, secretStore, setup)
        {
            ActivityName = "Uninstallation";
            ActivityID = 2;
        }

        protected override void ProcessRecord()
        {
            var setup = GetSetupHelper();

            if (!setup.IsRunningElevated())
            {
                throw new CmdletNotElevatedException("This cmdlet requires elevated (run-as administrator) privileges. Please re-launch the cmdlet in an elevated window.");
            }
            if (!Force && !ShouldProcess("Uninstalling Archivial Cloud Backup Software", "Are you sure that you would like to uninstall this software? It will permenantly delete the installation folder, state database, and log files. This action is not reversable.", "Uninstall Archivial Cloud Backup"))
            {
                throw new CmdletExecutionNotApprovedException("This action must be approved (or provide the -force switch) to run.");
            }

            WriteVerbose("Removing Archivial Cloud Backup installation.");

            var db = GetDatabaseConnection();

            WriteVerboseAndProgress(10, "Stopping ArchivialCloudBackup Windows Service.");
            setup.StopClientService();

            WriteVerboseAndProgress(40, "Removing Archivial client database.");
            db.DeleteClientDatabaseAsync().GetAwaiter().GetResult();

            WriteVerboseAndProgress(55, "Removing ArchivialCloudBackup Windows Service.");
            setup.DeleteClientService();

            WriteVerboseAndProgress(70, "Removing installation files and folders.");
            setup.DeleteInstallationDirectories();

            WriteVerboseAndProgress(80, "Removing custom event log source.");
            setup.DeleteEventLogContents();

            WriteVerboseAndProgress(90, "Removing core settings.");
            setup.DeleteCoreSettings();

            WriteVerboseAndProgress(100, "Uninstallation completed.");
        }
    }
}
