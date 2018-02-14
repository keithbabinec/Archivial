using OzetteLibrary.Crypto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        /// Resets existing copy progress state with the existing set of targets.
        /// </summary>
        public void ResetCopyState()
        {
            if (CopyState != null)
            {
                foreach (var item in CopyState.Values)
                {
                    item.ResetState();
                }
            }

            SetOverallStateFromCopyState();
        }

        /// <summary>
        /// Resets existing copy progress state with a new set of targets.
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

            SetOverallStateFromCopyState();
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
        public bool HasDataToTransfer(Targets targets)
        {
            // check the copy state.
            // return true if this file is capable of sending the next datablock.
            // this means it is not in a failed state, and has data needing to be transferred.

            if (CopyState == null || CopyState.Count == 0)
            {
                return false;
            }
            if (targets == null)
            {
                return false;
            }
            if (OverallState == FileStatus.Synced)
            {
                return false;
            }

            foreach (var item in CopyState)
            {
                var itemTarget = targets.FirstOrDefault(x => x.ID == item.Key);

                if (itemTarget == null)
                {
                    // bad copy state
                    // we somehow can't find the target for the copystate item.
                    throw new InvalidOperationException("No target was found for copystate item with target key: " + item.Key);
                }

                if (itemTarget.Availability == TargetAvailabilityState.Available 
                    && item.Value.TargetStatus != FileStatus.Synced)
                {
                    // the target is available/online.
                    // the item is not fully synced.
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the next transfer payload object
        /// </summary>
        /// <param name="Stream"></param>
        /// <param name="Hasher"></param>
        /// <returns></returns>
        public TransferPayload GenerateNextTransferPayload(FileStream Stream, Hasher Hasher)
        {
            if (Stream == null)
            {
                throw new ArgumentNullException(nameof(Stream));
            }
            if (Hasher == null)
            {
                throw new ArgumentNullException(nameof(Hasher));
            }
            if (OverallState == FileStatus.Synced)
            {
                throw new InvalidOperationException("File is already synced.");
            }
            if (Priority == FileBackupPriority.Unset)
            {
                throw new InvalidOperationException("File has no backup priority set.");
            }
            if (CopyState == null || CopyState.Count == 0)
            {
                throw new InvalidOperationException("File has no targets set.");
            }

            // determine which block we need to send next.
            // find the first available block.

            var nextBlock = GetNextBlockNumberToSend();

            if (nextBlock.HasValue == false)
            {
                throw new InvalidOperationException(string.Format("NextBlock was not found. CopyState and OverallState ({0}) are inconsistent.", OverallState.ToString()));
            }

            // construct a new payload object with basic metadata/ids

            TransferPayload payload = new TransferPayload();
            payload.FileID = FileID;
            payload.CurrentBlockNumber = nextBlock.Value;
            payload.TotalBlocks = CalculateTotalFileBlocks(Constants.Transfers.TransferChunkSizeBytes);
            payload.DestinationTargetIDs = FindTargetsThatCanTransferSpecifiedBlock(nextBlock.Value);

            // generate the 'data' payload: the next file chunk as byte[].
            // hash that chunk so we can validate it on the other side.

            payload.Data = ReadFileChunk(Stream, nextBlock.Value);
            payload.ExpectedHash = Hasher.HashFileChunkFromByteArray(Hasher.GetDefaultHashAlgorithm(Priority), payload.Data);

            return payload;
        }

        /// <summary>
        /// Reads the specified file block data into an array of bytes.
        /// </summary>
        /// <param name="Stream"></param>
        /// <param name="BlockNumber"></param>
        /// <returns></returns>
        private byte[] ReadFileChunk(FileStream Stream, int BlockNumber)
        {
            // set the position in the file to the current block offset.

            int chunkSize = Constants.Transfers.TransferChunkSizeBytes;
            long offset = chunkSize * BlockNumber;
            Stream.Seek(offset, SeekOrigin.Begin);

            // size the buffer appropriately.
            // to either the full chunk size, or a partial (last) chunk.

            long bytesRemainingToRead = FileSizeBytes - Stream.Position;
            if (bytesRemainingToRead > chunkSize)
            {
                // more than we can fit in the buffer.
                // then just use the full buffer size.

                bytesRemainingToRead = chunkSize;
            }

            // read the next chunk.
            // note: Just calling Read() is likely to return less then the full buffer ask.
            // read runs in a loop until the end of file, or end of the expected chunk size.

            byte[] buffer = new byte[bytesRemainingToRead];
            int bytesReadSoFar = 0;

            while (bytesRemainingToRead > 0)
            {
                int latestBytesReadCount = Stream.Read(buffer, bytesReadSoFar, (int)bytesRemainingToRead);
                if (latestBytesReadCount == 0)
                {
                    break;
                }

                bytesReadSoFar += latestBytesReadCount;
                bytesRemainingToRead -= latestBytesReadCount;
            }

            return buffer;
        }

        /// <summary>
        /// Returns the next block number to send/transfer.
        /// </summary>
        /// <returns></returns>
        private int? GetNextBlockNumberToSend()
        {
            int? minBlock = null;

            if (CopyState != null)
            {
                foreach (var target in CopyState)
                {
                    var targetState = target.Value.TargetStatus;

                    if (targetState == FileStatus.OutOfDate || targetState == FileStatus.Unsynced || targetState == FileStatus.InProgress)
                    {
                        int targetNextBlock = target.Value.LastCompletedFileChunkIndex + 1;
                        
                        if (minBlock.HasValue == false)
                        {
                            // found (first) min.
                            minBlock = targetNextBlock;
                        }
                        else if (targetNextBlock < minBlock.Value)
                        {
                            // found new min.
                            minBlock = targetNextBlock;
                        }
                    }
                    else if (targetState == FileStatus.Synced)
                    {
                        // disregard. file is already synced to this particular target.
                    }
                    else
                    {
                        throw new InvalidOperationException("Unexpected target state: " + targetState.ToString());
                    }
                }
            }

            return minBlock;
        }

        /// <summary>
        /// Returns a list of targets (IDs) that need a specified block number.
        /// </summary>
        /// <param name="blockNumber">The next block to transfer</param>
        /// <returns></returns>
        private List<int> FindTargetsThatCanTransferSpecifiedBlock(int blockNumber)
        {
            List<int> result = new List<int>();

            if (CopyState != null)
            {
                foreach (var target in CopyState)
                {
                    var targetState = target.Value.TargetStatus;

                    if (targetState == FileStatus.OutOfDate || targetState == FileStatus.Unsynced || targetState == FileStatus.InProgress)
                    {
                        int targetNextBlock = target.Value.LastCompletedFileChunkIndex + 1;
                        if (targetNextBlock == blockNumber)
                        {
                            result.Add(target.Key);
                        }
                    }
                    else if (targetState == FileStatus.Synced)
                    {
                        // disregard. file is already synced to this particular target.
                    }
                    else
                    {
                        throw new InvalidOperationException("Unexpected target state: " + targetState.ToString());
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Flags a particular block as sent for the specified targets.
        /// </summary>
        /// <param name="BlockNumber"></param>
        /// <param name="Destinations"></param>
        public void SetBlockAsSent(int BlockNumber, List<int> Destinations)
        {
            if (BlockNumber < 0)
            {
                throw new ArgumentException(nameof(BlockNumber) + " argument must be provided with a positive number.");
            }
            if (Destinations == null || Destinations.Count == 0)
            {
                throw new ArgumentException(nameof(Destinations) + " argument must be provided with at least one entry (not null or empty)");
            }
            if (OverallState == FileStatus.Synced)
            {
                throw new InvalidOperationException("File is already synced.");
            }
            if (CopyState == null || CopyState.Count == 0)
            {
                throw new InvalidOperationException("File has no targets set.");
            }

            foreach (var destinationID in Destinations)
            {
                if (CopyState.ContainsKey(destinationID))
                {
                    var state = CopyState[destinationID];
                    state.LastCompletedFileChunkIndex = BlockNumber;

                    if (state.LastCompletedFileChunkIndex == TotalFileChunks)
                    {
                        // flag this particular destination as completed.
                        state.TargetStatus = FileStatus.Synced;
                    }
                    else
                    {
                        // file transfer is still in progress.
                        state.TargetStatus = FileStatus.InProgress;
                    }
                }
                else
                {
                    throw new InvalidOperationException("Attempted to set copy state for destination that wasn't found in the copy state.");
                }
            }

            SetOverallStateFromCopyState();
        }

        /// <summary>
        /// Sets the OverallState from the current CopyState.
        /// </summary>
        private void SetOverallStateFromCopyState()
        {
            if (CopyState != null && CopyState.Count != 0)
            {
                int unsyncedCount = 0;
                int outofdateCount = 0;
                int inprogressCount = 0;
                int syncedCount = 0;
                int targetsCount = CopyState.Count;

                foreach (var target in CopyState)
                {
                    if (target.Value.TargetStatus == FileStatus.Unsynced)
                    {
                        unsyncedCount++;
                    }
                    else if (target.Value.TargetStatus == FileStatus.OutOfDate)
                    {
                        outofdateCount++;
                    }
                    else if (target.Value.TargetStatus == FileStatus.InProgress)
                    {
                        inprogressCount++;
                    }
                    else if (target.Value.TargetStatus == FileStatus.Synced)
                    {
                        syncedCount++;
                    }
                }

                // start at the highest condition and work backwards

                // condition 1: everything is synced

                if (syncedCount == targetsCount)
                {
                    OverallState = FileStatus.Synced;
                    return;
                }

                // condition 2: something is in-progress

                if (inprogressCount > 0)
                {
                    OverallState = FileStatus.InProgress;
                    return;
                }

                // condition 3: something is out-of-date

                if (outofdateCount > 0)
                {
                    OverallState = FileStatus.OutOfDate;
                    return;
                }

                // condition 4: something is unsynced

                if (unsyncedCount > 0)
                {
                    OverallState = FileStatus.Unsynced;
                    return;
                }
            }
            else
            {
                // no targets?
                // that means we can't be in a synced state.
                OverallState = FileStatus.Unsynced;
            }
        }
    }
}
