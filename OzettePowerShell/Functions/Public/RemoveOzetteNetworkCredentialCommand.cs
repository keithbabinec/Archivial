using OzetteLibrary.Database;
using OzetteLibrary.Secrets;
using OzettePowerShell.Utility;
using System.Linq;
using System.Management.Automation;

namespace OzettePowerShell.Functions.Public
{
    [Cmdlet(VerbsCommon.Remove, "OzetteNetworkCredential")]
    public class RemoveOzetteNetworkCredentialCommand : BaseOzetteCmdlet
    {
        [Parameter(Mandatory = true, ParameterSetName = "ByName")]
        [ValidateNotNullOrEmpty]
        public string CredentialName { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "ByObject", ValueFromPipeline = true)]
        [ValidateNotNull]
        public NetCredential NetCredential { get; set; }

        private IClientDatabase database { get; set; } 

        protected override void BeginProcessing()
        {
            database = GetDatabaseConnection();
        }

        protected override void ProcessRecord()
        {
            var credName = NetCredential != null ? NetCredential.CredentialName : CredentialName;

            WriteVerbose("Querying for existing network credential to verify that it exists.");

            var allCredentialsList = database.GetNetCredentialsAsync().GetAwaiter().GetResult();
            var credToRemove = allCredentialsList.FirstOrDefault(x => x.CredentialName == credName);

            if (credToRemove == null)
            {
                throw new ItemNotFoundException("No network credential was found with the specified name.");
            }

            WriteVerbose("Found a matching network credential, removing it now.");

            database.RemoveNetCredentialAsync(credName).ConfigureAwait(false);

            // remove provider specific secrets
            database.RemoveApplicationOptionAsync(string.Format(OzetteLibrary.Constants.Formats.NetCredentialUserNameKeyLookup, credName)).ConfigureAwait(false);
            database.RemoveApplicationOptionAsync(string.Format(OzetteLibrary.Constants.Formats.NetCredentialUserPasswordKeyLookup, credName)).ConfigureAwait(false);

            WriteVerbose("Successfully removed the credential from the database.");
        }
    }
}
