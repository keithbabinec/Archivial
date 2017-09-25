using System;

namespace OzetteLibrary.Models
{
    /// <summary>
    /// Describes a single file to be backed up.
    /// </summary>
    /// <remarks>
    /// Contains extra properties that only apply for files sitting in the target side.
    /// </remarks>
    public class TargetFile : BackupFile
    {
        /// <summary>
        /// The ID of the source location this file came from.
        /// </summary>
        public Guid SourceID { get; set; }

        /// <summary>
        /// The full file path for this file in the source.
        /// </summary>
        public string FullSourcePath { get; set; }
    }
}
