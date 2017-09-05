namespace OzetteLibrary.Shared
{
    /// <summary>
    /// Contains data and functionality for an external network backup target.
    /// </summary>
    public class ExternalNetworkBackupTarget : BackupTarget
    {
        /// <summary>
        /// The URL to connect to the target on.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The port to connect to the target on.
        /// </summary>
        public int Port { get; set; }
    }
}
