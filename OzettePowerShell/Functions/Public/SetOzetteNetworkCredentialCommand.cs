using OzetteLibrary.Secrets;
using OzetteLibrary.ServiceCore;
using OzettePowerShell.Utility;
using System;
using System.Linq;
using System.Management.Automation;
using System.Security.Cryptography;

namespace OzettePowerShell.Functions.Public
{
    /// <summary>
    ///   <para type="synopsis">Saves the credentials required to connect to an authenticated network resource (such as a UNC path share).</para>
    ///   <para type="description">Network Sources can be authenticated (require username/password), or unauthenticated (open access). If this Network Source requires authenticated access, you must provide use this command to pre-store the authentication details so the backup engine can connect to the resource.</para>
    ///   <para type="description">The credential username and password are both encrypted and saved to the database.</para>
    /// </summary>
    /// <example>
    ///   <code>C:\> Set-OzetteNetworkCredential -CredentialName "drobo-device" -ShareUser "drobo-private-user" -SharePassword ****</code>
    ///   <para>Encrypts and stores the network resource credentials in the database.</para>
    ///   <para></para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "OzetteNetworkCredential")]
    public class SetOzetteNetworkCredentialCommand : BaseOzetteCmdlet
    {
        /// <summary>
        ///   <para type="description">Specify the friendly name (description) to refer to this stored credential.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string CredentialName { get; set; }

        /// <summary>
        ///   <para type="description">Specify the username required to connect to the network resource.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string ShareUsername { get; set; }

        /// <summary>
        ///   <para type="description">Specify the password required to connect to the network resource.</para>
        /// </summary>
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

            var settingName = OzetteLibrary.Constants.RuntimeSettingNames.ProtectionIV;
            var protectionIvEncodedString = db.GetApplicationOptionAsync(settingName).GetAwaiter().GetResult();
            var ivkey = Convert.FromBase64String(protectionIvEncodedString);

            var pds = new ProtectedDataStore(db, scope, ivkey);

            WriteVerbose("Saving encrypted username setting.");

            pds.SetApplicationSecretAsync(string.Format(OzetteLibrary.Constants.Formats.NetCredentialUserNameKeyLookup, CredentialName), ShareUsername).GetAwaiter().GetResult();

            WriteVerbose("Saving encrypted password setting.");

            pds.SetApplicationSecretAsync(string.Format(OzetteLibrary.Constants.Formats.NetCredentialUserPasswordKeyLookup, CredentialName), SharePassword).ConfigureAwait(false);

            WriteVerbose("Successfully added or updated the network credentials.");
        }
    }
}
