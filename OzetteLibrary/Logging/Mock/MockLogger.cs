using System;
using System.Diagnostics;

namespace OzetteLibrary.Logging.Mock
{
    /// <summary>
    /// Contains mock functionality for logging and log setup.
    /// </summary>
    public class MockLogger : ILogger
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

        /// <summary>
        /// Writes an informational message to the trace log file on disk.
        /// </summary>
        /// <remarks>
        /// This logging method is the preferred method for trace-level debug messaging.
        /// An example would be things like individual file transfer messages, state changes, etc.
        /// </remarks>
        /// <param name="message"></param>
        public void WriteTraceMessage(string message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes a warning message to the trace log file on disk.
        /// </summary>
        /// <remarks>
        /// This logging method is the preferred method for trace-level debug messaging.
        /// An example would be things like individual file transfer messages, state changes, etc.
        /// </remarks>
        /// <param name="message"></param>
        public void WriteTraceWarning(string message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes an error message to the trace log file on disk.
        /// </summary>
        /// <remarks>
        /// This logging method is the preferred method for trace-level debug messaging.
        /// An example would be things like individual file transfer messages, state changes, etc.
        /// </remarks>
        /// <param name="message"></param>
        public void WriteTraceError(string message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes an error message (and exception) to the trace log file on disk.
        /// </summary>
        /// <remarks>
        /// This logging method is the preferred method for trace-level debug messaging.
        /// An example would be things like individual file transfer messages, state changes, etc.
        /// </remarks>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void WriteTraceError(string message, Exception exception)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes a system-level event message.
        /// </summary>
        /// <remarks>
        /// This logging method is used for the most important high-level events the user should be aware of.
        /// These events are logged into the Windows Event log instead of the trace debug log files, and would be
        /// used for logging exceptions, backup completed/sync status, configuration issues, etc.
        /// </remarks>
        /// <param name="message"></param>
        /// <param name="severity"></param>
        /// <param name="eventID"></param>
        public void WriteSystemEvent(string message, EventLogEntryType severity, int eventID)
        {
            throw new NotImplementedException();
        }
    }
}
