using OzetteLibrary.Database;
using OzetteLibrary.Events;
using OzetteLibrary.Logging;
using System;

namespace OzetteLibrary.Engine
{
    /// <summary>
    /// An abstract base class for deriving custom Engines.
    /// </summary>
    /// <remarks>
    /// The base engine contains implemented functionality common to all engines, such as logging, synchronization, and events.
    /// </remarks>
    public abstract class BaseEngine
    {
        /// <summary>
        /// Constructor that accepts a database and logger.
        /// </summary>
        /// <param name="database"><c>IDatabase</c></param>
        /// <param name="logger"><c>ILogger</c></param>
        protected BaseEngine(IDatabase database, ILogger logger)
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
        /// This event is triggered when the engine has been stopped.
        /// </summary>
        public event EventHandler<EngineStoppedEventArgs> Stopped;

        /// <summary>
        /// Begins to start the engine, returns immediately to the caller.
        /// </summary>
        abstract public void BeginStart();

        /// <summary>
        /// Begins to stop the engine, returns immediately to the caller.
        /// </summary>
        abstract public void BeginStop();

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
        protected bool StopRequested { get; set; }

        /// <summary>
        /// Thread locking mechanism.
        /// </summary>
        protected object StatusLock { get; set; }

        /// <summary>
        /// A reference to the logger.
        /// </summary>
        protected ILogger Logger { get; set; }

        /// <summary>
        /// A reference to the database.
        /// </summary>
        protected IDatabase Database { get; set; }
    }
}
