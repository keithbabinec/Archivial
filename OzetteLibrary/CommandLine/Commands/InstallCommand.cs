using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading.Tasks;
using OzetteLibrary.CommandLine.Arguments;
using OzetteLibrary.Logging.Default;
using OzetteLibrary.ServiceCore;

namespace OzetteLibrary.CommandLine.Commands
{
    /// <summary>
    /// A command for installing the program.
    /// </summary>
    /// <remarks>
    /// The reason for elevation requirement: This command must have admin permissions to create the service, write program files, configure event sources, etc.
    /// </remarks>
    [RequiresElevation]
    public class InstallCommand : ICommand
    {
        /// <summary>
        /// A logging helper instance.
        /// </summary>
        private Logger Logger;

        /// <summary>
        /// Constructor that requires a logging instance.
        /// </summary>
        /// <param name="logger"></param>
        public InstallCommand(Logger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Logger = logger;
        }

        /// <summary>
        /// Runs the install command.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns>True if successful, otherwise false.</returns>
        public async Task<bool> RunAsync(ArgumentBase arguments)
        {
            var installArgs = arguments as InstallationArguments;

            if (installArgs == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            try
            {
                Logger.WriteConsole("--- Starting Ozette Cloud Backup installation");

                Logger.WriteConsole("--- Step 1: Applying core settings.");
                CreateCoreSettings(installArgs);

                Logger.WriteConsole("--- Step 2: Creating custom event log source.");
                CreateEventLogSource();

                Logger.WriteConsole("--- Step 3: Setting up installation directories.");
                CreateInstallationDirectories();

                Logger.WriteConsole("--- Step 4: Copying installation files.");
                CopyProgramFiles();

                Logger.WriteConsole("--- Step 5: Creating Ozette Client Service.");
                CreateClientService();

                Logger.WriteConsole("--- Step 6: Add installation directory to system path variable.");
                AddSystemPath();

                Logger.WriteConsole("--- Installation completed successfully.");

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteConsole("--- Ozette Cloud Backup installation failed", EventLogEntryType.Error);
                Logger.WriteConsole(ex.ToString(), EventLogEntryType.Error);
                return false;
            }
        }

        /// <summary>
        /// Sets the core application settings.
        /// </summary>
        /// <param name="arguments"></param>
        private void CreateCoreSettings(InstallationArguments arguments)
        {
            // set the core settings.

            CoreSettings.InstallationDirectory = arguments.InstallDirectory;
            CoreSettings.LogFilesDirectory = Path.Combine(arguments.InstallDirectory, "Logs");
            CoreSettings.DatabaseDirectory = Path.Combine(arguments.InstallDirectory, "Database");
            CoreSettings.EventlogName = "Ozette";
            CoreSettings.BackupEngineInstanceCount = 1;

            // setting this flag indicates publish is required on next service startup.
            CoreSettings.DatabasePublishIsRequired = true;

            var dbConnectionString = string.Format("Data Source=.\\SQLExpress;Initial Catalog={0};Integrated Security=SSPI;", Constants.Database.DatabaseName);
            CoreSettings.DatabaseConnectionString = dbConnectionString;

            // this entropy/iv key is used only for saving/retrieving app secrets (like storage config tokens).
            // it is not used for encrypting files in the cloud.
            // the iv key must be 16 bytes.
            var encryptionIvBytes = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(encryptionIvBytes);
            var encryptionIvString = Convert.ToBase64String(encryptionIvBytes);
            CoreSettings.ProtectionIv = encryptionIvString;

            Logger.WriteConsole("Core settings successfully applied.");
            Logger.WriteConsole("InstallationDirectory=" + CoreSettings.InstallationDirectory);
            Logger.WriteConsole("LogFilesDirectory=" + CoreSettings.LogFilesDirectory);
            Logger.WriteConsole("DatabaseDirectory=" + CoreSettings.DatabaseDirectory);
            Logger.WriteConsole("EventlogName=" + CoreSettings.EventlogName);
            Logger.WriteConsole("DatabaseConnectionString=" + CoreSettings.DatabaseConnectionString);
            Logger.WriteConsole("BackupEngineInstancesCount=" + CoreSettings.BackupEngineInstanceCount);
        }

        /// <summary>
        /// Creates a custom event log and event source.
        /// </summary>
        private void CreateEventLogSource()
        {
            if (EventLog.Exists(CoreSettings.EventlogName) == false)
            {
                Logger.WriteConsole("Event log custom source was not found, creating it now.");
                EventLog.CreateEventSource(CoreSettings.EventlogName, CoreSettings.EventlogName);
                Logger.WriteConsole("Successfully created custom event source.");
            }
            else
            {
                Logger.WriteConsole("Event log custom source already exists, skipping step.");
            }
        }

        /// <summary>
        /// Creates the installation directories.
        /// </summary>
        private void CreateInstallationDirectories()
        {
            if (Directory.Exists(CoreSettings.InstallationDirectory) == false)
            {
                Logger.WriteConsole("Target installation directory was not found, creating it now.");
                Directory.CreateDirectory(CoreSettings.InstallationDirectory);
                Logger.WriteConsole("Successfully created target installation directory.");
            }
            else
            {
                Logger.WriteConsole("Target installation directory already exists, skipping step.");
            }

            if (Directory.Exists(CoreSettings.DatabaseDirectory) == false)
            {
                Logger.WriteConsole("Target database directory was not found, creating it now.");
                Directory.CreateDirectory(CoreSettings.DatabaseDirectory);

                Logger.WriteConsole("Successfully created target database directory.");
                Logger.WriteConsole("Applying SQLExpress account permissions to database folder.");

                var dirInfo = new DirectoryInfo(CoreSettings.DatabaseDirectory);
                var dirSecurity = dirInfo.GetAccessControl();

                dirSecurity.AddAccessRule(
                    new FileSystemAccessRule(
                        Constants.Database.DefaultSqlExpressUserAccount,
                        FileSystemRights.FullControl,
                        InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                        PropagationFlags.None,
                        AccessControlType.Allow
                    )
                );

                dirInfo.SetAccessControl(dirSecurity);

                Logger.WriteConsole("NTFS permissions applied successfully.");
            }
            else
            {
                Logger.WriteConsole("Target database directory already exists, skipping step.");
            }

            if (Directory.Exists(CoreSettings.LogFilesDirectory) == false)
            {
                Logger.WriteConsole("Target log files directory was not found, creating it now.");
                Directory.CreateDirectory(CoreSettings.LogFilesDirectory);
                Logger.WriteConsole("Successfully created target log files directory.");
            }
            else
            {
                Logger.WriteConsole("Target log files directory already exists, skipping step.");
            }
        }

        /// <summary>
        /// Copies the program files to the installation directory.
        /// </summary>
        private void CopyProgramFiles()
        {
            Logger.WriteConsole("Detecting application source location.");
            var sourcePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Logger.WriteConsole("Detected application source directory: " + sourcePath);

            // expected file manifest
            var fileManifest = new List<string>()
            {
                "Microsoft.Azure.KeyVault.Core.dll",
                "Microsoft.Azure.KeyVault.Core.xml",
                "Microsoft.Data.Tools.Schema.Sql.dll",
                "Microsoft.Data.Tools.Utilities.dll",
                "Microsoft.IdentityModel.Logging.dll",
                "Microsoft.IdentityModel.Tokens.dll",
                "Microsoft.IdentityModel.Tokens.xml",
                "Microsoft.SqlServer.Dac.dll",
                "Microsoft.SqlServer.Dac.xml",
                "Microsoft.SqlServer.Dac.Extensions.dll",
                "Microsoft.SqlServer.Dac.Extensions.xml",
                "Microsoft.SqlServer.TransactSql.ScriptDom.dll",
                "Microsoft.SqlServer.TransactSql.ScriptDom.xml",
                "Microsoft.SqlServer.Types.dll",
                "Microsoft.WindowsAzure.Storage.dll",
                "Microsoft.WindowsAzure.Storage.xml",
                "NCrontab.dll",
                "Newtonsoft.Json.dll",
                "Newtonsoft.Json.xml",
                "OzetteClientAgent.exe",
                "OzetteClientAgent.exe.config",
                "OzetteLibrary.dll",
                "OzetteCmd.exe",
                "OzetteCmd.exe.config",
                "System.IdentityModel.Tokens.Jwt.dll",
                "System.IdentityModel.Tokens.Jwt.xml",
                "Twilio.dll",
                "Twilio.xml"
            };

            foreach (var file in fileManifest)
            {
                Logger.WriteConsole("Copying file: " + file);

                var sourceFileFullPath = Path.Combine(sourcePath, file);
                var destFileFullPath = Path.Combine(CoreSettings.InstallationDirectory, file);

                if (File.Exists(sourceFileFullPath) == false)
                {
                    throw new FileNotFoundException("A required setup file was missing: " + sourceFileFullPath);
                }

                File.Copy(sourceFileFullPath, destFileFullPath, true);
            }

            Logger.WriteConsole("Successfully copied files.");
        }

        /// <summary>
        /// Creates the client windows service.
        /// </summary>
        private void CreateClientService()
        {
            Logger.WriteConsole("Checking to see if OzetteCloudBackup windows service already exists.");

            var existingServices = ServiceController.GetServices();
                        
            if (existingServices.Any(x => x.ServiceName == "OzetteCloudBackup") == false)
            {
                Logger.WriteConsole("Creating OzetteCloudBackup windows service now.");

                var installArgs = string.Format(
                    "create \"OzetteCloudBackup\" binPath= \"{0}\" start= \"auto\" DisplayName= \"Ozette Cloud Backup Agent\"",
                    Path.Combine(CoreSettings.InstallationDirectory, "OzetteClientAgent.exe"));

                Process createServiceProcess = new Process();
                createServiceProcess.StartInfo = new ProcessStartInfo()
                {
                    FileName = "sc.exe",
                    Arguments = installArgs
                };

                createServiceProcess.Start();
                createServiceProcess.WaitForExit();

                if (createServiceProcess.ExitCode == 0)
                {
                    Logger.WriteConsole("Successfully created the windows service.");
                }
                else
                {
                    throw new Exception("Failed to create the windows service. Sc.exe returned an error code: " + createServiceProcess.ExitCode);
                }

                var setDescriptionArgs = string.Format(
                    "description \"OzetteCloudBackup\" \"Ozette Cloud Backup is a data backup client service that copies data to cloud providers like Azure and AWS.\"",
                    CoreSettings.InstallationDirectory);

                Process setServiceDescriptionProcess = new Process();
                setServiceDescriptionProcess.StartInfo = new ProcessStartInfo()
                {
                    FileName = "sc.exe",
                    Arguments = setDescriptionArgs
                };

                setServiceDescriptionProcess.Start();
                setServiceDescriptionProcess.WaitForExit();

                if (setServiceDescriptionProcess.ExitCode == 0)
                {
                    Logger.WriteConsole("Successfully set the windows service description.");
                }
                else
                {
                    throw new Exception("Failed to set the windows service description. Sc.exe returned an error code: " + setServiceDescriptionProcess.ExitCode);
                }
            }
            else
            {
                Logger.WriteConsole("Windows service already exists, skipping step.");
            }
        }

        /// <summary>
        /// Adds the installation directory to the system's path variable.
        /// </summary>
        private void AddSystemPath()
        {
            Logger.WriteConsole("Querying for the existing system path variable.");

            var pathVar = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.Machine);

            if (pathVar.ToLower().Contains(CoreSettings.InstallationDirectory.ToLower()) == false)
            {
                Logger.WriteConsole("System path variable does not have the installation path. Adding it now.");

                var newVar = pathVar.TrimEnd(';') + ";" + CoreSettings.InstallationDirectory;
                Environment.SetEnvironmentVariable("Path", newVar, EnvironmentVariableTarget.Machine);

                Logger.WriteConsole("Successfully added the installation path to the system path variable.");
            }
            else
            {
                Logger.WriteConsole("System path variable is already configured with the installation path.");
            }
        }
    }
}
