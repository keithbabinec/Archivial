using System;
using System.Configuration;

namespace OzetteLibrary.ServiceCore
{
    /// <summary>
    /// Contains options for windows services.
    /// </summary>
    public class ServiceOptions
    {
        /// <summary>
        /// The log files directory.
        /// </summary>
        public string LogFilesDirectory { get; set; }

        /// <summary>
        /// The custom windows event log name.
        /// </summary>
        public string EventlogName { get; set; }

        /// <summary>
        /// The database file name.
        /// </summary>
        public string DatabaseConnectionString { get; set; }

        /// <summary>
        /// A path to the scan sources file.
        /// </summary>
        public string SourcesFilePath { get; set; }

        /// <summary>
        /// High priority scan frequency
        /// </summary>
        public int HighPriorityScanFrequencyInHours { get; set; }

        /// <summary>
        /// Medium priority scan frequency
        /// </summary>
        public int MedPriorityScanFrequencyInHours { get; set; }

        /// <summary>
        /// Low priority scan frequency
        /// </summary>
        public int LowPriorityScanFrequencyInHours { get; set; }

        /// <summary>
        /// Base constructor.
        /// </summary>
        public ServiceOptions()
        {
        }

        /// <summary>
        /// Constructor that accepts a settings property collection.
        /// </summary>
        /// <param name="settings"></param>
        public ServiceOptions(SettingsPropertyCollection settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            LogFilesDirectory = (settings[nameof(LogFilesDirectory)] as SettingsProperty)?.DefaultValue.ToString();
            EventlogName = (settings[nameof(EventlogName)] as SettingsProperty)?.DefaultValue.ToString();
            DatabaseConnectionString = (settings[nameof(DatabaseConnectionString)] as SettingsProperty)?.DefaultValue.ToString();
            SourcesFilePath = (settings[nameof(SourcesFilePath)] as SettingsProperty)?.DefaultValue.ToString();
            HighPriorityScanFrequencyInHours = Convert.ToInt32((settings[nameof(HighPriorityScanFrequencyInHours)] as SettingsProperty)?.DefaultValue);
            MedPriorityScanFrequencyInHours = Convert.ToInt32((settings[nameof(MedPriorityScanFrequencyInHours)] as SettingsProperty)?.DefaultValue);
            LowPriorityScanFrequencyInHours = Convert.ToInt32((settings[nameof(LowPriorityScanFrequencyInHours)] as SettingsProperty)?.DefaultValue);
        }
    }
}
