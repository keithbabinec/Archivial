using OzetteLibrary.Database;
using OzetteLibrary.Engine;
using OzetteLibrary.Events;
using OzetteLibrary.Logging;
using OzetteLibrary.ServiceCore;
using System;
using System.Threading;

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
        /// <param name="options"><c>ServiceOptions</c></param>
        public BackupEngine(IDatabase database, ILogger logger, ServiceOptions options) : base(database, logger, options) { }

        /// <summary>
        /// Begins to start the backup engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStart()
        {
            if (Running == true)
            {
                throw new InvalidOperationException("The engine cannot be started, it is already running.");
            }

            Running = true;

            Thread pl = new Thread(() => ProcessLoop());
            pl.Start();
        }

        /// <summary>
        /// Begins to stop the backup engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStop()
        {
            if (Running == true)
            {
                Running = false;
            }
        }

        /// <summary>
        /// Core processing loop.
        /// </summary>
        private void ProcessLoop()
        {
            while (true)
            {
                // todo: implement engine core loop

                Thread.Sleep(TimeSpan.FromSeconds(3));

                if (Running == false)
                {
                    OnStopped(new EngineStoppedEventArgs(EngineStoppedReason.StopRequested));
                    break;
                }
            }
        }
    }
}
