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
using OzetteLibrary.Providers;

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
            CancelSource.Cancel();
            Logger.WriteTraceMessage("Scan engine is shutting down by request.", InstanceID);
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
                    // make sure we actually have at least one storage provider configured.
                    // otherwise scanning files won't be helpful since we can't send them anywhere.

                    if (await StorageProvidersHaveBeenConfiguredAsync())
                    {
                        // first: grab current options from the database
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
                                    await Scanner.ScanAsync(source, CancelSource.Token).ConfigureAwait(false);

                                    if (CancelSource.Token.IsCancellationRequested == false)
                                    {
                                        // update the last scanned timestamp if the scan was completed (it wasn't cancelled).
                                        LastHeartbeatOrScanCompleted = DateTime.Now;
                                        await UpdateLastScannedTimeStamp(source).ConfigureAwait(false);
                                    }
                                }

                                if (CancelSource.Token.IsCancellationRequested)
                                {
                                    // stop was requested.
                                    // do not continue scanning any remaining sources.
                                    break;
                                }
                            }
                        }

                        if (LastHeartbeatOrScanCompleted.HasValue == false || LastHeartbeatOrScanCompleted.Value < DateTime.Now.Add(TimeSpan.FromMinutes(-1)))
                        {
                            LastHeartbeatOrScanCompleted = DateTime.Now;
                            Logger.WriteTraceMessage("Scan engine heartbeat: no recent activity.");
                        }
                    }

                    ThreadSleepWithStopRequestCheck(TimeSpan.FromSeconds(60));

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
        /// Checks to see if Storage Providers have been configured in the database.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> StorageProvidersHaveBeenConfiguredAsync()
        {
            try
            {
                var storageProviders = await Database.GetProvidersAsync(ProviderTypes.Storage);

                if (storageProviders.Count > 0)
                {
                    return true;
                }
                else
                {
                    // no providers setup yet.
                    Logger.WriteTraceWarning("No storage providers have been configured. Will be unable to scan sources until at least 1 storage provider is added.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteTraceError("Failed to lookup storage providers.", ex, Logger.GenerateFullContextStackTrace());
                return false;
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

            scan.MetaPriorityScanFrequencyInHours =
                Convert.ToInt32(await db.GetApplicationOptionAsync(Constants.RuntimeSettingNames.MetaPriorityScanFrequencyInHours).ConfigureAwait(false));

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
