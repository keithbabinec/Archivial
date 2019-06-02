using ArchivialLibrary.Logging;
using System;
using System.Diagnostics;

namespace ArchivialPowerShell.Utility
{
    /// <summary>
    /// Implements the logger for console only support.
    /// </summary>
    /// <remarks>
    /// This is a simplified implementation that only supports writing to the console. Event log entries and log files on disk are not supported here.
    /// </remarks>
    public class ConsoleLogger : ILogger
    {
        /// <summary>
        /// Initializes the cmdlet logger for a specific component source.
        /// </summary>
        /// <param name="verbose">If verbose logging should be enabled. Informational events are not logged unless verbose is enabled.</param>
        public ConsoleLogger(bool verbose)
        {
            Verbose = verbose;
        }

        /// <summary>
        /// Starts the logger.
        /// </summary>
        /// <param name="EventLogSource"></param>
        /// <param name="EventLogName"></param>
        /// <param name="TraceLogFolderPath"></param>
        public void Start(string EventLogSource, string EventLogName, string TraceLogFolderPath)
        {
            // not implemented in this class, but do not throw.
        }

        /// <summary>
        /// Stops the logger.
        /// </summary>
        public void Stop()
        {
            // not implemented in this class, but do not throw.
        }

        /// <summary>
        /// Writes an informational message to the trace log file on disk.
        /// </summary>
        /// <remarks>
        /// This logging method is the preferred method for trace-level debug messaging.
        /// An example would be things like individual file transfer messages, state changes, etc.
        /// </remarks>
        /// <param name="message"></param>
        /// <param name="engineID"></param>
        public void WriteTraceMessage(string message, int engineID = 0)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Argument cannot be null or empty/whitespace: " + nameof(message));
            }

            if (Verbose)
            {
                Console.WriteLine("INFO: " + message);
            }
        }

        /// <summary>
        /// Writes a warning message to the trace log file on disk.
        /// </summary>
        /// <remarks>
        /// This logging method is the preferred method for trace-level debug messaging.
        /// An example would be things like individual file transfer messages, state changes, etc.
        /// </remarks>
        /// <param name="message"></param>
        /// <param name="engineID"></param>
        public void WriteTraceWarning(string message, int engineID = 0)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Argument cannot be null or empty/whitespace: " + nameof(message));
            }

            Console.WriteLine("WARNING: " + message);
        }

        /// <summary>
        /// Writes an error message to the trace log file on disk.
        /// </summary>
        /// <remarks>
        /// This logging method is the preferred method for trace-level debug messaging.
        /// An example would be things like individual file transfer messages, state changes, etc.
        /// </remarks>
        /// <param name="message"></param>
        /// <param name="engineID"></param>
        public void WriteTraceError(string message, int engineID = 0)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Argument cannot be null or empty/whitespace: " + nameof(message));
            }

            Console.Error.WriteLine("ERROR: " + message);
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
        /// <param name="stackContext"></param>
        /// <param name="engineID"></param>
        public void WriteTraceError(string message, Exception exception, string stackContext, int engineID = 0)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Argument cannot be null or empty/whitespace: " + nameof(message));
            }
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            Console.Error.WriteLine("ERROR: " + message + ". EXCEPTION: " + exception.ToString());
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
        /// <param name="writeToTraceLog"></param>
        /// <param name="engineID"></param>
        public void WriteSystemEvent(string message, EventLogEntryType severity, int eventID, bool writeToTraceLog, int engineID = 0)
        {
            throw new NotImplementedException("This method is not supported in this implementation.");
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
        /// <param name="stackContext"></param>
        /// <param name="eventID"></param>
        /// <param name="writeToTraceLog"></param>
        /// <param name="engineID"></param>
        public void WriteSystemEvent(string message, Exception exception, string stackContext, int eventID, bool writeToTraceLog, int engineID = 0)
        {
            throw new NotImplementedException("This method is not supported in this implementation.");
        }

        /// <summary>
        /// Writes a system-level message to the console only.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="severity"></param>
        /// <param name="engineID"></param>
        public void WriteConsole(string message, EventLogEntryType severity = EventLogEntryType.Information, int engineID = 0)
        {
            throw new NotImplementedException("This method is not supported in this implementation.");
        }

        /// <summary>
        /// Verbose logging enabled.
        /// </summary>
        public bool Verbose { get; set; }

        /// <summary>
        /// Generates a full stack trace from the current method context.
        /// </summary>
        /// <returns></returns>
        public string GenerateFullContextStackTrace()
        {
            throw new NotImplementedException("This method is not supported in this implementation.");
        }
    }
}
