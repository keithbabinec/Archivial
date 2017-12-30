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
        /// Constructor that accepts a FileInfo object
        /// </summary>
        /// <param name="fileInfo"></param>
        public ClientFile(FileInfo fileInfo)
        {
            FileID = Guid.NewGuid();
            Filename = fileInfo.Name;
            Directory = fileInfo.DirectoryName;
            FullSourcePath = fileInfo.FullName;
            FileSizeBytes = Convert.ToUInt64(fileInfo.Length);
        }

        /// <summary>
        /// The last time this file was scanned in the backup source.
        /// </summary>
        public DateTime? LastChecked { get; set; }

        /// <summary>
        /// The state of this file across one or more targets.
        /// </summary>
        /// <remarks>
        /// The dictionary key is the target ID.
        /// The dictionary value is the copy state.
        /// </remarks>
        public Dictionary<int, TargetCopyState> CopyState { get; set; }

        /// <summary>
        /// An overall state across one or more targets.
        /// </summary>
        public FileStatus OverallState { get; set; }

        /// <summary>
        /// Resets existing copy progress state.
        /// </summary>
        /// <param name="targets"></param>
        public void ResetCopyState(Targets targets)
        {
            if (targets == null)
            {
                throw new ArgumentNullException(nameof(targets));
            }

            CopyState = new Dictionary<int, TargetCopyState>();
            OverallState = FileStatus.Unsynced;

            foreach (var target in targets)
            {
                CopyState.Add(target.ID, new TargetCopyState(target));
            }
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
            // TODO:
            // check the copy state.
            // return true if this file is capable of sending the next datablock.
            // this means it is not in a failed state, and has data needing to be transferred.

            throw new NotImplementedException();
        }
    }
}
