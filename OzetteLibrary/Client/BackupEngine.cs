using OzetteLibrary.Client.Transfer;
using OzetteLibrary.Database;
using OzetteLibrary.Engine;
using OzetteLibrary.Events;
using OzetteLibrary.Logging;
using OzetteLibrary.Models;
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
            Sender = new FileSender(Database as IClientDatabase, Logger);

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

                    ClientFile nextFileToBackup = SafeGetNextFileToBackup();

                    if (nextFileToBackup != null)
                    {
                        // initiate the file-send operation.

                        AsyncResult state = Sender.BeginSend(nextFileToBackup);

                        while (state.IsCompleted == false)
                        {
                            ThreadSleepWithStopRequestCheck(TimeSpan.FromSeconds(2));
                            if (Running == false)
                            {
                                // stop was requested.
                                // stop the currently in-progress file send operation.

                                Sender.StopSend();
                                break;
                            }
                        }

                        // do not sleep here.
                        // immediately move to backing up the next file.
                    }
                    else
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(5));
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
        /// <returns></returns>
        private ClientFile SafeGetNextFileToBackup()
        {
            throw new NotImplementedException();
        }
    }
}
