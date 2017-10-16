namespace OzetteLibrary.Logging
{
    public interface IEventLogSetup
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
    }
}
