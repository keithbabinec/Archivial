using OzetteLibrary.Client;
using OzetteLibrary.Database.LiteDB;
using OzetteLibrary.Logging;
using OzetteLibrary.Logging.Default;
using OzetteLibrary.Models;
using OzetteLibrary.ServiceCore;
using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace OzetteClientAgent
{
    /// <summary>
    /// Contains service functionality.
    /// </summary>
    public partial class OzetteClientAgent : ServiceBase
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public OzetteClientAgent()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// Runs when the service start is triggered.
        /// </summary>
        /// <remarks>
        /// Long running initialization code can confuse the service control manager (thinks it may be a hang).
        /// Instead launch the initialization tasks in a seperate thread so control returns to the SCM immediately.
        /// </remarks>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            CoreLog = new Logger(OzetteLibrary.Constants.Logging.CoreServiceComponentName);
            Initialize = new Initialization(CoreLog);
            Initialize.Completed += InitHelper_Completed;
            Initialize.BeginStart(Properties.Settings.Default.Properties);
        }

        /// <summary>
        /// A reference to the core service log.
        /// </summary>
        private ILogger CoreLog { get; set; }

        /// <summary>
        /// A reference to the initialization helper.
        /// </summary>
        private Initialization Initialize { get; set; }

        /// <summary>
        /// A reference to the scanning engine instance.
        /// </summary>
        private ScanEngine Scan { get; set; }

        /// <summary>
        /// A reference to the backup engine instance.
        /// </summary>
        private BackupEngine Backup { get; set; }
        
        /// <summary>
        /// Callback event for when the initialization thread has completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitHelper_Completed(object sender, System.EventArgs e)
        {
            if (Initialize.ResultCode == StartupResults.Success)
            {
                // in the client agent the core loop consists of two pieces.
                // first is the scan engine, and the second is the backup engine.
                // each one lives under it's own long-running thread and class.
                // prepare the database and then start both engines.

                CoreLog.WriteSystemEvent(
                    string.Format("Starting {0} client service.", OzetteLibrary.Constants.Logging.AppName), 
                    EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StartingService);

                PrepareDatabase();
                StartScanEngine();
                StartBackupEngine();

                CoreLog.WriteSystemEvent(
                    string.Format("Successfully started {0} client service.", OzetteLibrary.Constants.Logging.AppName),
                    EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StartedService);
            }
            else
            {
                // safe exit without crash.
                // set the exit code so service control manager knows there is a problem.

                ExitCode = (int)Initialize.ResultCode;
                Stop();
            }
        }

        /// <summary>
        /// Runs when the service stop is triggered.
        /// </summary>
        protected override void OnStop()
        {
            if (CoreLog != null)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Stopping {0} client service.", OzetteLibrary.Constants.Logging.AppName),
                    EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StoppingService);
            }

            if (Scan != null)
            {
                Scan.BeginStop();
            }
            if (Backup != null)
            {
                Backup.BeginStop();
            }

            if (CoreLog != null)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Successfully stopped {0} client service.", OzetteLibrary.Constants.Logging.AppName),
                    EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StoppedService);
            }
        }

        /// <summary>
        /// Prepares the database.
        /// </summary>
        /// <remarks>
        /// This operation will pre-create collections, indexes, and object mappings.
        /// </remarks>
        private void PrepareDatabase()
        {
            var db = new LiteDBClientDatabase(Initialize.Options.DatabaseConnectionString, CoreLog);
            db.PrepareDatabase();
        }

        /// <summary>
        /// Starts the scanning engine.
        /// </summary>
        private void StartScanEngine()
        {
            // note: each engine can get it's own instance of the LiteDBClientDatabase wrapper.
            // LiteDB is thread safe, but the wrapper is not; so give threads their own DB wrappers.

            var log = new Logger(OzetteLibrary.Constants.Logging.ScanningComponentName);
            var db = new LiteDBClientDatabase(Initialize.Options.DatabaseConnectionString, log);

            Scan = new ScanEngine(db, log, Initialize.Options);
            Scan.Stopped += Scan_Stopped;
            Scan.BeginStart();
        }

        /// <summary>
        /// Callback event for when scanning engine has stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Scan_Stopped(object sender, OzetteLibrary.Events.EngineStoppedEventArgs e)
        {
            if (CoreLog != null)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Scanning Engine has stopped."),
                    EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StoppedScanEngine);

                CoreLog.WriteTraceMessage("Scanning Engine has stopped.");
            }
        }

        /// <summary>
        /// Starts the backup engine.
        /// </summary>
        private void StartBackupEngine()
        {
            // note: each engine can get it's own instance of the LiteDBClientDatabase wrapper.
            // LiteDB is thread safe, but the wrapper is not; so give threads their own DB wrappers.

            var log = new Logger(OzetteLibrary.Constants.Logging.BackupComponentName);
            var db = new LiteDBClientDatabase(Initialize.Options.DatabaseConnectionString, log);

            Backup = new BackupEngine(db, log, Initialize.Options);
            Backup.Stopped += Backup_Stopped;
            Backup.BeginStart();
        }

        /// <summary>
        /// Callback event for when the backup engine has stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Backup_Stopped(object sender, OzetteLibrary.Events.EngineStoppedEventArgs e)
        {
            if (CoreLog != null)
            {
                CoreLog.WriteSystemEvent(
                    string.Format("Backup Engine has stopped."),
                    EventLogEntryType.Information, OzetteLibrary.Constants.EventIDs.StoppedBackupEngine);

                CoreLog.WriteTraceMessage("Backup Engine has stopped.");
            }
        }
    }
}
