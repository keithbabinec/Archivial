using OzetteLibrary.Database;
using OzetteLibrary.Events;
using OzetteLibrary.Logging;
using System;

namespace OzetteLibrary.Client
{
    /// <summary>
    /// Contains core backup engine functionality.
    /// </summary>
    public class BackupEngine
    {
        public BackupEngine(IClientDatabase database, ILogger logger)
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
            StatusLock = new object();
        }

        /// <summary>
        /// Begins to start the backup engine, returns immediately to the caller.
        /// </summary>
        public void BeginStart()
        {
            OnStopped(new EngineStoppedEventArgs(EngineStoppedReason.StopRequested));
        }

        /// <summary>
        /// Begins to stop the backup engine, returns immediately to the caller.
        /// </summary>
        public void BeginStop()
        {
            lock (StatusLock)
            {
                if (StopRequested == false)
                {
                    StopRequested = true;
                }
            }
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
        /// A flag to indicate if a stop has been requested.
        /// </summary>
        private bool StopRequested { get; set; }

        /// <summary>
        /// Thread locking mechanism.
        /// </summary>
        private object StatusLock { get; set; }

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
