using OzettePowerShell.Utility;
using System.Management.Automation;

namespace OzettePowerShell.Functions.Public
{
    /// <summary>
    ///   <para type="synopsis">Returns all of the configured Ozette Providers.</para>
    ///   <para type="description">Returns all of the configured Ozette Providers. An Ozette Provider is an interface to an external service, for either cloud storage (ex: Azure, AWS) or messaging (ex: Sendgrid, Twilio).</para>
    ///   <para type="description">Note: Only the name and ID of the provider will be returned. The providers encrypted secure setting values are not returned in this cmdlet.</para>
    ///   <para type="description">The output from this command can be piped to the Remove-OzetteProvider cmdlet.</para>
    /// </summary>
    /// <example>
    ///   <code>C:\> Get-OzetteProviders</code>
    ///   <para>Returns all of the configured Ozette Providers.</para>
    ///   <para></para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "OzetteProviders")]
    public class GetOzetteProvidersCommand : BaseOzetteCmdlet
    {
        protected override void ProcessRecord()
        {
            var db = GetDatabaseConnection();

            WriteVerbose("Querying for configured providers.");

            var allProviders = db.GetProvidersAsync(OzetteLibrary.Providers.ProviderTypes.Any).GetAwaiter().GetResult();

            WriteVerbose(string.Format("Writing output results to pipeline (Objects: {0})", allProviders.Count));

            foreach (var provider in allProviders)
            {
                WriteObject(provider);
            }
        }
    }
}
