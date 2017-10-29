namespace OzetteLibrary.Logging
{
    /// <summary>
    /// A generic interface for logging functionality.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Ensures the custom windows event log is present.
        /// </summary>
        /// <param name="logSource"></param>
        /// <param name="logName"></param>
        void SetupCustomWindowsEventLogIfNotPresent(string logSource, string logName);

        /// <summary>
        /// Ensures the detailed logging files folder is present on disk.
        /// </summary>
        /// <param name="path"></param>
        void SetupLogsFolderIfNotPresent(string path);

        /// <summary>
        /// Writes an informational message to the log file on disk.
        /// </summary>
        /// <param name="message"></param>
        void WriteMessage(string message);
    }
}
