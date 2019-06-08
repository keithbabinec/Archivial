using Microsoft.VisualStudio.TestTools.UnitTesting;
using ArchivialLibrary.Constants;
using ArchivialLibrary.Files;
using ArchivialLibrary.Folders;
using ArchivialLibrary.Providers;
using ArchivialLibrary.StorageProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using ArchivialLibrary.Crypto;
using ArchivialLibrary.Logging.Mock;
using ArchivialLibraryTests.Crypto;

namespace ArchivialLibraryTests.Files
{
    [TestClass]
    public class BackupFileTests
    {
        [TestMethod]
        public void BackupFileGetTotalFileBlocksHandlesZeroFileLength()
        {
            var file = new ArchivialLibrary.Files.BackupFile();
            file.FileSizeBytes = 0;
            Assert.AreEqual(0, file.CalculateTotalFileBlocks(ArchivialLibrary.Constants.Transfers.TransferBlockSizeBytes));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BackupFileGetTotalFileBlocksThrowsWhenProvidedZeroBlockSize()
        {
            var file = new ArchivialLibrary.Files.BackupFile();
            file.FileSizeBytes = 1048576; // 1 MB
            file.CalculateTotalFileBlocks(0);
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks1()
        {
            var file = new ArchivialLibrary.Files.BackupFile();
            file.FileSizeBytes = 2097152; // 2 MB
            Assert.AreEqual(1, file.CalculateTotalFileBlocks(ArchivialLibrary.Constants.Transfers.TransferBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks2()
        {
            var file = new ArchivialLibrary.Files.BackupFile();
            file.FileSizeBytes = 4194304; // 4 MB
            Assert.AreEqual(2, file.CalculateTotalFileBlocks(ArchivialLibrary.Constants.Transfers.TransferBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks3()
        {
            var file = new ArchivialLibrary.Files.BackupFile();
            file.FileSizeBytes = 8388608; // 8 MB
            Assert.AreEqual(4, file.CalculateTotalFileBlocks(ArchivialLibrary.Constants.Transfers.TransferBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks4()
        {
            var file = new ArchivialLibrary.Files.BackupFile();
            file.FileSizeBytes = 310378496; // 310 MB
            Assert.AreEqual(148, file.CalculateTotalFileBlocks(ArchivialLibrary.Constants.Transfers.TransferBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks5()
        {
            var file = new ArchivialLibrary.Files.BackupFile();
            file.FileSizeBytes = 2097151; // just under 2 MB
            Assert.AreEqual(1, file.CalculateTotalFileBlocks(ArchivialLibrary.Constants.Transfers.TransferBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks7()
        {
            var file = new ArchivialLibrary.Files.BackupFile();
            file.FileSizeBytes = 2097153; // just over 2 MB
            Assert.AreEqual(2, file.CalculateTotalFileBlocks(ArchivialLibrary.Constants.Transfers.TransferBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks8()
        {
            var file = new ArchivialLibrary.Files.BackupFile();
            file.FileSizeBytes = 2619922; // over 2 MB, with a halfway populated 2nd block.
            Assert.AreEqual(2, file.CalculateTotalFileBlocks(ArchivialLibrary.Constants.Transfers.TransferBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks9()
        {
            var file = new ArchivialLibrary.Files.BackupFile();
            file.FileSizeBytes = 4194301; // just under 4 MB
            Assert.AreEqual(2, file.CalculateTotalFileBlocks(ArchivialLibrary.Constants.Transfers.TransferBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks10()
        {
            var file = new ArchivialLibrary.Files.BackupFile();
            file.FileSizeBytes = 4194308; // just over 4 MB
            Assert.AreEqual(3, file.CalculateTotalFileBlocks(ArchivialLibrary.Constants.Transfers.TransferBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks11()
        {
            var file = new ArchivialLibrary.Files.BackupFile();
            file.FileSizeBytes = 189643547895; // very large file size
            Assert.AreEqual(90430, file.CalculateTotalFileBlocks(ArchivialLibrary.Constants.Transfers.TransferBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks12()
        {
            var file = new ArchivialLibrary.Files.BackupFile();
            file.FileSizeBytes = 43414941001; // very large file size
            Assert.AreEqual(20702, file.CalculateTotalFileBlocks(ArchivialLibrary.Constants.Transfers.TransferBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks13()
        {
            var file = new ArchivialLibrary.Files.BackupFile();
            file.FileSizeBytes = 7744931200569; // very large file size
            Assert.AreEqual(167955484, file.CalculateTotalFileBlocks(46113)); // unusual block size
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks14()
        {
            var file = new ArchivialLibrary.Files.BackupFile();
            file.FileSizeBytes = 1647999899992; // very large file size
            Assert.AreEqual(228539718, file.CalculateTotalFileBlocks(7211)); // unusual block size
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks15()
        {
            var file = new ArchivialLibrary.Files.BackupFile();
            file.FileSizeBytes = 15677400000;
            Assert.AreEqual(154, file.CalculateTotalFileBlocks(102400000));
        }

        [TestMethod]
        public void BackupFileHasDataToTransferReturnsValidFalseExample1()
        {
            var file = new ArchivialLibrary.Files.BackupFile();
            file.OverallState = ArchivialLibrary.Files.FileStatus.Synced;

            Assert.IsFalse(file.HasDataToTransfer());
        }

        [TestMethod]
        public void BackupFileHasDataToTransferReturnsValidTrueExample1()
        {
            var file = new ArchivialLibrary.Files.BackupFile();
            file.OverallState = ArchivialLibrary.Files.FileStatus.OutOfDate;

            Assert.IsTrue(file.HasDataToTransfer());
        }

        [TestMethod]
        public void BackupFileHasDataToTransferReturnsValidTrueExample2()
        {
            var file = new ArchivialLibrary.Files.BackupFile();
            file.OverallState = ArchivialLibrary.Files.FileStatus.Unsynced;

            Assert.IsTrue(file.HasDataToTransfer());
        }

        [TestMethod]
        public void BackupFileHasDataToTransferReturnsValidTrueExample3()
        {
            var file = new ArchivialLibrary.Files.BackupFile();
            file.OverallState = ArchivialLibrary.Files.FileStatus.InProgress;

            Assert.IsTrue(file.HasDataToTransfer());
        }

        [TestMethod]
        public void BackupFileResetCopyStateCorrectlyResetsState1()
        {
            var provider1 = ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS;
            var provider2 = ArchivialLibrary.StorageProviders.StorageProviderTypes.Azure;

            var file = new ArchivialLibrary.Files.BackupFile();

            file.CopyState = new System.Collections.Generic.Dictionary
                <ArchivialLibrary.StorageProviders.StorageProviderTypes,
                ArchivialLibrary.StorageProviders.StorageProviderFileStatus>();

            file.CopyState.Add(
                provider1,
                new ArchivialLibrary.StorageProviders.StorageProviderFileStatus()
                {
                    Provider = provider1,
                    SyncStatus = ArchivialLibrary.Files.FileStatus.InProgress
                });

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = (int)provider1, Name = provider1.ToString(), Type = ProviderTypes.Storage },
                new Provider() { ID = (int)provider2, Name = provider2.ToString(), Type = ProviderTypes.Storage }
            });

            Assert.AreEqual(2, file.CopyState.Count);
            Assert.AreEqual(ArchivialLibrary.Files.FileStatus.Unsynced, file.CopyState[provider1].SyncStatus);
            Assert.AreEqual(ArchivialLibrary.Files.FileStatus.Unsynced, file.CopyState[provider2].SyncStatus);
        }

        [TestMethod]
        public void BackupFileResetCopyStateCorrectlyResetsState2()
        {
            var provider1 = ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS;
            var provider2 = ArchivialLibrary.StorageProviders.StorageProviderTypes.Azure;

            var file = new ArchivialLibrary.Files.BackupFile();

            file.CopyState = new Dictionary
                <ArchivialLibrary.StorageProviders.StorageProviderTypes,
                ArchivialLibrary.StorageProviders.StorageProviderFileStatus>();

            file.CopyState.Add(
                provider1,
                new ArchivialLibrary.StorageProviders.StorageProviderFileStatus()
                {
                    Provider = provider1,
                    SyncStatus = ArchivialLibrary.Files.FileStatus.Synced
                });

            file.CopyState.Add(
                provider2,
                new ArchivialLibrary.StorageProviders.StorageProviderFileStatus()
                {
                    Provider = provider2,
                    SyncStatus = ArchivialLibrary.Files.FileStatus.Synced
                });

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = (int)provider1, Name = provider1.ToString(), Type = ProviderTypes.Storage }
            });

            Assert.AreEqual(1, file.CopyState.Count);
            Assert.AreEqual(ArchivialLibrary.Files.FileStatus.Unsynced, file.CopyState[provider1].SyncStatus);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BackupFileGenerateNextTransferPayloadThrowsOnInvalidArgs1()
        {
            // no filestream
            var file = new ArchivialLibrary.Files.BackupFile();
            file.GenerateNextTransferPayload(null, new ArchivialLibrary.Crypto.Hasher(new ArchivialLibrary.Logging.Mock.MockLogger()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BackupFileGenerateNextTransferPayloadThrowsOnInvalidArgs2()
        {
            var file = new ArchivialLibrary.Files.BackupFile();

            // no logger
            using (var filestream = new FileStream(".\\TestFiles\\Hasher\\SmallFile.txt", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                file.GenerateNextTransferPayload(filestream, null);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BackupFileGenerateNextTransferPayloadThrowsOnInvalidFileState1()
        {
            var file = new ArchivialLibrary.Files.BackupFile();

            // file is already synced
            using (var filestream = new FileStream(".\\TestFiles\\Hasher\\SmallFile.txt", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                file.OverallState = ArchivialLibrary.Files.FileStatus.Synced;
                file.GenerateNextTransferPayload(filestream, new ArchivialLibrary.Crypto.Hasher(new ArchivialLibrary.Logging.Mock.MockLogger()));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BackupFileGenerateNextTransferPayloadThrowsOnInvalidFileState2()
        {
            var file = new ArchivialLibrary.Files.BackupFile();

            // file priority is not set
            using (var filestream = new FileStream(".\\TestFiles\\Hasher\\SmallFile.txt", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                file.OverallState = ArchivialLibrary.Files.FileStatus.Unsynced;
                file.Priority = ArchivialLibrary.Files.FileBackupPriority.Unset;
                file.GenerateNextTransferPayload(filestream, new ArchivialLibrary.Crypto.Hasher(new ArchivialLibrary.Logging.Mock.MockLogger()));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BackupFileGenerateNextTransferPayloadThrowsOnInvalidFileState3()
        {
            var file = new ArchivialLibrary.Files.BackupFile();

            // copystate is not set
            using (var filestream = new FileStream(".\\TestFiles\\Hasher\\SmallFile.txt", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                file.OverallState = ArchivialLibrary.Files.FileStatus.Unsynced;
                file.Priority = ArchivialLibrary.Files.FileBackupPriority.Low;
                file.CopyState = null;
                file.GenerateNextTransferPayload(filestream, new ArchivialLibrary.Crypto.Hasher(new ArchivialLibrary.Logging.Mock.MockLogger()));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BackupFileGenerateNextTransferPayloadThrowsOnInvalidFileState4()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new ArchivialLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            // copy state is inconsistent

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = 1, Name = nameof(StorageProviderTypes.AWS), Type = ProviderTypes.Storage }
            });

            file.CopyState[ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS].SyncStatus = ArchivialLibrary.Files.FileStatus.Synced;
            file.OverallState = ArchivialLibrary.Files.FileStatus.Unsynced;

            using (var filestream = new FileStream(".\\TestFiles\\Hasher\\SmallFile.txt", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                file.GenerateNextTransferPayload(filestream, new ArchivialLibrary.Crypto.Hasher(new ArchivialLibrary.Logging.Mock.MockLogger()));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload1()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var hasher = new ArchivialLibrary.Crypto.Hasher(new ArchivialLibrary.Logging.Mock.MockLogger());
            var file = new ArchivialLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = 1, Name = nameof(StorageProviderTypes.AWS), Type = ProviderTypes.Storage }
            });

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(5, payload.TotalBlocks);
                Assert.AreEqual(0, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationProviders);
                Assert.AreEqual(1, payload.DestinationProviders.Count);
                Assert.AreEqual(ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS, payload.DestinationProviders[0]);

                Assert.AreEqual(ArchivialLibrary.Constants.Transfers.TransferBlockSizeBytes, payload.Data.Length);

                // assert that the contents of the payload match.
                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(File.ReadAllBytes(".\\TestFiles\\TransferPayloads\\payload1.data"), payload.Data));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload2()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Medium };
            var hasher = new ArchivialLibrary.Crypto.Hasher(new ArchivialLibrary.Logging.Mock.MockLogger());
            var file = new ArchivialLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = 1, Name = nameof(StorageProviderTypes.AWS), Type = ProviderTypes.Storage }
            });

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(5, payload.TotalBlocks);
                Assert.AreEqual(0, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationProviders);
                Assert.AreEqual(1, payload.DestinationProviders.Count);
                Assert.AreEqual(ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS, payload.DestinationProviders[0]);

                Assert.AreEqual(ArchivialLibrary.Constants.Transfers.TransferBlockSizeBytes, payload.Data.Length);

                // assert that the contents of the payload match.
                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(File.ReadAllBytes(".\\TestFiles\\TransferPayloads\\payload2.data"), payload.Data));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload3()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Medium };
            var hasher = new ArchivialLibrary.Crypto.Hasher(new ArchivialLibrary.Logging.Mock.MockLogger());
            var file = new ArchivialLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            // generate a payload for the second block (index 1)

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = 1, Name = nameof(StorageProviderTypes.AWS), Type = ProviderTypes.Storage }
            });

            file.CopyState[ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS].LastCompletedFileBlockIndex = 0;
            file.CopyState[ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS].SyncStatus = ArchivialLibrary.Files.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(5, payload.TotalBlocks);
                Assert.AreEqual(1, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationProviders);
                Assert.AreEqual(1, payload.DestinationProviders.Count);
                Assert.AreEqual(ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS, payload.DestinationProviders[0]);

                Assert.AreEqual(ArchivialLibrary.Constants.Transfers.TransferBlockSizeBytes, payload.Data.Length);

                // assert that the contents of the payload match.
                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(File.ReadAllBytes(".\\TestFiles\\TransferPayloads\\payload3.data"), payload.Data));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload4()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var hasher = new ArchivialLibrary.Crypto.Hasher(new ArchivialLibrary.Logging.Mock.MockLogger());
            var file = new ArchivialLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            // generate a payload for a block in the middle (index 3)

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = 1, Name = nameof(StorageProviderTypes.AWS), Type = ProviderTypes.Storage }
            });

            file.CopyState[ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS].LastCompletedFileBlockIndex = 2;
            file.CopyState[ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS].SyncStatus = ArchivialLibrary.Files.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(5, payload.TotalBlocks);
                Assert.AreEqual(3, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationProviders);
                Assert.AreEqual(1, payload.DestinationProviders.Count);
                Assert.AreEqual(ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS, payload.DestinationProviders[0]);

                Assert.AreEqual(ArchivialLibrary.Constants.Transfers.TransferBlockSizeBytes, payload.Data.Length);

                // assert that the contents of the payload match.
                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(File.ReadAllBytes(".\\TestFiles\\TransferPayloads\\payload4.data"), payload.Data));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload5()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var hasher = new ArchivialLibrary.Crypto.Hasher(new ArchivialLibrary.Logging.Mock.MockLogger());
            var file = new ArchivialLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            // generate a payload for the final block (index 4)

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = 1, Name = nameof(StorageProviderTypes.AWS), Type = ProviderTypes.Storage }
            });

            file.CopyState[ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS].LastCompletedFileBlockIndex = 3;
            file.CopyState[ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS].SyncStatus = ArchivialLibrary.Files.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(5, payload.TotalBlocks);
                Assert.AreEqual(4, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationProviders);
                Assert.AreEqual(1, payload.DestinationProviders.Count);
                Assert.AreEqual(ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS, payload.DestinationProviders[0]);

                Assert.AreEqual(102809, payload.Data.Length); // partial chunk

                // assert that the contents of the payload match.
                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(File.ReadAllBytes(".\\TestFiles\\TransferPayloads\\payload5.data"), payload.Data));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload6()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var hasher = new ArchivialLibrary.Crypto.Hasher(new ArchivialLibrary.Logging.Mock.MockLogger());
            var file = new ArchivialLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            var providers = new ArchivialLibrary.StorageProviders.StorageProviderTypes[] { ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS, ArchivialLibrary.StorageProviders.StorageProviderTypes.Azure };

            // generate a payload for the final block (index 4) for multiple providers

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = (int)providers[0], Name = providers[0].ToString(), Type = ProviderTypes.Storage },
                new Provider() { ID = (int)providers[1], Name = providers[1].ToString(), Type = ProviderTypes.Storage }
            });

            file.CopyState[providers[0]].LastCompletedFileBlockIndex = 3;
            file.CopyState[providers[0]].SyncStatus = ArchivialLibrary.Files.FileStatus.InProgress;

            file.CopyState[providers[1]].LastCompletedFileBlockIndex = 3;
            file.CopyState[providers[1]].SyncStatus = ArchivialLibrary.Files.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(5, payload.TotalBlocks);
                Assert.AreEqual(4, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationProviders);
                Assert.AreEqual(2, payload.DestinationProviders.Count);
                Assert.AreEqual(providers[0], payload.DestinationProviders[0]);
                Assert.AreEqual(providers[1], payload.DestinationProviders[1]);

                Assert.AreEqual(102809, payload.Data.Length); // partial chunk

                // assert that the contents of the payload match.
                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(File.ReadAllBytes(".\\TestFiles\\TransferPayloads\\payload6.data"), payload.Data));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload7()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var hasher = new ArchivialLibrary.Crypto.Hasher(new ArchivialLibrary.Logging.Mock.MockLogger());
            var file = new ArchivialLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            var providers = new ArchivialLibrary.StorageProviders.StorageProviderTypes[] {
                ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS,
                ArchivialLibrary.StorageProviders.StorageProviderTypes.Azure,
                ArchivialLibrary.StorageProviders.StorageProviderTypes.Google
            };

            // generate a payload for a block in the middle (index 3) for multiple providers.

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = (int)providers[0], Name = providers[0].ToString(), Type = ProviderTypes.Storage },
                new Provider() { ID = (int)providers[1], Name = providers[1].ToString(), Type = ProviderTypes.Storage },
                new Provider() { ID = (int)providers[2], Name = providers[2].ToString(), Type = ProviderTypes.Storage }
            });

            file.CopyState[providers[0]].LastCompletedFileBlockIndex = 2;
            file.CopyState[providers[0]].SyncStatus = ArchivialLibrary.Files.FileStatus.InProgress;

            file.CopyState[providers[1]].LastCompletedFileBlockIndex = 2;
            file.CopyState[providers[1]].SyncStatus = ArchivialLibrary.Files.FileStatus.InProgress;

            file.CopyState[providers[2]].LastCompletedFileBlockIndex = 2;
            file.CopyState[providers[2]].SyncStatus = ArchivialLibrary.Files.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(5, payload.TotalBlocks);
                Assert.AreEqual(3, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationProviders);
                Assert.AreEqual(3, payload.DestinationProviders.Count);
                Assert.AreEqual(providers[0], payload.DestinationProviders[0]);
                Assert.AreEqual(providers[1], payload.DestinationProviders[1]);
                Assert.AreEqual(providers[2], payload.DestinationProviders[2]);

                Assert.AreEqual(ArchivialLibrary.Constants.Transfers.TransferBlockSizeBytes, payload.Data.Length);

                // assert that the contents of the payload match.
                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(File.ReadAllBytes(".\\TestFiles\\TransferPayloads\\payload7.data"), payload.Data));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload8()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var hasher = new ArchivialLibrary.Crypto.Hasher(new ArchivialLibrary.Logging.Mock.MockLogger());
            var file = new ArchivialLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            var providers = new ArchivialLibrary.StorageProviders.StorageProviderTypes[] {
                ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS,
                ArchivialLibrary.StorageProviders.StorageProviderTypes.Azure,
                ArchivialLibrary.StorageProviders.StorageProviderTypes.Google
            };

            // generate a payload for a block in the middle (index 3)
            // this file is already synced in the first provider, so only two destination providers should be returned.

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = (int)providers[0], Name = providers[0].ToString(), Type = ProviderTypes.Storage },
                new Provider() { ID = (int)providers[1], Name = providers[1].ToString(), Type = ProviderTypes.Storage },
                new Provider() { ID = (int)providers[2], Name = providers[2].ToString(), Type = ProviderTypes.Storage }
            });

            file.CopyState[providers[0]].LastCompletedFileBlockIndex = 4;
            file.CopyState[providers[0]].SyncStatus = ArchivialLibrary.Files.FileStatus.Synced;

            file.CopyState[providers[1]].LastCompletedFileBlockIndex = 2;
            file.CopyState[providers[1]].SyncStatus = ArchivialLibrary.Files.FileStatus.InProgress;

            file.CopyState[providers[2]].LastCompletedFileBlockIndex = 2;
            file.CopyState[providers[2]].SyncStatus = ArchivialLibrary.Files.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(5, payload.TotalBlocks);
                Assert.AreEqual(3, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationProviders);
                Assert.AreEqual(2, payload.DestinationProviders.Count);
                Assert.AreEqual(providers[1], payload.DestinationProviders[0]);
                Assert.AreEqual(providers[2], payload.DestinationProviders[1]);

                Assert.AreEqual(ArchivialLibrary.Constants.Transfers.TransferBlockSizeBytes, payload.Data.Length);

                // assert that the contents of the payload match.
                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(File.ReadAllBytes(".\\TestFiles\\TransferPayloads\\payload8.data"), payload.Data));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload9()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var hasher = new ArchivialLibrary.Crypto.Hasher(new ArchivialLibrary.Logging.Mock.MockLogger());
            var file = new ArchivialLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            var providers = new ArchivialLibrary.StorageProviders.StorageProviderTypes[] {
                ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS,
                ArchivialLibrary.StorageProviders.StorageProviderTypes.Azure,
                ArchivialLibrary.StorageProviders.StorageProviderTypes.Google
            };

            // generate a payload for a block in the middle (index 2)
            // this file is already synced in the first provider, and the second provider is further along.
            // thus only the third provider should be returned, as that is the next available block to send.

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = (int)providers[0], Name = providers[0].ToString(), Type = ProviderTypes.Storage },
                new Provider() { ID = (int)providers[1], Name = providers[1].ToString(), Type = ProviderTypes.Storage },
                new Provider() { ID = (int)providers[2], Name = providers[2].ToString(), Type = ProviderTypes.Storage }
            });

            file.CopyState[providers[0]].LastCompletedFileBlockIndex = 4;
            file.CopyState[providers[0]].SyncStatus = ArchivialLibrary.Files.FileStatus.Synced;

            file.CopyState[providers[1]].LastCompletedFileBlockIndex = 2;
            file.CopyState[providers[1]].SyncStatus = ArchivialLibrary.Files.FileStatus.InProgress;

            file.CopyState[providers[2]].LastCompletedFileBlockIndex = 1;
            file.CopyState[providers[2]].SyncStatus = ArchivialLibrary.Files.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(5, payload.TotalBlocks);
                Assert.AreEqual(2, payload.CurrentBlockNumber);

                // ensure we are taking the minimum block number that could be sent.

                Assert.IsNotNull(payload.DestinationProviders);
                Assert.AreEqual(1, payload.DestinationProviders.Count);
                Assert.AreEqual(providers[2], payload.DestinationProviders[0]);

                Assert.AreEqual(ArchivialLibrary.Constants.Transfers.TransferBlockSizeBytes, payload.Data.Length);

                // assert that the contents of the payload match.
                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(File.ReadAllBytes(".\\TestFiles\\TransferPayloads\\payload9.data"), payload.Data));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BackupFileSetBlockAsSentThrowsOnInvalidBlockNumber()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new ArchivialLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            file.SetBlockAsSent(-5, ArchivialLibrary.StorageProviders.StorageProviderTypes.Azure);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BackupFileSetBlockAsSentThrowsOnFileAlreadySynced()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new ArchivialLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);
            file.OverallState = ArchivialLibrary.Files.FileStatus.Synced;

            file.SetBlockAsSent(2, ArchivialLibrary.StorageProviders.StorageProviderTypes.Azure);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BackupFileSetBlockAsSentThrowsOnMissingCopyState()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new ArchivialLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);
            file.OverallState = ArchivialLibrary.Files.FileStatus.InProgress;

            file.SetBlockAsSent(2, ArchivialLibrary.StorageProviders.StorageProviderTypes.Azure);
        }

        [TestMethod]
        public void BackupFileSetBlockAsSentCorrectlySetsCopyStateAndOverallState1()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new ArchivialLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = 1, Name = nameof(StorageProviderTypes.AWS), Type = ProviderTypes.Storage }
            });

            Assert.AreEqual(ArchivialLibrary.Files.FileStatus.Unsynced, file.OverallState);

            // this example file has 5 total blocks

            file.SetBlockAsSent(0, ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS);

            Assert.AreEqual(ArchivialLibrary.Files.FileStatus.InProgress, file.OverallState);
            Assert.AreEqual(ArchivialLibrary.Files.FileStatus.InProgress, file.CopyState[ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS].SyncStatus);
        }

        [TestMethod]
        public void BackupFileSetBlockAsSentCorrectlySetsCopyStateAndOverallState2()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new ArchivialLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = 1, Name = nameof(StorageProviderTypes.AWS), Type = ProviderTypes.Storage }
            });

            Assert.AreEqual(ArchivialLibrary.Files.FileStatus.Unsynced, file.OverallState);

            // this example file has 5 total blocks

            file.SetBlockAsSent(1, ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS);

            Assert.AreEqual(ArchivialLibrary.Files.FileStatus.InProgress, file.OverallState);
            Assert.AreEqual(ArchivialLibrary.Files.FileStatus.InProgress, file.CopyState[ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS].SyncStatus);
        }

        [TestMethod]
        public void BackupFileSetBlockAsSentCorrectlySetsCopyStateAndOverallState3()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new ArchivialLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = 1, Name = nameof(StorageProviderTypes.AWS), Type = ProviderTypes.Storage }
            });

            Assert.AreEqual(ArchivialLibrary.Files.FileStatus.Unsynced, file.OverallState);

            // this example file has 5 total blocks

            file.SetBlockAsSent(3, ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS);

            Assert.AreEqual(ArchivialLibrary.Files.FileStatus.InProgress, file.OverallState);
            Assert.AreEqual(ArchivialLibrary.Files.FileStatus.InProgress, file.CopyState[ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS].SyncStatus);
        }

        [TestMethod]
        public void BackupFileSetBlockAsSentCorrectlySetsCopyStateAndOverallState4()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new ArchivialLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = 1, Name = nameof(StorageProviderTypes.AWS), Type = ProviderTypes.Storage }
            });

            Assert.AreEqual(ArchivialLibrary.Files.FileStatus.Unsynced, file.OverallState);

            // this example file has 5 total blocks

            file.SetBlockAsSent(4, ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS);

            Assert.AreEqual(ArchivialLibrary.Files.FileStatus.Synced, file.OverallState);
            Assert.AreEqual(ArchivialLibrary.Files.FileStatus.Synced, file.CopyState[ArchivialLibrary.StorageProviders.StorageProviderTypes.AWS].SyncStatus);
        }

        [TestMethod]
        public void BackupFileGetRemoteFileNameReturnsCorrectlyFormattedNameForAzureProvider()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new ArchivialLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            // reset auto-generated id for stable test result.
            file.FileID = new Guid("387ef266-5635-4224-b8d3-980880ae1258");

            var remoteName = file.GetRemoteFileName(ArchivialLibrary.StorageProviders.StorageProviderTypes.Azure);
            var expected = "archivial-file-387ef266-5635-4224-b8d3-980880ae1258-v1.mp3";

            Assert.IsNotNull(remoteName);
            Assert.AreEqual(expected, remoteName);
        }

        [TestMethod]
        public void BackupFileGetRemoteFileNameReturnsCorrectlyFormattedExtensionlessNameForAzureProvider()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new ArchivialLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\AnExtensionlessFile"), source);

            // reset auto-generated id for stable test result.
            file.FileID = new Guid("387ef266-5635-4224-b8d3-980880ae1258");

            var remoteName = file.GetRemoteFileName(ArchivialLibrary.StorageProviders.StorageProviderTypes.Azure);
            var expected = "archivial-file-387ef266-5635-4224-b8d3-980880ae1258-v1";

            Assert.IsNotNull(remoteName);
            Assert.AreEqual(expected, remoteName);
        }

        [TestMethod]
        public void BackupFileGetRemoteFileNameWithLaterRevisionReturnsCorrectlyFormattedNameForAzureProvider()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new ArchivialLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            // reset auto-generated id for stable test result.
            file.FileID = new Guid("387ef266-5635-4224-b8d3-980880ae1258");

            // force set the version
            file.FileRevisionNumber = 4;

            var remoteName = file.GetRemoteFileName(ArchivialLibrary.StorageProviders.StorageProviderTypes.Azure);
            var expected = "archivial-file-387ef266-5635-4224-b8d3-980880ae1258-v4.mp3";

            Assert.IsNotNull(remoteName);
            Assert.AreEqual(expected, remoteName);
        }

        [TestMethod]
        public void BackupFileUpdateLocalStateIfRemoteStateDoesNotMatchCase1()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new BackupFile(new FileInfo(".\\TestFiles\\Hasher\\SmallFile.txt"), source);

            // scenario 1:
            // > local: unsynced
            // > remote: unsynced 
            // no changes.

            file.CopyState = new Dictionary<StorageProviderTypes, StorageProviderFileStatus>()
            {
                {
                    StorageProviderTypes.Azure,
                    new StorageProviderFileStatus()
                    {
                        Provider = StorageProviderTypes.Azure,
                        HydrationStatus = StorageProviderHydrationStatus.None,
                        LastCompletedFileBlockIndex = -1,
                        Metadata = null,
                        SyncStatus = FileStatus.Unsynced
                    }
                }
            };

            var remoteState = new StorageProviderFileStatus()
            {
                Provider = StorageProviderTypes.Azure,
                HydrationStatus = StorageProviderHydrationStatus.None,
                LastCompletedFileBlockIndex = -1,
                Metadata = null,
                SyncStatus = FileStatus.Unsynced
            };

            var hasChanged = file.UpdateLocalStateIfRemoteStateDoesNotMatch(remoteState);

            Assert.IsFalse(hasChanged);
        }

        [TestMethod]
        public void BackupFileUpdateLocalStateIfRemoteStateDoesNotMatchCase2()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new BackupFile(new FileInfo(".\\TestFiles\\Hasher\\SmallFile.txt"), source);

            // scenario 2:
            // > local: in-progress
            // > remote: in-progress
            // no changes.

            file.CopyState = new Dictionary<StorageProviderTypes, StorageProviderFileStatus>()
            {
                {
                    StorageProviderTypes.Azure,
                    new StorageProviderFileStatus()
                    {
                        Provider = StorageProviderTypes.Azure,
                        HydrationStatus = StorageProviderHydrationStatus.None,
                        LastCompletedFileBlockIndex = 1,
                        Metadata = new Dictionary<string,string>() { { ProviderMetadata.FileHashKeyName, "a1b2c3d4e5f6" } },
                        SyncStatus = FileStatus.InProgress
                    }
                }
            };

            var remoteState = new StorageProviderFileStatus()
            {
                Provider = StorageProviderTypes.Azure,
                HydrationStatus = StorageProviderHydrationStatus.None,
                LastCompletedFileBlockIndex = 1,
                Metadata = new Dictionary<string, string>() { { ProviderMetadata.FileHashKeyName, "a1b2c3d4e5f6" } },
                SyncStatus = FileStatus.InProgress
            };

            var hasChanged = file.UpdateLocalStateIfRemoteStateDoesNotMatch(remoteState);

            Assert.IsFalse(hasChanged);
        }

        [TestMethod]
        public void BackupFileUpdateLocalStateIfRemoteStateDoesNotMatchCase3()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new BackupFile(new FileInfo(".\\TestFiles\\Hasher\\SmallFile.txt"), source);

            // scenario 3:
            // > local: synced
            // > remote: synced
            // no changes.

            file.CopyState = new Dictionary<StorageProviderTypes, StorageProviderFileStatus>()
            {
                {
                    StorageProviderTypes.Azure,
                    new StorageProviderFileStatus()
                    {
                        Provider = StorageProviderTypes.Azure,
                        HydrationStatus = StorageProviderHydrationStatus.None,
                        LastCompletedFileBlockIndex = 1,
                        Metadata = new Dictionary<string,string>() { { ProviderMetadata.FileHashKeyName, "a1b2c3d4e5f6" } },
                        SyncStatus = FileStatus.Synced
                    }
                }
            };

            var remoteState = new StorageProviderFileStatus()
            {
                Provider = StorageProviderTypes.Azure,
                HydrationStatus = StorageProviderHydrationStatus.None,
                LastCompletedFileBlockIndex = 1,
                Metadata = new Dictionary<string, string>() { { ProviderMetadata.FileHashKeyName, "a1b2c3d4e5f6" } },
                SyncStatus = FileStatus.Synced
            };

            var hasChanged = file.UpdateLocalStateIfRemoteStateDoesNotMatch(remoteState);

            Assert.IsFalse(hasChanged);
        }

        [TestMethod]
        public void BackupFileUpdateLocalStateIfRemoteStateDoesNotMatchCase4()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new BackupFile(new FileInfo(".\\TestFiles\\Hasher\\SmallFile.txt"), source);

            // scenario 4:
            // > local: unsynced
            // > remote: in-progress
            // any remote changes should take precedence.

            file.CopyState = new Dictionary<StorageProviderTypes, StorageProviderFileStatus>()
            {
                {
                    StorageProviderTypes.Azure,
                    new StorageProviderFileStatus()
                    {
                        Provider = StorageProviderTypes.Azure,
                        HydrationStatus = StorageProviderHydrationStatus.None,
                        LastCompletedFileBlockIndex = -1,
                        Metadata = null,
                        SyncStatus = FileStatus.Unsynced
                    }
                }
            };

            var remoteState = new StorageProviderFileStatus()
            {
                Provider = StorageProviderTypes.Azure,
                HydrationStatus = StorageProviderHydrationStatus.None,
                LastCompletedFileBlockIndex = 1,
                Metadata = new Dictionary<string, string>() { { ProviderMetadata.FileHashKeyName, "a1b2c3d4e5f6" } },
                SyncStatus = FileStatus.InProgress
            };

            var hasChanged = file.UpdateLocalStateIfRemoteStateDoesNotMatch(remoteState);

            Assert.IsTrue(hasChanged);
            Assert.AreEqual(FileStatus.InProgress, file.CopyState[StorageProviderTypes.Azure].SyncStatus);
            Assert.AreEqual(1, file.CopyState[StorageProviderTypes.Azure].LastCompletedFileBlockIndex);
        }

        [TestMethod]
        public void BackupFileUpdateLocalStateIfRemoteStateDoesNotMatchCase5()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new BackupFile(new FileInfo(".\\TestFiles\\Hasher\\SmallFile.txt"), source);

            // scenario 5:
            // > local: unsynced
            // > remote: synced
            // any remote changes should take precedence.

            file.CopyState = new Dictionary<StorageProviderTypes, StorageProviderFileStatus>()
            {
                {
                    StorageProviderTypes.Azure,
                    new StorageProviderFileStatus()
                    {
                        Provider = StorageProviderTypes.Azure,
                        HydrationStatus = StorageProviderHydrationStatus.None,
                        LastCompletedFileBlockIndex = -1,
                        Metadata = null,
                        SyncStatus = FileStatus.Unsynced
                    }
                }
            };

            var remoteState = new StorageProviderFileStatus()
            {
                Provider = StorageProviderTypes.Azure,
                HydrationStatus = StorageProviderHydrationStatus.None,
                LastCompletedFileBlockIndex = 1,
                Metadata = new Dictionary<string, string>() { { ProviderMetadata.FileHashKeyName, "a1b2c3d4e5f6" } },
                SyncStatus = FileStatus.Synced
            };

            var hasChanged = file.UpdateLocalStateIfRemoteStateDoesNotMatch(remoteState);

            Assert.IsTrue(hasChanged);
            Assert.AreEqual(FileStatus.Synced, file.CopyState[StorageProviderTypes.Azure].SyncStatus);
            Assert.AreEqual(1, file.CopyState[StorageProviderTypes.Azure].LastCompletedFileBlockIndex);
        }

        [TestMethod]
        public void BackupFileUpdateLocalStateIfRemoteStateDoesNotMatchCase6()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new BackupFile(new FileInfo(".\\TestFiles\\Hasher\\SmallFile.txt"), source);

            // scenario 6:
            // > local: in-progress
            // > remote: unsynced
            // any remote changes should take precedence.

            file.CopyState = new Dictionary<StorageProviderTypes, StorageProviderFileStatus>()
            {
                {
                    StorageProviderTypes.Azure,
                    new StorageProviderFileStatus()
                    {
                        Provider = StorageProviderTypes.Azure,
                        HydrationStatus = StorageProviderHydrationStatus.None,
                        LastCompletedFileBlockIndex = 1,
                        Metadata = new Dictionary<string, string>() { { ProviderMetadata.FileHashKeyName, "a1b2c3d4e5f6" } },
                        SyncStatus = FileStatus.InProgress
                    }
                }
            };

            var remoteState = new StorageProviderFileStatus()
            {
                Provider = StorageProviderTypes.Azure,
                HydrationStatus = StorageProviderHydrationStatus.None,
                LastCompletedFileBlockIndex = -1,
                Metadata = null,
                SyncStatus = FileStatus.Unsynced
            };

            var hasChanged = file.UpdateLocalStateIfRemoteStateDoesNotMatch(remoteState);

            Assert.IsTrue(hasChanged);
            Assert.AreEqual(FileStatus.Unsynced, file.CopyState[StorageProviderTypes.Azure].SyncStatus);
            Assert.AreEqual(-1, file.CopyState[StorageProviderTypes.Azure].LastCompletedFileBlockIndex);
        }

        [TestMethod]
        public void BackupFileUpdateLocalStateIfRemoteStateDoesNotMatchCase7()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new BackupFile(new FileInfo(".\\TestFiles\\Hasher\\SmallFile.txt"), source);

            // scenario 7:
            // > local: in-progress
            // > remote: in-progress (different state)
            // any remote changes should take precedence.

            file.CopyState = new Dictionary<StorageProviderTypes, StorageProviderFileStatus>()
            {
                {
                    StorageProviderTypes.Azure,
                    new StorageProviderFileStatus()
                    {
                        Provider = StorageProviderTypes.Azure,
                        HydrationStatus = StorageProviderHydrationStatus.None,
                        LastCompletedFileBlockIndex = 1,
                        Metadata = new Dictionary<string, string>() { { ProviderMetadata.FileHashKeyName, "a1b2c3d4e5f6" } },
                        SyncStatus = FileStatus.InProgress
                    }
                }
            };

            var remoteState = new StorageProviderFileStatus()
            {
                Provider = StorageProviderTypes.Azure,
                HydrationStatus = StorageProviderHydrationStatus.None,
                LastCompletedFileBlockIndex = 2,
                Metadata = new Dictionary<string, string>() { { ProviderMetadata.FileHashKeyName, "a1b2c3d4e5f6" } },
                SyncStatus = FileStatus.InProgress
            };

            var hasChanged = file.UpdateLocalStateIfRemoteStateDoesNotMatch(remoteState);

            Assert.IsTrue(hasChanged);
            Assert.AreEqual(FileStatus.InProgress, file.CopyState[StorageProviderTypes.Azure].SyncStatus);
            Assert.AreEqual(2, file.CopyState[StorageProviderTypes.Azure].LastCompletedFileBlockIndex);
        }

        [TestMethod]
        public void BackupFileUpdateLocalStateIfRemoteStateDoesNotMatchCase8()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new BackupFile(new FileInfo(".\\TestFiles\\Hasher\\SmallFile.txt"), source);

            // scenario 8:
            // > local: in-progress
            // > remote: synced
            // any remote changes should take precedence.

            file.CopyState = new Dictionary<StorageProviderTypes, StorageProviderFileStatus>()
            {
                {
                    StorageProviderTypes.Azure,
                    new StorageProviderFileStatus()
                    {
                        Provider = StorageProviderTypes.Azure,
                        HydrationStatus = StorageProviderHydrationStatus.None,
                        LastCompletedFileBlockIndex = 1,
                        Metadata = new Dictionary<string, string>() { { ProviderMetadata.FileHashKeyName, "a1b2c3d4e5f6" } },
                        SyncStatus = FileStatus.InProgress
                    }
                }
            };

            var remoteState = new StorageProviderFileStatus()
            {
                Provider = StorageProviderTypes.Azure,
                HydrationStatus = StorageProviderHydrationStatus.None,
                LastCompletedFileBlockIndex = 3,
                Metadata = new Dictionary<string, string>() { { ProviderMetadata.FileHashKeyName, "a1b2c3d4e5f6" } },
                SyncStatus = FileStatus.Synced
            };

            var hasChanged = file.UpdateLocalStateIfRemoteStateDoesNotMatch(remoteState);

            Assert.IsTrue(hasChanged);
            Assert.AreEqual(FileStatus.Synced, file.CopyState[StorageProviderTypes.Azure].SyncStatus);
            Assert.AreEqual(3, file.CopyState[StorageProviderTypes.Azure].LastCompletedFileBlockIndex);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BackupFileSetFileHashWithAlgorithmThrowsIfHashIsMissing()
        {
            var backupFile = new BackupFile();
            backupFile.SetFileHashWithAlgorithm(null, HashAlgorithmName.SHA1);
        }

        [TestMethod]
        public void BackupFileSetFileHashWithAlgorithmCorrectlyAssignsFileHashBytes()
        {
            var hasher = new Hasher(new MockLogger());
            var backupFile = new BackupFile();

            var hash = hasher.HashFileBlockFromByteArray(HashAlgorithmName.SHA256, LargeByteStreamConstants.LargeByteStream);

            backupFile.SetFileHashWithAlgorithm(hash, HashAlgorithmName.SHA256);

            Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(hash, backupFile.FileHash));
        }

        [TestMethod]
        public void BackupFileSetFileHashWithAlgorithmCorrectlyAssignsFileHashAlgorithmType()
        {
            var hasher = new Hasher(new MockLogger());
            var backupFile = new BackupFile();

            var hash = hasher.HashFileBlockFromByteArray(HashAlgorithmName.SHA256, LargeByteStreamConstants.LargeByteStream);

            backupFile.SetFileHashWithAlgorithm(hash, HashAlgorithmName.SHA256);

            Assert.AreEqual(HashAlgorithmName.SHA256.Name, backupFile.HashAlgorithmType);
        }

        [TestMethod]
        public void BackupFileSetFileHashWithAlgorithmCorrectlyAssignsFileHashHexStringSha1()
        {
            var hasher = new Hasher(new MockLogger());
            var backupFile = new BackupFile();

            var hash = hasher.HashFileBlockFromByteArray(HashAlgorithmName.SHA1, LargeByteStreamConstants.LargeByteStream);

            backupFile.SetFileHashWithAlgorithm(hash, HashAlgorithmName.SHA1);

            Assert.AreEqual("BE648B1787644E6B9DEA26DF9D75AD96728580CA", backupFile.FileHashString);
        }

        [TestMethod]
        public void BackupFileSetFileHashWithAlgorithmCorrectlyAssignsFileHashHexStringSha256()
        {
            var hasher = new Hasher(new MockLogger());
            var backupFile = new BackupFile();

            var hash = hasher.HashFileBlockFromByteArray(HashAlgorithmName.SHA256, LargeByteStreamConstants.LargeByteStream);

            backupFile.SetFileHashWithAlgorithm(hash, HashAlgorithmName.SHA256);

            Assert.AreEqual("F30ACBEE04ECBC2CC0217958D81C4526940A329495D562AC320BA29C0076F65C", backupFile.FileHashString);
        }

        [TestMethod]
        public void BackupFileSetFileHashWithAlgorithmCorrectlyAssignsFileHashHexStringSha512()
        {
            var hasher = new Hasher(new MockLogger());
            var backupFile = new BackupFile();

            var hash = hasher.HashFileBlockFromByteArray(HashAlgorithmName.SHA512, LargeByteStreamConstants.LargeByteStream);

            backupFile.SetFileHashWithAlgorithm(hash, HashAlgorithmName.SHA512);

            Assert.AreEqual("4A1BD3D22C54DA83212632A1120A435D8B5FEE099F9D544E08D605C34B85F0B09148DE3975C7D66509DFB7D3F209ADBB4B1B9B61C48C6C3699B65B88CB235567", backupFile.FileHashString);
        }
    }
}
