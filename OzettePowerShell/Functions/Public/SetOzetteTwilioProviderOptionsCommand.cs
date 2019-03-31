using OzetteLibrary.MessagingProviders;
using OzetteLibrary.Providers;
using OzetteLibrary.Secrets;
using OzetteLibrary.ServiceCore;
using OzettePowerShell.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace OzettePowerShell.Functions.Public
{
    [Cmdlet(VerbsCommon.Set, "OzetteTwilioProviderOptions")]
    public class SetOzetteTwilioProviderOptionsCommand : BaseOzetteCmdlet
    {
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string TwilioAccountID { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string TwilioAuthToken { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string TwilioSourcePhone { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string TwilioDestinationPhones { get; set; }

        protected override void ProcessRecord()
        {
            var db = GetDatabaseConnection();

            WriteVerbose("Fetching current providers configuration from the database.");

            var existingProviders = db.GetProvidersAsync(ProviderTypes.Messaging).GetAwaiter().GetResult();

            if (existingProviders.Any(x => x.Name == nameof(MessagingProviderTypes.Twilio)) == false)
            {
                WriteVerbose("Twilio is not configured as a provider in the client database. Adding it now.");

                var newProvider =
                    new Provider()
                    {
                        Type = ProviderTypes.Messaging,
                        Name = nameof(MessagingProviderTypes.Twilio)
                    };

                db.AddProviderAsync(newProvider).GetAwaiter().GetResult();

                WriteVerbose("Successfully configured Twilio as a messaging provider.");
            }
            else
            {
                WriteVerbose("Twilio is already configured as a messaging provider in the client database. No action required.");
            }

            WriteVerbose("Initializing protected data store.");

            var scope = System.Security.Cryptography.DataProtectionScope.LocalMachine;
            var ivkey = Convert.FromBase64String(CoreSettings.ProtectionIv);
            var pds = new ProtectedDataStore(db, scope, ivkey);

            WriteVerbose("Saving encrypted Twilio configuration setting: TwilioAccountID.");
            pds.SetApplicationSecretAsync(OzetteLibrary.Constants.RuntimeSettingNames.TwilioAccountID, TwilioAccountID).GetAwaiter().GetResult();

            WriteVerbose("Saving encrypted Twilio configuration setting: TwilioAuthToken.");
            pds.SetApplicationSecretAsync(OzetteLibrary.Constants.RuntimeSettingNames.TwilioAuthToken, TwilioAuthToken).GetAwaiter().GetResult();

            WriteVerbose("Saving encrypted Twilio configuration setting: TwilioSourcePhone.");
            pds.SetApplicationSecretAsync(OzetteLibrary.Constants.RuntimeSettingNames.TwilioSourcePhone, TwilioSourcePhone).GetAwaiter().GetResult();

            WriteVerbose("Saving encrypted Twilio configuration setting: TwilioDestinationPhones.");
            pds.SetApplicationSecretAsync(OzetteLibrary.Constants.RuntimeSettingNames.TwilioDestinationPhones, TwilioDestinationPhones).GetAwaiter().GetResult();
        }
    }
}
