using OzetteLibrary.Files;
using OzetteLibrary.Folders;
using OzetteLibrary.Secrets;
using OzetteLibrary.ServiceCore;
using System;
using OzetteLibrary.Providers;
using System.Threading.Tasks;

namespace OzetteLibrary.Database
{
    /// <summary>
    /// A generic database interface for the client database.
    /// </summary>
    public interface IClientDatabase
    {
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
        BackupFileLookup GetBackupFile(string FullFilePath, long FileSizeBytes, DateTime FileLastModified);

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
        /// Adds a new client file to the database.
        /// </summary>
        /// <param name="File"><c>BackupFile</c></param>
        /// <param name="UpdateCopyState">Flag to indicate if we should be updating copy state.</param>
        Task SetBackupFileAsync(BackupFile File, bool UpdateCopyState);

        /// <summary>
        /// Gets the next file that needs to be backed up.
        /// </summary>
        /// <remarks>
        /// If no files need to be backed up, return null.
        /// </remarks>
        /// <returns><c>BackupFile</c></returns>
        BackupFile GetNextFileToBackup();

        /// <summary>
        /// Calculates and returns the overall backup progress.
        /// </summary>
        /// <returns></returns>
        Task<BackupProgress> GetBackupProgressAsync();
    }
}
