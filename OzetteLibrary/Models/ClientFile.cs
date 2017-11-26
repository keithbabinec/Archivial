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

            foreach (var target in targets)
            {
                CopyState.Add(target.ID, new TargetCopyState(target));
            }
        }
    }
}
