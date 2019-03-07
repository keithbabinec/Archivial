namespace OzetteLibrary.Constants
{
    /// <summary>
    /// A constants class for the bootstrap setting names.
    /// </summary>
    /// <remarks>
    /// Bootstrap settings are stored in environment variables and are the minimum settings required to launch the application.
    /// </remarks>
    public class BootstrapSettingNames
    {
        /// <summary>
        /// The protection IV setting name.
        /// </summary>
        public const string ProtectionIV = "OZETTE_PROTECTIONIV";

        /// <summary>
        /// The log files path setting.
        /// </summary>
        public const string LogFilesDirectory = "OZETTE_LOGFILESDIRECTORY";

        /// <summary>
        /// The database directory path setting.
        /// </summary>
        public const string DatabaseDirectory = "OZETTE_DATABASEDIRECTORY";

        /// <summary>
        /// The database backups directory path setting.
        /// </summary>
        public const string DatabaseBackupsDirectory = "OZETTE_DATABASEBACKUPSDIRECTORY";

        /// <summary>
        /// The installation directory path.
        /// </summary>
        public const string InstallationDirectory = "OZETTE_INSTALLATIONDIRECTORY";

        /// <summary>
        /// The event log name.
        /// </summary>
        public const string EventlogName = "OZETTE_EVENTLOGNAME";

        /// <summary>
        /// The database connection string name.
        /// </summary>
        public const string DatabaseConnectionString = "OZETTE_DATABASECONNECTIONSTRING";

        /// <summary>
        /// The number of backup engine instances to run.
        /// </summary>
        public const string BackupEngineInstancesCount = "OZETTE_BACKUPENGINEINSTANCESCOUNT";

        /// <summary>
        /// A flag to indicate if the database needs to be published.
        /// </summary>
        public const string DatabasePublishIsRequired = "OZETTE_DATABASEPUBLISHREQUIRED";
    }
}
