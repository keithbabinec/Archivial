using OzetteLibrary.Database;
using OzetteLibrary.Engine;
using OzetteLibrary.Events;
using OzetteLibrary.Logging;

namespace OzetteLibrary.Client
{
    /// <summary>
    /// Contains core backup engine functionality.
    /// </summary>
    public class BackupEngine : BaseEngine
    {
        /// <summary>
        /// Constructor that accepts a database and logger.
        /// </summary>
        /// <param name="database"><c>IDatabase</c></param>
        /// <param name="logger"><c>ILogger</c></param>
        public BackupEngine(IDatabase database, ILogger logger) : base(database, logger) { }

        /// <summary>
        /// Begins to start the backup engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStart()
        {
            OnStopped(new EngineStoppedEventArgs(EngineStoppedReason.StopRequested));
        }

        /// <summary>
        /// Begins to stop the backup engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStop()
        {
            lock (StatusLock)
            {
                if (StopRequested == false)
                {
                    StopRequested = true;
                }
            }
        }
    }
}
