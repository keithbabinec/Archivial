using OzetteLibrary.Files;

namespace OzetteLibrary.CommandLine.Arguments
{
    /// <summary>
    /// A set of Ozette Azure configuration arguments.
    /// </summary>
    public class AddNetSourceArguments : ArgumentBase
    {
        /// <summary>
        /// The folder path to backup.
        /// </summary>
        public string UncPath { get; set; }

        /// <summary>
        /// The name of the credential lookup key.
        /// </summary>
        public string CredentialName { get; set; }

        /// <summary>
        /// The Azure cloud storage account access token.
        /// </summary>
        public FileBackupPriority Priority { get; set; }

        /// <summary>
        /// The Azure cloud storage account access token.
        /// </summary>
        public int Revisions { get; set; }

        /// <summary>
        /// The Azure cloud storage account access token.
        /// </summary>
        public string Matchfilter { get; set; }
    }
}
