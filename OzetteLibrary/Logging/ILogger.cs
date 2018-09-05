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
        /// Starts the logger.
        /// </summary>
        /// <param name="EventLogSource"></param>
        /// <param name="EventLogName"></param>
        /// <param name="TraceLogFolderPath"></param>
        void Start(string EventLogSource, string EventLogName, string TraceLogFolderPath);

        /// <summary>
        /// Stops the logger.
        /// </summary>
        void Stop();

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
        /// <param name="stackContext"></param>
        void WriteTraceError(string message, Exception exception, string stackContext);

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
        /// <param name="writeToTraceLog"></param>
        void WriteSystemEvent(string message, EventLogEntryType severity, int eventID, bool writeToTraceLog);

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
        /// <param name="stackContext"></param>
        /// <param name="eventID"></param>
        /// <param name="writeToTraceLog"></param>
        void WriteSystemEvent(string message, Exception exception, string stackContext, int eventID, bool writeToTraceLog);

        /// <summary>
        /// Generates a full stack trace from the current method context.
        /// </summary>
        /// <returns></returns>
        string GenerateFullContextStackTrace();
    }
}
