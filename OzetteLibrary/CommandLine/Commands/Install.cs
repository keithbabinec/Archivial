using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using OzetteLibrary.Database.LiteDB;
using OzetteLibrary.Logging.Default;
using OzetteLibrary.ServiceCore;

namespace OzetteLibrary.CommandLine.Commands
{
    /// <summary>
    /// A command for installing the program.
    /// </summary>
    public static class Install
    {
        /// <summary>
        /// A logging helper instance.
        /// </summary>
        private static Logger Logger = new Logger(Constants.Logging.InstallationComponentName);

        /// <summary>
        /// Runs the installation command.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns>True if successful, otherwise false.</returns>
        public static bool Run(InstallationArguments arguments)
        {
            try
            {
                Logger.WriteConsole("--- Starting Ozette Cloud Backup installation");

                Logger.WriteConsole("--- Step 1: Applying core settings.");
                CreateCoreSettings(arguments);

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
        private static void CreateCoreSettings(InstallationArguments arguments)
        {
            // set the core settings.

            CoreSettings.InstallationDirectory = arguments.InstallDirectory;
            CoreSettings.LogFilesDirectory = Path.Combine(arguments.InstallDirectory, "Logs");
            CoreSettings.EventlogName = "Ozette";

            var dbPath = Path.Combine(arguments.InstallDirectory, "Database\\OzetteCloudBackup.db");
            CoreSettings.DatabaseConnectionString = string.Format("Filename={0};Journal=true;Mode=Shared", dbPath);

            Logger.WriteConsole("Core settings successfully applied.");
            Logger.WriteConsole("InstallationDirectory=" + CoreSettings.InstallationDirectory);
            Logger.WriteConsole("LogFilesDirectory=" + CoreSettings.LogFilesDirectory);
            Logger.WriteConsole("EventlogName=" + CoreSettings.EventlogName);
            Logger.WriteConsole("DatabaseConnectionString=" + CoreSettings.DatabaseConnectionString);
        }

        /// <summary>
        /// Creates a custom event log and event source.
        /// </summary>
        private static void CreateEventLogSource()
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
        private static void CreateInstallationDirectories()
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
        private static void CopyProgramFiles()
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
        private static void CreateInitialDatabase()
        {
            Logger.WriteConsole("Preparing database now.");

            var db = new LiteDBClientDatabase(CoreSettings.DatabaseConnectionString);
            db.PrepareDatabase();

            Logger.WriteConsole("Database successfully prepared.");
        }

        /// <summary>
        /// Creates the client windows service.
        /// </summary>
        private static void CreateClientService()
        {
            Logger.WriteConsole("Checking to see if OzetteCloudBackup windows service already exists.");

            var existingServices = ServiceController.GetServices();
                        
            if (existingServices.Any(x => x.ServiceName == "OzetteCloudBackup") == false)
            {
                Logger.WriteConsole("Creating OzetteCloudBackup windows service now.");

                var installArgs = string.Format(
                    "create \"OzetteCloudBackup\" binPath= \"{0}\" start= \"auto\" DisplayName= \"Ozette Cloud Backup Agent\"",
                    CoreSettings.InstallationDirectory);

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
    }
}
