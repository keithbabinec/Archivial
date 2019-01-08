namespace OzetteLibrary.MessagingProviders
{
    /// <summary>
    /// Describes the different possible messaging providers.
    /// </summary>
    public enum MessagingProviderTypes
    {
        /// <summary>
        /// Twilio (SMS/Text) messaging provider.
        /// </summary>
        Twilio = 10,

        /// <summary>
        /// SendGrid (Email) messaging provider.
        /// </summary>
        SendGrid = 11
    }
}
