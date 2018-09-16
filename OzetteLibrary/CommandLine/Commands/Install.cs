using System;
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
        /// Runs the installation command.
        /// </summary>
        /// <param name="arguments"></param>
        public static void Run(InstallationArguments arguments)
        {
            var logger = new Logger(Constants.Logging.InstallationComponentName);
            logger.WriteConsole("Starting Ozette Cloud Backup installation");

            logger.WriteConsole("Applying core settings.");
            CreateCoreSettings(arguments);

            logger.WriteConsole("Setting up installation directories.");
            CreateInstallationDirectories();

            logger.WriteConsole("Copying installation files.");
            CopyProgramFiles();

            logger.WriteConsole("Creating initial database.");
            CreateInitialDatabase();

            logger.WriteConsole("Creating Ozette Client Service.");
            CreateClientService();

            logger.WriteConsole("Starting Ozette Client Service.");
            StartClientService();
        }

        /// <summary>
        /// Creates the installation directories.
        /// </summary>
        private static void CreateInstallationDirectories()
        {
            throw new NotImplementedException();
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
