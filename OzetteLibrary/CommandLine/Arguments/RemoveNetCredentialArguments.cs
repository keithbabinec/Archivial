namespace OzetteLibrary.CommandLine.Arguments
{
    /// <summary>
    /// A set of Ozette credential removal arguments.
    /// </summary>
    public class RemoveNetCredentialArguments : ArgumentBase
    {
        /// <summary>
        /// The name of the credential to remove.
        /// </summary>
        public string CredentialName { get; set; }
    }
}
