using OzetteLibrary.Client.Transfer;
using OzetteLibrary.Database;
using OzetteLibrary.Engine;
using OzetteLibrary.Events;
using OzetteLibrary.Files;
using OzetteLibrary.Logging;
using OzetteLibrary.Providers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
        /// <param name="database">The client database connection.</param>
        /// <param name="logger">A logging instance.</param>
        /// <param name="providerConnections">A collection of cloud backup provider connections.</param>
        public BackupEngine(IDatabase database, ILogger logger, ProviderConnectionsCollection providerConnections) : base(database, logger, providerConnections) { }

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
            Sender = new FileSender(Database as IClientDatabase, Logger, Providers);

            Logger.WriteTraceMessage("BackupEngine is starting up.");

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
                Logger.WriteTraceMessage("BackupEngine is shutting down.");
                Running = false;
            }
        }

        /// <summary>
        /// The file copy/transfer utility.
        /// </summary>
        private FileSender Sender { get; set; }

        /// <summary>
        /// Core processing loop.
        /// </summary>
        private void ProcessLoop()
        {
            try
            {
                while (true)
                {
                    // check to see if we have any files to backup.
                    // return the next one to backup.

                    BackupFile nextFileToBackup = SafeGetNextFileToBackup();

                    if (nextFileToBackup != null)
                    {
                        // initiate the file-send operation.

                        var cancel = new CancellationTokenSource();
                        var transferFinished = false;
                        var transferTask = Sender.TransferAsync(nextFileToBackup, cancel.Token);

                        while (!transferFinished)
                        {
                            ThreadSleepWithStopRequestCheck(TimeSpan.FromSeconds(2));

                            switch (transferTask.Status)
                            {
                                case TaskStatus.RanToCompletion:
                                case TaskStatus.Faulted:
                                case TaskStatus.Canceled:
                                    {
                                        // task has completed, failed, or canceled.
                                        // quit the status loop.
                                        transferFinished = true;
                                        break;
                                    }
                                default:
                                    {
                                        // task is still starting or running.
                                        // do nothing here.
                                        break;
                                    }
                            }

                            if (Running == false)
                            {
                                // stop was requested.
                                // stop the currently in-progress file send operation.
                                cancel.Cancel();
                            }
                        }

                        // do not sleep here.
                        // immediately move to backing up the next file.
                    }
                    else
                    {
                        ThreadSleepWithStopRequestCheck(TimeSpan.FromSeconds(60));
                    }

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
        /// Grabs the next file that needs to be backed up.
        /// </summary>
        /// <remarks>
        /// This function is marked as safe and should not throw exceptions.
        /// </remarks>
        /// <returns></returns>
        private BackupFile SafeGetNextFileToBackup()
        {
            try
            {
                var clientDB = Database as IClientDatabase;
                return clientDB.GetNextFileToBackup();
            }
            catch (Exception ex)
            {
                string err = "Failed to capture the next file ready for backup.";
                Logger.WriteSystemEvent(err, ex, Logger.GenerateFullContextStackTrace(), Constants.EventIDs.FailedToGetNextFileToBackup, true);

                return null;
            }
        }
    }
}
