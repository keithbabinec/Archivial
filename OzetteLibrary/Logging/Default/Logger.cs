using OzetteLibrary.Constants;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace OzetteLibrary.Logging.Default
{
    /// <summary>
    /// Contains default logging and log setup functionality.
    /// </summary>
    public class Logger : ILogger
    {
        /// <summary>
        /// Initializes the logger for a specific component source.
        /// </summary>
        /// <param name="component">A source component name, such as 'Backup', 'Scanner', etc.</param>
        public Logger(string component)
        {
            if (string.IsNullOrEmpty(component))
            {
                throw new ArgumentException("Argument cannot be null or empty/whitespace: " + nameof(component));
            }

            SourceComponent = component;
            TraceMessageQueue = new ConcurrentQueue<string>();
        }

        /// <summary>
        /// Starts the logger.
        /// </summary>
        /// <param name="EventLogSource"></param>
        /// <param name="EventLogName"></param>
        /// <param name="TraceLogFolderPath"></param>
        public void Start(string EventLogSource, string EventLogName, string TraceLogFolderPath)
        {
            if (Running == true)
            {
                throw new InvalidOperationException("Logger has already been started.");
            }

            Running = true;

            SetupCustomWindowsEventLogIfNotPresent(EventLogSource, EventLogName);
            SetupTraceLogsFolderIfNotPresent(TraceLogFolderPath);

            StringBuilder setupMessage = new StringBuilder();
            setupMessage.AppendLine(string.Format("Logging has been configured for component: {0}", SourceComponent));
            setupMessage.AppendLine(string.Format("Tracelog Path: {0}", GetCurrentTraceLogFilePath()));

            WriteSystemEvent(setupMessage.ToString(), EventLogEntryType.Information, EventIDs.LoggingInitialized);

            Thread tmw = new Thread(() => TraceMessageWriter());
            tmw.Start();
        }

        /// <summary>
        /// Stops the logger.
        /// </summary>
        public void Stop()
        {
            if (Running == true)
            {
                // this flags the running trace writer thread to exit.
                Running = false;
            }
        }

        /// <summary>
        /// Ensures the custom windows event log is present.
        /// </summary>
        /// <remarks>
        /// Will throw exception if running under non-elevated user context.
        /// The reason is that creating event log sources requires administrator privileges. 
        /// </remarks>
        /// <param name="logSource"></param>
        /// <param name="logName"></param>
        private void SetupCustomWindowsEventLogIfNotPresent(string logSource, string logName)
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
            EventLogInitialized = true;
        }

        /// <summary>
        /// Ensures the detailed trace logging files folder is present on disk.
        /// </summary>
        /// <param name="path"></param>
        private void SetupTraceLogsFolderIfNotPresent(string path)
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
            TraceInitialized = true;
        }

        /// <summary>
        /// A long-running loop/thread to write trace messages to disk.
        /// </summary>
        /// <remarks>
        /// The general idea here is that when writing to a file on disk there may be write/lock issues.
        /// They usually crop up from things like antivirus scans. If a single log message fails to write,
        /// then this shouldn't crash the application. This function is a loop to write with retries and delays.
        /// </remarks>
        public void TraceMessageWriter()
        {
            while (Running == true)
            {
                string messageToWrite = null;

                if (TraceMessageQueue.Count > 0)
                {
                    TraceMessageQueue.TryDequeue(out messageToWrite);
                }

                if (messageToWrite == null)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(500));
                }
                else
                {
                    bool successfulWrite = false;
                    Exception lastError = null;
                    int attempts = 0;

                    while (true)
                    {
                        attempts++;

                        try
                        {
                            File.AppendAllText(GetCurrentTraceLogFilePath(), messageToWrite + Environment.NewLine);
                            successfulWrite = true;
                            break;
                        }
                        catch (Exception ex)
                        {
                            if (attempts >= MaxWriteAttempts)
                            {
                                lastError = ex;
                                break;
                            }
                            else
                            {
                                Thread.Sleep(TimeSpan.FromSeconds(1));
                            }
                        }
                    }

                    if (successfulWrite == false)
                    {
                        WriteSystemEvent("Failed to write a message to the tracelog.", lastError, null, Constants.EventIDs.FailedToWriteToTraceLog);
                    }
                }
            }
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
            if (TraceInitialized == false)
            {
                throw new InvalidOperationException("Trace has not been initialized.");
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Argument cannot be null or empty/whitespace: " + nameof(message));
            }

            TraceMessageQueue.Enqueue(PrependMessageWithDateAndSeverity(message, EventLogEntryType.Information));
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
            if (TraceInitialized == false)
            {
                throw new InvalidOperationException("Trace has not been initialized.");
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Argument cannot be null or empty/whitespace: " + nameof(message));
            }

            TraceMessageQueue.Enqueue(PrependMessageWithDateAndSeverity(message, EventLogEntryType.Warning));
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
            if (TraceInitialized == false)
            {
                throw new InvalidOperationException("Trace has not been initialized.");
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Argument cannot be null or empty/whitespace: " + nameof(message));
            }

            TraceMessageQueue.Enqueue(PrependMessageWithDateAndSeverity(message, EventLogEntryType.Error));
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
        public void WriteTraceError(string message, Exception exception, string stackContext)
        {
            if (TraceInitialized == false)
            {
                throw new InvalidOperationException("Trace has not been initialized.");
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Argument cannot be null or empty/whitespace: " + nameof(message));
            }
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            TraceMessageQueue.Enqueue(GenerateExceptionLoggingMessage(message, exception, stackContext));
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
            if (EventLogInitialized == false)
            {
                throw new InvalidOperationException("Log has not been initialized.");
            }
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
        /// <param name="stackContext"></param>
        /// <param name="eventID"></param>
        public void WriteSystemEvent(string message, Exception exception, string stackContext, int eventID)
        {
            if (EventLogInitialized == false)
            {
                throw new InvalidOperationException("Log has not been initialized.");
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Argument cannot be null or empty/whitespace: " + nameof(message));
            }
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            var loggableMessage = GenerateExceptionLoggingMessage(message, exception, stackContext);
            EventLog.WriteEntry(LogSource, loggableMessage, EventLogEntryType.Error, eventID);
        }

        /// <summary>
        /// A flag to indicate if the logger is running.
        /// </summary>
        private volatile bool Running = false;

        /// <summary>
        /// A flag to indicate if we have initialized trace logs.
        /// </summary>
        private bool TraceInitialized = false;

        /// <summary>
        /// A flag to indicate if we have initialized system event logs.
        /// </summary>
        private bool EventLogInitialized = false;

        /// <summary>
        /// The number of tracelog write attempts before giving up.
        /// </summary>
        private const int MaxWriteAttempts = 5;
        
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
        /// Source component using this logger instance.
        /// </summary>
        private string SourceComponent { get; set; }

        /// <summary>
        /// A thread-safe queue of messages to write to the trace log.
        /// </summary>
        private ConcurrentQueue<string> TraceMessageQueue { get; set; }
        
        /// <summary>
        /// Returns the full file name/path of the current tracelog file.
        /// </summary>
        /// <returns></returns>
        private string GetCurrentTraceLogFilePath()
        {
            return string.Format("{0}\\{1}_{2}_{3}.log",
                TraceLogFolderPath,
                Constants.Logging.AppName,
                SourceComponent,
                DateTime.Now.ToString(Constants.Logging.SortableDateFormat));
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
                DateTime.Now.ToString(Constants.Logging.SortableDateTimeFormat),
                severity.ToString(),
                message
            );
        }

        /// <summary>
        /// Generates an event log message with exception details.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="contextStack"></param>
        /// <returns></returns>
        private string GenerateExceptionLoggingMessage(string message, Exception exception, string contextStack)
        {
            StringBuilder r = new StringBuilder();

            r.AppendLine(string.Format("{0} [{1}]: {2}",
                DateTime.Now.ToString(Constants.Logging.SortableDateTimeFormat),
                EventLogEntryType.Error.ToString(),
                message
            ));

            r.AppendLine(string.Format("Exception Message: {0}", exception.Message));
            r.AppendLine(string.Format("Exception Type: {0}", exception.GetType().FullName));
            r.AppendLine();
            r.AppendLine(string.Format("Exception Stack:"));
            r.AppendLine(string.Format(exception.StackTrace));
            r.AppendLine();
            r.AppendLine(string.Format("Additional Context:"));
            r.AppendLine(string.Format("{0}", contextStack));

            while (exception.InnerException != null)
            {
                exception = exception.InnerException;

                r.AppendLine();
                r.AppendLine(string.Format("Inner Exception Message: {0}", exception.Message));
                r.AppendLine(string.Format("Inner Exception Type: {0}", exception.GetType().FullName));
                r.AppendLine();
                r.AppendLine(string.Format("Inner Exception Stack:"));
                r.AppendLine(string.Format(exception.StackTrace));
            }

            return r.ToString();
        }

        /// <summary>
        /// Generates a full stack trace from the current method context.
        /// </summary>
        /// <returns></returns>
        public string GenerateFullContextStackTrace()
        {
            StringBuilder stackLog = new StringBuilder();

            var stackTrace = new StackTrace(true);
            var frames = stackTrace.GetFrames();
            
            for (int i = 1; i < frames.Length; i++) // skip the first frame (this method)
            {
                if (frames[i].GetFileName() != null)
                {
                    var filename = frames[i].GetFileName();
                    var method = frames[i].GetMethod();
                    var fileline = frames[i].GetFileLineNumber();

                    stackLog.AppendLine(string.Format("at {0}.{1}({2}) in {3}:line {4}",
                        method.ReflectedType.FullName,
                        method.Name,
                        GetMethodParamsAsString(method),
                        filename,
                        fileline
                    ));
                }
            }

            return stackLog.ToString();
        }

        /// <summary>
        /// Prints a set of method parameters to a string.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private static string GetMethodParamsAsString(System.Reflection.MethodBase method)
        {
            StringBuilder sb = new StringBuilder();

            var mparams = method.GetParameters();

            if (mparams != null)
            {
                foreach (var mp in mparams)
                {
                    sb.Append(mp.ToString() + ",");
                }
            }

            return sb.ToString().TrimEnd(',');
        }
    }
}
