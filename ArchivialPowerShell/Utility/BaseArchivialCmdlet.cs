using ArchivialLibrary.Database;
using ArchivialLibrary.Database.SQLServer;
using ArchivialLibrary.Exceptions;
using ArchivialLibrary.Logging.Default;
using ArchivialLibrary.Secrets;
using ArchivialLibrary.ServiceCore;
using System;
using System.Management.Automation;

namespace ArchivialPowerShell.Utility
{
    /// <summary>
    /// A base cmdlet object that provides common functionality.
    /// </summary>
    public class BaseArchivialCmdlet : Cmdlet
    {
        /// <summary>
        /// A reference to the client database.
        /// </summary>
        internal IClientDatabase Database { get; set; }

        /// <summary>
        /// A reference to the secret store.
        /// </summary>
        internal ISecretStore SecretStore { get; set; }

        /// <summary>
        /// The name of this cmdlet activity for progress tracking.
        /// </summary>
        internal string ActivityName { get; set; }

        /// <summary>
        /// The name of this cmdlet activity ID for progress tracking.
        /// </summary>
        internal int ActivityID { get; set; }

        /// <summary>
        /// Writes progress output.
        /// </summary>
        /// <param name="Percentage"></param>
        /// <param name="Description"></param>
        internal void WriteProgress(int Percentage, string Description)
        {
            base.WriteProgress(
                new ProgressRecord(ActivityID, ActivityName, Description) { PercentComplete = Percentage }
            );
        }

        /// <summary>
        /// Writes progress output and verbose logs at the same time.
        /// </summary>
        /// <param name="Percentage"></param>
        /// <param name="Description"></param>
        internal void WriteVerboseAndProgress(int Percentage, string Description)
        {
            base.WriteProgress(
                new ProgressRecord(ActivityID, ActivityName, Description) { PercentComplete = Percentage }
            );

            base.WriteVerbose(Description);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BaseArchivialCmdlet()
        {
        }

        /// <summary>
        /// Secondary constructor for dependency injection.
        /// </summary>
        /// <param name="database"></param>
        public BaseArchivialCmdlet(IClientDatabase database)
        {
            Database = database;
        }

        /// <summary>
        /// Secondary constructor for dependency injection.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="secretStore"></param>
        public BaseArchivialCmdlet(IClientDatabase database, ISecretStore secretStore)
        {
            Database = database;
            SecretStore = secretStore;
        }

        /// <summary>
        /// Returns the database connection.
        /// </summary>
        /// <returns></returns>
        internal IClientDatabase GetDatabaseConnection()
        {
            if (Database != null)
            {
                return Database;
            }
            else
            {
                base.WriteVerbose("Preparing Archivial Database Connection.");

                string dbConnectionString = null;

                try
                {
                    dbConnectionString = CoreSettings.DatabaseConnectionString;

                }
                catch (ApplicationCoreSettingMissingException)
                {
                    throw new ApplicationException("Cannot run the requested command. Archivial Cloud Backup installation is missing and the product must be installed first.");
                }

                base.WriteVerbose("Database connection string: " + dbConnectionString);

                var logger = new Logger("ArchivialPowerShell");
                var db = new SQLServerClientDatabase(dbConnectionString, logger);

                Database = db;
                return db;
            }
        }

        /// <summary>
        /// Returns the secret store.
        /// </summary>
        /// <returns></returns>
        internal ISecretStore GetSecretStore()
        {
            if (SecretStore != null)
            {
                return SecretStore;
            }
            else
            {
                base.WriteVerbose("Initializing protected data store.");

                var db = GetDatabaseConnection();

                var scope = System.Security.Cryptography.DataProtectionScope.LocalMachine;
                var settingName = ArchivialLibrary.Constants.RuntimeSettingNames.ProtectionIV;

                var protectionIvEncodedString = db.GetApplicationOptionAsync(settingName).GetAwaiter().GetResult();
                var ivkey = Convert.FromBase64String(protectionIvEncodedString);

                var pds = new ProtectedDataStore(db, scope, ivkey);

                SecretStore = pds;
                return pds;
            }
        }
    }
}
