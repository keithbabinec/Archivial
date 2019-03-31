using OzetteLibrary.Database;
using OzetteLibrary.Folders;
using OzettePowerShell.Utility;
using System.Linq;
using System.Management.Automation;

namespace OzettePowerShell.Functions.Public
{
    [Cmdlet(VerbsLifecycle.Start, "OzetteLocalSourceRescan")]
    public class StartOzetteLocalSourceRescanCommand : BaseOzetteCmdlet
    {
        [Parameter(Mandatory = true, ParameterSetName = "ByName")]
        public int SourceID { get; set; }

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
