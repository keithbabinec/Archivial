using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OzetteLibrary.Logging;
using OzetteLibrary.ServiceCore;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace OzetteLibrary.MessagingProviders.Twilio
{
    /// <summary>
    /// Implements the required operations for the Twilio messaging provider.
    /// </summary>
    public class TwilioMessagingProviderOperations : IMessagingProviderOperations
    {
        /// <summary>
        /// A reference to the logging utility.
        /// </summary>
        private ILogger Logger;

        /// <summary>
        /// The source phone number for SMS delivery.
        /// </summary>
        private string SourcePhone;

        /// <summary>
        /// The destination phone number(s) for SMS delivery.
        /// </summary>
        private List<string> DestinationPhones;

        /// <summary>
        /// The SMS message format.
        /// </summary>
        private string MessageFormat;

        /// <summary>
        /// Constructor that accepts Twilio API connectivity and delivery information.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="twilioAccountID"></param>
        /// <param name="twilioAuthToken"></param>
        /// <param name="twilioSourcePhone"></param>
        /// <param name="destinationPhones"></param>
        public TwilioMessagingProviderOperations(ILogger logger, string twilioAccountID, string twilioAuthToken, string twilioSourcePhone, string destinationPhones)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            if (string.IsNullOrWhiteSpace(twilioAccountID))
            {
                throw new ArgumentException(nameof(twilioAccountID));
            }
            if (string.IsNullOrWhiteSpace(twilioAuthToken))
            {
                throw new ArgumentException(nameof(twilioAuthToken));
            }
            if (string.IsNullOrWhiteSpace(twilioSourcePhone))
            {
                throw new ArgumentException(nameof(twilioSourcePhone));
            }
            if (string.IsNullOrWhiteSpace(destinationPhones))
            {
                throw new ArgumentException(nameof(destinationPhones));
            }

            Logger = logger;

            // initialize the twilio client.
            TwilioClient.Init(twilioAccountID, twilioAuthToken);

            // store phones
            SourcePhone = twilioSourcePhone.Trim();
            DestinationPhones = new List<string>();

            foreach (var phone in destinationPhones.Split(';'))
            {
                if (!string.IsNullOrWhiteSpace(phone))
                {
                    DestinationPhones.Add(phone.Trim());
                }
            }

            // construct the message format.
            // since the message is multiple lines its just a bit easier read here.

            var format = new StringBuilder();
            format.AppendLine("Ozette Backup Status {0} {1}"); // <MachineName> <Overall Percent Completed>
            format.AppendLine("Completed {2} / {3}"); // <Files Completed> <File Size Completed>
            format.AppendLine("Remaining {4} / {5}"); // <Files Remaining> <File Size Remaining>

            MessageFormat = format.ToString();
        }

        /// <summary>
        /// Sends the backup progress status to the messaging provider for delivery.
        /// </summary>
        /// <param name="Progress">An initialized <c>BackupProgress</c> object.</param>
        public async Task SendBackupProgressStatusMessageAsync(BackupProgress Progress)
        {
            var finalMessage = string.Format(MessageFormat,
                Environment.MachineName.ToUpper(), Progress.OverallPercentage,
                Progress.BackedUpFileCount, Progress.BackedUpFileSize,
                Progress.RemainingFileCount, Progress.RemainingFileCount);

            Logger.WriteTraceMessage("Sending backup status update messages through Twilio.");

            foreach (var destPhone in DestinationPhones)
            {
                try
                {
                    var message = await MessageResource.CreateAsync(
                        body: finalMessage,
                        from: new PhoneNumber(SourcePhone),
                        to: new PhoneNumber(destPhone)
                    );

                    Logger.WriteTraceMessage(string.Format("Twilio message {0} delivery attempt result: {1}", message.Sid, message.Status));
                }
                catch (ApiException ex)
                {
                    Logger.WriteTraceError("Failed to send an SMS message through Twilio.", ex, Logger.GenerateFullContextStackTrace());
                }
            }
        }
    }
}
