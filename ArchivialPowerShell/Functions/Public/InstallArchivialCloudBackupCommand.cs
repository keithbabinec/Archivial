using ArchivialPowerShell.Exceptions;
using ArchivialPowerShell.Utility;
using System;
using System.IO;
using System.Management.Automation;

namespace ArchivialPowerShell.Functions.Public
{
    /// <summary>
    ///   <para type="synopsis">Installs the Archivial Cloud Backup software on this computer.</para>
    ///   <para type="description">Installs the Archivial Cloud Backup software on this computer. The default installation will be placed in the Program Files directory, but this can optionally be changed by specifying the -InstallDirectory parameter.</para>
    ///   <para type="description">This command requires an elevated (run-as administrator) PowerShell prompt to complete. It will also prompt for comfirmation unless the -Force switch is applied.</para>
    ///   <para type="description">IMPORTANT: SQL Server is required to maintain the state database. You can use any edition, but here are links to the two recommended free versions:</para>
    ///   <para type="description">1) SQL Server Express Edition is available for free from Microsoft at https://www.microsoft.com/en-us/sql-server/sql-server-editions-express. </para>
    ///   <para type="description">2) SQL Server Express with Advanced Services Edition is also free under the Visual Studio Dev Essentials program (https://visualstudio.microsoft.com/dev-essentials/). This version features full-text search, which Archivial is capable of leveraging for faster search on very large databases.</para>
    ///   <para type="description">NOTE: This command is used for fresh installations. For upgrades to existing installations use the Update-ArchivialCloudBackup command.</para>
    /// </summary>
    /// <example>
    ///   <code>C:\> Install-ArchivialCloudBackup</code>
    ///   <para>Starts the installation with default options. The user will be prompted for confirmation.</para>
    ///   <para></para>
    /// </example>
    /// <example>
    ///   <code>C:\> Install-ArchivialCloudBackup -InstallDirectory "D:\Applications\Archivial Cloud Backup" -Force</code>
    ///   <para>Starts the installation to the custom directory and suppresses the confirmation prompt.</para>
    ///   <para></para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Install, "ArchivialCloudBackup", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true)]
    public class InstallArchivialCloudBackupCommand : BaseArchivialCmdlet
    {
        /// <summary>
        ///   <para type="description">Specify a custom installation directory, otherwise the default Program Files location will be used.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        [ValidateNotNullOrEmpty]
        public string InstallDirectory { get; set; }

        /// <summary>
        ///   <para type="description">Suppresses the confirmation prompt.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Force = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public InstallArchivialCloudBackupCommand() : base()
        {
            ActivityName = "Installation";
            ActivityID = 1;
        }

        /// <summary>
        /// Secondary constructor for dependency injection.
        /// </summary>
        /// <param name="dependencies"></param>
        public InstallArchivialCloudBackupCommand(CmdletDependencies dependencies) : base(dependencies)
        {
            ActivityName = "Installation";
            ActivityID = 1;
        }

        protected override void ProcessRecord()
        {
            var setup = GetSetupHelper();
            var coreSettings = GetCoreSettingsAccessor();

            if (!setup.IsRunningElevated())
            {
                throw new CmdletNotElevatedException("This cmdlet requires elevated (run-as administrator) privileges. Please re-launch the cmdlet in an elevated window.");
            }
            if (!Force && !ShouldProcess("Installing Archivial Cloud Backup Software", "Are you sure that you would like to install the Archivial Cloud Backup software?", "Install Archivial Cloud Backup"))
            {
                throw new CmdletExecutionNotApprovedException("This action must be approved (or provide the -force switch) to run.");
            }
            if (!setup.SqlServerPrerequisiteIsAvailable())
            {
                throw new CmdletPrerequisiteNotFoundException("Unable to install Archivial Cloud Backup. A required prerequisite (SQL Server Express) was not found.");
            }

            var installedVersion = setup.GetInstalledVersionAsync().GetAwaiter().GetResult();

            if (installedVersion != null)
            {
                throw new CmdletExecutionFailedProductAlreadyInstalledException("Unable to install Archivial Cloud Backup. The product is already installed. Please use the 'Update-ArchivialCloudBackup' command if you need to update your installation.");
            }

            if (InstallDirectory == null)
            {
                InstallDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Archivial Cloud Backup");
            }

            WriteVerbose("Starting Archivial Cloud Backup installation.");

            WriteVerboseAndProgress(10, "Applying core settings.");
            setup.CreateCoreSettings(InstallDirectory);

            WriteVerbose("InstallationDirectory=" + coreSettings.GetInstallationDirectory());
            WriteVerbose("EventlogName=" + coreSettings.GetEventlogName());
            WriteVerbose("DatabaseConnectionString=" + coreSettings.GetDatabaseConnectionString());

            WriteVerboseAndProgress(25, "Registering custom event log source.");
            setup.CreateEventLogSource();

            WriteVerboseAndProgress(40, "Creating installation directories.");
            setup.CreateInstallationDirectories();

            WriteVerboseAndProgress(55, "Copying program files to the installation directory.");
            setup.CopyProgramFiles();

            WriteVerboseAndProgress(70, "Configuring ArchivialCloudBackup Windows Service.");
            setup.CreateClientService();

            WriteVerboseAndProgress(85, "Starting ArchivialCloudBackup Windows Service.");
            setup.StartClientService();

            WriteVerboseAndProgress(90, "Waiting for ArchivialCloudBackup Windows Service to finish initializing.");
            setup.WaitForFirstTimeSetup();

            WriteVerboseAndProgress(100, "Installation completed.");
        }
    }
}
