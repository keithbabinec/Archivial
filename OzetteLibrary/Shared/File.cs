using System;
using System.Collections.Generic;

namespace OzetteLibrary.Shared
{
    /// <summary>
    /// Describes a single file to be backed up.
    /// </summary>
    public class File
    {
        /// <summary>
        /// The name of the file including extension.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// The full filepath location on the local client source.
        /// </summary>
        public string SourcePath { get; set; }

        /// <summary>
        /// The hash of the file. 
        /// </summary>
        public byte[] FileHash { get; set; }

        /// <summary>
        /// The hash of the file stored as a string.
        /// </summary>
        public string FileHashString { get; set; }

        /// <summary>
        /// The files size measured in bytes.
        /// </summary>
        public ulong FileSizeBytes { get; set; }

        /// <summary>
        /// The last time this file was scanned in the backup source.
        /// </summary>
        public DateTime? LastChecked { get; set; }

        /// <summary>
        /// Maintains the status of the backup against one or more targets.
        /// </summary>
        public List<TargetFileState> TargetStates { get; set; }
    }
}
