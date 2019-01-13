using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using OzetteLibrary.Files;
using OzetteLibrary.Folders;
using OzetteLibrary.Providers;
using OzetteLibrary.Secrets;
using OzetteLibrary.ServiceCore;

namespace OzetteLibrary.Database.SQLServer
{
    /// <summary>
    /// A SQL Server implementation of the client database. 
    /// </summary>
    public class SQLServerClientDatabase : IClientDatabase
    {
        /// <summary>
        /// Instantiates a client DB from database connection string.
        /// </summary>
        /// <param name="connectionString"></param>
        public SQLServerClientDatabase(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(nameof(connectionString));
            }

            DatabaseConnectionString = connectionString;
        }

        /// <summary>
        /// The database connection string.
        /// </summary>
        /// <remarks>
        /// A memory stream or database file is used, but not both.
        /// </remarks>
        private string DatabaseConnectionString;

        /// <summary>
        /// Saves an application setting to the database.
        /// </summary>
        /// <param name="OptionName">Option name</param>
        /// <param name="OptionValue">Option value</param>
        public async Task SetApplicationOptionAsync(string OptionName, string OptionValue)
        {
            if (string.IsNullOrWhiteSpace(OptionName))
            {
                throw new ArgumentException(nameof(OptionName) + " must be provided.");
            }
            if (string.IsNullOrWhiteSpace(OptionValue))
            {
                throw new ArgumentException(nameof(OptionValue) + " must be provided.");
            }

            try
            {
                using (SqlConnection sqlcon = new SqlConnection(DatabaseConnectionString))
                {
                    await sqlcon.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = sqlcon;
                        cmd.CommandText = "dbo.SetApplicationOption";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Name", OptionName);
                        cmd.Parameters.AddWithValue("@Value", OptionValue);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Retrieves an application setting value from the database.
        /// </summary>
        /// <remarks>
        /// Returns null if the setting is not found.
        /// </remarks>
        /// <param name="OptionName">Option name</param>
        /// <returns>The setting value.</returns>
        public async Task<string> GetApplicationOptionAsync(string OptionName)
        {
            if (string.IsNullOrWhiteSpace(OptionName))
            {
                throw new ArgumentException(nameof(OptionName) + " must be provided.");
            }

            try
            {
                using (SqlConnection sqlcon = new SqlConnection(DatabaseConnectionString))
                {
                    await sqlcon.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = sqlcon;
                        cmd.CommandText = "dbo.GetApplicationOption";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Name", OptionName);

                        var value = await cmd.ExecuteScalarAsync();

                        if (value != null)
                        {
                            return value.ToString();
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Removes an application setting value from the database.
        /// </summary>
        /// <param name="OptionName">Option name</param>
        public async Task RemoveApplicationOptionAsync(string OptionName)
        {
            if (string.IsNullOrWhiteSpace(OptionName))
            {
                throw new ArgumentException(nameof(OptionName) + " must be provided.");
            }

            try
            {
                using (SqlConnection sqlcon = new SqlConnection(DatabaseConnectionString))
                {
                    await sqlcon.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = sqlcon;
                        cmd.CommandText = "dbo.RemoveApplicationOption";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Name", OptionName);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Returns a list of all providers defined in the database for the specified type.
        /// </summary>
        /// <param name="Type">The type of providers to return.</param>
        /// <returns><c>ProviderCollection</c></returns>
        public ProviderCollection GetProviders(ProviderTypes Type)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the specified provider by ID.
        /// </summary>
        /// <param name="ProviderID">Provider ID.</param>
        public void RemoveProvider(int ProviderID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds the specified Provider object to the database.
        /// </summary>
        /// <param name="Provider"></param>
        public void AddProvider(Provider Provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Commits the net credentials collection to the database.
        /// </summary>
        /// <param name="Credentials">A collection of net credentials.</param>
        public void SetNetCredentialsList(NetCredentialsCollection Credentials)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns all of the net credentials defined in the database.
        /// </summary>
        /// <returns>A collection of net credentials.</returns>
        public NetCredentialsCollection GetNetCredentialsList()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks the index for a file matching the provided name, path, filesize, and lastmodified date.
        /// </summary>
        /// <param name="FullFilePath">Full file path (file name and path)</param>
        /// <param name="FileSizeBytes">File size in bytes</param>
        /// <param name="FileLastModified">File last modified timestamp</param>
        /// <returns></returns>
        public BackupFileLookup GetBackupFile(string FullFilePath, long FileSizeBytes, DateTime FileLastModified)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns all of the client files in the database.
        /// </summary>
        /// <returns><c>BackupFile</c></returns>
        public BackupFiles GetAllBackupFiles()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the directory map item for the specified local directory.
        /// </summary>
        /// <remarks>
        /// A new directory map item will be created if none currently exists for the specified folder.
        /// </remarks>
        /// <param name="DirectoryPath">Local directory path. Ex: 'C:\bin\programs'</param>
        /// <returns><c>DirectoryMapItem</c></returns>
        public DirectoryMapItem GetDirectoryMapItem(string DirectoryPath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns all source locations defined in the database.
        /// </summary>
        /// <returns><c>SourceLocations</c></returns>
        public SourceLocations GetAllSourceLocations()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets a new source locations collection in the database (this will wipe out existing sources).
        /// </summary>
        /// <param name="Locations"><c>SourceLocations</c></param>
        public void SetSourceLocations(SourceLocations Locations)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates a single source location with the specified source.
        /// </summary>
        /// <param name="Location"><c>SourceLocation</c></param>
        public void UpdateSourceLocation(SourceLocation Location)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a new client file to the database.
        /// </summary>
        /// <param name="File"><c>BackupFile</c></param>
        public void AddBackupFile(BackupFile File)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates an existing client file in the database.
        /// </summary>
        /// <param name="File"><c>BackupFile</c></param>
        public void UpdateBackupFile(BackupFile File)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the next file that needs to be backed up.
        /// </summary>
        /// <remarks>
        /// If no files need to be backed up, return null.
        /// </remarks>
        /// <returns><c>BackupFile</c></returns>
        public BackupFile GetNextFileToBackup()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates and returns the overall backup progress.
        /// </summary>
        /// <returns></returns>
        public BackupProgress GetBackupProgress()
        {
            throw new NotImplementedException();
        }
    }
}
