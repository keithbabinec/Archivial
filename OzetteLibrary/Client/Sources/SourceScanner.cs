﻿using OzetteLibrary.Crypto;
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
    public class SourceScanner
    {
        /// <summary>
        /// Default constructor that takes a <c>SourceLocation</c>, <c>IDatabase</c>, and <c>ILogger</c> as input.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="logger"></param>
        public SourceScanner(IClientDatabase database, ILogger logger)
        {
            if (database == null)
            {
                throw new ArgumentNullException(nameof(database));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Database = database;
            Logger = logger;
            ScanStatusLock = new object();
            Hasher = new Hasher(logger);
        }

        /// <summary>
        /// Starts asynchronously scanning a source.
        /// </summary>
        /// <param name="source">The source definition</param>
        public void BeginScan(SourceLocation source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            lock (ScanStatusLock)
            {
                if (ScanInProgress)
                {
                    throw new InvalidOperationException("Cannot start the scan. It is already in progress.");
                }

                ScanInProgress = true;
                ScanStopRequested = false;
            }

            Logger.WriteTraceMessage(string.Format("Starting scan for source: {0}", source.ToString()));
            
            Thread scanThread = new Thread(() => Scan(source));
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
        /// Performs a scan of the source location.
        /// </summary>
        /// <param name="source">The source definition</param>
        private void Scan(SourceLocation source)
        {
            var results = new ScanResults();

            var directoriesToScan = new Queue<DirectoryInfo>();
            directoriesToScan.Enqueue(new DirectoryInfo(source.FolderPath));

            // note: constructing DirectoryInfo objects with non-existent paths will not throw exceptions.
            // however calling EnumerateFiles() or EnumerateDirectories() will throw exceptions, so these are wrapped.
            
            while (directoriesToScan.Count > 0)
            {
                var currentDirectory = directoriesToScan.Dequeue();

                Logger.WriteTraceMessage(string.Format("Scanning directory: {0}", currentDirectory.FullName));

                var subDirs = SafeEnumerateDirectories(currentDirectory);
                if (subDirs != null)
                {
                    foreach (var subDir in subDirs)
                    {
                        directoriesToScan.Enqueue(subDir);
                    }
                }

                var foundFiles = SafeEnumerateFiles(currentDirectory, source.FileMatchFilter);
                if (foundFiles != null)
                {
                    foreach (var foundFile in foundFiles)
                    {
                        ScanFile(results, foundFile, source);
                    }
                }

                results.ScannedDirectoriesCount++;
            }

            WriteScanResultsToLog(results, source);
            OnScanCompleted(results);
        }

        /// <summary>
        /// Attempts to safely enumerate the directories under the specified directory.
        /// </summary>
        /// <remarks>
        /// Exceptions are automatically handled and logged to the trace log.
        /// </remarks>
        /// <param name="directory">DirectoryInfo</param>
        /// <returns>IEnumerable<DirectoryInfo></returns>
        private IEnumerable<DirectoryInfo> SafeEnumerateDirectories(DirectoryInfo directory)
        {
            try
            {
                return directory.EnumerateDirectories();
            }
            catch (Exception ex)
            {
                Logger.WriteTraceError("Failed to list directories under path: " + directory.FullName, ex);
                return null;
            }
        }

        /// <summary>
        /// Attempts to safely enumerate the files under the specified directory.
        /// </summary>
        /// <remarks>
        /// Exceptions are automatically handled and logged to the trace log.
        /// </remarks>
        /// <param name="directory">DirectoryInfo</param>
        /// <param name="matchFilter">Match filter (may be null/empty)</param>
        /// <returns>IEnumerable<DirectoryInfo></returns>
        private IEnumerable<FileInfo> SafeEnumerateFiles(DirectoryInfo directory, string matchFilter)
        {
            try
            {
                return directory.EnumerateFiles(matchFilter);
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(matchFilter))
                {
                    Logger.WriteTraceError(
                        string.Format("Failed to list files under path: {0}. No match filter was used.", directory.FullName), 
                        ex);
                }
                else
                {
                    Logger.WriteTraceError(
                        string.Format("Failed to list files under path: {0}. Match filter was used: {1}", 
                            directory.FullName,
                            matchFilter),
                        ex);
                }
                
                return null;
            }
        }

        /// <summary>
        /// Writes the results of the scan to the log.
        /// </summary>
        /// <param name="results">Result counters object.</param>
        /// <param name="source">The source definition</param>
        private void WriteScanResultsToLog(ScanResults results, SourceLocation source)
        {
            Logger.WriteTraceMessage(string.Format("Completed scan of source: {0}", source.ToString()));
            Logger.WriteTraceMessage(string.Format("Scan results: ScannedDirectoriesCount={0}", results.ScannedDirectoriesCount));
            Logger.WriteTraceMessage(string.Format("Scan results: NewFilesFound={0}", results.NewFilesFound));
            Logger.WriteTraceMessage(string.Format("Scan results: NewBytesFound={0}", results.NewBytesFound));
            Logger.WriteTraceMessage(string.Format("Scan results: UpdatedFilesFound={0}", results.UpdatedFilesFound));
            Logger.WriteTraceMessage(string.Format("Scan results: UpdatedBytesFound={0}", results.UpdatedBytesFound));
            Logger.WriteTraceMessage(string.Format("Scan results: TotalFilesFound={0}", results.TotalFilesFound));
            Logger.WriteTraceMessage(string.Format("Scan results: TotalBytesFound={0}", results.TotalBytesFound));
        }

        /// <summary>
        /// Processes a scanned file into the database.
        /// </summary>
        /// <param name="results">Result counters object.</param>
        /// <param name="fileInfo">FileInfo details</param>
        /// <param name="fileHash">The computed hash</param>
        /// <param name="algorithm">Hash algorithm used to compute the hash</param>
        /// <param name="source">The source definition</param>
        private void ScanFile(ScanResults results, FileInfo fileInfo, SourceLocation source)
        {
            Logger.WriteTraceMessage(string.Format("Scanning file: {0}", fileInfo.FullName));

            var hashType = Hasher.GetDefaultHashAlgorithm(source.Priority);
            var hash = Hasher.GenerateDefaultHash(fileInfo.FullName, source.Priority);

            if (hash.Length == 0)
            {
                // failed to generate a hash.
                // cant properly lookup the file in the database without it.
                // error has already been logged by the hasher
                return;
            }
            
            var fileIndexLookup = Database.GetClientFile(fileInfo.Name, fileInfo.DirectoryName, hash);

            if (fileIndexLookup.Result == ClientFileLookupResult.New)
            {
                ProcessNewFile(fileInfo, hash, hashType);
                results.NewFilesFound++;
                results.NewBytesFound += (ulong)fileInfo.Length;
            }
            else if (fileIndexLookup.Result == ClientFileLookupResult.Updated)
            {
                ProcessUpdatedFile(fileIndexLookup, fileInfo, hash, hashType);
                results.UpdatedFilesFound++;
                results.UpdatedBytesFound += (ulong)fileInfo.Length;
            }
            else if (fileIndexLookup.Result == ClientFileLookupResult.Existing)
            {
                ProcessExistingFile(fileIndexLookup, fileInfo);
            }
            else
            {
                throw new InvalidOperationException("Unexpected ClientFileLookupResult type: " + fileIndexLookup.Result);
            }

            results.TotalFilesFound++;
            results.TotalBytesFound += (ulong)fileInfo.Length;
        }

        /// <summary>
        /// Processes a new scanned file into the database.
        /// </summary>
        /// <param name="fileInfo">FileInfo details</param>
        /// <param name="fileHash">The computed hash</param>
        /// <param name="algorithm">Hash algorithm used to compute the hash</param>
        private void ProcessNewFile(FileInfo fileInfo, byte[] fileHash, HashAlgorithmName algorithm)
        {
            Logger.WriteTraceMessage(string.Format("Scanned file ({0}) is new.", fileInfo.Name));

            // brand new file
            var clientFile = new ClientFile(fileInfo);
            clientFile.FileHash = fileHash;
            clientFile.HashAlgorithmType = algorithm;
            clientFile.ResetCopyState(Database.GetAllTargets());
            clientFile.LastChecked = DateTime.Now;

            Database.AddClientFile(clientFile);
        }

        /// <summary>
        /// Processes an updated scanned file into the database.
        /// </summary>
        /// <param name="fileLookup">File index lookup result</param>
        /// <param name="fileInfo">FileInfo details</param>
        /// <param name="fileHash">The computed hash</param>
        /// <param name="algorithm">Hash algorithm used to compute the hash</param>
        private void ProcessUpdatedFile(ClientFileLookup fileLookup, FileInfo fileInfo, byte[] fileHash, HashAlgorithmName algorithm)
        {
            Logger.WriteTraceMessage(string.Format("Scanned file ({0}) is updated.", fileInfo.Name));

            // updated file
            fileLookup.File.FileHash = fileHash;
            fileLookup.File.HashAlgorithmType = algorithm;
            fileLookup.File.ResetCopyState(Database.GetAllTargets());
            fileLookup.File.LastChecked = DateTime.Now;

            Database.UpdateClientFile(fileLookup.File);
        }

        /// <summary>
        /// Processes an existing scanned file into the database.
        /// </summary>
        /// <param name="fileLookup">File index lookup result</param>
        /// <param name="fileInfo">FileInfo details</param>
        private void ProcessExistingFile(ClientFileLookup fileLookup, FileInfo fileInfo)
        {
            Logger.WriteTraceMessage(string.Format("Scanned file ({0}) is unchanged.", fileInfo.Name));

            // existing file
            // should update the last checked flag
            fileLookup.File.LastChecked = DateTime.Now;
            Database.UpdateClientFile(fileLookup.File);
        }
    }
}