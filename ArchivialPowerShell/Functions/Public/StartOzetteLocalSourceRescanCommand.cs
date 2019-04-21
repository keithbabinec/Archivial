using ArchivialLibrary.Database;
using ArchivialLibrary.Folders;
using ArchivialPowerShell.Utility;
using System.Linq;
using System.Management.Automation;

namespace ArchivialPowerShell.Functions.Public
{
    /// <summary>
    ///   <para type="synopsis">Forces the re-scan of a Local Source being monitored by Ozette.</para>
    ///   <para type="description">All sources are monitored for new or updated files on a regular schedule. This cmdlet is used to request an immediate rescan, outside of its regular schedule. The rescan will start as soon as there is scanning engine availability.</para>
    ///   <para type="description">The automated scanning schedule for Low priority sources is once every 48 hours. Medium priority sources are scanned every 12 hours. High priority sources are scanned every hour.</para>
    ///   <para type="description">Please see the Get-OzetteLocalSources command to find the ID of an existing source you would like to rescan.</para>
    /// </summary>
    /// <example>
    ///   <code>C:\> Start-OzetteLocalSourceRescan -SourceID 2</code>
    ///   <para>Forces a rescan of the Local Source with the specified ID.</para>
    ///   <para></para>
    /// </example>
    /// <example>
    ///   <code>C:\> Get-OzetteLocalSources | Start-OzetteLocalSourceRescan</code>
    ///   <para>Forces a rescan of all defined Local Sources being monitored by Ozette.</para>
    ///   <para></para>
    /// </example>
    /// <example>
    ///   <code>C:\> Get-OzetteLocalSources | Where Path -like "*D:\temp*" | Start-OzetteLocalSourceRescan</code>
    ///   <para>Forces a rescan of any Local Sources that match the path filter.</para>
    ///   <para></para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Start, "OzetteLocalSourceRescan")]
    public class StartOzetteLocalSourceRescanCommand : BaseOzetteCmdlet
    {
        /// <summary>
        ///   <para type="description">Specify the ID of the Local Source to rescan.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "ByName")]
        public int SourceID { get; set; }

        /// <summary>
        ///   <para type="description">Specify the Local Source object to rescan.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "ByObject", ValueFromPipeline = true)]
        [ValidateNotNull]
        public LocalSourceLocation LocalSource { get; set; }

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
