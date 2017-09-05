namespace OzetteLibrary.Shared
{
    /// <summary>
    /// Contains data and functionality for an internal network backup target.
    /// </summary>
    public class InternalNetworkBackupTarget : BackupTarget
    {
        /// <summary>
        /// The UNC path for the target backup location.
        /// </summary>
        public string UncPath { get; set; }
    }
}
