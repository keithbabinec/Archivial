using System;

namespace OzetteLibrary.Models
{
    /// <summary>
    /// Describes a single file to be backed up.
    /// </summary>
    /// <remarks>
    /// Contains extra properties that only apply for files sitting in the client side.
    /// </remarks>
    public class ClientFile : BackupFile
    {
        /// <summary>
        /// The last time this file was scanned in the backup source.
        /// </summary>
        public DateTime? LastChecked { get; set; }
    }
}
