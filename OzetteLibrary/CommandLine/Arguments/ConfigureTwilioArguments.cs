namespace OzetteLibrary.CommandLine.Arguments
{
    /// <summary>
    /// A set of Ozette Twilio configuration arguments.
    /// </summary>
    public class ConfigureTwilioArguments : ArgumentBase
    {
        /// <summary>
        /// The Twilio account ID.
        /// </summary>
        public string TwilioAccountID { get; set; }

        /// <summary>
        /// The Twilio account authentication/API token.
        /// </summary>
        public string TwilioAuthToken { get; set; }

        /// <summary>
        /// The Twilio source phone number.
        /// </summary>
        public string TwilioSourcePhone { get; set; }

        /// <summary>
        /// The Twilio destination phone number(s).
        /// </summary>
        /// <remarks>
        /// If multiple numbers, separate by semicolon.
        /// </remarks>
        public string TwilioDestinationPhones { get; set; }
    }
}
