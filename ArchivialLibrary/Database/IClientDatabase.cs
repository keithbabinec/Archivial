using ArchivialLibrary.Files;
using ArchivialLibrary.Folders;
using ArchivialLibrary.Secrets;
using ArchivialLibrary.ServiceCore;
using System;
using ArchivialLibrary.Providers;
using System.Threading.Tasks;

namespace ArchivialLibrary.Database
{
    /// <summary>
    /// A generic database interface for the client database.
    /// </summary>
    public interface IClientDatabase
    {
        /// <summary>
        /// Prepares the database.
        /// </summary>
        /// <returns></returns>
        Task PrepareDatabaseAsync();

        /// <summary>
        /// Deletes the database.
        /// </summary>
        /// <returns></returns>
        Task DeleteClientDatabaseAsync();

        /// <summary>
        /// Creates a backup of the database.
        /// </summary>
        /// <param name="BackupType">The type of backup to perform.</param>
        /// <returns></returns>
        Task CreateDatabaseBackupAsync(DatabaseBackupType BackupType);

        /// <summary>
        /// Returns the client database backup status.
        /// </summary>
        /// <returns><c>DatabaseBackupStatus</c></returns>
        Task<DatabaseBackupStatus> GetClientDatabaseBackupStatusAsync();

        /// <summary>
        /// Flags a client database backup as complete.
        /// </summary>
        /// <param name="BackupType">The type of backup that was completed.</param>
        /// <returns></returns>
        Task SetClientDatabaseBackupCompletedAsync(DatabaseBackupType BackupType);

        /// <summary>
        /// Saves an application setting to the database.
        /// </summary>
        /// <param name="OptionName">Option name</param>
        /// <param name="OptionValue">Option value</param>
        Task SetApplicationOptionAsync(string OptionName, string OptionValue);

        /// <summary>
        /// Retrieves an application setting value from the database.
        /// </summary>
        /// <remarks>
        /// Returns null if the setting is not found.
        /// </remarks>
        /// <param name="OptionName">Option name</param>
        /// <returns>The setting value.</returns>
        Task<string> GetApplicationOptionAsync(string OptionName);

        /// <summary>
        /// Removes an application setting value from the database.
        /// </summary>
        /// <param name="OptionName">Option name</param>
        Task RemoveApplicationOptionAsync(string OptionName);

        /// <summary>
        /// Returns a list of all providers defined in the database for the specified type.
        /// </summary>
        /// <param name="Type">The type of providers to return.</param>
        /// <returns><c>ProviderCollection</c></returns>
        Task<ProviderCollection> GetProvidersAsync(ProviderTypes Type);

        /// <summary>
        /// Removes the specified provider by Name.
        /// </summary>
        /// <param name="ProviderName">Provider name.</param>
        Task RemoveProviderAsync(string ProviderName);

        /// <summary>
        /// Adds the specified Provider object to the database.
        /// </summary>
        /// <param name="Provider"></param>
        Task AddProviderAsync(Provider Provider);

        /// <summary>
        /// Adds a net credential to the database.
        /// </summary>
        /// <param name="Credential"></param>
        /// <returns></returns>
        Task AddNetCredentialAsync(NetCredential Credential);

        /// <summary>
        /// Removes the specified net credential by Name.
        /// </summary>
        /// <param name="CredentialName">Provider name.</param>
        Task RemoveNetCredentialAsync(string CredentialName);

        /// <summary>
        /// Returns all of the net credentials defined in the database.
        /// </summary>
        /// <returns>A collection of net credentials.</returns>
        Task<NetCredentialsCollection> GetNetCredentialsAsync();

        /// <summary>
        /// Checks the index for a file matching the provided name, path, filesize, and lastmodified date.
        /// </summary>
        /// <param name="FullFilePath">Full file path (file name and path)</param>
        /// <param name="FileSizeBytes">File size in bytes</param>
        /// <param name="FileLastModified">File last modified timestamp</param>
        /// <returns></returns>
        Task<BackupFileLookup> FindBackupFileAsync(string FullFilePath, long FileSizeBytes, DateTime FileLastModified);

