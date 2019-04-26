using ArchivialLibrary.Database;
using ArchivialPowerShell.Utility;
using System.Management.Automation;

namespace ArchivialPowerShell.Functions.Public
{
    /// <summary>
    ///   <para type="synopsis">Returns all of the saved Network Credentials used to connect to Network Sources.</para>
    ///   <para type="description">Some Network Sources (UNC Paths) being monitored for backup will require authentication (username and password). This command will return the list of named credentials that have been saved.</para>
    ///   <para type="description">Note: Only the name of the credential will be returned. The encrypted username and password values will not be returned in the output.</para>
    ///   <para type="description">The output from this command can be piped to the Remove-ArchivialNetworkCredential cmdlet.</para>
    /// </summary>
    /// <example>
    ///   <code>C:\> Get-ArchivialNetworkCredentials</code>
    ///   <para>Returns all of the configured Network Credentials saved in the system.</para>
    ///   <para></para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "ArchivialNetworkCredentials")]
    public class GetArchivialNetworkCredentialsCommand : BaseArchivialCmdlet
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public GetArchivialNetworkCredentialsCommand() : base() { }

        /// <summary>
        /// A secondary constructor for dependency injection.
        /// </summary>
        /// <param name="database"></param>
        public GetArchivialNetworkCredentialsCommand(IClientDatabase database) : base(database) { }

        protected override void ProcessRecord()
        {
            var db = GetDatabaseConnection();

            WriteVerbose("Querying for network credentials.");

            var allNetCreds = db.GetNetCredentialsAsync().GetAwaiter().GetResult();

            WriteVerbose(string.Format("Writing output results to pipeline (Objects: {0})", allNetCreds.Count));

            foreach (var netCred in allNetCreds)
            {
                WriteObject(netCred);
            }
        }
    }
}
