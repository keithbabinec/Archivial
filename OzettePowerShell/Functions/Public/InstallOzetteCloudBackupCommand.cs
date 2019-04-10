using OzetteLibrary.ServiceCore;
using OzettePowerShell.Exceptions;
using OzettePowerShell.Utility;
using System;
using System.IO;
using System.Management.Automation;

namespace OzettePowerShell.Functions.Public
{
    /// <summary>
    ///   <para type="synopsis">Installs the Ozette Cloud Backup software on this computer.</para>
    ///   <para type="description">Installs the Ozette Cloud Backup software on this computer. The default installation will be placed in the Program Files directory, but this can optionally be changed by specifying the -InstallDirectory parameter.</para>
    ///   <para type="description">This command requires an elevated (run-as administrator) PowerShell prompt to complete. It will also prompt for comfirmation unless the -Force switch is applied.</para>
    /// </summary>
    /// <example>
    ///   <code>C:\> Install-OzetteCloudBackup</code>
    ///   <para>Starts the installation with default options. The user will be prompted for confirmation.</para>
    ///   <para></para>
    /// </example>
    /// <example>
    ///   <code>C:\> Install-OzetteCloudBackup -InstallDirectory "D:\Applications\Ozette Cloud Backup" -Force</code>
    ///   <para>Starts the installation to the custom directory and suppresses the confirmation prompt.</para>
    ///   <para></para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Install, "OzetteCloudBackup", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true)]
    public class InstallOzetteCloudBackupCommand : BaseOzetteCmdlet
    {
        /// <summary>
        ///   <para type="description">Specify a custom installation directory, otherwise the default Program Files location will be used.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        [ValidateNotNullOrEmpty]
        public string InstallDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Ozette Cloud Backup");

        /// <summary>
        ///   <para type="description">Suppresses the confirmation prompt.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Force = false;

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
            if (!Force && !ShouldProcess("Installing Ozette Cloud Backup Software", "Are you sure that you would like to install the Ozette Cloud Backup software?", "Install Ozette Cloud Backup"))
            {
                throw new CmdletExecutionNotApprovedException("This action must be approved (or provide the -force switch) to run.");
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

            WriteVerboseAndProgress(90, "Waiting for OzetteCloudBackup Windows Service to finish initializing.");
            Installation.WaitForFirstTimeSetup();

            WriteVerboseAndProgress(100, "Installation completed.");
        }
    }
}
