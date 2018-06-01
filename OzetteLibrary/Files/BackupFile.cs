﻿using OzetteLibrary.Crypto;
using OzetteLibrary.Providers;
using System;
using System.Collections.Generic;
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
        /// This is required for serialization purposes.
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
        /// A unique identifier for this file which should be shared among both client and providers.
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
        /// The state of this file across one or more providers.
        /// </summary>
        /// <remarks>
        /// The dictionary key is the provider type.
        /// The dictionary value is the provider file state.
        /// </remarks>
        public Dictionary<ProviderTypes, ProviderFileStatus> CopyState { get; set; }

        /// <summary>
        /// An overall state across one or more providers.
        /// </summary>
        public FileStatus OverallState { get; set; }

        /// <summary>
        /// Resets existing copy progress state with the specified providers.
        /// </summary>
        /// <param name="providers"></param>
        public void ResetCopyState(ProviderTypes[] providers)
        {
            CopyState = new Dictionary<ProviderTypes, ProviderFileStatus>();

            foreach (var provider in providers)
            {
                CopyState.Add(provider, new ProviderFileStatus(provider));
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
                int providersCount = CopyState.Count;

                foreach (var provider in CopyState)
                {
                    if (provider.Value.SyncStatus == FileStatus.Unsynced)
                    {
                        unsyncedCount++;
                    }
                    else if (provider.Value.SyncStatus == FileStatus.OutOfDate)
                    {
                        outofdateCount++;
                    }
                    else if (provider.Value.SyncStatus == FileStatus.InProgress)
                    {
                        inprogressCount++;
                    }
                    else if (provider.Value.SyncStatus == FileStatus.Synced)
                    {
                        syncedCount++;
                    }
                }

                // start at the highest condition and work backwards

                // condition 1: everything is synced

                if (syncedCount == providersCount)
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
                // no providers?
                // that means we can't be in a synced state.
                OverallState = FileStatus.Unsynced;
            }
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
            payload.TotalBlocks = CalculateTotalFileBlocks(Constants.Transfers.TransferBlockSizeBytes);
            payload.DestinationProviders = FindProvidersThatCanTransferSpecifiedBlock(nextBlock.Value);

            // generate the 'data' payload: the next file block as byte[].
            // hash that block so we can validate it on the other side.

            payload.Data = ReadFileBlock(Stream, nextBlock.Value);
            payload.ExpectedHash = Hasher.HashFileBlockFromByteArray(Hasher.GetDefaultHashAlgorithm(Priority), payload.Data);

            return payload;
        }

        /// <summary>
        /// Reads the specified file block data into an array of bytes.
        /// </summary>
        /// <param name="Stream"></param>
        /// <param name="BlockNumber"></param>
        /// <returns></returns>
        private byte[] ReadFileBlock(FileStream Stream, int BlockNumber)
        {
            // set the position in the file to the current block offset.

            int blockSize = Constants.Transfers.TransferBlockSizeBytes;
            long offset = blockSize * BlockNumber;
            Stream.Seek(offset, SeekOrigin.Begin);

            // size the buffer appropriately.
            // to either the full block size, or a partial (last) block.

            long bytesRemainingToRead = FileSizeBytes - Stream.Position;
            if (bytesRemainingToRead > blockSize)
            {
                // more than we can fit in the buffer.
                // then just use the full buffer size.

                bytesRemainingToRead = blockSize;
            }

            // read the next block.
            // note: Just calling Read() is likely to return less then the full buffer ask.
            // read runs in a loop until the end of file, or end of the expected block size.

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
                foreach (var provider in CopyState)
                {
                    var providerState = provider.Value.SyncStatus;

                    if (providerState == FileStatus.OutOfDate || providerState == FileStatus.Unsynced || providerState == FileStatus.InProgress)
                    {
                        int providerNextBlock = provider.Value.LastCompletedFileBlockIndex + 1;

                        if (minBlock.HasValue == false)
                        {
                            // found (first) min.
                            minBlock = providerNextBlock;
                        }
                        else if (providerNextBlock < minBlock.Value)
                        {
                            // found new min.
                            minBlock = providerNextBlock;
                        }
                    }
                    else if (providerState == FileStatus.Synced)
                    {
                        // disregard. file is already synced to this particular provider.
                    }
                    else
                    {
                        throw new InvalidOperationException("Unexpected provider state: " + providerState.ToString());
                    }
                }
            }

            return minBlock;
        }

        /// <summary>
        /// Returns a list of providers that need a specified block number.
        /// </summary>
        /// <param name="blockNumber">The next block to transfer</param>
        /// <returns></returns>
        private List<ProviderTypes> FindProvidersThatCanTransferSpecifiedBlock(int blockNumber)
        {
            List<ProviderTypes> result = new List<ProviderTypes>();

            if (CopyState != null)
            {
                foreach (var provider in CopyState)
                {
                    var providerState = provider.Value.SyncStatus;

                    if (providerState == FileStatus.OutOfDate || providerState == FileStatus.Unsynced || providerState == FileStatus.InProgress)
                    {
                        int providerNextBlock = provider.Value.LastCompletedFileBlockIndex + 1;
                        if (providerNextBlock == blockNumber)
                        {
                            result.Add(provider.Key);
                        }
                    }
                    else if (providerState == FileStatus.Synced)
                    {
                        // disregard. file is already synced to this particular provider.
                    }
                    else
                    {
                        throw new InvalidOperationException("Unexpected provider state: " + providerState.ToString());
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Flags a particular block as sent for the specified provider.
        /// </summary>
        /// <param name="BlockNumber"></param>
        /// <param name="Providers"></param>
        public void SetBlockAsSent(int BlockNumber, ProviderTypes Provider)
        {
            if (BlockNumber < 0)
            {
                throw new ArgumentException(nameof(BlockNumber) + " argument must be provided with a positive number.");
            }
            if (OverallState == FileStatus.Synced)
            {
                throw new InvalidOperationException("File is already synced.");
            }
            if (CopyState == null || CopyState.Count == 0)
            {
                throw new InvalidOperationException("File has no copystate set.");
            }

            if (CopyState.ContainsKey(Provider))
            {
                var state = CopyState[Provider];
                state.LastCompletedFileBlockIndex = BlockNumber;

                if (state.LastCompletedFileBlockIndex == TotalFileBlocks)
                {
                    // flag this particular destination as completed.
                    state.SyncStatus = FileStatus.Synced;
                }
                else
                {
                    // file transfer is still in progress.
                    state.SyncStatus = FileStatus.InProgress;
                }
            }
            else
            {
                throw new InvalidOperationException("Attempted to set copy state for destination that wasn't found in the copy state.");
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