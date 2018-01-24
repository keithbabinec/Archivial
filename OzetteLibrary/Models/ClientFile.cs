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
        /// Constructor that accepts a FileInfo object
        /// </summary>
        /// <param name="fileInfo"></param>
        public ClientFile(FileInfo fileInfo)
        {
            FileID = Guid.NewGuid();
            Filename = fileInfo.Name;
            Directory = fileInfo.DirectoryName;
            FullSourcePath = fileInfo.FullName;
            FileSizeBytes = fileInfo.Length;
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
            if (OverallState == FileStatus.Synced)
            {
                throw new InvalidOperationException("File is already synced.");
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
            payload.TotalBlocks = GetTotalFileBlocks(Constants.Transfers.TransferChunkSizeBytes);

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
            int? nextBlockNumberToSend = null;

            if (CopyState != null)
            {
                foreach (var target in CopyState)
                {
                    var targetState = target.Value.TargetStatus;

                    if (targetState == FileStatus.InProgress)
                    {
                        nextBlockNumberToSend = target.Value.LastCompletedFileChunk + 1;
                        break;
                    }
                    else if (targetState == FileStatus.OutOfDate || targetState == FileStatus.Unsynced)
                    {
                        nextBlockNumberToSend = 0;
                        break;
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

            return nextBlockNumberToSend;
        }
    }
}
