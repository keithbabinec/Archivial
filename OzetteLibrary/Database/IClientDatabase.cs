using OzetteLibrary.Files;
using OzetteLibrary.Folders;
using OzetteLibrary.Providers;
using OzetteLibrary.ServiceCore;
using System.Collections.Generic;

namespace OzetteLibrary.Database
{
    /// <summary>
    /// A generic database interface for the client database.
    /// </summary>
    public interface IClientDatabase : IDatabase
    {
        /// <summary>
        /// Saves an application setting to the database.
        /// </summary>
        /// <param name="option">ServiceOption</param>
        void SetApplicationOption(ServiceOption option);

        /// <summary>
        /// Retrieves an application setting value from the database.
        /// </summary>
        /// <remarks>
        /// Returns null if the setting is not found.
        /// </remarks>
        /// <param name="SettingName">The setting name.</param>
        /// <returns>The setting value.</returns>
        string GetApplicationOption(string SettingName);

        /// <summary>
        /// Commits the provider options to the database.
        /// </summary>
        /// <param name="Provider">A list of Provider options</param>
        void SetProviders(List<ProviderOptions> Provider);

        /// <summary>
        /// Returns all of the providers defined in the database.
        /// </summary>
        /// <returns>An array of Provider types</returns>
        ProviderTypes[] GetProvidersList();

        /// <summary>
        /// Returns the provider options for the specified provider.
        /// </summary>
        /// <param name="ProviderType">A single provider type.</param>
        /// <returns><c>ProviderOptions</c></returns>
        ProviderOptions GetProviderOptions(ProviderTypes ProviderType);

        /// <summary>
        /// Checks the index for a file matching the provided name, path, and hash.
        /// </summary>
        /// <remarks>
        /// The lookup result object is returned that contains:
        /// 1. A reference to the indexed file, if present.
        /// 2. An enumeration that describes the file state (new, updated, moved, renamed, etc).
        /// </remarks>
        /// <param name="FileName">Name of the file (ex: document.doc)</param>
        /// <param name="DirectoryPath">Full directory path (ex: C:\folder\documents)</param>
        /// <param name="FileHash">File hash expressed as a byte array.</param>
        /// <returns><c>BackupFileLookup</c></returns>
        BackupFileLookup GetBackupFile(string FileName, string DirectoryPath, byte[] FileHash);

        /// <summary>
        /// Returns all of the client files in the database.
        /// </summary>
        /// <returns><c>BackupFile</c></returns>
        BackupFiles GetAllBackupFiles();

        /// <summary>
        /// Returns the directory map item for the specified local directory.
        /// </summary>
        /// <remarks>
        /// A new directory map item will be created if none currently exists for the specified folder.
        /// </remarks>
        /// <param name="DirectoryPath">Local directory path. Ex: 'C:\bin\programs'</param>
        /// <returns><c>DirectoryMapItem</c></returns>
        DirectoryMapItem GetDirectoryMapItem(string DirectoryPath);

        /// <summary>
        /// Returns all source locations defined in the database.
        /// </summary>
        /// <returns><c>SourceLocations</c></returns>
        SourceLocations GetAllSourceLocations();

        /// <summary>
        /// Sets a new source locations collection in the database (this will wipe out existing sources).
        /// </summary>
        /// <param name="Locations"><c>SourceLocations</c></param>
        void SetSourceLocations(SourceLocations Locations);

        /// <summary>
        /// Updates a single source location with the specified source.
        /// </summary>
        /// <param name="Location"><c>SourceLocation</c></param>
        void UpdateSourceLocation(SourceLocation Location);

        /// <summary>
        /// Adds a new client file to the database.
        /// </summary>
        /// <param name="File"><c>BackupFile</c></param>
        void AddBackupFile(BackupFile File);

        /// <summary>
        /// Updates an existing client file in the database.
        /// </summary>
        /// <param name="File"><c>BackupFile</c></param>
        void UpdateBackupFile(BackupFile File);

        /// <summary>
        /// Gets the next file that needs to be backed up.
        /// </summary>
        /// <remarks>
        /// If no files need to be backed up, return null.
        /// </remarks>
        /// <returns><c>BackupFile</c></returns>
        BackupFile GetNextFileToBackup();
    }
}
