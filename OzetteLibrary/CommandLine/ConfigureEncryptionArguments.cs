namespace OzetteLibrary.CommandLine
{
    /// <summary>
    /// A set of Ozette encryption configuration arguments.
    /// </summary>
    public class ConfigureEncryptionArguments : Arguments
    {
        /// <summary>
        /// The encryption initialization vector.
        /// </summary>
        public string ProtectionIv { get; set; }

    }
}
