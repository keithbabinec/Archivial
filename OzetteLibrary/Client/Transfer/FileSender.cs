using System;
using OzetteLibrary.Database;
using OzetteLibrary.Events;
using OzetteLibrary.Logging;
using System.Collections.Generic;
using System.Threading;
using OzetteLibrary.Crypto;
using System.IO;
using OzetteLibrary.Files;
using OzetteLibrary.Providers;

namespace OzetteLibrary.Client.Transfer
{
    public class FileSender
    {
        /// <summary>
        /// A constructor that accepts a database and logger.
        /// </summary>
        /// <param name="database">The client database connection.</param>
        /// <param name="logger">A logging instance.</param>
        /// <param name="providers">A collection of cloud backup providers.</param>
        public FileSender(IClientDatabase database, ILogger logger, Dictionary<ProviderTypes, IProviderFileOperations> providers)
        {
            if (database == null)
            {
                throw new ArgumentNullException(nameof(database));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            if (providers == null)
            {
                throw new ArgumentNullException(nameof(providers));
            }
            if (providers.Count == 0)
            {
                throw new ArgumentException(nameof(providers) + " must be provided.");
            }

            Database = database;
            Logger = logger;
            Providers = providers;
            Hasher = new Hasher(Logger);
        }

        /// <summary>
        /// A reference to the database.
        /// </summary>
        private IClientDatabase Database { get; set; }

        /// <summary>
        /// A reference to the logger.
        /// </summary>
        private ILogger Logger { get; set; }

        /// <summary>
        /// A reference to the hasher.
        /// </summary>
        private Hasher Hasher { get; set; }

        /// <summary>
        /// A reference to the provider connections.
        /// </summary>
        private Dictionary<ProviderTypes, IProviderFileOperations> Providers { get; set; }

        /// <summary>
        /// Flag to indicate if the transfer operation is currently in progress.
        /// </summary>
        private volatile bool TransferInProgress = false;

        /// <summary>
        /// Flag to indicate if the transfer operation stop has been requested.
        /// </summary>
        private volatile bool TransferStopRequested = false;

        /// <summary>
        /// Begins a file transfer operation.
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>
        public AsyncResult BeginTransfer(BackupFile File)
        {
            if (File == null)
            {
                throw new ArgumentNullException(nameof(File));
            }
            if (TransferInProgress)
            {
                throw new InvalidOperationException("Cannot start the transfer operation. It is already in progress.");
            }

            TransferInProgress = true;
            TransferStopRequested = false;

            Logger.WriteTraceMessage(string.Format("Starting transfer operation for file: {0}", File.ToString()));

            AsyncResult resultState = new AsyncResult();

            Thread transferThread = new Thread(() => Transfer(File, resultState));
            transferThread.Start();

            return resultState;
        }

        /// <summary>
        /// Stops the in-progress file transfer operation.
        /// </summary>
        public void StopTransfer()
        {
            Logger.WriteTraceMessage("Stopping the in-progress transfer operation by request.");

            if (TransferInProgress)
            {
                TransferStopRequested = true;
            }
        }

        /// <summary>
        /// Performs a transfer of the specified file to the target providers.
        /// </summary>
        /// <param name="File"></param>
        /// <param name="AsyncState"></param>
        private void Transfer(BackupFile File, AsyncResult AsyncState)
        {
            bool canceled = false;

            try
            {
                // step 1: open up a filestream to the specified file.
                // use a read-only lock: this prevents the file from being modified while this lock is open.
                // but others can still open for read.

                using (FileStream fs = new FileStream(File.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    // step 2: verify/update the hash.
                    // if the file contents have changed between the time it was scanned and now, then this would result in a hash mismatch. 
                    // re-verify the hash of this file, and update the database if needed.

                    UpdateHashIfFileHasChangedRecently(File, fs);

                    // step 3: see if this file is already on the destination target(s).
                    // this avoids resending the file if for some reason the client DB/states got wiped out.

                    UpdateFileCopyStateIfFileAlreadyExistsOnTargets(File, fs);

                    // step 4: while the file has data that needs to be transferred- transfer it.
                    // this includes transferring to each potential target that needs this same file block.

                    while (File.HasDataToTransfer())
                    {
                        // step 4A: generate the next transfer data block.
                        // var payload = GenerateNextTransferPayload(File, fs);

                        // step 4B: send the transfer payload.
                        // SendTransferPayloadToFileTargets(File, payload);
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: handle this
                // will need logging and retry functionality, since I/O errors are common.
            }

            TransferInProgress = false;
            TransferStopRequested = false;

            if (canceled)
            {
                AsyncState.Cancel();
            }
            else
            {
                AsyncState.Complete();
            }
        }

        /// <summary>
        /// Generates the next transfer block payload.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fs"></param>
        /// <returns></returns>
        private TransferPayload GenerateNextTransferPayload(BackupFile File, FileStream Stream)
        {
            return File.GenerateNextTransferPayload(Stream, Hasher);
        }

        /// <summary>
        /// Updates the local file copy state if the file has already been transferred (and local DB doesn't know about it).
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fs"></param>
        private void UpdateFileCopyStateIfFileAlreadyExistsOnTargets(BackupFile file, FileStream fs)
        {
            // TODO:
            // for each target that needs this file:
            // > just double check that we haven't already transferred this whole file.
            // > to avoid resending if for some reason had lost local client DB state.

            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the existing hash for the specified file is correct in the database.
        /// </summary>
        /// <param name="File"></param>
        /// <param name="Stream"></param>
        private void UpdateHashIfFileHasChangedRecently(BackupFile File, FileStream Stream)
        {
            if (File == null)
            {
                throw new ArgumentNullException(nameof(File));
            }
            if (Stream == null)
            {
                throw new ArgumentNullException(nameof(Stream));
            }

            var currentHash = Hasher.GenerateFileHash(File.GetFileHashAlgorithm(), Stream);

            if (currentHash.Length != 0)
            {
                if (Hasher.CheckTwoByteHashesAreTheSame(File.GetFileHash(), currentHash) == false)
                {
                    File.SetFileHashWithAlgorithm(currentHash, File.GetFileHashAlgorithm());
                    File.SetLastCheckedTimeStamp();
                    File.ResetCopyState(Database.GetProvidersList());
                    Database.UpdateBackupFile(File);
                }
            }
            else
            {
                throw new Exception("Failed to calculate the file hash.");
            }
        }
    }
}
