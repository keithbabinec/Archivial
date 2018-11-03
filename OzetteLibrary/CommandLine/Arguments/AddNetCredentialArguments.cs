namespace OzetteLibrary.CommandLine.Arguments
{
    /// <summary>
    /// A set of Ozette Azure credential configuration arguments.
    /// </summary>
    public class AddNetCredentialArguments : ArgumentBase
    {
        /// <summary>
        /// The friendly name of the credentials.
        /// </summary>
        public string CredentialName { get; set; }

        /// <summary>
        /// The network credential username.
        /// </summary>
        public string ShareUser { get; set; }

        /// <summary>
        /// The network credential password.
        /// </summary>
        public string SharePassword { get; set; }
    }
}
