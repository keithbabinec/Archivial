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
        /// Core processing loop.
        /// </summary>
        private void ProcessLoop()
        {
            while (true)
            {
                var sources = SafeImportSources(Options.SourcesFilePath);

                if (sources != null)
                {
                    // scan sources that are overdue for a scan

                    // if multiple scans need to occur: run in sequence, in order of priority
                }

                Thread.Sleep(TimeSpan.FromSeconds(3));

                if (Running == false)
                {
                    OnStopped(new EngineStoppedEventArgs(EngineStoppedReason.StopRequested));
                    break;
                }
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

                Logger.WriteTraceMessage("Successfully imported scan sources.");

                if (ValidateSources(result) == true)
                {
                    return loader.SortSources(result);
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
        /// Validates the provided sources are usable.
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        private bool ValidateSources(SourceLocations sources)
        {
            if (sources == null || sources.Count == 0)
            {
                return false;
            }

            try
            {
                Logger.WriteTraceMessage("Validating scan sources.");

                foreach (var src in sources)
                {
                    Logger.WriteTraceMessage("Validating scan source: " + src.ToString());
                    src.Validate();
                    Logger.WriteTraceMessage("Scan source validated.");
                }

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
