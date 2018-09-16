using System;
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
            Logger.WriteConsole("Step 1: Starting Ozette Cloud Backup installation");

            Logger.WriteConsole("Step 2: Applying core settings.");
            CreateCoreSettings(arguments);

            Logger.WriteConsole("Step 3: Setting up installation directories.");
            CreateInstallationDirectories();

            Logger.WriteConsole("Step 4: Copying installation files.");
            CopyProgramFiles();

            Logger.WriteConsole("Step 5: Creating initial database.");
            CreateInitialDatabase();

            Logger.WriteConsole("Step 6: Creating Ozette Client Service.");
            CreateClientService();

            Logger.WriteConsole("Step 7: Starting Ozette Client Service.");
            StartClientService();
        }

        /// <summary>
        /// Creates the installation directories.
        /// </summary>
        private static void CreateInstallationDirectories()
        {
            if (Directory.Exists(CoreSettings.InstallationDirectory) == false)
            {
                Directory.CreateDirectory(CoreSettings.InstallationDirectory);
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

        /// <summary>
        /// Sets the core application settings.
        /// </summary>
        /// <param name="arguments"></param>
        private static void CreateCoreSettings(InstallationArguments arguments)
        {
            // set the core settings.

            CoreSettings.InstallationDirectory = arguments.InstallDirectory;
            CoreSettings.LogFilesDirectory = System.IO.Path.Combine(arguments.InstallDirectory, "Logs");
            CoreSettings.EventlogName = "OzetteCloudBackup";

            CoreSettings.DatabaseConnectionString = 
                string.Format("Filename={0};Journal=true;Mode=Shared", arguments.DatabasePath);
        }
    }
}
