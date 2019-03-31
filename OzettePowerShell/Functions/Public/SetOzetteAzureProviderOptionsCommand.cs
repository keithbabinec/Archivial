using OzetteLibrary.Providers;
using OzetteLibrary.Secrets;
using OzetteLibrary.ServiceCore;
using OzetteLibrary.StorageProviders;
using OzettePowerShell.Utility;
using System;
using System.Linq;
using System.Management.Automation;

namespace OzettePowerShell.Functions.Public
{
    [Cmdlet(VerbsCommon.Set, "OzetteAzureProviderOptions")]
    public class SetOzetteAzureProviderOptionsCommand : BaseOzetteCmdlet
    {
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string AzureStorageAccountName { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string AzureStorageAccountToken { get; set; }

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

            WriteVerbose("Initializing protected data store.");

            var scope = System.Security.Cryptography.DataProtectionScope.LocalMachine;
            var ivkey = Convert.FromBase64String(CoreSettings.ProtectionIv);
            var pds = new ProtectedDataStore(db, scope, ivkey);

            WriteVerbose("Saving encrypted Azure configuration setting: AzureStorageAccountName.");

            pds.SetApplicationSecretAsync(OzetteLibrary.Constants.RuntimeSettingNames.AzureStorageAccountName, AzureStorageAccountName).GetAwaiter().GetResult();

            WriteVerbose("Saving encrypted Azure configuration setting: AzureStorageAccountToken.");

            pds.SetApplicationSecretAsync(OzetteLibrary.Constants.RuntimeSettingNames.AzureStorageAccountToken, AzureStorageAccountToken).GetAwaiter().GetResult();
        }
    }
}
