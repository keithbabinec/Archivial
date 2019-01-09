using System;
using System.Threading.Tasks;
using OzetteLibrary.Logging;
using OzetteLibrary.ServiceCore;

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

            Logger = logger;
        }

        /// <summary>
        /// Sends the backup progress status to the messaging provider for delivery.
        /// </summary>
        /// <param name="Progress">An initialized <c>BackupProgress</c> object.</param>
        public async Task SendBackupProgressStatusMessageAsync(BackupProgress Progress)
        {
            throw new NotImplementedException();
        }
    }
}
