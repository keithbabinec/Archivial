using OzetteLibrary.Crypto;
using OzetteLibrary.Database;
using OzetteLibrary.Logging;
using OzetteLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace OzetteLibrary.Client.Sources
{
    /// <summary>
    /// Contains functionality for scanning a source location.
    /// </summary>
    public class Scanner
    {
        /// <summary>
        /// Default constructor that takes a <c>SourceLocation</c>, <c>IDatabase</c>, and <c>ILogger</c> as input.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="database"></param>
        /// <param name="logger"></param>
        public Scanner(SourceLocation source, IClientDatabase database, ILogger logger)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (database == null)
            {
                throw new ArgumentNullException(nameof(database));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Source = source;
            Database = database;
            Logger = logger;
            ScanStatusLock = new object();
            Hasher = new Hasher();
            Results = new ScanResults();
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

            Logger.WriteTraceMessage(string.Format("Starting scan for source: {0}", Source.ToString()));

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
        /// Internal function to invoke the ScanCompleted event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnScanCompleted(ScanResults e)
        {
            ScanCompleted?.Invoke(this, e);
            lock (ScanStatusLock)
            {
                ScanInProgress = false;
                ScanStopRequested = false;
            }
        }

        /// <summary>
        /// A reference to the database.
        /// </summary>
        private IClientDatabase Database { get; set; }

        /// <summary>
        /// A reference to the logger.
        /// </summary>
        private ILogger Logger { get; set; }

        /// <summary>
        /// A reference to the Source details.
        /// </summary>
        private SourceLocation Source { get; set; }

        /// <summary>
        /// A reference to the file hashing helper.
        /// </summary>
        private Hasher Hasher { get; set; }

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
        /// The file scan results.
        /// </summary>
        private ScanResults Results { get; set; }

        /// <summary>
        /// Performs a scan of the source location.
        /// </summary>
        private void Scan()
        {
            var directoriesToScan = new Queue<DirectoryInfo>();
            directoriesToScan.Enqueue(new DirectoryInfo(Source.FolderPath));
            
            while (directoriesToScan.Count > 0)
            {
                var currentDirectory = directoriesToScan.Dequeue();

                Logger.WriteTraceMessage(string.Format("Scanning directory: {0}", currentDirectory.FullName));

                var subDirs = currentDirectory.EnumerateDirectories();

                foreach (var subDir in subDirs)
                {
                    directoriesToScan.Enqueue(subDir);
                }

                var foundFiles = currentDirectory.EnumerateFiles(Source.FileMatchFilter);

                foreach (var foundFile in foundFiles)
                {
                    Logger.WriteTraceMessage(string.Format("Scanning file: {0}", foundFile.FullName));

                    AddOrUpdateScannedFile(
                        foundFile,
                        Hasher.GenerateDefaultHash(foundFile.FullName, Source.Priority),
                        Hasher.GetDefaultHashAlgorithm(Source.Priority)
                    );
                }
            }

            WriteScanResultsToLog();
            OnScanCompleted(Results);
        }

        /// <summary>
        /// Writes the results of the scan to the log.
        /// </summary>
        private void WriteScanResultsToLog()
        {
            Logger.WriteTraceMessage(string.Format("Completed scan of source: {0}", Source.ToString()));
            Logger.WriteTraceMessage(string.Format("Scan results: ScannedDirectoriesCount={0}", Results.ScannedDirectoriesCount));
            Logger.WriteTraceMessage(string.Format("Scan results: TotalFilesFound={0}", Results.TotalFilesFound));
            Logger.WriteTraceMessage(string.Format("Scan results: TotalBytesFound={0}", Results.TotalBytesFound));
            Logger.WriteTraceMessage(string.Format("Scan results: NewOrUpdatedFilesFound={0}", Results.NewOrUpdatedFilesFound));
            Logger.WriteTraceMessage(string.Format("Scan results: NewOrUpdatedBytesFound={0}", Results.NewOrUpdatedBytesFound));
        }

        /// <summary>
        /// Adds or updates a scanned file into the database.
        /// </summary>
        /// <param name="fileInfo">FileInfo details</param>
        /// <param name="fileHash">The computed hash</param>
        /// <param name="algorithm">Hash algorithm used to compute the hash</param>
        private void AddOrUpdateScannedFile(FileInfo fileInfo, byte[] fileHash, HashAlgorithmName algorithm)
        {
            ClientFile clientFile = Database.GetClientFile(fileInfo.FullName);
            clientFile.LastChecked = DateTime.Now;

            if (clientFile == null)
            {
                Logger.WriteTraceMessage(string.Format("Scanned file ({0}) is new.", fileInfo.Name));

                // brand new file
                clientFile = new ClientFile(fileInfo);
                clientFile.FileHash = fileHash;
                clientFile.HashAlgorithmType = algorithm;
                clientFile.ResetCopyState(Database.GetTargets());

                Database.AddClientFile(clientFile);
            }
            else
            {
                if (Hasher.TwoHashesAreTheSame(fileHash, clientFile.FileHash) == false)
                {
                    // existing file updated
                    Logger.WriteTraceMessage(string.Format("Scanned file ({0}) is updated.", fileInfo.Name));
                    clientFile.ResetCopyState(Database.GetTargets());
                }
                else
                {
                    Logger.WriteTraceMessage(string.Format("Scanned file ({0}) is unchanged since previous check.", fileInfo.Name));
                }

                Database.UpdateClientFile(clientFile);
            }
        }
    }
}