        /// <summary>
        /// Returns the directory map item for the specified local directory.
        /// </summary>
        /// <remarks>
        /// A new directory map item will be created if none currently exists for the specified folder.
        /// </remarks>
        /// <param name="DirectoryPath">Local directory path. Ex: 'C:\bin\programs'</param>
        /// <returns><c>DirectoryMapItem</c></returns>
        Task<DirectoryMapItem> GetDirectoryMapItemAsync(string DirectoryPath);

        /// <summary>
        /// Get the source location by ID and type.
        /// </summary>
        /// <param name="sourceID">The ID of the source location.</param>
        /// <param name="sourceType">The type of source location.</param>
        /// <returns></returns>
        Task<SourceLocation> GetSourceLocationAsync(int sourceID, SourceLocationType sourceType);

        /// <summary>
        /// Returns all source locations defined in the database.
        /// </summary>
        /// <returns><c>SourceLocations</c></returns>
        Task<SourceLocations> GetSourceLocationsAsync();

        /// <summary>
        /// Adds or updates a single source location.
        /// </summary>
        /// <param name="Location"><c>SourceLocation</c></param>
        Task SetSourceLocationAsync(SourceLocation Location);

        /// <summary>
        /// Removes a single source location.
        /// </summary>
        /// <param name="Location"></param>
        /// <returns></returns>
        Task RemoveSourceLocationAsync(SourceLocation Location);

        /// <summary>
        /// Forces a rescan of a single source location.
        /// </summary>
        /// <param name="Location"><c>SourceLocation</c></param>
        Task RescanSourceLocationAsync(SourceLocation Location);

        /// <summary>
        /// Gets the next file that needs to be backed up.
        /// </summary>
        /// <remarks>
        /// If no files need to be backed up, return null.
        /// </remarks>
        /// <param name="EngineInstanceID">The engine instance.</param>
        /// <param name="Priority">The file backup priority.</param>
        /// <returns><c>BackupFile</c></returns>
        Task<BackupFile> FindNextFileToBackupAsync(int EngineInstanceID, FileBackupPriority Priority);

        /// <summary>
        /// Gets the next file that needs to be cleaned up.
        /// </summary>
        /// <remarks>
        /// If no files need to be cleaned up, return null.
        /// </remarks>
        /// <param name="EngineInstanceID">The engine instance.</param>
        /// <returns><c>BackupFile</c></returns>
        Task<BackupFile> FindNextFileToCleanupAsync(int EngineInstanceID);

        /// <summary>
        /// Calculates and returns the overall backup progress.
        /// </summary>
        /// <returns></returns>
        Task<BackupProgress> GetBackupProgressAsync();

        /// <summary>
        /// Adds a new file to the database.
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>
        Task AddBackupFileAsync(BackupFile File);

        /// <summary>
        /// Resets the copy state of a backup file back to unsynced.
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>
        Task ResetBackupFileStateAsync(BackupFile File);

        /// <summary>
        /// Resets the last scanned date of a backup file.
        /// </summary>
        /// <param name="FileID"></param>
        /// <returns></returns>
        Task SetBackupFileLastScannedAsync(Guid FileID);

        /// <summary>
        /// Deletes the backup file from the backup files table/index.
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>
        Task DeleteBackupFileAsync(BackupFile File);

        /// <summary>
        /// Flags a backup file as failed state.
        /// </summary>
        /// <param name="File"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        Task SetBackupFileAsFailedAsync(BackupFile File, string Message);

        /// <summary>
        /// Updates the backup file hash.
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>
        Task SetBackupFileHashAsync(BackupFile File);

        /// <summary>
        /// Updates the backup files copy state.
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>
        Task UpdateBackupFileCopyStateAsync(BackupFile File);

        /// <summary>
        /// Removes a file from the backup file queue.
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>
        Task RemoveFileFromBackupQueueAsync(BackupFile File);

        /// <summary>
        /// Removes a file from the cleanup file queue.
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>
        Task RemoveFileFromCleanupQueueAsync(BackupFile File);
    }
}
