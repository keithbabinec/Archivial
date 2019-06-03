namespace ArchivialLibrary.ServiceCore
{
    /// <summary>
    /// Describes access to get or set core application settings.
    /// </summary>
    public interface ICoreSettings
    {
        /// <summary>
        /// Gets the database connection string.
        /// </summary>
        string GetDatabaseConnectionString();

        /// <summary>
        /// Sets the database connection string.
        /// </summary>
        /// <remarks>The new setting value.</remarks>
        void SetDatabaseConnectionString(string newValue);

        /// <summary>
        /// Gets the event log name/source.
        /// </summary>
        string GetEventlogName();

        /// <summary>
        /// Sets the event log name.
        /// </summary>
        /// <remarks>The new setting value.</remarks>
        void SetEventlogName(string newValue);

        /// <summary>
        /// Gets the log files directory.
        /// </summary>
        string GetInstallationDirectory();

        /// <summary>
        /// Sets the installation directory.
        /// </summary>
        /// <remarks>The new setting value.</remarks>
        void SetInstallationDirectory(string newValue);

        /// <summary>
        /// Gets the log files directory.
        /// </summary>
        string GetLogFilesDirectory();

        /// <summary>
        /// Gets the log files archive directory.
        /// </summary>
        string GetLogFilesArchiveDirectory();

        /// <summary>
        /// Gets the database directory.
        /// </summary>
        string GetDatabaseDirectory();

        /// <summary>
        /// Gets the database backups directory.
        /// </summary>
        string GetDatabaseBackupsDirectory();

        /// <summary>
        /// Gets the flag to indicate if the database needs to be published.
        /// </summary>
        bool GetDatabasePublishIsRequired();

        /// <summary>
        /// Sets the flag to indicate if the database needs to be published.
        /// </summary>
        /// <remarks>The new setting value.</remarks>
        void SetDatabasePublishIsRequired(bool newValue);

        /// <summary>
        /// Removes a core setting by name.
        /// </summary>
        /// <param name="name">Setting name.</param>
        void RemoveCoreSetting(string name);
    }
}
