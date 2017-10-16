namespace OzetteLibrary.Logging
{
    /// <summary>
    /// Contains functionality for configuring custom windows event logs.
    /// </summary>
    public class MockEventLogSetup : IEventLogSetup
    {
        /// <summary>
        /// Flag to set when the mock runs.
        /// </summary>
        public bool SetupCustomWindowsEventLogIfNotPresentHasBeenCalled = false;

        /// <summary>
        /// Flag to set when the mock runs.
        /// </summary>
        public bool SetupLogsFolderIfNotPresentHasBeenCalled = false;

        /// <summary>
        /// Ensures the custom windows event log is present.
        /// </summary>
        /// <remarks>
        /// Will throw exception if running under non-elevated user context.
        /// The reason is that creating event log sources requires administrator privileges. 
        /// </remarks>
        /// <param name="logSource"></param>
        /// <param name="logName"></param>
        public void SetupCustomWindowsEventLogIfNotPresent(string logSource, string logName)
        {
            SetupCustomWindowsEventLogIfNotPresentHasBeenCalled = true;
        }

        /// <summary>
        /// Ensures the detailed logging files folder is present on disk.
        /// </summary>
        /// <param name="path"></param>
        public void SetupLogsFolderIfNotPresent(string path)
        {
            SetupLogsFolderIfNotPresentHasBeenCalled = true;
        }
    }
}
