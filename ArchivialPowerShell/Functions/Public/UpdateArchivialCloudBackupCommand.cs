using ArchivialPowerShell.Exceptions;
using ArchivialPowerShell.Utility;
using System.Management.Automation;

namespace ArchivialPowerShell.Functions.Public
{
    /// <summary>
    ///   <para type="synopsis">Updates the Archivial Cloud Backup software on this computer.</para>
    ///   <para type="description">IMPORTANT: The Archivial version that will be installed with this command is tied to the version number of this module. For example if this module is Archivial PowerShell version v1.0.0, then running this command will attempt to upgrade your current installation to v1.0.0.</para>
    ///   <para type="description">To ensure you upgrade using the latest software, always update this PowerShell module (then restart PowerShell) before running this upgrade command. See the examples for more details.</para>
    ///   <para type="description">This command requires an elevated (run-as administrator) PowerShell prompt to complete. It will also prompt for comfirmation unless the -Force switch is applied.</para>
    /// </summary>
    /// <example>
    ///   <code>C:\> $latestVersion = (Find-Module -Name ArchivialPowerShell).Version</code>
    ///   <para>C:\> Update-Module -Name ArchivialPowerShell -RequiredVersion $latestVersion</para>
    ///   <para>The two above commands will update your Archivial PowerShell module to latest. After that has completed, close and restart the PowerShell window.</para>
    ///   <para>C:\> Update-ArchivialCloudBackup</para>
    ///   <para>With the latest management tools installed, this command updates your installation.</para>
    ///   <para></para>
    /// </example>
    [Cmdlet(VerbsData.Update, "ArchivialCloudBackup", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true)]
    public class UpdateArchivialCloudBackupCommand : BaseArchivialCmdlet
    {
        /// <summary>
        ///   <para type="description">Suppresses the confirmation prompt.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Force = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UpdateArchivialCloudBackupCommand() : base()
        {
            ActivityName = "Update";
            ActivityID = 3;
        }

        /// <summary>
        /// Secondary constructor for dependency injection.
        /// </summary>
        /// <param name="dependencies"></param>
        public UpdateArchivialCloudBackupCommand(CmdletDependencies dependencies) : base(dependencies)
        {
            ActivityName = "Update";
            ActivityID = 3;
        }

        protected override void ProcessRecord()
        {
            var setup = GetSetupHelper();

            WriteVerbose("Checking prerequisites to see if we can perform the software update.");

            if (!setup.IsRunningElevated())
            {
                throw new CmdletNotElevatedException("This cmdlet requires elevated (run-as administrator) privileges. Please re-launch the cmdlet in an elevated window.");
            }
            if (!Force && !ShouldProcess("Updating Archivial Cloud Backup Software", "Are you sure that you would like to update the Archivial Cloud Backup software?", "Update Archivial Cloud Backup"))
            {
                throw new CmdletExecutionNotApprovedException("This action must be approved (or provide the -force switch) to run.");
            }

            var installedVersion = setup.GetInstalledVersionAsync().GetAwaiter().GetResult();

            if (installedVersion == null)
            {
                throw new CmdletExecutionFailedProductNotInstalledException("Unable to update Archivial Cloud Backup. The product is not installed. Please use the 'Install-ArchivialCloudBackup' command if you need to perform a fresh installation.");
            }

            var psModuleVersion = setup.GetPowerShellModuleVersion();

            if (psModuleVersion == installedVersion)
            {
                WriteWarning(string.Format("Archivial Cloud Backup software is already up to date (version {0}), there is nothing to update.", installedVersion.ToString()));
                return;
            }
            if (psModuleVersion < installedVersion)
            {
                throw new CmdletExecutionFailedCannotDowngradeSoftwareException(
                    string.Format("Unable to update Archivial Cloud Backup. The version tied to this module (version {0}) is older than your current installation (version {1}), which would lead to a downgrade.", psModuleVersion.ToString(), installedVersion.ToString()));
            }

            WriteVerbose("Starting Archivial Cloud Backup software update.");

            // stop services, wait until exit
            WriteVerboseAndProgress(10, "Stopping ArchivialCloudBackup windows service.");
            setup.StopClientService();

            // copy bin files
            WriteVerboseAndProgress(30, "Copying updated program files to the installation directory.");
            setup.CopyProgramFiles();

            // set db publish flag
            WriteVerboseAndProgress(50, "Updating setup/publish options.");
            setup.SetDatabasePublishRequiredCoreOption();

            // start services
            WriteVerboseAndProgress(60, "Starting ArchivialCloudBackup windows service.");
            setup.StartClientService();

            // wait for ready
            WriteVerboseAndProgress(80, "Waiting for service startup to complete.");
            setup.WaitForFirstTimeSetup();

            WriteVerboseAndProgress(100, "Software update completed.");
        }
    }
}
