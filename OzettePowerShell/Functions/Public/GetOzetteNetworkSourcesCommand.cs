using OzetteLibrary.Folders;
using OzettePowerShell.Utility;
using System.Linq;
using System.Management.Automation;

namespace OzettePowerShell.Functions.Public
{
    /// <summary>
    ///   <para type="synopsis">Returns all of the Network Source folders being monitored by Ozette.</para>
    ///   <para type="description">Returns all of the Network Source folders being monitored by Ozette. A network source folder is one that resides on your local network and is accessible via UNC path, and may or may not be authenticated.</para>
    ///   <para type="description">The output from this command can be piped to the Remove-OzetteNetworkSource cmdlet.</para>
    /// </summary>
    /// <example>
    ///   <code>C:\> Get-OzetteNetworkSources</code>
    ///   <para>Returns all of the Network Source folders being monitored by Ozette.</para>
    ///   <para></para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "OzetteNetworkSources")]
    public class GetOzetteNetworkSourcesCommand : BaseOzetteCmdlet
    {
        protected override void ProcessRecord()
        {
            var db = GetDatabaseConnection();

            WriteVerbose("Querying for existing backup sources.");

            var allSources = db.GetSourceLocationsAsync().GetAwaiter().GetResult();

            var filtered = allSources.Where(x => x is NetworkSourceLocation).ToArray();

            WriteVerbose(string.Format("Writing output results to pipeline (Objects: {0})", filtered.Length));

            foreach (var source in filtered)
            {
                WriteObject(source);
            }
        }
    }
}
