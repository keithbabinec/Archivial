using OzetteLibrary.Folders;
using OzettePowerShell.Utility;
using System.Management.Automation;

namespace OzettePowerShell.Functions.Public
{
    [Cmdlet(VerbsCommon.Get, "OzetteNetworkSources")]
    public class GetOzetteNetworkSourcesCommand : BaseOzetteCmdlet
    {
        protected override void ProcessRecord()
        {
            var db = GetDatabaseConnection();

            WriteVerbose("Querying for existing backup sources.");

            var allSources = db.GetSourceLocationsAsync().GetAwaiter().GetResult();

            WriteVerbose("Writing output results to pipeline.");

            foreach (var source in allSources)
            {
                if (source is NetworkSourceLocation)
                {
                    WriteObject(source);
                }
            }
        }
    }
}
