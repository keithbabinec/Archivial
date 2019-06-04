using ArchivialLibrary.Database;
using ArchivialLibrary.Events;
using ArchivialLibrary.Logging;
using ArchivialLibrary.ServiceCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ArchivialLibrary.Engine
{
    /// <summary>
    /// Contains core scan engine functionality.
    /// </summary>
    public class RestoreEngine : BaseEngine
    {
        /// <summary>
        /// Constructor that accepts a database and logger.
        /// </summary>
        /// <param name="database">The client database connection.</param>
        /// <param name="logger">A logging instance.</param>
        /// <param name="instanceID">A parameter to specify the engine instance ID.</param>
        /// <param name="coreSettings">The core settings accessor.</param>
        public RestoreEngine(IClientDatabase database,
                          ILogger logger,
                          int instanceID,
                          ICoreSettings coreSettings)
            : base(database, logger, instanceID, coreSettings) { }

        /// <summary>
        /// Begins to start the scanning engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStart()
        {
            Logger.WriteTraceMessage("Scan engine is starting up.");

            Thread pl = new Thread(() => ProcessLoopAsync().Wait());
            pl.Start();

            Logger.WriteTraceMessage("Scan engine is now running.");
        }

        /// <summary>
        /// Begins to stop the scanning engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStop()
        {
            CancelSource.Cancel();
            Logger.WriteTraceMessage("Scan engine is shutting down by request.", InstanceID);
        }

        /// <summary>
        /// The last time a heartbeat message or scan of a folder was completed.
        /// </summary>
        private DateTime? LastHeartbeatOrActivityCompleted { get; set; }

        /// <summary>
        /// Core processing loop.
        /// </summary>
        private async Task ProcessLoopAsync()
        {
            try
            {
                while (true)
                {
                    // TODO: setup cloud storage provider connections, if required.

                    // TODO: perform restore activity work, if required.

                    if (LastHeartbeatOrActivityCompleted.HasValue == false || LastHeartbeatOrActivityCompleted.Value < DateTime.Now.Add(TimeSpan.FromMinutes(-1)))
                    {
                        LastHeartbeatOrActivityCompleted = DateTime.Now;
                        Logger.WriteTraceMessage("Restore engine heartbeat: no recent activity.");
                    }

                    await WaitAsync(TimeSpan.FromMinutes(1)).ConfigureAwait(false);

                    if (CancelSource.Token.IsCancellationRequested)
                    {
                        OnStopped(new EngineStoppedEventArgs(EngineStoppedReason.StopRequested, InstanceID));
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                OnStopped(new EngineStoppedEventArgs(ex, InstanceID));
            }
        }
    }
}
