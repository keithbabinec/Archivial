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
            throw new NotImplementedException();
        }
    }
}
