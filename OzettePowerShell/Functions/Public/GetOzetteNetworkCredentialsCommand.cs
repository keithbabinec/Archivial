using OzettePowerShell.Utility;
using System.Management.Automation;

namespace OzettePowerShell.Functions.Public
{
    [Cmdlet(VerbsCommon.Get, "OzetteNetworkCredentials")]
    public class GetOzetteNetworkCredentialsCommand : BaseOzetteCmdlet
    {
        protected override void ProcessRecord()
        {
            var db = GetDatabaseConnection();

            WriteVerbose("Querying for network credentials.");

            var allNetCreds = db.GetNetCredentialsAsync().GetAwaiter().GetResult();

            WriteVerbose("Writing output results to pipeline.");

            foreach (var netCred in allNetCreds)
            {
                WriteObject(netCred);
            }
        }
    }
}
