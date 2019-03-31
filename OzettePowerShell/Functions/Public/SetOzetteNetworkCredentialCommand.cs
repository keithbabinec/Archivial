using OzetteLibrary.Secrets;
using OzetteLibrary.ServiceCore;
using OzettePowerShell.Utility;
using System;
using System.Linq;
using System.Management.Automation;
using System.Security.Cryptography;

namespace OzettePowerShell.Functions.Public
{
    [Cmdlet(VerbsCommon.Set, "OzetteNetworkCredential")]
    public class SetOzetteNetworkCredentialCommand : BaseOzetteCmdlet
    {
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string CredentialName { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string ShareUsername { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string SharePassword { get; set; }

        protected override void ProcessRecord()
        {
            var db = GetDatabaseConnection();

            WriteVerbose("Fetching current network credentials from the database.");

            var existingCredsList = db.GetNetCredentialsAsync().GetAwaiter().GetResult();

            if (existingCredsList.Any(x => x.CredentialName == CredentialName) == false)
            {
                WriteVerbose("The specified credential is not configured in the client database. Adding it now.");

                var newcreds = new NetCredential() { CredentialName = CredentialName };

                db.AddNetCredentialAsync(newcreds).GetAwaiter().GetResult();

                WriteVerbose("Successfully added the network credential.");
            }
            else
            {
                WriteVerbose("The specified network credential is already in the client database. No action required.");
            }

            WriteVerbose("Initializing protected data store.");

            var scope = DataProtectionScope.LocalMachine;
            var ivkey = Convert.FromBase64String(CoreSettings.ProtectionIv);
            var pds = new ProtectedDataStore(db, scope, ivkey);

            WriteVerbose("Saving encrypted username setting.");

            pds.SetApplicationSecretAsync(string.Format(OzetteLibrary.Constants.Formats.NetCredentialUserNameKeyLookup, CredentialName), ShareUsername).GetAwaiter().GetResult();

            WriteVerbose("Saving encrypted password setting.");

            pds.SetApplicationSecretAsync(string.Format(OzetteLibrary.Constants.Formats.NetCredentialUserPasswordKeyLookup, CredentialName), SharePassword).ConfigureAwait(false);

            WriteVerbose("Successfully added or updated the network credentials.");
        }
    }
}
