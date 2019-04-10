using OzetteLibrary.Database;
using OzetteLibrary.Engine;
using OzetteLibrary.Events;
using OzetteLibrary.Logging;
using OzetteLibrary.MessagingProviders;
using OzetteLibrary.Providers;
using OzetteLibrary.ServiceCore;
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
        /// <param name="instanceID">A parameter to specify the engine instance ID.</param>
        public StatusEngine(IClientDatabase database,
                            ILogger logger,
                            int instanceID)
            : base(database, logger, instanceID) { }

        /// <summary>
        /// Begins to start the status engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStart()
        {
            StatusCheckTimes = new Queue<DateTime>();

            Thread pl = new Thread(() => ProcessLoopAsync().Wait());
            pl.Start();
        }

        /// <summary>
        /// Begins to stop the status engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStop()
        {
            CancelSource.Cancel();
            Logger.WriteTraceMessage("Status engine is shutting down by request.", InstanceID);
        }

        /// <summary>
        /// A queue that keeps track of the next status update times.
        /// </summary>
        private Queue<DateTime> StatusCheckTimes { get; set; }

        /// <summary>
        /// A collection of messaging provider connections.
        /// </summary>
        private MessagingProviderConnectionsCollection MessagingConnections { get; set; }

        /// <summary>
        /// Core processing loop.
        /// </summary>
        private async Task ProcessLoopAsync()
        {
            try
            {
                while (true)
                {
                    if (await MessagingProvidersAreConfiguredAsync().ConfigureAwait(false))
                    {
                        if (await CanSendStatusUpdateAsync().ConfigureAwait(false))
                        {
                            Logger.WriteTraceMessage("Querying for backup progress.");
                            var progress = await Database.GetBackupProgressAsync().ConfigureAwait(false);
                            await SendStatusUpdateAsync(progress).ConfigureAwait(false);
                        }
                    }

                    await WaitAsync(TimeSpan.FromSeconds(60)).ConfigureAwait(false);

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

        /// <summary>
        /// Checks to see if messaging providers are configured.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> MessagingProvidersAreConfiguredAsync()
        {
            try
            {
                if (MessagingConnections != null && MessagingConnections.Count > 0)
                {
                    // if we have already setup the connections, then we are configured.
                    return true;
                }
                else
                {
                    // otherwise check the database to see if we have any providers.

                    var messagingProviders = await Database.GetProvidersAsync(ProviderTypes.Messaging).ConfigureAwait(false);

                    if (messagingProviders.Count > 0)
                    {
                        // attemp to configure the providers.
                        var connections = new ProviderConnections(Database);
                        var messageProviderConnections = await connections.ConfigureMessagingProviderConnectionsAsync(Logger).ConfigureAwait(false);

                        if (messageProviderConnections != null)
                        {
                            MessagingConnections = messageProviderConnections;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // no providers setup yet.
                        Logger.WriteTraceMessage("No messaging providers have been configured yet. The status engine won't work until these have been configured.");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteTraceError("Failed to lookup or configure messaging providers.", ex, Logger.GenerateFullContextStackTrace());
                return false;
            }
        }

        /// <summary>
        /// Checks to see if we can send the current status to the status provider.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> CanSendStatusUpdateAsync()
        {
            // add the next status check time, if it isn't already in the queue.

            var scheduleString = await Database.GetApplicationOptionAsync(Constants.RuntimeSettingNames.StatusUpdateSchedule).ConfigureAwait(false);
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
            if (MessagingConnections.Count == 0)
            {
                Logger.WriteTraceWarning("Unable to send status updates. There are no messaging providers configured/enabled.");
                return;
            }

            foreach (var messageProvider in MessagingConnections)
            {
                Logger.WriteTraceMessage("Attempting to send status update for messaging provider: " + messageProvider.Key);

                try
                {
                    await messageProvider.Value.SendBackupProgressStatusMessageAsync(Progress).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger.WriteTraceError("Failed to send a backup progress update message: " + ex.ToString());
                }
            }
        }
    }
}
