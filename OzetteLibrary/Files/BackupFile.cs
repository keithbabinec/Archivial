using System;
using System.IO;
using System.Security.Cryptography;

namespace OzetteLibrary.Files
{
    /// <summary>
    /// Describes a single file to be backed up.
    /// </summary>
    public class BackupFile
    {
        /// <summary>
        /// Default/empty constructor.
        /// </summary>
        /// <remarks>
        /// This is required for database operations.
        /// </remarks>
        public BackupFile()
        {
        }

        /// <summary>
        /// Constructor that accepts a FileInfo object and a priority.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="priority"></param>
        public BackupFile(FileInfo fileInfo, FileBackupPriority priority)
        {
            if (priority == FileBackupPriority.Unset)
            {
                throw new ArgumentException(nameof(priority) + " must be provided.");
            }

            FileID = Guid.NewGuid();
            Filename = fileInfo.Name;
            Directory = fileInfo.DirectoryName;
            FullSourcePath = fileInfo.FullName;
            FileSizeBytes = fileInfo.Length;
            TotalFileBlocks = CalculateTotalFileBlocks(Constants.Transfers.TransferBlockSizeBytes);
            Priority = priority;
        }

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
        /// The total number of file transfer blocks.
        /// </summary>
        public int TotalFileBlocks { get; set; }

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
        /// The last time this file was scanned in the backup source.
        /// </summary>
        public DateTime? LastChecked { get; set; }

        /// <summary>
        /// An overall state across one or more targets.
        /// </summary>
        public FileStatus OverallState { get; set; }

        /// <summary>
        /// Resets existing copy progress state.
        /// </summary>
        public void ResetCopyState()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the last checked timestamp to the current time.
        /// </summary>
        public void SetLastCheckedTimeStamp()
        {
            LastChecked = DateTime.Now;
        }

        /// <summary>
        /// Gets the last checked timestamp
        /// </summary>
        /// <returns>Byte[]</returns>
        public DateTime? GetLastCheckedTimeStamp()
        {
            return LastChecked;
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (FullSourcePath != null)
            {
                return string.Format("{0}", FullSourcePath);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Determines if there is data remaining to transfer.
        /// </summary>
        /// <returns></returns>
        public bool HasDataToTransfer()
        {
            if (OverallState == FileStatus.Synced)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

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
        /// <param name="BlockSize"></param>
        /// <returns></returns>
        public int CalculateTotalFileBlocks(int BlockSize)
        {
            if (BlockSize == 0)
            {
                throw new ArgumentException("Provide a non-zero value for block size.");
            }

            int blocks = Convert.ToInt32((FileSizeBytes / BlockSize));
            int remainder = Convert.ToInt32(FileSizeBytes % BlockSize);

            if (remainder > 0)
            {
                blocks++;
            }

            return blocks;
        }
    }
}
