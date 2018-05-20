using OzetteLibrary.Client.Sources;
using OzetteLibrary.Database;
using OzetteLibrary.Engine;
using OzetteLibrary.Events;
using OzetteLibrary.Logging;
using OzetteLibrary.Models;
using OzetteLibrary.Models.Exceptions;
using OzetteLibrary.Providers;
using OzetteLibrary.ServiceCore;
using System;
using System.Collections.Generic;
using System.Threading;

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
        /// <param name="database"><c>IDatabase</c></param>
        /// <param name="logger"><c>ILogger</c></param>
        /// <param name="options"><c>ServiceOptions</c></param>
        public ScanEngine(IDatabase database, ILogger logger, ServiceOptions options) : base(database, logger, options) { }

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
            Scanner = new SourceScanner(Database as IClientDatabase, Logger);

            Thread pl = new Thread(() => ProcessLoop());
            pl.Start();
        }

        /// <summary>
        /// Begins to stop the scanning engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStop()
        {
            if (Running == true)
            {
                Running = false;
            }
        }

        /// <summary>
        /// The source scanning instance.
        /// </summary>
        private SourceScanner Scanner { get; set; }

        /// <summary>
        /// Core processing loop.
        /// </summary>
        private void ProcessLoop()
        {
            try
            {
                while (true)
                {
                    // first: check to see if we have any valid sources defined.
                    // the sources found are returned in the order they should be scanned.

                    var sources = SafeImportSources(Options.SourcesFilePath);

                    // second: check to see if we have any valid providers defined.
                    // we can't backup to any target locations without providers loaded.

                    bool loadedProviders = SafeImportProviders(Options.ProviderOptionsFilePath);

                    if (sources != null && loadedProviders)
                    {
                        foreach (var source in sources)
                        {
                            // should we actually scan this source?
                            // checks the DB to see if it has been scanned recently.
                            WriteLastScannedInfoToTraceLog(source);

                            if (ShouldScanSource(source))
                            {
                                // begin-invoke the asynchronous scan operation.
                                // watch the IAsyncResult status object to check for status updates
                                // and wait until the scan has completed.

                                AsyncResult state = Scanner.BeginScan(source);
                                while (state.IsCompleted == false)
                                {
                                    ThreadSleepWithStopRequestCheck(TimeSpan.FromSeconds(2));
                                    if (Running == false)
                                    {
                                        // stop was requested.
                                        // stop the currently in-progress scanning operation.
                                        Scanner.StopScan();
                                        break;
                                    }
                                }

                                if (state.IsCanceled == false)
                                {
                                    // the scan completed successfully (no cancel)
                                    // update the last scanned timestamp.
                                    UpdateLastScannedTimeStamp(source);
                                }
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
        /// Imports sources for scanning.
        /// </summary>
        /// <remarks>
        /// This function is marked as safe and should not throw exceptions.
        /// </remarks>
        /// <param name="sourceFile"></param>
        /// <returns></returns>
        private SourceLocations SafeImportSources(string sourceFile)
        {
            try
            {
                Logger.WriteTraceMessage("Importing scan sources from: " + sourceFile);

                Loader loader = new Loader();
                SourceLocations result = loader.LoadSourcesFile(sourceFile);

                Logger.WriteTraceMessage("Successfully loaded scan source file.");

                if (result == null || result.Count == 0)
                {
                    Logger.WriteTraceMessage("No sources defined in the scan source file.");
                }

                if (ValidateSources(result) == true)
                {
                    // have the source locations changed from what we have in the client DB?
                    // if yes, then refresh/update them.
                    RefreshDatabaseSourcesIfChanged(result);

                    // grab the current copy from DB (this includes last scanned timestamp)
                    var dbSources = GetSourceLocationsFromDatabase();

                    // return sorted sources.
                    return loader.SortSources(dbSources);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                string err = "Failed to import scan sources.";
                Logger.WriteTraceError(err, ex, Logger.GenerateFullContextStackTrace());
                Logger.WriteSystemEvent(err, ex, Logger.GenerateFullContextStackTrace(), Constants.EventIDs.FailedToLoadScanSources);

                return null;
            }
        }

        /// <summary>
        /// Imports providers into the database.
        /// </summary>
        /// <remarks>
        /// This function is marked as safe and should not throw exceptions.
        /// </remarks>
        /// <param name="providersFile"></param>
        /// <returns></returns>
        private bool SafeImportProviders(string providersFile)
        {
            try
            {
                Logger.WriteTraceMessage("Importing provider options sources from: " + providersFile);

                ProviderOptionsLoader loader = new ProviderOptionsLoader();
                List<ProviderOptions> result = loader.LoadOptionsFile(providersFile);

                Logger.WriteTraceMessage("Successfully loaded provider options file.");

                if (result == null || result.Count == 0)
                {
                    Logger.WriteTraceMessage("No provider options defined in the scan source file.");
                    return false;
                }

                if (ValidateProviderOptions(result) == true)
                {
                    // always ensure the providers are updated in the database.
                    var clientDB = Database as IClientDatabase;
                    clientDB.SetProviders(result);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                string err = "Failed to import provider options.";
                Logger.WriteTraceError(err, ex, Logger.GenerateFullContextStackTrace());
                Logger.WriteSystemEvent(err, ex, Logger.GenerateFullContextStackTrace(), Constants.EventIDs.FailedToLoadProviderOptions);

                return false;
            }
        }

        /// <summary>
        /// Validates that provider options are usable.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool ValidateProviderOptions(List<ProviderOptions> providers)
        {
            try
            {
                Logger.WriteTraceMessage(string.Format("Validating {0} provider option set(s).", providers.Count));

                foreach (var optionSet in providers)
                {
                    optionSet.Validate();
                }

                Logger.WriteTraceMessage("All scan sources validated.");

                return true;
            }
            catch (ProviderOptionsException ex)
            {
                string err = "Failed to validate provider options.";
                Logger.WriteTraceError(err, ex, Logger.GenerateFullContextStackTrace());
                Logger.WriteSystemEvent(err, ex, Logger.GenerateFullContextStackTrace(), Constants.EventIDs.FailedToValidateProviderOptions);

                return false;
            }
        }

        /// <summary>
        /// Detects if we should scan the current source.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private bool ShouldScanSource(SourceLocation source)
        {
            if (source.ShouldScan(Options))
            {
                Logger.WriteTraceMessage("This source needs to be scanned. Preparing scan operation now.");
                return true;
            }
            else
            {
                Logger.WriteTraceMessage("This source location does not need to be scanned at this time.");
                return false;
            }
        }

        /// <summary>
        /// Updates the source locations in the database, if changed.
        /// </summary>
        /// <param name="sources"></param>
        private void RefreshDatabaseSourcesIfChanged(SourceLocations sources)
        {
            // get existing sources.

            var dbSources = GetSourceLocationsFromDatabase();

            if (dbSources.CollectionHasSameContent(sources) == false)
            {
                // delete all sources
                // re-save sources to db

                var clientDB = Database as IClientDatabase;
                clientDB.SetSourceLocations(sources);
            }
        }

        /// <summary>
        /// Writes source location last-scanned date info to tracelog.
        /// </summary>
        /// <param name="source"></param>
        private void WriteLastScannedInfoToTraceLog(SourceLocation source)
        {
            Logger.WriteTraceMessage("Checking source location: " + source.ToString());

            if (source.LastCompletedScan.HasValue)
            {
                Logger.WriteTraceMessage(
                    string.Format("The last completed scan for this source location was on: {0}.",
                    source.LastCompletedScan.Value.ToString(Constants.Logging.SortableDateTimeFormat)));
            }
            else
            {
                Logger.WriteTraceMessage("This source location hasn't been scanned before, or source location definitions have recently been updated.");
            }
        }

        /// <summary>
        /// Updates the last completed scan timestamp in the database for the specified source.
        /// </summary>
        /// <param name="source"></param>
        private void UpdateLastScannedTimeStamp(SourceLocation source)
        {
            source.LastCompletedScan = DateTime.Now;

            var clientDB = Database as IClientDatabase;
            clientDB.UpdateSourceLocation(source);
        }

        /// <summary>
        /// Gets a copy of the source locations from the database.
        /// </summary>
        private SourceLocations GetSourceLocationsFromDatabase()
        {
            var clientDB = Database as IClientDatabase;
            return clientDB.GetAllSourceLocations();
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
                Logger.WriteTraceError(msg);
                Logger.WriteSystemEvent(msg, System.Diagnostics.EventLogEntryType.Error, Constants.EventIDs.FailedToValidateScanSources);

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
            else if (ex is SourceLocationInvalidFolderPathException)
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
