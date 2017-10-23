using OzetteLibrary.Database;
using OzetteLibrary.Models;
using System;
using System.Threading;

namespace OzetteLibrary.Client.Sources
{
    /// <summary>
    /// Contains functionality for scanning a source location.
    /// </summary>
    public class Scanner
    {
        /// <summary>
        /// Default constructor that takes a <c>SourceLocation</c> and <c>IDatabase</c> as input.
        /// </summary>
        /// <param name="source"></param>
        public Scanner(SourceLocation source, IDatabase database)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (database == null)
            {
                throw new ArgumentNullException(nameof(database));
            }

            Source = source;
            Database = database;
        }

        /// <summary>
        /// Starts asynchronously scanning a source.
        /// </summary>
        public void BeginScan()
        {
            lock (ScanStatusLock)
            {
                if (ScanInProgress)
                {
                    throw new InvalidOperationException("Cannot start the scan. It is already in progress.");
                }

                ScanInProgress = true;
                ScanStopRequested = false;
            }

            Thread scanThread = new Thread(() => Scan());
            scanThread.Start();
        }

        /// <summary>
        /// Stops scanning a source if it is in-progress.
        /// </summary>
        public void StopScan()
        {
            lock (ScanStatusLock)
            {
                if (ScanInProgress)
                {
                    ScanStopRequested = true;
                }
            }
        }

        /// <summary>
        /// Event triggered when the scan has completed.
        /// </summary>
        public event EventHandler<ScanResults> ScanCompleted;

        /// <summary>
        /// A reference to the database.
        /// </summary>
        private IDatabase Database { get; set; }

        /// <summary>
        /// A reference to the Source details.
        /// </summary>
        private SourceLocation Source { get; set; }

        /// <summary>
        /// Flag to indicate if the scan is already in progress.
        /// </summary>
        private bool ScanInProgress { get; set; }

        /// <summary>
        /// Flag to indicate if the scan stop has been requested.
        /// </summary>
        private bool ScanStopRequested { get; set; }

        /// <summary>
        /// Thread locking mechanism.
        /// </summary>
        private object ScanStatusLock { get; set; }

        /// <summary>
        /// Performs a scan of the source location.
        /// </summary>
        private void Scan()
        {

        }
    }
}
