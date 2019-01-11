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
using OzetteLibrary.CommandLine.Arguments;
using OzetteLibrary.Database.LiteDB;
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
        public bool Run(ArgumentBase arguments)
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

                Logger.WriteConsole("--- Step 5: Creating initial database.");
                CreateInitialDatabase();

                Logger.WriteConsole("--- Step 6: Creating Ozette Client Service.");
                CreateClientService();

                Logger.WriteConsole("--- Step 7: Add installation directory to system path variable.");
                AddSystemPath();

                Logger.WriteConsole("--- Step 8: Set database file permissions.");
                SetDbFilePermissions();

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
            CoreSettings.EventlogName = "Ozette";
            CoreSettings.BackupEngineInstanceCount = 4;

            var dbPath = Path.Combine(arguments.InstallDirectory, "Database\\OzetteCloudBackup.db");
            CoreSettings.DatabaseConnectionString = string.Format("Filename={0};Journal=true;Mode=Shared", dbPath);

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

            var dbDirectory = Path.Combine(CoreSettings.InstallationDirectory, "Database");
            if (Directory.Exists(dbDirectory) == false)
            {
                Logger.WriteConsole("Target database directory was not found, creating it now.");
                Directory.CreateDirectory(dbDirectory);
                Logger.WriteConsole("Successfully created target database directory.");
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
                "LiteDB.dll",
                "LiteDB.xml",
                "Microsoft.Azure.KeyVault.Core.dll",
                "Microsoft.Azure.KeyVault.Core.xml",
                "Microsoft.WindowsAzure.Storage.dll",
                "Microsoft.WindowsAzure.Storage.xml",
                "NCrontab.dll",
                "Newtonsoft.Json.dll",
                "Newtonsoft.Json.xml",
                "OzetteClientAgent.exe",
                "OzetteClientAgent.exe.config",
                "OzetteLibrary.dll",
                "OzetteCmd.exe",
                "OzetteCmd.exe.config"
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
        /// Creates the initial database.
        /// </summary>
        private void CreateInitialDatabase()
        {
            Logger.WriteConsole("Preparing database now.");

            var db = new LiteDBClientDatabase(CoreSettings.DatabaseConnectionString);
            db.PrepareDatabase();

            Logger.WriteConsole("Database successfully prepared.");
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

        /// <summary>
        /// Sets the database file permissions.
        /// </summary>
        private void SetDbFilePermissions()
        {
            Logger.WriteConsole("Querying for existing database file permissions.");

            var dbPath = Path.Combine(CoreSettings.InstallationDirectory, "Database\\OzetteCloudBackup.db");

            var dbfileInfo = new FileInfo(dbPath);
            var securityInfo = dbfileInfo.GetAccessControl();
            var accessRules = securityInfo.GetAccessRules(true, true, typeof(SecurityIdentifier));
            var builtInUsersIdentity = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);

            // find out if rule already exists

            bool permissionHasBeenSet = false;

            foreach (FileSystemAccessRule rule in accessRules)
            {
                if (rule.IdentityReference == builtInUsersIdentity 
                    && rule.FileSystemRights.HasFlag(FileSystemRights.Modify)
                    && rule.AccessControlType.HasFlag(AccessControlType.Allow))
                {
                    permissionHasBeenSet = true;
                }
            }

            if (permissionHasBeenSet)
            {
                Logger.WriteConsole("Database file permissions are already set correctly.");
            }
            else
            {
                Logger.WriteConsole("Database file permissions are not set. Setting them now.");

                securityInfo.AddAccessRule(new FileSystemAccessRule(builtInUsersIdentity, FileSystemRights.Modify, AccessControlType.Allow));
                File.SetAccessControl(dbPath, securityInfo);

                Logger.WriteConsole("Successfully set the database file permissions.");
            }
        }
    }
}
