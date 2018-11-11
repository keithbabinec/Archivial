using OzetteLibrary.Database;
using OzetteLibrary.Engine;
using OzetteLibrary.Logging;
using OzetteLibrary.Providers;
using System;
using System.Threading;

namespace OzetteLibrary.Client
{
    /// <summary>
    /// Contains core connection engine functionality.
    /// </summary>
    public class ConnectionEngine : BaseEngine
    {
        /// <summary>
        /// Constructor that accepts a database and logger.
        /// </summary>
        /// <param name="database">The client database connection.</param>
        /// <param name="logger">A logging instance.</param>
        /// <param name="providerConnections">A collection of cloud backup provider connections.</param>
        public ConnectionEngine(IDatabase database, ILogger logger, ProviderConnectionsCollection providerConnections) : base(database, logger, providerConnections) { }

        /// <summary>
        /// Begins to start the connection engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStart()
        {
            if (Running == true)
            {
                throw new InvalidOperationException("The engine cannot be started, it is already running.");
            }

            Running = true;

            Logger.WriteTraceMessage("Connection engine is starting up.");

            Thread pl = new Thread(() => ProcessLoop());
            pl.Start();

            Logger.WriteTraceMessage("Connection engine is now running.");
        }

        /// <summary>
        /// Begins to stop the connection engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStop()
        {
            if (Running == true)
            {
                Logger.WriteTraceMessage("Connection engine is shutting down.");
                Running = false;
            }
        }

        /// <summary>
        /// Core processing loop.
        /// </summary>
        private void ProcessLoop()
        {
        }
    }
}
