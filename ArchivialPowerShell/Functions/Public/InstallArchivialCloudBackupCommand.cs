using ArchivialLibrary.Database;
using ArchivialLibrary.ServiceCore;
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

        protected override void ProcessRecord()
        {
            if (!Elevation.IsRunningElevated())
            {
                throw new CmdletNotElevatedException("This cmdlet requires elevated (run-as administrator) privileges. Please re-launch the cmdlet in an elevated window.");
            }
            if (!Force && !ShouldProcess("Installing Archivial Cloud Backup Software", "Are you sure that you would like to install the Archivial Cloud Backup software?", "Install Archivial Cloud Backup"))
            {
                throw new CmdletExecutionNotApprovedException("This action must be approved (or provide the -force switch) to run.");
            }
            if (InstallDirectory == null)
            {
                InstallDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Archivial Cloud Backup");
            }

            WriteVerbose("Starting Archivial Cloud Backup installation.");

            WriteVerboseAndProgress(10, "Applying core settings.");
            Installation.CreateCoreSettings(InstallDirectory);

            WriteVerbose("InstallationDirectory=" + CoreSettings.InstallationDirectory);
            WriteVerbose("EventlogName=" + CoreSettings.EventlogName);
            WriteVerbose("DatabaseConnectionString=" + CoreSettings.DatabaseConnectionString);

            WriteVerboseAndProgress(25, "Registering custom event log source.");
            Installation.CreateEventLogSource();

            WriteVerboseAndProgress(40, "Creating installation directories.");
            Installation.CreateInstallationDirectories();

            WriteVerboseAndProgress(55, "Copying program files to the installation directory.");
            Installation.CopyProgramFiles();

            WriteVerboseAndProgress(70, "Configuring ArchivialCloudBackup Windows Service.");
            Installation.CreateClientService();

            WriteVerboseAndProgress(85, "Starting ArchivialCloudBackup Windows Service.");
            Installation.StartClientService();

            WriteVerboseAndProgress(90, "Waiting for ArchivialCloudBackup Windows Service to finish initializing.");
            Installation.WaitForFirstTimeSetup();

            WriteVerboseAndProgress(100, "Installation completed.");
        }
    }
}
