using OzetteLibrary.Client.Sources;
using OzetteLibrary.Database;
using OzetteLibrary.Engine;
using OzetteLibrary.Events;
using OzetteLibrary.Logging;
using OzetteLibrary.Exceptions;
using System;
using System.Threading;
using OzetteLibrary.Folders;
using System.Threading.Tasks;

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
        /// <param name="database">The client database connection.</param>
        /// <param name="logger">A logging instance.</param>
        /// <param name="instanceID">A parameter to specify the engine instance ID.</param>
        public ScanEngine(IClientDatabase database,
                          ILogger logger,
                          int instanceID)
            : base(database, logger, instanceID) { }

        /// <summary>
        /// Begins to start the scanning engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStart()
        {
            if (Running == true)
            {
                throw new InvalidOperationException("The engine cannot be started, it is already running.");
            }

            Running = true;
            Scanner = new SourceScanner(Database, Logger);

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
            if (Running == true)
            {
                Logger.WriteTraceMessage("Scan engine is shutting down.");
                Running = false;
            }
        }

        /// <summary>
        /// The source scanning instance.
        /// </summary>
        private SourceScanner Scanner { get; set; }

        /// <summary>
        /// The last time a heartbeat message or scan of a folder was completed.
        /// </summary>
        private DateTime? LastHeartbeatOrScanCompleted { get; set; }

        /// <summary>
        /// Core processing loop.
        /// </summary>
        private async Task ProcessLoopAsync()
        {
            try
            {
                while (true)
                {
                    // first: grab current options from the database

                    var sourcesFilePath = await Database.GetApplicationOptionAsync(Constants.RuntimeSettingNames.SourcesFilePath).ConfigureAwait(false);
                    var providersFilePath = await Database.GetApplicationOptionAsync(Constants.RuntimeSettingNames.ProvidersFilePath).ConfigureAwait(false);
                    var scanOptions = await GetScanFrequenciesAsync(Database).ConfigureAwait(false);

                    // second: check to see if we have any valid sources defined.
                    // the sources found are returned in the order they should be scanned.

                    var sources = await SafeImportSourcesAsync().ConfigureAwait(false);

                    if (sources != null)
                    {
                        foreach (var source in sources)
                        {
                            // should we actually scan this source?
                            // checks the DB to see if it has been scanned recently.

                            if (source.ShouldScan(scanOptions))
                            {
                                if (source is NetworkSourceLocation && !(source as NetworkSourceLocation).IsConnected)
                                {
                                    // this is a network source and in a disconnected or failed state
                                    // scanning won't be possible at this time.
                                    Logger.WriteTraceMessage(string.Format("Unable to scan network source: {0}. It is currently disconnected or unreachable.", source.Path));
                                    continue;
                                }

                                // invoke the scan
                                await Scanner.ScanAsync(source).ConfigureAwait(false);

                                // update the last scanned timestamp.
                                LastHeartbeatOrScanCompleted = DateTime.Now;
                                await UpdateLastScannedTimeStamp(source).ConfigureAwait(false);
                            }

                            if (Running == false)
                            {
                                // stop was requested.
                                // do not continue scanning any remaining sources.
                                break;
                            }
                        }
                    }

                    ThreadSleepWithStopRequestCheck(TimeSpan.FromSeconds(60));

                    if (LastHeartbeatOrScanCompleted.HasValue == false || LastHeartbeatOrScanCompleted.Value < DateTime.Now.Add(TimeSpan.FromMinutes(-1)))
                    {
                        LastHeartbeatOrScanCompleted = DateTime.Now;
                        Logger.WriteTraceMessage("Scan engine heartbeat: no recent activity.");
                    }

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
        /// Pull the current scanning options from the database.
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private async Task<ScanFrequencies> GetScanFrequenciesAsync(IClientDatabase db)
        {
            ScanFrequencies scan = new ScanFrequencies();

            scan.LowPriorityScanFrequencyInHours = 
                Convert.ToInt32(await db.GetApplicationOptionAsync(Constants.RuntimeSettingNames.LowPriorityScanFrequencyInHours).ConfigureAwait(false));

            scan.MedPriorityScanFrequencyInHours =
                Convert.ToInt32(await db.GetApplicationOptionAsync(Constants.RuntimeSettingNames.MedPriorityScanFrequencyInHours).ConfigureAwait(false));

            scan.HighPriorityScanFrequencyInHours =
                Convert.ToInt32(await db.GetApplicationOptionAsync(Constants.RuntimeSettingNames.HighPriorityScanFrequencyInHours).ConfigureAwait(false));

            return scan;
        }

        /// <summary>
        /// Imports sources for scanning.
        /// </summary>
        /// <remarks>
        /// This function is marked as safe and should not throw exceptions.
        /// </remarks>
        /// <returns></returns>
        private async Task<SourceLocations> SafeImportSourcesAsync()
        {
            try
            {
                // grab the current copy from DB (this includes last scanned timestamp)
                var dbSources = await GetSourceLocationsFromDatabaseAsync().ConfigureAwait(false);

                if (dbSources == null || dbSources.Count == 0)
                {
                    Logger.WriteTraceMessage("No scan sources are defined. No files will be backed up until scan sources have been added.");
                    return null;
                }

                return dbSources;
            }
            catch (Exception ex)
            {
                string err = "Failed to import scan sources.";
                Logger.WriteSystemEvent(err, ex, Logger.GenerateFullContextStackTrace(), Constants.EventIDs.FailedToLoadScanSources, true);

                return null;
            }
        }

        /// <summary>
        /// Updates the last completed scan timestamp in the database for the specified source.
        /// </summary>
        /// <param name="source"></param>
        private async Task UpdateLastScannedTimeStamp(SourceLocation source)
        {
            source.LastCompletedScan = DateTime.Now;
            await Database.SetSourceLocationAsync(source).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a copy of the source locations from the database.
        /// </summary>
        private async Task<SourceLocations> GetSourceLocationsFromDatabaseAsync()
        {
            return await Database.GetSourceLocationsAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Validates the provided sources are usable.
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        private bool ValidateSources(SourceLocations sources)
        {
            if (sources == null)
            {
                return false;
            }

            try
            {
                Logger.WriteTraceMessage(string.Format("Validating {0} scan source(s).", sources.Count));
                sources.Validate();
                Logger.WriteTraceMessage("All scan sources validated.");

                return true;
            }
            catch (SourceLocationException ex)
            {
                var msg = ConvertToFriendlyError(ex);
                Logger.WriteSystemEvent(msg, System.Diagnostics.EventLogEntryType.Error, Constants.EventIDs.FailedToValidateScanSources, true);

                return false;
            }
        }

        /// <summary>
        /// Converts an exception object into a friendly error message.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private string ConvertToFriendlyError(SourceLocationException ex)
        {
            if (ex is SourceLocationInvalidFileMatchFilterException)
            {
                return string.Format(
                    "Failed to validate scan sources: A source location has an invalid (or missing) file match filter. The invalid source was: {0}",
                    ex.Message
                );
            }
            else if (ex is SourceLocationInvalidLocalFolderPathException)
            {
                return string.Format(
                    "Failed to validate scan sources: A source location has an invalid (or missing) folder path. The invalid source was: {0}",
                    ex.Message
                );
            }
            else if (ex is SourceLocationInvalidIDException)
            {
                return string.Format(
                    "Failed to validate scan sources: A source location has an invalid (or missing) ID. The invalid source was: {0}",
                    ex.Message
                );
            }
            else if (ex is SourceLocationInvalidRevisionCountException)
            {
                return string.Format(
                    "Failed to validate scan sources: A source location has an invalid (or missing) revision count. The invalid source was: {0}",
                    ex.Message
                );
            }
            else if (ex is SourceLocationsDuplicateIDException)
            {
                return string.Format(
                    "Failed to validate scan sources: More than one source location shares the same ID. The invalid source was: {0}",
                    ex.Message
                );
            }
            else
            {
                throw new InvalidOperationException("Unexpected exception type provided: " + ex.GetType().FullName);
            }
        }
    }
}
