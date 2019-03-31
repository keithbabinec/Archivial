using OzetteLibrary.Database;
using OzetteLibrary.MessagingProviders;
using OzetteLibrary.Providers;
using OzetteLibrary.StorageProviders;
using OzettePowerShell.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace OzettePowerShell.Functions.Public
{
    [Cmdlet(VerbsCommon.Remove, "OzetteProvider")]
    public class RemoveOzetteProviderCommand : BaseOzetteCmdlet
    {
        [Parameter(Mandatory = true, ParameterSetName = "ByName")]
        [ValidateNotNullOrEmpty]
        public string ProviderName { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "ByObject", ValueFromPipeline = true)]
        [ValidateNotNull]
        public Provider Provider { get; set; }

        private IClientDatabase database { get; set; }

        protected override void BeginProcessing()
        {
            database = GetDatabaseConnection();
        }

        protected override void ProcessRecord()
        {
            var providerName = Provider != null ? Provider.Name : ProviderName;

            WriteVerbose("Querying for existing provider to verify that it exists.");

            var allProviders = database.GetProvidersAsync(ProviderTypes.Any).GetAwaiter().GetResult();
            var providerToRemove = allProviders.FirstOrDefault(x => x.Name == providerName);

            if (providerToRemove == null)
            {
                throw new ItemNotFoundException("No provider was found with the specified name.");
            }

            WriteVerbose("Found a matching provider, removing it now.");
            database.RemoveProviderAsync(providerToRemove.Name).GetAwaiter().GetResult();

            var settingsToRemove = new List<string>();

            if (providerToRemove.Name == StorageProviderTypes.Azure.ToString())
            {
                // remove provider specific secrets
                settingsToRemove.Add(OzetteLibrary.Constants.RuntimeSettingNames.AzureStorageAccountName);
                settingsToRemove.Add(OzetteLibrary.Constants.RuntimeSettingNames.AzureStorageAccountToken);
            }
            else if (providerToRemove.Name == MessagingProviderTypes.Twilio.ToString())
            {
                // remove provider specific secrets
                settingsToRemove.Add(OzetteLibrary.Constants.RuntimeSettingNames.TwilioAccountID);
                settingsToRemove.Add(OzetteLibrary.Constants.RuntimeSettingNames.TwilioAuthToken);
                settingsToRemove.Add(OzetteLibrary.Constants.RuntimeSettingNames.TwilioDestinationPhones);
                settingsToRemove.Add(OzetteLibrary.Constants.RuntimeSettingNames.TwilioSourcePhone);
            }
            else
            {
                throw new NotImplementedException("Unexpected provider type: " + providerToRemove.Name);
            }

            foreach (var setting in settingsToRemove)
            {
                database.RemoveApplicationOptionAsync(setting).GetAwaiter().GetResult();
            }

            WriteVerbose("Successfully removed the cloud provider from the database.");
        }
    }
}
