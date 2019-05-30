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
    ///   <para type="synopsis">Returns all of the Local Source folders being monitored by Archivial.</para>
    ///   <para type="description">A Local Source is a folder on your computer (or a directly attached external drive) that Archivial backs up and automatically monitors for new and updated files.</para>
    ///   <para type="description">The output from this command can be piped to the Remove-ArchivialLocalSource cmdlet.</para>
    /// </summary>
    /// <example>
    ///   <code>C:\> Get-ArchivialLocalSources</code>
    ///   <para>Returns all of the Local Source folders being monitored by Archivial.</para>
    ///   <para></para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "ArchivialLocalSources")]
    public class GetArchivialLocalSourcesCommand : BaseArchivialCmdlet
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public GetArchivialLocalSourcesCommand() : base() { }

        /// <summary>
        /// Secondary constructor for dependency injection.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="secretStore"></param>
        /// <param name="setup"></param>
        public GetArchivialLocalSourcesCommand(IClientDatabase database, ISecretStore secretStore, ISetup setup) : base(database, secretStore, setup) { }

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
