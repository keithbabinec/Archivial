using System;
using System.Diagnostics;
using System.IO;
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
        public static void Run(InstallationArguments arguments)
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

            Logger.WriteConsole("--- Step 7: Starting Ozette Client Service.");
            StartClientService();
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
            CoreSettings.EventlogName = "OzetteCloudBackup";

            CoreSettings.DatabaseConnectionString =
                string.Format("Filename={0};Journal=true;Mode=Shared", arguments.DatabasePath);

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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the initial database.
        /// </summary>
        private static void CreateInitialDatabase()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the client windows service.
        /// </summary>
        private static void CreateClientService()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Starts the client windows service.
        /// </summary>
        private static void StartClientService()
        {
            throw new NotImplementedException();
        }
    }
}
