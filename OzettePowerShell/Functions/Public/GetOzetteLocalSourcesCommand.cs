using OzetteLibrary.Folders;
using OzettePowerShell.Utility;
using System.Linq;
using System.Management.Automation;

namespace OzettePowerShell.Functions.Public
{
    /// <summary>
    ///   <para type="synopsis">Returns all of the Local Source folders being monitored by Ozette.</para>
    ///   <para type="description">Returns all of the Local Source folders being monitored by Ozette. A local source folder is one that resides on your computer (including directly attached external hard drives).</para>
    ///   <para type="description">The output from this command can be piped to the Remove-OzetteLocalSource cmdlet.</para>
    /// </summary>
    /// <example>
    ///   <code>C:\> Get-OzetteLocalSources</code>
    ///   <para>Returns all of the Local Source folders being monitored by Ozette.</para>
    ///   <para></para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "OzetteLocalSources")]
    public class GetOzetteLocalSourcesCommand : BaseOzetteCmdlet
    {
        protected override void ProcessRecord()
        {
            var db = GetDatabaseConnection();

            WriteVerbose("Querying for existing backup sources.");

            var allSources = db.GetSourceLocationsAsync().GetAwaiter().GetResult();

            var filtered = allSources.Where(x => x is LocalSourceLocation).ToArray();

            WriteVerbose(string.Format("Writing output results to pipeline (Objects: {0})", filtered.Length));

            foreach (var source in filtered)
            {
                WriteObject(source);
            }
        }
    }
}
