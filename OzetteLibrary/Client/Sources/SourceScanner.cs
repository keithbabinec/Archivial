using OzetteLibrary.Crypto;
using OzetteLibrary.Database;
using OzetteLibrary.Events;
using OzetteLibrary.Files;
using OzetteLibrary.Folders;
using OzetteLibrary.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

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
            Hasher = new Hasher(logger);
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
        /// Performs a scan of a source location.
        /// </summary>
        /// <param name="source">The local source definition</param>
        /// <param name="asyncState">The async result state</param>
        public async Task ScanAsync(SourceLocation source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            Logger.WriteTraceMessage(string.Format("Starting scan for source: {0}", source.ToString()));

            var results = new ScanResults();

            // note: constructing DirectoryInfo objects with non-existent paths will not throw exceptions.
            // however calling EnumerateFiles() or EnumerateDirectories() will throw exceptions, so these are wrapped.

            var directoriesToScan = new Queue<DirectoryInfo>();
            directoriesToScan.Enqueue(new DirectoryInfo(source.Path));

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
                        await ScanFileAsync(results, foundFile, source);
                    }
                }

                results.ScannedDirectoriesCount++;
            }

            WriteScanResultsToLog(results, source);
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
        /// <param name="fileHash">The computed hash</param>
        /// <param name="algorithm">Hash algorithm used to compute the hash</param>
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
                Logger.WriteTraceMessage(string.Format("Unsupported File (Path too long): {0}", fileInfo.FullName));
                results.UnsupportedFilesFound++;
                results.UnsupportedBytesFound += (ulong)fileInfo.Length;
                results.TotalFilesFound++;
                results.TotalBytesFound += (ulong)fileInfo.Length;
                return;
            }

            // do a simple file lookup, based on the name/path, size, and date modified
            // do not hash yet-- we dont need it until backup time.

            var fileIndexLookup = await Database.FindBackupFileAsync(fileInfo.FullName, fileInfo.Length, fileInfo.LastWriteTime);

            if (fileIndexLookup.Result == BackupFileLookupResult.New)
            {
                await ProcessNewFileAsync(fileInfo, source.Priority);
                results.NewFilesFound++;
                results.NewBytesFound += (ulong)fileInfo.Length;
            }
            else if (fileIndexLookup.Result == BackupFileLookupResult.Updated)
            {
                await ProcessUpdatedFileAsync(fileIndexLookup, fileInfo);
                results.UpdatedFilesFound++;
                results.UpdatedBytesFound += (ulong)fileInfo.Length;
            }
            else if (fileIndexLookup.Result == BackupFileLookupResult.Existing)
            {
                await ProcessExistingFileAsync(fileIndexLookup, fileInfo);
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
        /// <param name="priority">The source priority</param>
        private async Task ProcessNewFileAsync(FileInfo fileInfo, FileBackupPriority priority)
        {
            Logger.WriteTraceMessage(string.Format("New File: {0}", fileInfo.FullName));

            // brand new file
            var backupFile = new BackupFile(fileInfo, priority);
            await Database.AddBackupFileAsync(backupFile);
        }

        /// <summary>
        /// Processes an updated scanned file into the database.
        /// </summary>
        /// <param name="fileLookup">File index lookup result</param>
        /// <param name="fileInfo">FileInfo details</param>
        private async Task ProcessUpdatedFileAsync(BackupFileLookup fileLookup, FileInfo fileInfo)
        {
            Logger.WriteTraceMessage(string.Format("Updated File: {0}", fileInfo.FullName));

            await Database.ResetBackupFileStateAsync(fileLookup.File);
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

            // existing file
            // should update the last checked flag
            await Database.SetBackupFileLastScannedAsync(fileLookup.File.FileID);
        }
    }
}
