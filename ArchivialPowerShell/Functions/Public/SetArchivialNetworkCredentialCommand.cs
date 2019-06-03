using ArchivialLibrary.Secrets;
using ArchivialPowerShell.Utility;
using System.Linq;
using System.Management.Automation;

namespace ArchivialPowerShell.Functions.Public
{
    /// <summary>
    ///   <para type="synopsis">Saves the credentials required to connect to an authenticated network resource (such as a UNC path share).</para>
    ///   <para type="description">Network Sources can be authenticated (require username/password), or unauthenticated (open access). If this Network Source requires authenticated access, you must provide use this command to pre-store the authentication details so the backup engine can connect to the resource.</para>
    ///   <para type="description">The credential username and password are both encrypted and saved to the database.</para>
    /// </summary>
    /// <example>
    ///   <code>C:\> Set-ArchivialNetworkCredential -CredentialName "drobo-device" -ShareUser "drobo-private-user" -SharePassword ****</code>
    ///   <para>Encrypts and stores the network resource credentials in the database.</para>
    ///   <para></para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ArchivialNetworkCredential")]
    public class SetArchivialNetworkCredentialCommand : BaseArchivialCmdlet
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

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SetArchivialNetworkCredentialCommand() : base() { }

        /// <summary>
        /// Secondary constructor for dependency injection.
        /// </summary>
        /// <param name="dependencies"></param>
        public SetArchivialNetworkCredentialCommand(CmdletDependencies dependencies) : base(dependencies) { }

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

            var pds = GetSecretStore();

            WriteVerbose("Saving encrypted username setting.");

            pds.SetApplicationSecretAsync(string.Format(ArchivialLibrary.Constants.Formats.NetCredentialUserNameKeyLookup, CredentialName), ShareUsername).GetAwaiter().GetResult();

            WriteVerbose("Saving encrypted password setting.");

            pds.SetApplicationSecretAsync(string.Format(ArchivialLibrary.Constants.Formats.NetCredentialUserPasswordKeyLookup, CredentialName), SharePassword).ConfigureAwait(false);

            WriteVerbose("Successfully added or updated the network credentials.");
        }
    }
}
