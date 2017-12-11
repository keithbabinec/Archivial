using OzetteLibrary.Database;
using OzetteLibrary.Events;
using OzetteLibrary.Logging;
using System;

namespace OzetteLibrary.Client
{
    /// <summary>
    /// Contains core scan engine functionality.
    /// </summary>
    public class ScanEngine
    {
        public ScanEngine(IClientDatabase database, ILogger logger)
        {
            if (database == null)
            {
                throw new ArgumentNullException(nameof(database));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Database = database;
            Logger = logger;
        }

        /// <summary>
        /// Begins to start the scanning engine, returns immediately to the caller.
        /// </summary>
        public void BeginStart()
        {
        }

        /// <summary>
        /// Begins to stop the scanning engine, returns immediately to the caller.
        /// </summary>
        public void BeginStop()
        {
            Stopped?.Invoke(this, new EngineStoppedEventArgs());
        }

        /// <summary>
        /// This event is triggered when the engine has been stopped.
        /// </summary>
        public event EventHandler<EngineStoppedEventArgs> Stopped;

        /// <summary>
        /// Internal function to invoke the Stopped event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnStopped(EngineStoppedEventArgs e)
        {
            Stopped?.Invoke(this, e);
        }

        /// <summary>
        /// A reference to the database.
        /// </summary>
        private IClientDatabase Database { get; set; }

        /// <summary>
        /// A reference to the logger.
        /// </summary>
        private ILogger Logger { get; set; }
    }
}
