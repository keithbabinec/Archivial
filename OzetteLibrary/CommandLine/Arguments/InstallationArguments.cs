namespace OzetteLibrary.CommandLine.Arguments
{
    /// <summary>
    /// A set of Ozette Installation arguments.
    /// </summary>
    public class InstallationArguments : ArgumentBase
    {
        /// <summary>
        /// The product installation directory
        /// </summary>
        public string InstallDirectory { get; set; }
    }
}
