namespace OzetteLibrary.Secrets
{
    /// <summary>
    /// Describes a single network access credential.
    /// </summary>
    public class NetCredential
    {
        /// <summary>
        /// The internal database ID of this credential.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The credential friendly name.
        /// </summary>
        public string CredentialName { get; set; }
    }
}
