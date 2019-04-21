using ArchivialLibrary.Database;
using ArchivialLibrary.Events;
using ArchivialLibrary.Logging;
using ArchivialLibrary.MessagingProviders;
using ArchivialLibrary.StorageProviders;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ArchivialLibrary.Engine
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
        /// <param name="database">The client database connection.</param>
        /// <param name="logger">A logging instance.</param>
        /// <param name="instanceID">A parameter to specify the engine instance ID.</param>
        protected BaseEngine(IClientDatabase database, ILogger logger, int instanceID)
        {
            // note: its ok to have no messaging providers (zero count).
            // it is not ok to have zero backup providers.

            if (database == null)
            {
                throw new ArgumentNullException(nameof(database));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            if (instanceID < 0)
            {
                throw new ArgumentException(nameof(instanceID) + " must be a positive integer.");
            }

            Database = database;
            Logger = logger;
            InstanceID = instanceID;
            CancelSource = new CancellationTokenSource();
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
        /// An instance ID for the engine.
        /// </summary>
        protected int InstanceID { get; set; }

        /// <summary>
        /// A reference to the logger.
        /// </summary>
        protected ILogger Logger { get; set; }

        /// <summary>
        /// A reference to the database.
        /// </summary>
        protected IClientDatabase Database { get; set; }

        /// <summary>
        /// A reference to the engine cancellation token source.
        /// </summary>
        protected CancellationTokenSource CancelSource { get; set; }

        /// <summary>
        /// Sleeps the engine for the specified time.
        /// </summary>
        /// <param name="SleepTime"></param>
        protected async Task WaitAsync(TimeSpan SleepTime)
        {
            try
            {
                await Task.Delay(SleepTime, CancelSource.Token).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                // no-op.
                // in this situation something has requested cancellation. 
                // just let this exit safely.
            }
        }
    }
}
