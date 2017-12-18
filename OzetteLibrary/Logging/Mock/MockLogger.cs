using System;
using System.Diagnostics;
using System.Text;

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
        /// Flag to set when the mock runs.
        /// </summary>
        public bool WriteTraceMessageHasBeenCalled = false;

        /// <summary>
        /// Flag to set when the mock runs.
        /// </summary>
        public bool WriteTraceWarningHasBeenCalled = false;

        /// <summary>
        /// Flag to set when the mock runs.
        /// </summary>
        public bool WriteTraceErrorHasBeenCalled = false;

        /// <summary>
        /// Flag to set when the mock runs.
        /// </summary>
        public bool WriteTraceErrorWithExceptionHasBeenCalled = false;

        /// <summary>
        /// Flag to set when the mock runs.
        /// </summary>
        public bool WriteSystemEventHasBeenCalled = false;

        /// <summary>
        /// Flag to set when the mock runs.
        /// </summary>
        public bool WriteSystemEventWithExceptionHasBeenCalled = false;

        /// <summary>
        /// Flag to set when the start method has been called.
        /// </summary>
        public bool StartHasBeenCalled = false;

        /// <summary>
        /// Flag to set when the stop method has been called.
        /// </summary>
        public bool StopHasBeenCalled = false;

        /// <summary>
        /// The last exception that has been logged.
        /// </summary>
        public Exception ExceptionWritten = null;

        /// <summary>
        /// An in-memory trace log.
        /// </summary>
        public StringBuilder TraceLog = new StringBuilder();

        /// <summary>
        /// Starts the logger.
        /// </summary>
        /// <param name="EventLogSource"></param>
        /// <param name="EventLogName"></param>
        /// <param name="TraceLogFolderPath"></param>
        public void Start(string EventLogSource, string EventLogName, string TraceLogFolderPath)
        {
            StartHasBeenCalled = true;
        }

        /// <summary>
        /// Stops the logger.
        /// </summary>
        public void Stop()
        {
            StopHasBeenCalled = true;
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
            WriteTraceMessageHasBeenCalled = true;
            var loggableMessage = PrependMessageWithDateAndSeverity(message, EventLogEntryType.Information);
            TraceLog.AppendLine(loggableMessage);
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
            WriteTraceWarningHasBeenCalled = true;
            var loggableMessage = PrependMessageWithDateAndSeverity(message, EventLogEntryType.Warning);
            TraceLog.AppendLine(loggableMessage);
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
            WriteTraceErrorHasBeenCalled = true;
            var loggableMessage = PrependMessageWithDateAndSeverity(message, EventLogEntryType.Error);
            TraceLog.AppendLine(loggableMessage);
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
            WriteTraceErrorWithExceptionHasBeenCalled = true;
            ExceptionWritten = exception;
            var loggableMessage = PrependMessageWithDateAndSeverity(message, EventLogEntryType.Error);
            TraceLog.AppendLine(loggableMessage);
            TraceLog.AppendLine(exception.ToString());
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
            WriteSystemEventHasBeenCalled = true;
        }

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
        public void WriteSystemEvent(string message, Exception exception, int eventID)
        {
            WriteSystemEventWithExceptionHasBeenCalled = true;
        }

        /// <summary>
        /// Prepends an event message with a date and severity level.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="severity"></param>
        /// <returns></returns>
        private string PrependMessageWithDateAndSeverity(string message, EventLogEntryType severity)
        {
            return string.Format("{0} [{1}]: {2}",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                severity.ToString(),
                message
            );
        }
    }
}
