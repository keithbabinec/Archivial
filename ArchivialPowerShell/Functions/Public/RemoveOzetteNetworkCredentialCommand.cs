using ArchivialLibrary.Database;
using ArchivialLibrary.Secrets;
using ArchivialPowerShell.Utility;
using System.Linq;
using System.Management.Automation;

namespace ArchivialPowerShell.Functions.Public
{
    /// <summary>
    ///   <para type="synopsis">Removes the stored network credential used for connecting to network resources.</para>
    ///   <para type="description">Authenticated Network Source locations have an encrypted credential stored with them. This command is used to remove that stored credential.</para>
    ///   <para type="description">To view existing network credentials, run Get-OzetteNetworkCredentials. This command supports piping from Get-OzetteNetworkCredentials or manual invoke from credential name</para>
    /// </summary>
    /// <example>
    ///   <code>C:\> Remove-OzetteNetworkCredential -CredentialName "drobo-nas"</code>
    ///   <para>Removes the stored network credential with the specified name.</para>
    ///   <para></para>
    /// </example>
    /// <example>
    ///   <code>C:\> Get-OzetteNetworkCredentials | Where CredentialName -eq "drobo-nas" | Remove-OzetteNetworkCredential</code>
    ///   <para>Removes the stored network credential, but using the pipeline scenario.</para>
    ///   <para></para>
    /// </example>
    [Cmdlet(VerbsCommon.Remove, "OzetteNetworkCredential")]
    public class RemoveOzetteNetworkCredentialCommand : BaseOzetteCmdlet
    {
        /// <summary>
        ///   <para type="description">Specify the name of the credential to remove.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "ByName")]
        [ValidateNotNullOrEmpty]
        public string CredentialName { get; set; }

        /// <summary>
        ///   <para type="description">Specify the object (from pipeline) to remove.</para>
        /// </summary>
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
            database.RemoveApplicationOptionAsync(string.Format(ArchivialLibrary.Constants.Formats.NetCredentialUserNameKeyLookup, credName)).ConfigureAwait(false);
            database.RemoveApplicationOptionAsync(string.Format(ArchivialLibrary.Constants.Formats.NetCredentialUserPasswordKeyLookup, credName)).ConfigureAwait(false);

            WriteVerbose("Successfully removed the credential from the database.");
        }
    }
}
