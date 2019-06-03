using ArchivialLibrary.Constants;
using ArchivialLibrary.Exceptions;
using System;
using System.IO;

namespace ArchivialLibrary.ServiceCore
{
    /// <summary>
    /// Contains functionality for retrieving core settings from a Windows installation.
    /// </summary>
    public class WindowsCoreSettings : ICoreSettings
    {
        /// <summary>
        /// Gets the database connection string.
        /// </summary>
        public string GetDatabaseConnectionString()
        {
            return GetCoreStringSetting(BootstrapSettingNames.DatabaseConnectionString);
        }

        /// <summary>
        /// Sets the database connection string.
        /// </summary>
        /// <remarks>The new setting value.</remarks>
        public void SetDatabaseConnectionString(string newValue)
        {
            SetCoreStringSetting(BootstrapSettingNames.DatabaseConnectionString, newValue);
        }

        /// <summary>
        /// Gets the event log name/source.
        /// </summary>
        public string GetEventlogName()
        {
            return GetCoreStringSetting(BootstrapSettingNames.EventlogName);
        }

        /// <summary>
        /// Sets the event log name.
        /// </summary>
        /// <remarks>The new setting value.</remarks>
        public void SetEventlogName(string newValue)
        {
            SetCoreStringSetting(BootstrapSettingNames.EventlogName, newValue);
        }

        /// <summary>
        /// Gets the log files directory.
        /// </summary>
        public string GetInstallationDirectory()
        {
            return GetCoreStringSetting(BootstrapSettingNames.InstallationDirectory);
        }

        /// <summary>
        /// Sets the installation directory.
        /// </summary>
        /// <remarks>The new setting value.</remarks>
        public void SetInstallationDirectory(string newValue)
        {
            SetCoreStringSetting(BootstrapSettingNames.InstallationDirectory, newValue);
        }

        /// <summary>
        /// Gets the log files directory.
        /// </summary>
        public string GetLogFilesDirectory()
        {
            return Path.Combine(GetInstallationDirectory(), "Logs");
        }

        /// <summary>
        /// Gets the log files archive directory.
        /// </summary>
        public string GetLogFilesArchiveDirectory()
        {
            return Path.Combine(GetInstallationDirectory(), "Logs\\Archive");
        }

        /// <summary>
        /// Gets the database directory.
        /// </summary>
        public string GetDatabaseDirectory()
        {
            return Path.Combine(GetInstallationDirectory(), "Database");
        }

        /// <summary>
        /// Gets the database backups directory.
        /// </summary>
        public string GetDatabaseBackupsDirectory()
        {
            return Path.Combine(GetInstallationDirectory(), "Database\\Backups");
        }

        /// <summary>
        /// Gets the flag to indicate if the database needs to be published.
        /// </summary>
        public bool GetDatabasePublishIsRequired()
        {
            return GetCoreBoolSetting(BootstrapSettingNames.DatabasePublishIsRequired);
        }

        /// <summary>
        /// Sets the flag to indicate if the database needs to be published.
        /// </summary>
        /// <remarks>The new setting value.</remarks>
        public void SetDatabasePublishIsRequired(bool newValue)
        {
            SetCoreBoolSetting(BootstrapSettingNames.DatabasePublishIsRequired, newValue.ToString());
        }

        /// <summary>
        /// Removes a core setting by name.
        /// </summary>
        /// <param name="name">Setting name.</param>
        public void RemoveCoreSetting(string name)
        {
            Environment.SetEnvironmentVariable(name, null, EnvironmentVariableTarget.Machine);
        }

        /// <summary>
        /// Gets a core string setting value.
        /// </summary>
        /// <param name="name">Name of the setting.</param>
        /// <returns>The setting value.</returns>
        private static string GetCoreStringSetting(string name)
        {
            var value = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Machine);

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ApplicationCoreSettingMissingException(name);
            }

            return value;
        }

        /// <summary>
        /// Sets a core string setting value.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private static void SetCoreStringSetting(string name, string value)
        {
            Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.Machine);
        }

        /// <summary>
        /// Gets a core boolean setting value.
        /// </summary>
        /// <param name="name">Name of the setting.</param>
        /// <returns>The setting value.</returns>
        private static bool GetCoreBoolSetting(string name)
        {
            var value = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Machine);

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ApplicationCoreSettingMissingException(name);
            }

            if (bool.TryParse(value, out bool result))
            {
                return result;
            }
            else
            {
                throw new ApplicationCoreSettingInvalidValueException(name);
            }
        }

        /// <summary>
        /// Sets a core boolean setting value.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private static void SetCoreBoolSetting(string name, string value)
        {
            Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.Machine);
        }
    }
}
