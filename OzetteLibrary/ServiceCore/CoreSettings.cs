using OzetteLibrary.Constants;
using OzetteLibrary.Exceptions;
using System;

namespace OzetteLibrary.ServiceCore
{
    /// <summary>
    /// Contains functionality for retrieving core settings.
    /// </summary>
    public static class CoreSettings
    {
        /// <summary>
        /// The database connection string.
        /// </summary>
        public static string DatabaseConnectionString
        {
            get
            {
                return GetCoreStringSetting(BootstrapSettingNames.DatabaseConnectionString);
            }
            set
            {
                SetCoreStringSetting(BootstrapSettingNames.DatabaseConnectionString, value);
            }
        }

        /// <summary>
        /// The event log name/source.
        /// </summary>
        public static string EventlogName
        {
            get
            {
                return GetCoreStringSetting(BootstrapSettingNames.EventlogName);
            }
            set
            {
                SetCoreStringSetting(BootstrapSettingNames.EventlogName, value);
            }
        }

        /// <summary>
        /// The log files directory.
        /// </summary>
        public static string InstallationDirectory
        {
            get
            {
                return GetCoreStringSetting(BootstrapSettingNames.InstallationDirectory);
            }
            set
            {
                SetCoreStringSetting(BootstrapSettingNames.InstallationDirectory, value);
            }
        }

        /// <summary>
        /// The log files directory.
        /// </summary>
        public static string LogFilesDirectory
        {
            get
            {
                return GetCoreStringSetting(BootstrapSettingNames.LogFilesDirectory);
            }
            set
            {
                SetCoreStringSetting(BootstrapSettingNames.LogFilesDirectory, value);
            }
        }

        /// <summary>
        /// The log files archive directory.
        /// </summary>
        public static string LogFilesArchiveDirectory
        {
            get
            {
                return GetCoreStringSetting(BootstrapSettingNames.LogFilesArchiveDirectory);
            }
            set
            {
                SetCoreStringSetting(BootstrapSettingNames.LogFilesArchiveDirectory, value);
            }
        }

        /// <summary>
        /// The log files retention length.
        /// </summary>
        public static int LogFilesRetentionInDays
        {
            get
            {
                return GetCoreIntSetting(BootstrapSettingNames.LogFilesRetentionInDays);
            }
            set
            {
                SetCoreIntSetting(BootstrapSettingNames.LogFilesRetentionInDays, value.ToString());
            }
        }

        /// <summary>
        /// The database backup files retention length.
        /// </summary>
        public static int DatabaseBackupsRetentionInDays
        {
            get
            {
                return GetCoreIntSetting(BootstrapSettingNames.DatabaseBackupsRetentionInDays);
            }
            set
            {
                SetCoreIntSetting(BootstrapSettingNames.DatabaseBackupsRetentionInDays, value.ToString());
            }
        }

        /// <summary>
        /// The database directory.
        /// </summary>
        public static string DatabaseDirectory
        {
            get
            {
                return GetCoreStringSetting(BootstrapSettingNames.DatabaseDirectory);
            }
            set
            {
                SetCoreStringSetting(BootstrapSettingNames.DatabaseDirectory, value);
            }
        }

        /// <summary>
        /// The database backups directory.
        /// </summary>
        public static string DatabaseBackupsDirectory
        {
            get
            {
                return GetCoreStringSetting(BootstrapSettingNames.DatabaseBackupsDirectory);
            }
            set
            {
                SetCoreStringSetting(BootstrapSettingNames.DatabaseBackupsDirectory, value);
            }
        }

        /// <summary>
        /// The encryption IV value.
        /// </summary>
        public static string ProtectionIv
        {
            get
            {
                return GetCoreStringSetting(BootstrapSettingNames.ProtectionIV);
            }
            set
            {
                SetCoreStringSetting(BootstrapSettingNames.ProtectionIV, value);
            }
        }

        /// <summary>
        /// The number of backup engine instances to use.
        /// </summary>
        public static int BackupEngineInstanceCount
        {
            get
            {
                return GetCoreIntSetting(BootstrapSettingNames.BackupEngineInstancesCount);
            }
            set
            {
                SetCoreIntSetting(BootstrapSettingNames.BackupEngineInstancesCount, value.ToString());
            }
        }

        /// <summary>
        /// A flag to indicate if the database needs to be published.
        /// </summary>
        public static bool DatabasePublishIsRequired
        {
            get
            {
                return GetCoreBoolSetting(BootstrapSettingNames.DatabasePublishIsRequired);
            }
            set
            {
                SetCoreBoolSetting(BootstrapSettingNames.DatabasePublishIsRequired, value.ToString());
            }
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
        /// Gets a core int setting value.
        /// </summary>
        /// <param name="name">Name of the setting.</param>
        /// <returns>The setting value.</returns>
        private static int GetCoreIntSetting(string name)
        {
            var value = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Machine);

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ApplicationCoreSettingMissingException(name);
            }

            if (int.TryParse(value, out int result))
            {
                return result;
            }
            else
            {
                throw new ApplicationCoreSettingInvalidValueException(name);
            }
        }

        /// <summary>
        /// Sets a core int setting value.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private static void SetCoreIntSetting(string name, string value)
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

        /// <summary>
        /// Removes a core setting by name.
        /// </summary>
        /// <param name="name"></param>
        public static void RemoveCoreSetting(string name)
        {
            Environment.SetEnvironmentVariable(name, null, EnvironmentVariableTarget.Machine);
        }
    }
}
