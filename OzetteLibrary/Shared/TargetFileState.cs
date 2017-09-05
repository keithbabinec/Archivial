using System;

namespace OzetteLibrary.Shared
{
    /// <summary>
    /// Contains a target file state information.
    /// </summary>
    public class TargetFileState
    {
        /// <summary>
        /// The ID of the backup target.
        /// </summary>
        public Guid TargetID { get; set; }

        /// <summary>
        /// The file transfer status to this location.
        /// </summary>
        public FileBackupStatus FileStatus { get; set; }

        /// <summary>
        /// The full filepath location on the remote target.
        /// </summary>
        public string RemoteFilepath { get; set; }
    }
}
