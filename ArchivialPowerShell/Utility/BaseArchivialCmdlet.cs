using ArchivialLibrary.Database.SQLServer;
using ArchivialLibrary.Exceptions;
using ArchivialLibrary.Logging.Default;
using ArchivialLibrary.ServiceCore;
using System;
using System.Management.Automation;

namespace ArchivialPowerShell.Utility
{
    public class BaseArchivialCmdlet : Cmdlet
    {
        internal string ActivityName { get; set; }

        internal int ActivityID { get; set; }

        internal void WriteProgress(int Percentage, string Description)
        {
            base.WriteProgress(
                new ProgressRecord(ActivityID, ActivityName, Description) { PercentComplete = Percentage }
            );
        }

        internal void WriteVerboseAndProgress(int Percentage, string Description)
        {
            base.WriteProgress(
                new ProgressRecord(ActivityID, ActivityName, Description) { PercentComplete = Percentage }
            );

            base.WriteVerbose(Description);
        }

        internal SQLServerClientDatabase GetDatabaseConnection()
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

            return db;
        }
    }
}
