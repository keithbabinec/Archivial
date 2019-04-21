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
        /// A flag to indicate if the database needs to be published.
        /// </summary>
        public const string DatabasePublishIsRequired = "OZETTE_DATABASEPUBLISHREQUIRED";
    }
}
