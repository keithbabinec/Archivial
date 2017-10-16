using System;
using System.Configuration;
using OzetteLibrary.Logging;
using System.Threading;

namespace OzetteLibrary.ServiceCore
{
    /// <summary>
    /// Contains windows service initialization functionality.
    /// </summary>
    public class Initialization
    {
        /// <summary>
        /// A result code (success, error, etc)
        /// </summary>
        public StartupResults ResultCode { get; set; }

        /// <summary>
        /// A collection of options parsed from the provided settings input.
        /// </summary>
        public ServiceOptions Options { get; set; }

        /// <summary>
        /// This event is triggered when Start() has completed.
        /// </summary>
        public event EventHandler Completed;

        /// <summary>
        /// Stores an instance of the logging setup class.
        /// </summary>
        private IEventLogSetup Logger { get; set; }

        /// <summary>
        /// Internal function to invoke the Completed event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnCompleted(EventArgs e)
        {
            Completed?.Invoke(this, e);
        }

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="logger"></param>
        public Initialization(IEventLogSetup logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Logger = logger;
        }

        /// <summary>
        /// Starts the service initialization (asynchronously).
        /// </summary>
        /// <param name="properties"></param>
        public void BeginStart(SettingsPropertyCollection properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            Thread initThread = new Thread(() => Start(properties));
            initThread.Start();
        }

        /// <summary>
        /// Starts the service initialization (synchronously).
        /// </summary>
        /// <param name="properties"></param>
        private void Start(SettingsPropertyCollection properties)
        {
            // perform service launch tasks

            if (!SafeParseOptions(properties))
            {
                return;
            }

            if (!SafeSetupLogsFolder(Options, Logger))
            {
                return;
            }

            if (!SafeSetupCustomEventLog(Options, Logger))
            {
                return;
            }

            ResultCode = StartupResults.Success;
            OnCompleted(EventArgs.Empty);
        }

        /// <summary>
        /// Safely parse settings options.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        private bool SafeParseOptions(SettingsPropertyCollection properties)
        {
            try
            {
                Options = new ServiceOptions(properties);
                return true;
            }
            catch
            {
                ResultCode = StartupResults.FailedToParseServiceOptions;
                OnCompleted(EventArgs.Empty);
                return false;
            }
        }

        /// <summary>
        /// Safely setup the logs folder.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private bool SafeSetupLogsFolder(ServiceOptions options, IEventLogSetup logger)
        {
            try
            {
                logger.SetupLogsFolderIfNotPresent(Options.LogFilesDirectory);
                return true;
            }
            catch
            {
                ResultCode = StartupResults.FailedToConfigureLogsFolder;
                OnCompleted(EventArgs.Empty);
                return false;
            }
        }

        /// <summary>
        /// Safely setup the custom event log.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private bool SafeSetupCustomEventLog(ServiceOptions options, IEventLogSetup logger)
        {
            try
            {
                logger.SetupCustomWindowsEventLogIfNotPresent(Options.EventlogName, Options.EventlogName);
                return true;
            }
            catch
            {
                ResultCode = StartupResults.FailedToConfigureCustomEventLog;
                OnCompleted(EventArgs.Empty);
                return false;
            }
        }
    }
}
