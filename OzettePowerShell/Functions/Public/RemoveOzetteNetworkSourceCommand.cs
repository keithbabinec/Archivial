using OzetteLibrary.Database;
using OzetteLibrary.Folders;
using OzettePowerShell.Utility;
using System.Linq;
using System.Management.Automation;

namespace OzettePowerShell.Functions.Public
{
    [Cmdlet(VerbsCommon.Remove, "OzetteNetworkSource")]
    public class RemoveOzetteNetworkSourceCommand : BaseOzetteCmdlet
    {
        [Parameter(Mandatory = true, ParameterSetName = "ByName")]
        public int SourceID { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "ByObject", ValueFromPipeline = true)]
        [ValidateNotNull]
        public NetworkSourceLocation NetworkSource { get; set; }

        private IClientDatabase database { get; set; }

        protected override void BeginProcessing()
        {
            database = GetDatabaseConnection();
        }

        protected override void ProcessRecord()
        {
            if (NetworkSource == null)
            {
                WriteVerbose("Querying for existing scan sources to see if the specified source exists.");

                var allSources = database.GetSourceLocationsAsync().GetAwaiter().GetResult();

                var sourceMatch = allSources.FirstOrDefault(x => x.ID == SourceID && x is NetworkSourceLocation);

                if (sourceMatch == null)
                {
                    throw new ItemNotFoundException("There was no local source location found with the specified ID: " + SourceID);
                }
                else
                {
                    NetworkSource = (NetworkSourceLocation)sourceMatch;
                }
            }

            WriteVerbose("Removing the network source location now.");

            database.RemoveSourceLocationAsync(NetworkSource).GetAwaiter().GetResult();

            WriteVerbose("Successfully removed the network source location from the database.");
        }
    }
}
