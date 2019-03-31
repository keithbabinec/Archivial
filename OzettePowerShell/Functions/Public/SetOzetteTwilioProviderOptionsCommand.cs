using OzetteLibrary.MessagingProviders;
using OzetteLibrary.Providers;
using OzetteLibrary.Secrets;
using OzetteLibrary.ServiceCore;
using OzettePowerShell.Utility;
using System;
using System.Linq;
using System.Management.Automation;

namespace OzettePowerShell.Functions.Public
{
    /// <summary>
    ///   <para type="synopsis">Configures the Twilio messaging provider as a status update recipient.</para>
    ///   <para type="description">Messaging providers are an optional way to be automatically notified of your backup status/progress. This command configures the Twilio (SMS/Text) provider for that purpose.</para>
    ///   <para type="description">This command requires that you have already setup a Twilio account, phone number, and have the required access token details ready. Twilio expects phone numbers to be provided in the E.164 format. If providing multiple destination phone numbers, they can be seperated by a semicolon.</para>
    ///   <para type="description">If your access token has changed, you can safely re-run this command with the new token, and then restart the Ozette Cloud Backup service for the changes to take effect.</para>
    ///   <para type="description">If you would like to disable this provider, please run the Remove-OzetteProvider cmdlet.</para>
    ///   <para type="description">All provided options here (ex: account name, token, phone numbers) are encrypted before saving to the database.</para>
    /// </summary>
    /// <example>
    ///   <code>C:\> Set-OzetteTwilioProviderOptions -TwilioAccountID "myaccount" -TwilioAuthToken "--token--" -TwilioSourcePhone "+12065551234" -TwilioDestinationPhones "+12065554567;+12065556789"</code>
    ///   <para>Configures Twilio as a status messaging recipient.</para>
    ///   <para></para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "OzetteTwilioProviderOptions")]
    public class SetOzetteTwilioProviderOptionsCommand : BaseOzetteCmdlet
    {
        /// <summary>
        ///   <para type="description">Specify the Twilio Account ID.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string TwilioAccountID { get; set; }

        /// <summary>
        ///   <para type="description">Specify the Twilio Authentication token.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string TwilioAuthToken { get; set; }

        /// <summary>
        ///   <para type="description">Specify the Twilio phone number (sender).</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string TwilioSourcePhone { get; set; }

        /// <summary>
        ///   <para type="description">Specify the phone number(s) to send updates to. If multiple, seperate by semicolon.</para>
        /// </summary>
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
