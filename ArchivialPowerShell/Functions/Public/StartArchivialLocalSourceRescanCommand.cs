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
    ///   <para type="synopsis">Forces the re-scan of a Local Source being monitored by Archivial.</para>
    ///   <para type="description">All sources are monitored for new or updated files on a regular schedule. This cmdlet is used to request an immediate rescan, outside of its regular schedule. The rescan will start as soon as there is scanning engine availability.</para>
    ///   <para type="description">The automated scanning schedule for Low priority sources is once every 48 hours. Medium priority sources are scanned every 12 hours. High priority sources are scanned every hour.</para>
    ///   <para type="description">Please see the Get-ArchivialLocalSources command to find the ID of an existing source you would like to rescan.</para>
    /// </summary>
    /// <example>
    ///   <code>C:\> Start-ArchivialLocalSourceRescan -SourceID 2</code>
    ///   <para>Forces a rescan of the Local Source with the specified ID.</para>
    ///   <para></para>
    /// </example>
    /// <example>
    ///   <code>C:\> Get-ArchivialLocalSources | Start-ArchivialLocalSourceRescan</code>
    ///   <para>Forces a rescan of all defined Local Sources being monitored by Archivial.</para>
    ///   <para></para>
    /// </example>
    /// <example>
    ///   <code>C:\> Get-ArchivialLocalSources | Where Path -like "*D:\temp*" | Start-ArchivialLocalSourceRescan</code>
    ///   <para>Forces a rescan of any Local Sources that match the path filter.</para>
    ///   <para></para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Start, "ArchivialLocalSourceRescan")]
    public class StartArchivialLocalSourceRescanCommand : BaseArchivialCmdlet
    {
        /// <summary>
        ///   <para type="description">Specify the ID of the Local Source to rescan.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "ByName")]
        [ValidateRange(0, int.MaxValue)]
        public int SourceID { get; set; }

        /// <summary>
        ///   <para type="description">Specify the Local Source object to rescan.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "ByObject", ValueFromPipeline = true)]
        [ValidateNotNull]
        public LocalSourceLocation LocalSource { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public StartArchivialLocalSourceRescanCommand() : base() { }

        /// <summary>
        /// Secondary constructor for dependency injection.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="secretStore"></param>
        /// <param name="setup"></param>
        public StartArchivialLocalSourceRescanCommand(IClientDatabase database, ISecretStore secretStore, ISetup setup) : base(database, secretStore, setup) { }

        private IClientDatabase database { get; set; }

        protected override void BeginProcessing()
        {
            database = GetDatabaseConnection();
        }

        protected override void ProcessRecord()
        {
            if (LocalSource == null)
            {
                WriteVerbose("Querying for existing scan sources to see if the specified source exists.");

                var allSources = database.GetSourceLocationsAsync().GetAwaiter().GetResult();

                var sourceMatch = allSources.FirstOrDefault(x => x.ID == SourceID && x is LocalSourceLocation);

                if (sourceMatch == null)
                {
                    throw new ItemNotFoundException("There was no local source location found with the specified ID: " + SourceID);
                }
                else
                {
                    LocalSource = (LocalSourceLocation)sourceMatch;
                }
            }

            WriteVerbose("Starting a forced rescan of the local source location now.");

            database.RescanSourceLocationAsync(LocalSource);

            WriteVerbose("Successfully queued the local source location for re-scan.");
        }
    }
}
