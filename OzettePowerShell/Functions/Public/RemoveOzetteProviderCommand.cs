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
    /// <summary>
    ///   <para type="synopsis">Removes the configured storage or messaging provider.</para>
    ///   <para type="description">Providers are used to connect to external services for cloud storage backup or message notifications.</para>
    ///	  <para type="description">Removing a cloud storage provider means that new or updated files will not be backed up to that provider anymore, but existing files stored at that provider will remain.</para>
    ///   <para type="description">To view existing configured providers, run Get-OzetteProviders. This command supports piping from Get-OzetteProviders or manual invoke from the provider name.</para>
    /// </summary>
    /// <example>
    ///   <code>C:\> Remove-OzetteProvider -ProviderName "Azure"</code>
    ///   <para>Removes the configured Azure provider.</para>
    ///   <para></para>
    /// </example>
    /// <example>
    ///   <code>C:\> Get-OzetteProvider | Where Name -eq "Azure" | Remove-OzetteProvider</code>
    ///   <para>Removes the configured Azure provider, but using the pipeline scenario.</para>
    ///   <para></para>
    /// </example>
    [Cmdlet(VerbsCommon.Remove, "OzetteProvider")]
    public class RemoveOzetteProviderCommand : BaseOzetteCmdlet
    {
        /// <summary>
        ///   <para type="description">Specify the name of the provider to remove.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "ByName")]
        [ValidateNotNullOrEmpty]
        public string ProviderName { get; set; }

        /// <summary>
        ///   <para type="description">Specify the object (from pipeline) to remove.</para>
        /// </summary>
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
