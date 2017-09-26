using System;

namespace OzetteLibrary.Logging
{
    /// <summary>
    /// Contains functionality for configuring custom windows event logs.
    /// </summary>
    public class EventLogSetup
    {
        /// <summary>
        /// Ensures the custom windows event log is present.
        /// </summary>
        /// <param name="logName"></param>
        /// <param name="logSource"></param>
        public void SetupCustomWindowsEventLogIfNotPresent(string logName, string logSource)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Ensures the detailed logging files folder is present on disk.
        /// </summary>
        /// <param name="path"></param>
        public void SetupLogsFolderIfNotPresent(string path)
        {
            throw new NotImplementedException();
        }
    }
}
