using System;
using System.Data.SqlClient;
using System.Globalization;
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
        public async Task<ProviderCollection> GetProvidersAsync(ProviderTypes Type)
        {
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(DatabaseConnectionString))
                {
                    await sqlcon.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = sqlcon;
                        cmd.CommandText = "dbo.GetProviders";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Type", Type);

                        var result = new ProviderCollection();

                        using (var rdr = await cmd.ExecuteReaderAsync())
                        {
                            if (rdr.HasRows)
                            {
                                while (await rdr.ReadAsync())
                                {
                                    result.Add(new Provider()
                                    {
                                        ID = rdr.GetInt32(0),
                                        Name = rdr.GetString(1),
                                        Type = (ProviderTypes)rdr.GetInt32(2)
                                    });
                                }
                            }
                        }

                        return result;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Removes the specified provider by name.
        /// </summary>
        /// <param name="ProviderName">Provider name.</param>
        public async Task RemoveProviderAsync(string ProviderName)
        {
            if (string.IsNullOrWhiteSpace(ProviderName))
            {
                throw new ArgumentException(nameof(ProviderName) + " must be provided.");
            }

            try
            {
                using (SqlConnection sqlcon = new SqlConnection(DatabaseConnectionString))
                {
                    await sqlcon.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = sqlcon;
                        cmd.CommandText = "dbo.RemoveProvider";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Name", ProviderName);

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
        /// Adds the specified Provider object to the database.
        /// </summary>
        /// <param name="Provider"></param>
        public async Task AddProviderAsync(Provider Provider)
        {
            if (Provider == null)
            {
                throw new ArgumentNullException(nameof(Provider));
            }
            if (string.IsNullOrWhiteSpace(Provider.Name))
            {
                throw new ArgumentException(nameof(Provider.Name) + " must be provided.");
            }

            try
            {
                using (SqlConnection sqlcon = new SqlConnection(DatabaseConnectionString))
                {
                    await sqlcon.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = sqlcon;
                        cmd.CommandText = "dbo.AddProvider";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Name", Provider.Name);
                        cmd.Parameters.AddWithValue("@Type", Provider.Type);

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
        /// Adds a net credential to the database.
        /// </summary>
        /// <param name="Credential"></param>
        /// <returns></returns>
        public async Task AddNetCredentialAsync(NetCredential Credential)
        {
            if (Credential == null)
            {
                throw new ArgumentNullException(nameof(Credential));
            }
            if (string.IsNullOrWhiteSpace(Credential.CredentialName))
            {
                throw new ArgumentException(nameof(Credential.CredentialName) + " must be provided.");
            }

            try
            {
                using (SqlConnection sqlcon = new SqlConnection(DatabaseConnectionString))
                {
                    await sqlcon.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = sqlcon;
                        cmd.CommandText = "dbo.AddNetCredential";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Name", Credential.CredentialName);

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
        /// Removes the specified net credential by Name.
        /// </summary>
        /// <param name="CredentialName">Provider name.</param>
        public async Task RemoveNetCredentialAsync(string CredentialName)
        {
            if (string.IsNullOrWhiteSpace(CredentialName))
            {
                throw new ArgumentException(nameof(CredentialName) + " must be provided.");
            }

            try
            {
                using (SqlConnection sqlcon = new SqlConnection(DatabaseConnectionString))
                {
                    await sqlcon.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = sqlcon;
                        cmd.CommandText = "dbo.RemoveNetCredential";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Name", CredentialName);

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
        /// Returns all of the net credentials defined in the database.
        /// </summary>
        /// <returns>A collection of net credentials.</returns>
        public async Task<NetCredentialsCollection> GetNetCredentialsAsync()
        {
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(DatabaseConnectionString))
                {
                    await sqlcon.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = sqlcon;
                        cmd.CommandText = "dbo.GetNetCredentials";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        var result = new NetCredentialsCollection();

                        using (var rdr = await cmd.ExecuteReaderAsync())
                        {
                            if (rdr.HasRows)
                            {
                                while (await rdr.ReadAsync())
                                {
                                    result.Add(new NetCredential()
                                    {
                                        ID = rdr.GetInt32(0),
                                        CredentialName = rdr.GetString(1),
                                    });
                                }
                            }
                        }

                        return result;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
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
        /// Returns the directory map item for the specified local directory.
        /// </summary>
        /// <remarks>
        /// A new directory map item will be created if none currently exists for the specified folder.
        /// </remarks>
        /// <param name="DirectoryPath">Local directory path. Ex: 'C:\bin\programs'</param>
        /// <returns><c>DirectoryMapItem</c></returns>
        public async Task<DirectoryMapItem> GetDirectoryMapItemAsync(string DirectoryPath)
        {
            if (string.IsNullOrWhiteSpace(DirectoryPath))
            {
                throw new ArgumentException(nameof(DirectoryPath) + " must be provided.");
            }

            try
            {
                using (SqlConnection sqlcon = new SqlConnection(DatabaseConnectionString))
                {
                    await sqlcon.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = sqlcon;
                        cmd.CommandText = "dbo.GetDirectoryMapItem";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@DirectoryPath", DirectoryPath.ToLower());

                        using (var rdr = await cmd.ExecuteReaderAsync())
                        {
                            if (rdr.HasRows)
                            {
                                // should only be exactly one row.
                                await rdr.ReadAsync();

                                var item = new DirectoryMapItem()
                                {
                                    ID = rdr.GetGuid(0),
                                    LocalPath = rdr.GetString(1)
                                };

                                return item;
                            }
                            else
                            {
                                throw new Exception("Failed to generate a directory map item. No output was returned from the database.");
                            }
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
        /// Returns all source locations defined in the database.
        /// </summary>
        /// <returns><c>SourceLocations</c></returns>
        public async Task<SourceLocations> GetSourceLocationsAsync()
        {
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(DatabaseConnectionString))
                {
                    await sqlcon.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = sqlcon;
                        cmd.CommandText = "dbo.GetSourceLocations";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        var result = new SourceLocations();

                        using (var rdr = await cmd.ExecuteReaderAsync())
                        {
                            // local sources

                            while (await rdr.ReadAsync())
                            {
                                result.Add(new LocalSourceLocation()
                                {
                                    ID = rdr.GetInt32(0),
                                    Path = rdr.GetString(1),
                                    FileMatchFilter = rdr.GetString(2),
                                    Priority = (FileBackupPriority)rdr.GetInt32(3),
                                    RevisionCount = rdr.GetInt32(4),
                                    LastCompletedScan = rdr.GetDateTime(5)
                                });
                            }

                            // network sources

                            if (await rdr.NextResultAsync())
                            {
                                while (await rdr.ReadAsync())
                                {
                                    result.Add(new NetworkSourceLocation()
                                    {
                                        ID = rdr.GetInt32(0),
                                        Path = rdr.GetString(1),
                                        FileMatchFilter = rdr.GetString(2),
                                        Priority = (FileBackupPriority)rdr.GetInt32(3),
                                        RevisionCount = rdr.GetInt32(4),
                                        LastCompletedScan = rdr.GetDateTime(5),
                                        CredentialName = rdr.GetString(6),
                                        IsConnected = rdr.GetBoolean(7),
                                        IsFailed = rdr.GetBoolean(8),
                                        LastConnectionCheck = rdr.IsDBNull(9) ? (DateTime?)null : rdr.GetDateTime(9)
                                    });
                                }
                            }
                        }

                        return result;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Adds or updates a single source location.
        /// </summary>
        /// <param name="Location"><c>SourceLocation</c></param>
        public async Task SetSourceLocationAsync(SourceLocation Location)
        {
            if (Location == null)
            {
                throw new ArgumentNullException(nameof(Location));
            }

            try
            {
                using (SqlConnection sqlcon = new SqlConnection(DatabaseConnectionString))
                {
                    await sqlcon.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = sqlcon;

                        if (Location is LocalSourceLocation)
                        {
                            cmd.CommandText = "dbo.SetLocalSourceLocation";
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;

                            var lsl = Location as LocalSourceLocation;

                            cmd.Parameters.AddWithValue("@Path", lsl.Path);
                            cmd.Parameters.AddWithValue("@FileMatchFilter", lsl.FileMatchFilter);
                            cmd.Parameters.AddWithValue("@Priority", lsl.Priority);
                            cmd.Parameters.AddWithValue("@RevisionCount", lsl.RevisionCount);
                            cmd.Parameters.AddWithValue("@LastCompletedScan", lsl.LastCompletedScan);
                        }
                        else if (Location is NetworkSourceLocation)
                        {
                            cmd.CommandText = "dbo.SetNetworkSourceLocation";
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;

                            var nsl = Location as NetworkSourceLocation;

                            cmd.Parameters.AddWithValue("@Path", nsl.Path);
                            cmd.Parameters.AddWithValue("@FileMatchFilter", nsl.FileMatchFilter);
                            cmd.Parameters.AddWithValue("@Priority", nsl.Priority);
                            cmd.Parameters.AddWithValue("@RevisionCount", nsl.RevisionCount);
                            cmd.Parameters.AddWithValue("@LastCompletedScan", nsl.LastCompletedScan);
                            cmd.Parameters.AddWithValue("@CredentialName", nsl.CredentialName);
                            cmd.Parameters.AddWithValue("@IsConnected", nsl.IsConnected);
                            cmd.Parameters.AddWithValue("@IsFailed", nsl.IsFailed);
                            cmd.Parameters.AddWithValue("@LastConnectionCheck", nsl.LastConnectionCheck);
                        }
                        else
                        {
                            throw new NotImplementedException("Unexpected source location type: " + Location.GetType().FullName);
                        }

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
        /// Removes a single source location.
        /// </summary>
        /// <param name="Location"></param>
        /// <returns></returns>
        public async Task RemoveSourceLocationAsync(SourceLocation Location)
        {
            if (Location == null)
            {
                throw new ArgumentNullException(nameof(Location));
            }
            if (Location.ID <= 0)
            {
                throw new ArgumentException(nameof(Location.ID) + " must be provided.");
            }

            try
            {
                using (SqlConnection sqlcon = new SqlConnection(DatabaseConnectionString))
                {
                    await sqlcon.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = sqlcon;

                        if (Location is LocalSourceLocation)
                        {
                            cmd.CommandText = "dbo.RemoveLocalSourceLocation";
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ID", Location.ID);
                        }
                        else if (Location is NetworkSourceLocation)
                        {
                            cmd.CommandText = "dbo.RemoveNetworkSourceLocation";
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ID", Location.ID);
                        }
                        else
                        {
                            throw new NotImplementedException("Unexpected source location type: " + Location.GetType().FullName);
                        }

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
        public async Task<BackupProgress> GetBackupProgressAsync()
        {
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(DatabaseConnectionString))
                {
                    await sqlcon.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = sqlcon;
                        cmd.CommandText = "dbo.GetBackupProgress";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        using (var rdr = await cmd.ExecuteReaderAsync())
                        {
                            if (rdr.HasRows)
                            {
                                // should only be exactly one row.
                                await rdr.ReadAsync();

                                var item = new BackupProgress()
                                {
                                    TotalFileCount = rdr.GetInt64(0),
                                    TotalFileSizeBytes = rdr.GetInt64(1),
                                    BackedUpFileCount = rdr.GetInt64(2),
                                    BackedUpFileSizeBytes = rdr.GetInt64(3),
                                    RemainingFileCount = rdr.GetInt64(4),
                                    RemainingFileSizeBytes = rdr.GetInt64(5),
                                    FailedFileCount = rdr.GetInt64(6),
                                    FailedFileSizeBytes = rdr.GetInt64(7)
                                };

                                // set the overall completion rate
                                if (item.TotalFileSizeBytes != 0)
                                {
                                    item.OverallPercentage = ((double)item.BackedUpFileSizeBytes / item.TotalFileSizeBytes).ToString("P2", CultureInfo.CreateSpecificCulture("US"));
                                }
                                else
                                {
                                    item.OverallPercentage = "0.00 %";
                                }

                                return item;
                            }
                            else
                            {
                                throw new Exception("Failed to generate the backup progress. No output was returned from the database.");
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
