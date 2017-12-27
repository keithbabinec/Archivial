using OzetteLibrary.Client.Sources;
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

                    if (sources != null)
                    {
                        foreach (var source in sources)
                        {
                            // should we actually scan this source?
                            // checks the DB to see if it has been scanned recently.        

                            if (source.ShouldScan(Options))
                            {
                                // begin-invoke the asynchronous scan operation.
                                // watch the IAsyncResult status object to check for status updates
                                // and wait until the scan has completed.

                                var state = Scanner.BeginScan(source);

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
                            }

                            if (Running == false)
                            {
                                // stop was requested.
                                // do not continue scanning any remaining sources.
                                break;
                            }
                        }

                        ThreadSleepWithStopRequestCheck(TimeSpan.FromSeconds(10));
                    }
                    else
                    {
                        ThreadSleepWithStopRequestCheck(TimeSpan.FromSeconds(30));
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
                Logger.WriteTraceError(err, ex);
                Logger.WriteSystemEvent(err, ex, Constants.EventIDs.FailedToLoadScanSources);

                return null;
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
            catch (Exception ex)
            {
                string err = "Failed to validate scan sources.";
                Logger.WriteTraceError(err, ex);
                Logger.WriteSystemEvent(err, ex, Constants.EventIDs.FailedToValidateScanSources);

                return false;
            }
        }
    }
}
