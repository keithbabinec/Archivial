using ArchivialLibrary.Database;
using ArchivialLibrary.Folders;
using ArchivialLibrary.Secrets;
using ArchivialPowerShell.Setup;
using ArchivialPowerShell.Utility;
using System.Linq;
using System.Management.Automation;

namespace ArchivialPowerShell.Functions.Public
{
    /// <summary>
    ///   <para type="synopsis">Returns all of the Network Source folders being monitored by Archivial.</para>
    ///   <para type="description">A Network Source is a folder on your network (referenced by UNC Path) that Archivial backs up and automatically monitors for new and updated files.</para>
    ///   <para type="description">The output from this command can be piped to the Remove-ArchivialNetworkSource cmdlet.</para>
    /// </summary>
    /// <example>
    ///   <code>C:\> Get-ArchivialNetworkSources</code>
    ///   <para>Returns all of the Network Source folders being monitored by Archivial.</para>
    ///   <para></para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "ArchivialNetworkSources")]
    public class GetArchivialNetworkSourcesCommand : BaseArchivialCmdlet
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public GetArchivialNetworkSourcesCommand() : base() { }

        /// <summary>
        /// Secondary constructor for dependency injection.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="secretStore"></param>
        /// <param name="setup"></param>
        public GetArchivialNetworkSourcesCommand(IClientDatabase database, ISecretStore secretStore, ISetup setup) : base(database, secretStore, setup) { }

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
