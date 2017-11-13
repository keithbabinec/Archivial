using System;
using System.Diagnostics;

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
        void SetupTraceLogsFolderIfNotPresent(string path);

        /// <summary>
        /// Writes an informational message to the trace log file on disk.
        /// </summary>
        /// <remarks>
        /// This logging method is the preferred method for trace-level debug messaging.
        /// An example would be things like individual file transfer messages, state changes, etc.
        /// </remarks>
        /// <param name="message"></param>
        void WriteTraceMessage(string message);

        /// <summary>
        /// Writes a warning message to the trace log file on disk.
        /// </summary>
        /// <remarks>
        /// This logging method is the preferred method for trace-level debug messaging.
        /// An example would be things like individual file transfer messages, state changes, etc.
        /// </remarks>
        /// <param name="message"></param>
        void WriteTraceWarning(string message);

        /// <summary>
        /// Writes an error message to the trace log file on disk.
        /// </summary>
        /// <remarks>
        /// This logging method is the preferred method for trace-level debug messaging.
        /// An example would be things like individual file transfer messages, state changes, etc.
        /// </remarks>
        /// <param name="message"></param>
        void WriteTraceError(string message);

        /// <summary>
        /// Writes an error message (and exception) to the trace log file on disk.
        /// </summary>
        /// <remarks>
        /// This logging method is the preferred method for trace-level debug messaging.
        /// An example would be things like individual file transfer messages, state changes, etc.
        /// </remarks>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void WriteTraceError(string message, Exception exception);

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
        void WriteSystemEvent(string message, EventLogEntryType severity, int eventID);

        /// <summary>
        /// Writes a system-level error message with exception.
        /// </summary>
        /// <remarks>
        /// This logging method is used for the most important high-level events the user should be aware of.
        /// These events are logged into the Windows Event log instead of the trace debug log files, and would be
        /// used for logging exceptions, backup completed/sync status, configuration issues, etc.
        /// </remarks>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="eventID"></param>
        void WriteSystemEvent(string message, Exception exception, int eventID);
    }
}
