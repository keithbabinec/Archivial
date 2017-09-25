using System;
using System.Security.Cryptography;

namespace OzetteLibrary.Models
{
    /// <summary>
    /// Describes a single file to be backed up.
    /// </summary>
    /// <remarks>
    /// Contains common properties that apply to file when it sits in either the client and target sides.
    /// </remarks>
    public class BackupFile
    {
        /// <summary>
        /// A unique identifier for this file which should be shared among both client and targets.
        /// </summary>
        public Guid FileID { get; set; }

        /// <summary>
        /// The name of the file including extension.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// The full file path for this file in the source.
        /// </summary>
        public string FullSourcePath { get; set; }

        /// <summary>
        /// The files size measured in bytes.
        /// </summary>
        public ulong FileSizeBytes { get; set; }

        /// <summary>
        /// The hash of the file. 
        /// </summary>
        public byte[] FileHash { get; set; }

        /// <summary>
        /// The type of hash algorithm.
        /// </summary>
        public HashAlgorithmName HashAlgorithmType { get; set; }
    }
}
