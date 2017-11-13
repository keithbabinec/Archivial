using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace OzetteLibrary.Logging.Default
{
    /// <summary>
    /// Contains default logging and log setup functionality.
    /// </summary>
    public class Logger : ILogger
    {
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
            if (string.IsNullOrEmpty(logSource))
            {
                throw new ArgumentException(nameof(logSource));
            }

            if (string.IsNullOrEmpty(logName))
            {
                throw new ArgumentException(nameof(logName));
            }

            if (!EventLog.SourceExists(logSource))
            {
                EventLog.CreateEventSource(logSource, logName);
            }

            LogSource = logSource;
            LogName = logName;
        }

        /// <summary>
        /// Ensures the detailed trace logging files folder is present on disk.
        /// </summary>
        /// <param name="path"></param>
        public void SetupTraceLogsFolderIfNotPresent(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(nameof(path));
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            TraceLogFolderPath = path.TrimEnd('\\');
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
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Argument cannot be null or empty/whitespace: " + nameof(message));
            }

            var logFilePath = GetCurrentTraceLogFilePath();
            var loggableMessage = PrependMessageWithDateAndSeverity(message, EventLogEntryType.Information);

            File.AppendAllText(logFilePath, loggableMessage);
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
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Argument cannot be null or empty/whitespace: " + nameof(message));
            }

            var logFilePath = GetCurrentTraceLogFilePath();
            var loggableMessage = PrependMessageWithDateAndSeverity(message, EventLogEntryType.Warning);

            File.AppendAllText(logFilePath, loggableMessage);
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
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Argument cannot be null or empty/whitespace: " + nameof(message));
            }

            var logFilePath = GetCurrentTraceLogFilePath();
            var loggableMessage = PrependMessageWithDateAndSeverity(message, EventLogEntryType.Error);

            File.AppendAllText(logFilePath, loggableMessage);
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
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Argument cannot be null or empty/whitespace: " + nameof(message));
            }
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            var logFilePath = GetCurrentTraceLogFilePath();
            var loggableMessage = GenerateExceptionLoggingMessage(message, exception);

            File.AppendAllText(logFilePath, loggableMessage);
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
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Argument cannot be null or empty/whitespace: " + nameof(message));
            }

            var loggableMessage = PrependMessageWithDateAndSeverity(message, severity);
            EventLog.WriteEntry(LogSource, loggableMessage, severity, eventID);
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
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Argument cannot be null or empty/whitespace: " + nameof(message));
            }
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            var loggableMessage = GenerateExceptionLoggingMessage(message, exception);
            EventLog.WriteEntry(LogSource, loggableMessage, EventLogEntryType.Error, eventID);
        }
        
        /// <summary>
        /// The trace log folder path.
        /// </summary>
        private string TraceLogFolderPath { get; set; }

        /// <summary>
        /// Eventlog source.
        /// </summary>
        private string LogSource { get; set; }

        /// <summary>
        /// Eventlog name.
        /// </summary>
        private string LogName { get; set; }

        /// <summary>
        /// Returns the full file name/path of the current tracelog file.
        /// </summary>
        /// <returns></returns>
        private string GetCurrentTraceLogFilePath()
        {
            return string.Format("{0}\\{1}_{2}.log",
                TraceLogFolderPath,
                "TraceLog",
                DateTime.Now.ToString("yyyy_MM_dd"));
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

        /// <summary>
        /// Generates an event log message with exception details.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        private string GenerateExceptionLoggingMessage(string message, Exception exception)
        {
            StringBuilder r = new StringBuilder();

            r.AppendLine(string.Format("{0} [{1}]: {2}",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                EventLogEntryType.Error.ToString(),
                message
            ));

            r.AppendLine(string.Format("Exception Message: {0}", exception.Message));
            r.AppendLine(string.Format("Exception Type: {0}", exception.GetType().FullName));
            r.AppendLine(string.Format("Exception Stack: {0}", exception.StackTrace));

            while (exception.InnerException != null)
            {
                exception = exception.InnerException;

                r.AppendLine(string.Format("Inner Exception Message: {0}", exception.Message));
                r.AppendLine(string.Format("Inner Exception Type: {0}", exception.GetType().FullName));
                r.AppendLine(string.Format("Inner Exception Stack: {0}", exception.StackTrace));
            }

            return r.ToString();
        }
    }
}
