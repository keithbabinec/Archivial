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

            logger.WriteConsole("Appling core settings.");
            SetCoreSettings(arguments);
        }

        private static void SetCoreSettings(InstallationArguments arguments)
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
