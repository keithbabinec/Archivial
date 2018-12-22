using OzetteLibrary.Database;
using OzetteLibrary.Engine;
using OzetteLibrary.Events;
using OzetteLibrary.Logging;
using OzetteLibrary.Providers;
using System;
using System.Collections.Generic;
using System.Threading;

namespace OzetteLibrary.Client
{
    /// <summary>
    /// Contains core status engine functionality.
    /// </summary>
    public class StatusEngine : BaseEngine
    {
        /// <summary>
        /// Constructor that accepts a database and logger.
        /// </summary>
        /// <param name="database">The client database connection.</param>
        /// <param name="logger">A logging instance.</param>
        /// <param name="providerConnections">A collection of cloud backup provider connections.</param>
        public StatusEngine(IClientDatabase database, ILogger logger, ProviderConnectionsCollection providerConnections) : base(database, logger, providerConnections) { }

        /// <summary>
        /// Begins to start the status engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStart()
        {
            if (Running == true)
            {
                throw new InvalidOperationException("The engine cannot be started, it is already running.");
            }

            Running = true;
            StatusCheckTimes = new Queue<DateTime>();

            Thread pl = new Thread(() => ProcessLoop());
            pl.Start();
        }

        /// <summary>
        /// Begins to stop the status engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStop()
        {
            if (Running == true)
            {
                Running = false;
            }
        }

        /// <summary>
        /// A queue that keeps track of the next status update times.
        /// </summary>
        private Queue<DateTime> StatusCheckTimes { get; set; }

        /// <summary>
        /// Core processing loop.
        /// </summary>
        private void ProcessLoop()
        {
            try
            {
                while (true)
                {
                    if (CanSendStatusUpdate())
                    {
                        SendStatusUpdate();
                    }

                    ThreadSleepWithStopRequestCheck(TimeSpan.FromSeconds(60));

                    if (Running == false)
                    {
                        OnStopped(new EngineStoppedEventArgs(EngineStoppedReason.StopRequested));
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                OnStopped(new EngineStoppedEventArgs(ex));
            }
        }

        /// <summary>
        /// Checks to see if we can send the current status to the status provider.
        /// </summary>
        /// <returns></returns>
        private bool CanSendStatusUpdate()
        {
            // add the next status check time, if it isn't already in the queue.

            var scheduleString = Database.GetApplicationOption(Constants.RuntimeSettingNames.StatusUpdateSchedule);
            var schedule = NCrontab.CrontabSchedule.Parse(scheduleString);
            var nextRun = schedule.GetNextOccurrence(DateTime.Now);

            if (StatusCheckTimes.Contains(nextRun) == false)
            {
                StatusCheckTimes.Enqueue(nextRun);
            }

            // now check the status time queue.
            // if the next item in the queue has passed, then dequeue it for processing.

            if (StatusCheckTimes.Count == 0 || StatusCheckTimes.Peek() > DateTime.Now)
            {
                return false;
            }
            else
            {
                // we know that the time has passed.
                // we don't need to do anything with the dequeued object-- just send the status update.
                StatusCheckTimes.Dequeue();
                return true;
            }
        }

        /// <summary>
        /// Sends the status update to the current status provider.
        /// </summary>
        private void SendStatusUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
