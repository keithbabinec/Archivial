using OzetteLibrary.Database;
using OzetteLibrary.Folders;
using OzettePowerShell.Utility;
using System.Linq;
using System.Management.Automation;

namespace OzettePowerShell.Functions.Public
{
    /// <summary>
    ///   <para type="synopsis">Removes the specified Local Source.</para>
    ///   <para type="description">A Local Source is a folder on your computer (or a directly attached external drive) that Ozette backs up and automatically monitors for new and updated files.</para>
    ///	  <para type="description">Removing a Local Source means that new or updated files from that location will not be backed up anymore, but existing files already backed up to cloud storage will remain.</para>
    ///   <para type="description">To view existing Local Sources, run Get-OzetteLocalSource. This command supports piping from Get-OzetteLocalSource or manual invoke from the specified source ID.</para>
    /// </summary>
    /// <example>
    ///   <code>C:\> Remove-OzetteLocalSource -ID 3</code>
    ///   <para>Removes the Local Source with the specified ID.</para>
    ///   <para></para>
    /// </example>
    /// <example>
    ///   <code>C:\> Get-OzetteLocalSource | Where Path -like "*C:\users\test\documents*" | Remove-OzetteLocalSource</code>
    ///   <para>Removes any configured Local Source that matches a path containing the specified filter (using the pipeline scenario).</para>
    ///   <para></para>
    /// </example>
    [Cmdlet(VerbsCommon.Remove, "OzetteLocalSource")]
    public class RemoveOzetteLocalSourceCommand : BaseOzetteCmdlet
    {
        /// <summary>
        ///   <para type="description">Specify the ID of the Local Source to remove.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "ByName")]
        public int SourceID { get; set; }

        /// <summary>
        ///   <para type="description">Specify the object (from pipeline) to remove.</para>
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

            WriteVerbose("Removing the local source location now.");

            database.RemoveSourceLocationAsync(LocalSource).GetAwaiter().GetResult();

            WriteVerbose("Successfully removed the local source location from the database.");
        }
    }
}
