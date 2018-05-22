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
        private ILogger Logger { get; set; }

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
        public Initialization(ILogger logger)
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

            if (!SafeStartLogger(Options, Logger))
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
        /// Safely starts the logger.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private bool SafeStartLogger(ServiceOptions options, ILogger logger)
        {
            try
            {
                logger.Start(options.EventlogName, options.EventlogName, options.LogFilesDirectory);
                return true;
            }
            catch
            {
                ResultCode = StartupResults.FailedToConfigureLogging;
                OnCompleted(EventArgs.Empty);
                return false;
            }
        }
    }
}
