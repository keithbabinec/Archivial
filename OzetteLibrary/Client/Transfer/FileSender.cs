using System;
using OzetteLibrary.Database;
using OzetteLibrary.Events;
using OzetteLibrary.Logging;
using OzetteLibrary.Models;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using OzetteLibrary.Crypto;
using System.IO;

namespace OzetteLibrary.Client.Transfer
{
    public class FileSender
    {
        /// <summary>
        /// A constructor that accepts a database and logger.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="logger"></param>
        public FileSender(IClientDatabase database, ILogger logger)
        {
            if (database == null)
            {
                throw new ArgumentNullException(nameof(database));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Database = database;
            Logger = logger;
            Clients = new Dictionary<string, TcpClient>();
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
        /// A reference to the TCP client connections.
        /// </summary>
        private Dictionary<string, TcpClient> Clients { get; set; }

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
        public AsyncResult BeginTransfer(ClientFile File)
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
        /// Performs a transfer of the specified file to the target locations.
        /// </summary>
        /// <param name="File"></param>
        /// <param name="AsyncState"></param>
        private void Transfer(ClientFile File, AsyncResult AsyncState)
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
                        var payload = GenerateNextTransferPayload(File, fs);

                        // step 4B: send the transfer payload.
                        SendTransferPayloadToFileTargets(File, payload);
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
        /// Updates the local file copy state if the file has already been transferred (and local DB doesn't know about it).
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fs"></param>
        private void UpdateFileCopyStateIfFileAlreadyExistsOnTargets(ClientFile file, FileStream fs)
        {
            // TODO:
            // for each target that needs this file:
            // > just double check that we haven't already transferred this whole file.
            // > to avoid resending if for some reason had lost local client DB state.

            throw new NotImplementedException();
        }

        /// <summary>
        /// Transfers a payload to one or more targets.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="payload"></param>
        private void SendTransferPayloadToFileTargets(ClientFile file, TransferPayload payload)
        {
            // TODO:
            // for each target that needs this block:
            // > grab an existing (or create a new) TcpClient connection to the target.
            // > ensure it is connected/authenticated.
            // > serialize the payload using binaryformatter
            // > write the message to the stream
            // > read the response from the server.
            // > update the file's progress to the next block.

            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates the next transfer block payload.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fs"></param>
        /// <returns></returns>
        private TransferPayload GenerateNextTransferPayload(ClientFile File, FileStream Stream)
        {
            return File.GenerateNextTransferPayload(Stream, Hasher);
        }

        /// <summary>
        /// Verifies that the existing hash for the specified file is correct in the database.
        /// </summary>
        /// <param name="File"></param>
        /// <param name="Stream"></param>
        private void UpdateHashIfFileHasChangedRecently(ClientFile File, FileStream Stream)
        {
            if (File == null)
            {
                throw new ArgumentNullException(nameof(File));
            }
            if (Stream == null)
            {
                throw new ArgumentNullException(nameof(Stream));
            }

            var currentHash = Hasher.HashCompleteFileFromFilePath(File.HashAlgorithmType, File.FullSourcePath);

            if (currentHash.Length != 0)
            {
                if (Hasher.CheckTwoByteHashesAreTheSame(File.FileHash, currentHash) == false)
                {
                    File.FileHash = currentHash;
                    File.LastChecked = DateTime.Now;
                    File.ResetCopyState();
                    Database.UpdateClientFile(File);
                }
            }
            else
            {
                throw new Exception("Failed to calculate the file hash.");
            }
        }
    }
}
