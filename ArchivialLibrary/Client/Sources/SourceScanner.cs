﻿using ArchivialLibrary.Crypto;
using ArchivialLibrary.Database;
using ArchivialLibrary.Files;
using ArchivialLibrary.Folders;
using ArchivialLibrary.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ArchivialLibrary.Client.Sources
{
    /// <summary>
    /// Contains functionality for scanning a source location.
    /// </summary>
    public class SourceScanner
    {
        /// <summary>
        /// Default constructor that takes a <c>IDatabase</c>, <c>ILogger</c>, and optional exclusion patterns as input.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="logger"></param>
        /// <param name="exclusionPatterns"></param>
        public SourceScanner(IClientDatabase database, ILogger logger, string[] exclusionPatterns)
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
            Hasher = new Hasher(logger);

            if (exclusionPatterns != null)
            {
                Exclusions = new Regex[exclusionPatterns.Length];

                for (int i = 0; i < exclusionPatterns.Length; i++)
                {
                    Exclusions[i] = new Regex(exclusionPatterns[i]);
                }
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
        /// A list of master exclusions.
        /// </summary>
        private Regex[] Exclusions { get; set; }

        /// <summary>
        /// Performs a scan of a source location.
        /// </summary>
        /// <param name="source">The local source definition</param>
        /// <param name="cancelToken">Cancellation token.</param>
        public async Task ScanAsync(SourceLocation source, CancellationToken cancelToken)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            Logger.WriteTraceMessage(string.Format("Starting scan for source: {0}", source.ToString()));

            var results = new ScanResults();

            try
            {
                // cancel if the caller is shutting down.
                // add these in a few different places to ensure we aren't holding the threads open too long.
                cancelToken.ThrowIfCancellationRequested();

                // note: constructing DirectoryInfo objects with non-existent paths will not throw exceptions.
                // however calling EnumerateFiles() or EnumerateDirectories() will throw exceptions, so these are wrapped.

                var directoriesToScan = new Queue<DirectoryInfo>();
                directoriesToScan.Enqueue(new DirectoryInfo(source.Path));

                while (directoriesToScan.Count > 0)
                {
                    var currentDirectory = directoriesToScan.Dequeue();

                    Logger.WriteTraceMessage(string.Format("Scanning directory: {0}", currentDirectory.FullName));

                    cancelToken.ThrowIfCancellationRequested();
                    var subDirs = SafeEnumerateDirectories(results, currentDirectory);
                    if (subDirs != null)
                    {
                        foreach (var subDir in subDirs)
                        {
                            directoriesToScan.Enqueue(subDir);
                        }
                    }

                    cancelToken.ThrowIfCancellationRequested();
                    var foundFiles = SafeEnumerateFiles(currentDirectory, source.FileMatchFilter);
                    if (foundFiles != null)
                    {
                        foreach (var foundFile in foundFiles)
                        {
                            cancelToken.ThrowIfCancellationRequested();
                            await ScanFileAsync(results, foundFile, source).ConfigureAwait(false);
                        }
                    }

                    results.ScannedDirectoriesCount++;
                }

                WriteScanResultsToLog(results, source);
            }
            catch (OperationCanceledException)
            {
                // the caller has requested that we stop.
                Logger.WriteTraceWarning("An in-progress source scan has been cancelled by request of the Scan Engine. It will be resumed the next time the engine starts up.", 0);
            }
        }

        /// <summary>
        /// Attempts to safely enumerate the directories under the specified directory.
        /// </summary>
        /// <remarks>
        /// Exceptions are automatically handled and logged to the trace log.
        /// </remarks>
        /// <param name="results">Scan Results</param>
        /// <param name="directory">DirectoryInfo</param>
        /// <returns>IEnumerable<DirectoryInfo></returns>
        private IEnumerable<DirectoryInfo> SafeEnumerateDirectories(ScanResults results, DirectoryInfo directory)
        {
            try
            {
                return directory.EnumerateDirectories();
            }
            catch (PathTooLongException)
            {
                results.UnsupportedFoldersFound++;
                Logger.WriteTraceWarning(string.Format("Unsupported Directory (Path too long): {0}", directory.FullName));
                return null;
            }
            catch (Exception ex)
            {
                Logger.WriteTraceError("Failed to list directories under path: " + directory.FullName, ex, Logger.GenerateFullContextStackTrace());
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
            catch (PathTooLongException)
            {
                Logger.WriteTraceWarning(string.Format("Cannot enumerate files in this directory, the path is too long: {0}", directory.FullName));
                return null;
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(matchFilter))
                {
                    Logger.WriteTraceError(
                        string.Format("Failed to list files under path: {0}. No match filter was used.", directory.FullName), 
                        ex,
                        Logger.GenerateFullContextStackTrace());
                }
                else
                {
                    Logger.WriteTraceError(
                        string.Format("Failed to list files under path: {0}. Match filter was used: {1}", 
                            directory.FullName,
                            matchFilter),
                        ex,
                        Logger.GenerateFullContextStackTrace());
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
            Logger.WriteTraceMessage(string.Format("Scan results: UnsupportedFoldersFound={0}", results.UnsupportedFoldersFound));
            Logger.WriteTraceMessage(string.Format("Scan results: UnsupportedFilesFound={0}", results.UnsupportedFilesFound));
            Logger.WriteTraceMessage(string.Format("Scan results: UnsupportedBytesFound={0}", results.UnsupportedBytesFound));
            Logger.WriteTraceMessage(string.Format("Scan results: TotalFilesFound={0}", results.TotalFilesFound));
            Logger.WriteTraceMessage(string.Format("Scan results: TotalBytesFound={0}", results.TotalBytesFound));
        }

        /// <summary>
        /// Processes a scanned file into the database.
        /// </summary>
        /// <param name="results">Result counters object.</param>
        /// <param name="fileInfo">FileInfo details</param>
        /// <param name="source">The source definition</param>
        private async Task ScanFileAsync(ScanResults results, FileInfo fileInfo, SourceLocation source)
        {
            if (fileInfo.Length == 0)
            {
                // this file is empty (has no contents).
                // unable to back up empty files.
                Logger.WriteTraceMessage(string.Format("Unsupported File (Empty): {0}", fileInfo.FullName));
                results.UnsupportedFilesFound++;
                results.TotalFilesFound++;
                return;
            }
            if (fileInfo.FullName.Length >= 260)
            {
                // this filename is too long, windows won't open it correctly without some additional code/support.
                // unable to back up.
                // The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters
                Logger.WriteTraceWarning(string.Format("Unsupported File (Path too long): {0}", fileInfo.FullName));
                results.UnsupportedFilesFound++;
                results.UnsupportedBytesFound += (ulong)fileInfo.Length;
                results.TotalFilesFound++;
                results.TotalBytesFound += (ulong)fileInfo.Length;
                return;
            }
            if (Exclusions != null)
            {
                foreach (var exclusion in Exclusions)
                {
                    if (exclusion.IsMatch(fileInfo.Name))
                    {
                        // file is excluded due to an exclusion match pattern.
                        return;
                    }
                }
            }

            // do a simple file lookup, based on the name/path, size, and date modified
            // do not hash yet-- we dont need it until backup time.

            var fileIndexLookup = await Database.FindBackupFileAsync(fileInfo.FullName, fileInfo.Length, fileInfo.LastWriteTime).ConfigureAwait(false);

            if (fileIndexLookup.Result == BackupFileLookupResult.New)
            {
                await ProcessNewFileAsync(fileInfo, source).ConfigureAwait(false);
                results.NewFilesFound++;
                results.NewBytesFound += (ulong)fileInfo.Length;
            }
            else if (fileIndexLookup.Result == BackupFileLookupResult.Updated)
            {
                await ProcessUpdatedFileAsync(fileIndexLookup, fileInfo).ConfigureAwait(false);
                results.UpdatedFilesFound++;
                results.UpdatedBytesFound += (ulong)fileInfo.Length;
            }
            else if (fileIndexLookup.Result == BackupFileLookupResult.Existing)
            {
                await ProcessExistingFileAsync(fileIndexLookup, fileInfo).ConfigureAwait(false);
            }
            else
            {
                throw new InvalidOperationException("Unexpected BackupFileLookupResult type: " + fileIndexLookup.Result);
            }

            results.TotalFilesFound++;
            results.TotalBytesFound += (ulong)fileInfo.Length;
        }

        /// <summary>
        /// Processes a new scanned file into the database.
        /// </summary>
        /// <param name="fileInfo">FileInfo details</param>
        /// <param name="source">The source location</param>
        private async Task ProcessNewFileAsync(FileInfo fileInfo, SourceLocation source)
        {
            Logger.WriteTraceMessage(string.Format("New File: {0}", fileInfo.FullName));

            // brand new file
            var backupFile = new BackupFile(fileInfo, source);
            await Database.AddBackupFileAsync(backupFile).ConfigureAwait(false);
        }

        /// <summary>
        /// Processes an updated scanned file into the database.
        /// </summary>
        /// <param name="fileLookup">File index lookup result</param>
        /// <param name="fileInfo">FileInfo details</param>
        private async Task ProcessUpdatedFileAsync(BackupFileLookup fileLookup, FileInfo fileInfo)
        {
            Logger.WriteTraceMessage(string.Format("Updated File: {0}", fileInfo.FullName));

            // update the file metadata (size, last written, blocks)
            fileLookup.File.LastModified = fileInfo.LastWriteTime;
            fileLookup.File.FileSizeBytes = fileInfo.Length;
            fileLookup.File.TotalFileBlocks = fileLookup.File.CalculateTotalFileBlocks(Constants.Transfers.TransferBlockSizeBytes);
            
            // commit the updated file metadata.
            // this resets the backup copy state.
            await Database.ResetBackupFileStateAsync(fileLookup.File).ConfigureAwait(false);
        }

        /// <summary>
        /// Processes an existing scanned file into the database.
        /// </summary>
        /// <param name="fileLookup">File index lookup result</param>
        /// <param name="fileInfo">FileInfo details</param>
        private async Task ProcessExistingFileAsync(BackupFileLookup fileLookup, FileInfo fileInfo)
        {
            // do not write trace messages for unchanged files
            // it becomes too noisy in the logs.

            if (fileLookup.File.OverallState == FileStatus.ProviderError || fileLookup.File.WasDeleted.HasValue)
            {
                Logger.WriteTraceMessage(string.Format("Reset File: {0}", fileInfo.FullName));

                // existing file but in a failed state or deleted state.
                // since we have rescanned we should reset the failed state to allow for a retry.
                await Database.ResetBackupFileStateAsync(fileLookup.File).ConfigureAwait(false);
            }
            else
            {
                // existing file
                // should update the last checked flag
                await Database.SetBackupFileLastScannedAsync(fileLookup.File.FileID).ConfigureAwait(false);
            }
        }
    }
}
