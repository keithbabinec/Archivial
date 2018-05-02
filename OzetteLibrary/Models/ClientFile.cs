using OzetteLibrary.Crypto;
using System;
using System.Collections.Generic;
using System.IO;

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
        /// Default/empty constructor.
        /// </summary>
        /// <remarks>
        /// This is required for database operations.
        /// </remarks>
        public ClientFile()
        {
        }

        /// <summary>
        /// Constructor that accepts a FileInfo object and a priority.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="priority"></param>
        public ClientFile(FileInfo fileInfo, FileBackupPriority priority)
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
            TotalFileChunks = CalculateTotalFileBlocks(Constants.Transfers.TransferChunkSizeBytes);
            Priority = priority;
        }

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
    }
}
