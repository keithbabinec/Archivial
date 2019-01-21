using OzetteLibrary.Database;
using OzetteLibrary.Engine;
using OzetteLibrary.Events;
using OzetteLibrary.Logging;
using OzetteLibrary.MessagingProviders;
using OzetteLibrary.ServiceCore;
using OzetteLibrary.StorageProviders;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
        /// <param name="storageProviders">A collection of cloud backup storage provider connections.</param>
        /// <param name="messagingProviders">A collection of messaging provider connections.</param>
        /// <param name="instanceID">A parameter to specify the engine instance ID.</param>
        public StatusEngine(IClientDatabase database,
                            ILogger logger,
                            StorageProviderConnectionsCollection storageProviders,
                            MessagingProviderConnectionsCollection messagingProviders,
                            int instanceID)
            : base(database, logger, storageProviders, messagingProviders, instanceID) { }

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

            Thread pl = new Thread(() => ProcessLoopAsync().Wait());
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
        private async Task ProcessLoopAsync()
        {
            try
            {
                while (true)
                {
                    if (await CanSendStatusUpdateAsync())
                    {
                        Logger.WriteTraceMessage("Querying for backup progress.");
                        var progress = await Database.GetBackupProgressAsync();
                        await SendStatusUpdateAsync(progress);
                    }

                    ThreadSleepWithStopRequestCheck(TimeSpan.FromSeconds(60));

                    if (Running == false)
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

        /// <summary>
        /// Checks to see if we can send the current status to the status provider.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> CanSendStatusUpdateAsync()
        {
            // add the next status check time, if it isn't already in the queue.

            var scheduleString = await Database.GetApplicationOptionAsync(Constants.RuntimeSettingNames.StatusUpdateSchedule);
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
        private async Task SendStatusUpdateAsync(BackupProgress Progress)
        {
            if (MessagingProviders.Count == 0)
            {
                Logger.WriteTraceWarning("Unable to send status updates. There are no messaging providers configured/enabled.");
                return;
            }

            foreach (var messageProvider in MessagingProviders)
            {
                Logger.WriteTraceMessage("Attempting to send status update for messaging provider: " + messageProvider.Key);

                try
                {
                    await messageProvider.Value.SendBackupProgressStatusMessageAsync(Progress);
                }
                catch (Exception ex)
                {
                    Logger.WriteTraceError("Failed to send a backup progress update message: " + ex.ToString());
                }
            }
        }
    }
}
