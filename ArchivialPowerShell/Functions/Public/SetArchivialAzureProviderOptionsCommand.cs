using ArchivialLibrary.Providers;
using ArchivialLibrary.StorageProviders;
using ArchivialPowerShell.Utility;
using System.Linq;
using System.Management.Automation;

namespace ArchivialPowerShell.Functions.Public
{
    /// <summary>
    ///   <para type="synopsis">Configures the Azure cloud storage provider as a backup destination.</para>
    ///   <para type="description">In order to backup files to the cloud, at least one cloud storage provider must be configured. This command will enable the Azure cloud storage provider for this purpose.</para>
    ///   <para type="description">This command assumes that you have already deployed an Azure storage account and have the access token ready.</para>
    ///   <para type="description">If your access token has changed, you can safely re-run this command with the new token, and then restart the Archivial Cloud Backup service for the changes to take effect.</para>
    ///   <para type="description">If you would like to disable this provider, please run the Remove-ArchivialProvider cmdlet.</para>
    ///   <para type="description">All provided options here (account name and token) are encrypted before saving to the database.</para>
    /// </summary>
    /// <example>
    ///   <code>C:\> Set-ArchivialAzureProviderOptions -AzureStorageAccountName "myaccount" -AzureStorageAccountToken "--my token--"</code>
    ///   <para>Configures Azure as a cloud storage backup destination.</para>
    ///   <para></para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ArchivialAzureProviderOptions")]
    public class SetArchivialAzureProviderOptionsCommand : BaseArchivialCmdlet
    {
        /// <summary>
        ///   <para type="description">Specify the name of the Azure storage account to upload backup data to.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string AzureStorageAccountName { get; set; }

        /// <summary>
        ///   <para type="description">Specify the access token of the Azure storage account.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string AzureStorageAccountToken { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SetArchivialAzureProviderOptionsCommand() : base() { }

        /// <summary>
        /// Secondary constructor for dependency injection.
        /// </summary>
        /// <param name="dependencies"></param>
        public SetArchivialAzureProviderOptionsCommand(CmdletDependencies dependencies) : base(dependencies) { }

        protected override void ProcessRecord()
        {
            var db = GetDatabaseConnection();

            WriteVerbose("Fetching current providers configuration from the database.");

            var existingProviders = db.GetProvidersAsync(ProviderTypes.Storage).GetAwaiter().GetResult();

            if (existingProviders.Any(x => x.Name == nameof(StorageProviderTypes.Azure)) == false)
            {
                WriteVerbose("Azure is not configured as a provider in the client database. Adding it now.");

                var newProvider =
                    new Provider()
                    {
                        Type = ProviderTypes.Storage,
                        Name = nameof(StorageProviderTypes.Azure)
                    };

                db.AddProviderAsync(newProvider).GetAwaiter().GetResult();

                WriteVerbose("Successfully configured Azure as a cloud backup provider.");
            }
            else
            {
                WriteVerbose("Azure is already configured as a provider in the client database. No action required.");
            }

            var pds = GetSecretStore();

            WriteVerbose("Saving encrypted Azure configuration setting: AzureStorageAccountName.");

            pds.SetApplicationSecretAsync(ArchivialLibrary.Constants.RuntimeSettingNames.AzureStorageAccountName, AzureStorageAccountName).GetAwaiter().GetResult();

            WriteVerbose("Saving encrypted Azure configuration setting: AzureStorageAccountToken.");

            pds.SetApplicationSecretAsync(ArchivialLibrary.Constants.RuntimeSettingNames.AzureStorageAccountToken, AzureStorageAccountToken).GetAwaiter().GetResult();
        }
    }
}
