using OzettePowerShell.Utility;
using System.Management.Automation;

namespace OzettePowerShell.Functions.Public
{
    [Cmdlet(VerbsCommon.Get, "OzetteCloudBackupStatus")]
    public class GetOzetteCloudBackupStatusCommand : BaseOzetteCmdlet
    {
        protected override void ProcessRecord()
        {
            var db = GetDatabaseConnection();

            WriteVerbose("Querying for backup status.");

            var progress = db.GetBackupProgressAsync().GetAwaiter().GetResult();

            WriteObject(progress);
        }
    }
}
