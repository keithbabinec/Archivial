using OzettePowerShell.Utility;
using System.Management.Automation;

namespace OzettePowerShell.Functions.Public
{
    [Cmdlet(VerbsCommon.Get, "OzetteProviders")]
    public class GetOzetteProvidersCommand : BaseOzetteCmdlet
    {
        protected override void ProcessRecord()
        {
            var db = GetDatabaseConnection();

            WriteVerbose("Querying for configured providers.");

            var allProviders = db.GetProvidersAsync(OzetteLibrary.Providers.ProviderTypes.Any).GetAwaiter().GetResult();

            WriteVerbose(string.Format("Writing output results to pipeline (Objects: {0})", allProviders.Count));

            foreach (var provider in allProviders)
            {
                WriteObject(provider);
            }
        }
    }
}
