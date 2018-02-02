using LiteDB;
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
        /// <example>
        /// 'test.exe'
        /// </example>
        public string Filename { get; set; }

        /// <summary>
        /// The directory path that hosts this file.
        /// </summary>
        /// <example>
        /// 'C:\bin\programs'
        /// </example>
        public string Directory { get; set; }

        /// <summary>
        /// The full file path (directory + filename) for this file in the source.
        /// </summary>
        /// <example>
        /// 'C:\bin\programs\test.exe'
        /// </example>
        public string FullSourcePath { get; set; }

        /// <summary>
        /// The files size measured in bytes.
        /// </summary>
        public long FileSizeBytes { get; set; }

        /// <summary>
        /// The hash of the file. 
        /// </summary>
        public byte[] FileHash { get; set; }

        /// <summary>
        /// The file backup priority of this file.
        /// </summary>
        public FileBackupPriority Priority { get; set; }

        /// <summary>
        /// The type of hash algorithm.
        /// </summary>
        public HashAlgorithmName HashAlgorithmType { get; set; }

        /// <summary>
        /// Sets the file hash and hash algorithm.
        /// </summary>
        /// <param name="filehash"></param>
        /// <param name="algorithm"></param>
        public void SetFileHashWithAlgorithm(byte[] filehash, HashAlgorithmName algorithm)
        {
            if (filehash == null)
            {
                throw new ArgumentNullException(nameof(filehash));
            }

            FileHash = filehash;
            HashAlgorithmType = algorithm;
        }

        /// <summary>
        /// Gets the file hash.
        /// </summary>
        /// <returns>Byte[]</returns>
        public byte[] GetFileHash()
        {
            return FileHash;
        }

        /// <summary>
        /// Gets the file hash.
        /// </summary>
        /// <returns>HashAlgorithmName</returns>
        public HashAlgorithmName GetFileHashAlgorithm()
        {
            return HashAlgorithmType;
        }

        /// <summary>
        /// Returns the expected number of file blocks.
        /// </summary>
        /// <param name="ChunkSize"></param>
        /// <returns></returns>
        public int GetTotalFileBlocks(int ChunkSize)
        {
            if (ChunkSize == 0)
            {
                throw new ArgumentException("Provide a non-zero value for chunksize.");
            }

            int blocks = Convert.ToInt32((FileSizeBytes / ChunkSize));
            int remainder = Convert.ToInt32(FileSizeBytes % ChunkSize);

            if (remainder > 0)
            {
                blocks++;
            }

            return blocks;
        }
    }
}
