using ArchivialLibrary.Client.Sources;
using ArchivialLibrary.Database;
using ArchivialLibrary.Engine;
using ArchivialLibrary.Events;
using ArchivialLibrary.Logging;
using ArchivialLibrary.Exceptions;
using System;
using System.Linq;
using System.Threading;
using ArchivialLibrary.Folders;
using System.Threading.Tasks;
using ArchivialLibrary.Providers;
using ArchivialLibrary.ServiceCore;

namespace ArchivialLibrary.Client
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
                string[] exclusionPatterns = await GetGlobalExclusionMatchPatternsAsync();
                Scanner = new SourceScanner(Database, Logger, exclusionPatterns);

                while (true)
                {
                    // make sure we actually have at least one storage provider configured.
                    // otherwise scanning files won't be helpful since we can't send them anywhere.

                    if (await StorageProvidersHaveBeenConfiguredAsync().ConfigureAwait(false))
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

        private async Task<string[]> GetGlobalExclusionMatchPatternsAsync()
        {
            var settingName = Constants.RuntimeSettingNames.MasterExclusionMatches;
            var settingValue = await Database.GetApplicationOptionAsync(settingName).ConfigureAwait(false);

            if (settingValue.Length == 0)
            {
                return null;
            }
            else
            {
                return settingValue.Split(';');
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
                var storageProviders = await Database.GetProvidersAsync(ProviderTypes.Storage).ConfigureAwait(false);

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

                if (dbSources.Any(x => x.Path == CoreSettings.DatabaseBackupsDirectory) == false)
                {
                    // add the base meta DB backup folder.

                    var databaseFolderSource = new LocalSourceLocation();
                    databaseFolderSource.Path = CoreSettings.DatabaseBackupsDirectory;
                    databaseFolderSource.FileMatchFilter = "*.bak";
                    databaseFolderSource.RevisionCount = 1;
                    databaseFolderSource.Priority = Files.FileBackupPriority.Meta;
                    databaseFolderSource.DestinationContainerName = "ozette-core-database-backups";

                    await Database.SetSourceLocationAsync(databaseFolderSource).ConfigureAwait(false);
                }

                if (dbSources.Any(x => x.Path == CoreSettings.LogFilesArchiveDirectory) == false)
                {
                    // add the base meta log backup folder.

                    var logFolderSource = new LocalSourceLocation();
                    logFolderSource.Path = CoreSettings.LogFilesArchiveDirectory;
                    logFolderSource.FileMatchFilter = "*.bak";
                    logFolderSource.RevisionCount = 1;
                    logFolderSource.Priority = Files.FileBackupPriority.Meta;
                    logFolderSource.DestinationContainerName = "ozette-core-log-backups";

                    await Database.SetSourceLocationAsync(logFolderSource).ConfigureAwait(false);
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
