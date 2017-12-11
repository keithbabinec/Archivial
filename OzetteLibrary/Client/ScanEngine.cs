using OzetteLibrary.Database;
using OzetteLibrary.Engine;
using OzetteLibrary.Events;
using OzetteLibrary.Logging;

namespace OzetteLibrary.Client
{
    /// <summary>
    /// Contains core scan engine functionality.
    /// </summary>
    public class ScanEngine : BaseEngine
    {
        /// <summary>
        /// Constructor that accepts a database and logger.
        /// </summary>
        /// <param name="database"><c>IDatabase</c></param>
        /// <param name="logger"><c>ILogger</c></param>
        public ScanEngine(IDatabase database, ILogger logger) : base(database, logger) { }

        /// <summary>
        /// Begins to start the scanning engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStart()
        {
            OnStopped(new EngineStoppedEventArgs(EngineStoppedReason.StopRequested));
        }

        /// <summary>
        /// Begins to stop the scanning engine, returns immediately to the caller.
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
