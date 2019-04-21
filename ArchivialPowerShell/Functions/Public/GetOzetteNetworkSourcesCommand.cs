using ArchivialLibrary.Folders;
using ArchivialPowerShell.Utility;
using System.Linq;
using System.Management.Automation;

namespace ArchivialPowerShell.Functions.Public
{
    /// <summary>
    ///   <para type="synopsis">Returns all of the Network Source folders being monitored by Ozette.</para>
    ///   <para type="description">A Network Source is a folder on your network (referenced by UNC Path) that Ozette backs up and automatically monitors for new and updated files.</para>
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
