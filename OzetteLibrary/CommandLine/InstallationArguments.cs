namespace OzetteLibrary.CommandLine
{
    /// <summary>
    /// A set of Ozette Installation arguments.
    /// </summary>
    public class InstallationArguments : Arguments
    {
        /// <summary>
        /// The product installation directory
        /// </summary>
        public string InstallDirectory { get; set; }

        /// <summary>
        /// The database file path.
        /// </summary>
        /// <remarks>
        /// File extension should be *.db 
        /// </remarks>
        public string DatabasePath { get; set; }
    }
}
