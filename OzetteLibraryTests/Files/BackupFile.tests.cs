using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzetteLibrary.Exceptions;
using OzetteLibrary.Files;
using OzetteLibrary.Folders;
using OzetteLibrary.Providers;
using OzetteLibrary.StorageProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OzetteLibraryTests.Files
{
    [TestClass]
    public class BackupFileTests
    {
        [TestMethod]
        public void BackupFileGetTotalFileBlocksHandlesZeroFileLength()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 0;
            Assert.AreEqual(0, file.CalculateTotalFileBlocks(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BackupFileGetTotalFileBlocksThrowsWhenProvidedZeroBlockSize()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 1048576; // 1 MB
            file.CalculateTotalFileBlocks(0);
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks1()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 2097152; // 2 MB
            Assert.AreEqual(1, file.CalculateTotalFileBlocks(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks2()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 4194304; // 4 MB
            Assert.AreEqual(2, file.CalculateTotalFileBlocks(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks3()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 8388608; // 8 MB
            Assert.AreEqual(4, file.CalculateTotalFileBlocks(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks4()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 310378496; // 310 MB
            Assert.AreEqual(148, file.CalculateTotalFileBlocks(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks5()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 2097151; // just under 2 MB
            Assert.AreEqual(1, file.CalculateTotalFileBlocks(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks7()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 2097153; // just over 2 MB
            Assert.AreEqual(2, file.CalculateTotalFileBlocks(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks8()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 2619922; // over 2 MB, with a halfway populated 2nd block.
            Assert.AreEqual(2, file.CalculateTotalFileBlocks(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks9()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 4194301; // just under 4 MB
            Assert.AreEqual(2, file.CalculateTotalFileBlocks(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks10()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 4194308; // just over 4 MB
            Assert.AreEqual(3, file.CalculateTotalFileBlocks(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks11()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 189643547895; // very large file size
            Assert.AreEqual(90430, file.CalculateTotalFileBlocks(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks12()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 43414941001; // very large file size
            Assert.AreEqual(20702, file.CalculateTotalFileBlocks(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks13()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 7744931200569; // very large file size
            Assert.AreEqual(167955484, file.CalculateTotalFileBlocks(46113)); // unusual block size
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks14()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 1647999899992; // very large file size
            Assert.AreEqual(228539718, file.CalculateTotalFileBlocks(7211)); // unusual block size
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks15()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 15677400000;
            Assert.AreEqual(154, file.CalculateTotalFileBlocks(102400000));
        }

        [TestMethod]
        public void BackupFileHasDataToTransferReturnsValidFalseExample1()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.OverallState = OzetteLibrary.Files.FileStatus.Synced;

            Assert.IsFalse(file.HasDataToTransfer());
        }

        [TestMethod]
        public void BackupFileHasDataToTransferReturnsValidTrueExample1()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.OverallState = OzetteLibrary.Files.FileStatus.OutOfDate;

            Assert.IsTrue(file.HasDataToTransfer());
        }

        [TestMethod]
        public void BackupFileHasDataToTransferReturnsValidTrueExample2()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.OverallState = OzetteLibrary.Files.FileStatus.Unsynced;

            Assert.IsTrue(file.HasDataToTransfer());
        }

        [TestMethod]
        public void BackupFileHasDataToTransferReturnsValidTrueExample3()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.OverallState = OzetteLibrary.Files.FileStatus.InProgress;

            Assert.IsTrue(file.HasDataToTransfer());
        }

        [TestMethod]
        public void BackupFileResetCopyStateCorrectlyResetsState1()
        {
            var provider1 = OzetteLibrary.StorageProviders.StorageProviderTypes.AWS;
            var provider2 = OzetteLibrary.StorageProviders.StorageProviderTypes.Azure;

            var file = new OzetteLibrary.Files.BackupFile();

            file.CopyState = new System.Collections.Generic.Dictionary
                <OzetteLibrary.StorageProviders.StorageProviderTypes,
                OzetteLibrary.StorageProviders.StorageProviderFileStatus>();

            file.CopyState.Add(
                provider1,
                new OzetteLibrary.StorageProviders.StorageProviderFileStatus()
                {
                    Provider = provider1,
                    SyncStatus = OzetteLibrary.Files.FileStatus.InProgress
                });

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = (int)provider1, Name = provider1.ToString(), Type = ProviderTypes.Storage },
                new Provider() { ID = (int)provider2, Name = provider2.ToString(), Type = ProviderTypes.Storage }
            });

            Assert.AreEqual(2, file.CopyState.Count);
            Assert.AreEqual(OzetteLibrary.Files.FileStatus.Unsynced, file.CopyState[provider1].SyncStatus);
            Assert.AreEqual(OzetteLibrary.Files.FileStatus.Unsynced, file.CopyState[provider2].SyncStatus);
        }

        [TestMethod]
        public void BackupFileResetCopyStateCorrectlyResetsState2()
        {
            var provider1 = OzetteLibrary.StorageProviders.StorageProviderTypes.AWS;
            var provider2 = OzetteLibrary.StorageProviders.StorageProviderTypes.Azure;

            var file = new OzetteLibrary.Files.BackupFile();

            file.CopyState = new Dictionary
                <OzetteLibrary.StorageProviders.StorageProviderTypes,
                OzetteLibrary.StorageProviders.StorageProviderFileStatus>();

            file.CopyState.Add(
                provider1,
                new OzetteLibrary.StorageProviders.StorageProviderFileStatus()
                {
                    Provider = provider1,
                    SyncStatus = OzetteLibrary.Files.FileStatus.Synced
                });

            file.CopyState.Add(
                provider2,
                new OzetteLibrary.StorageProviders.StorageProviderFileStatus()
                {
                    Provider = provider2,
                    SyncStatus = OzetteLibrary.Files.FileStatus.Synced
                });

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = (int)provider1, Name = provider1.ToString(), Type = ProviderTypes.Storage }
            });

            Assert.AreEqual(1, file.CopyState.Count);
            Assert.AreEqual(OzetteLibrary.Files.FileStatus.Unsynced, file.CopyState[provider1].SyncStatus);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BackupFileGenerateNextTransferPayloadThrowsOnInvalidArgs1()
        {
            // no filestream
            var file = new OzetteLibrary.Files.BackupFile();
            file.GenerateNextTransferPayload(null, new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BackupFileGenerateNextTransferPayloadThrowsOnInvalidArgs2()
        {
            var file = new OzetteLibrary.Files.BackupFile();

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
            var file = new OzetteLibrary.Files.BackupFile();

            // file is already synced
            using (var filestream = new FileStream(".\\TestFiles\\Hasher\\SmallFile.txt", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                file.OverallState = OzetteLibrary.Files.FileStatus.Synced;
                file.GenerateNextTransferPayload(filestream, new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger()));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BackupFileGenerateNextTransferPayloadThrowsOnInvalidFileState2()
        {
            var file = new OzetteLibrary.Files.BackupFile();

            // file priority is not set
            using (var filestream = new FileStream(".\\TestFiles\\Hasher\\SmallFile.txt", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                file.OverallState = OzetteLibrary.Files.FileStatus.Unsynced;
                file.Priority = OzetteLibrary.Files.FileBackupPriority.Unset;
                file.GenerateNextTransferPayload(filestream, new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger()));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BackupFileGenerateNextTransferPayloadThrowsOnInvalidFileState3()
        {
            var file = new OzetteLibrary.Files.BackupFile();

            // copystate is not set
            using (var filestream = new FileStream(".\\TestFiles\\Hasher\\SmallFile.txt", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                file.OverallState = OzetteLibrary.Files.FileStatus.Unsynced;
                file.Priority = OzetteLibrary.Files.FileBackupPriority.Low;
                file.CopyState = null;
                file.GenerateNextTransferPayload(filestream, new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger()));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BackupFileGenerateNextTransferPayloadThrowsOnInvalidFileState4()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            // copy state is inconsistent

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = 1, Name = nameof(StorageProviderTypes.AWS), Type = ProviderTypes.Storage }
            });

            file.CopyState[OzetteLibrary.StorageProviders.StorageProviderTypes.AWS].SyncStatus = OzetteLibrary.Files.FileStatus.Synced;
            file.OverallState = OzetteLibrary.Files.FileStatus.Unsynced;

            using (var filestream = new FileStream(".\\TestFiles\\Hasher\\SmallFile.txt", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                file.GenerateNextTransferPayload(filestream, new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger()));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload1()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

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
                Assert.AreEqual(OzetteLibrary.StorageProviders.StorageProviderTypes.AWS, payload.DestinationProviders[0]);

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes, payload.Data.Length);

                // assert that the contents of the payload match.
                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(File.ReadAllBytes(".\\TestFiles\\TransferPayloads\\payload1.data"), payload.Data));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload2()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Medium };
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

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
                Assert.AreEqual(OzetteLibrary.StorageProviders.StorageProviderTypes.AWS, payload.DestinationProviders[0]);

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes, payload.Data.Length);

                // assert that the contents of the payload match.
                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(File.ReadAllBytes(".\\TestFiles\\TransferPayloads\\payload2.data"), payload.Data));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload3()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Medium };
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            // generate a payload for the second block (index 1)

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = 1, Name = nameof(StorageProviderTypes.AWS), Type = ProviderTypes.Storage }
            });

            file.CopyState[OzetteLibrary.StorageProviders.StorageProviderTypes.AWS].LastCompletedFileBlockIndex = 0;
            file.CopyState[OzetteLibrary.StorageProviders.StorageProviderTypes.AWS].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(5, payload.TotalBlocks);
                Assert.AreEqual(1, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationProviders);
                Assert.AreEqual(1, payload.DestinationProviders.Count);
                Assert.AreEqual(OzetteLibrary.StorageProviders.StorageProviderTypes.AWS, payload.DestinationProviders[0]);

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes, payload.Data.Length);

                // assert that the contents of the payload match.
                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(File.ReadAllBytes(".\\TestFiles\\TransferPayloads\\payload3.data"), payload.Data));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload4()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            // generate a payload for a block in the middle (index 3)

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = 1, Name = nameof(StorageProviderTypes.AWS), Type = ProviderTypes.Storage }
            });

            file.CopyState[OzetteLibrary.StorageProviders.StorageProviderTypes.AWS].LastCompletedFileBlockIndex = 2;
            file.CopyState[OzetteLibrary.StorageProviders.StorageProviderTypes.AWS].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(5, payload.TotalBlocks);
                Assert.AreEqual(3, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationProviders);
                Assert.AreEqual(1, payload.DestinationProviders.Count);
                Assert.AreEqual(OzetteLibrary.StorageProviders.StorageProviderTypes.AWS, payload.DestinationProviders[0]);

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes, payload.Data.Length);

                // assert that the contents of the payload match.
                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(File.ReadAllBytes(".\\TestFiles\\TransferPayloads\\payload4.data"), payload.Data));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload5()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            // generate a payload for the final block (index 4)

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = 1, Name = nameof(StorageProviderTypes.AWS), Type = ProviderTypes.Storage }
            });

            file.CopyState[OzetteLibrary.StorageProviders.StorageProviderTypes.AWS].LastCompletedFileBlockIndex = 3;
            file.CopyState[OzetteLibrary.StorageProviders.StorageProviderTypes.AWS].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(5, payload.TotalBlocks);
                Assert.AreEqual(4, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationProviders);
                Assert.AreEqual(1, payload.DestinationProviders.Count);
                Assert.AreEqual(OzetteLibrary.StorageProviders.StorageProviderTypes.AWS, payload.DestinationProviders[0]);

                Assert.AreEqual(102809, payload.Data.Length); // partial chunk

                // assert that the contents of the payload match.
                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(File.ReadAllBytes(".\\TestFiles\\TransferPayloads\\payload5.data"), payload.Data));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload6()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            var providers = new OzetteLibrary.StorageProviders.StorageProviderTypes[] { OzetteLibrary.StorageProviders.StorageProviderTypes.AWS, OzetteLibrary.StorageProviders.StorageProviderTypes.Azure };

            // generate a payload for the final block (index 4) for multiple providers

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = (int)providers[0], Name = providers[0].ToString(), Type = ProviderTypes.Storage },
                new Provider() { ID = (int)providers[1], Name = providers[1].ToString(), Type = ProviderTypes.Storage }
            });

            file.CopyState[providers[0]].LastCompletedFileBlockIndex = 3;
            file.CopyState[providers[0]].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

            file.CopyState[providers[1]].LastCompletedFileBlockIndex = 3;
            file.CopyState[providers[1]].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

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
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            var providers = new OzetteLibrary.StorageProviders.StorageProviderTypes[] {
                OzetteLibrary.StorageProviders.StorageProviderTypes.AWS,
                OzetteLibrary.StorageProviders.StorageProviderTypes.Azure,
                OzetteLibrary.StorageProviders.StorageProviderTypes.Google
            };

            // generate a payload for a block in the middle (index 3) for multiple providers.

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = (int)providers[0], Name = providers[0].ToString(), Type = ProviderTypes.Storage },
                new Provider() { ID = (int)providers[1], Name = providers[1].ToString(), Type = ProviderTypes.Storage },
                new Provider() { ID = (int)providers[2], Name = providers[2].ToString(), Type = ProviderTypes.Storage }
            });

            file.CopyState[providers[0]].LastCompletedFileBlockIndex = 2;
            file.CopyState[providers[0]].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

            file.CopyState[providers[1]].LastCompletedFileBlockIndex = 2;
            file.CopyState[providers[1]].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

            file.CopyState[providers[2]].LastCompletedFileBlockIndex = 2;
            file.CopyState[providers[2]].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

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

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes, payload.Data.Length);

                // assert that the contents of the payload match.
                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(File.ReadAllBytes(".\\TestFiles\\TransferPayloads\\payload7.data"), payload.Data));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload8()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            var providers = new OzetteLibrary.StorageProviders.StorageProviderTypes[] {
                OzetteLibrary.StorageProviders.StorageProviderTypes.AWS,
                OzetteLibrary.StorageProviders.StorageProviderTypes.Azure,
                OzetteLibrary.StorageProviders.StorageProviderTypes.Google
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
            file.CopyState[providers[0]].SyncStatus = OzetteLibrary.Files.FileStatus.Synced;

            file.CopyState[providers[1]].LastCompletedFileBlockIndex = 2;
            file.CopyState[providers[1]].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

            file.CopyState[providers[2]].LastCompletedFileBlockIndex = 2;
            file.CopyState[providers[2]].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

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

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes, payload.Data.Length);

                // assert that the contents of the payload match.
                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(File.ReadAllBytes(".\\TestFiles\\TransferPayloads\\payload8.data"), payload.Data));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload9()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            var providers = new OzetteLibrary.StorageProviders.StorageProviderTypes[] {
                OzetteLibrary.StorageProviders.StorageProviderTypes.AWS,
                OzetteLibrary.StorageProviders.StorageProviderTypes.Azure,
                OzetteLibrary.StorageProviders.StorageProviderTypes.Google
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
            file.CopyState[providers[0]].SyncStatus = OzetteLibrary.Files.FileStatus.Synced;

            file.CopyState[providers[1]].LastCompletedFileBlockIndex = 2;
            file.CopyState[providers[1]].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

            file.CopyState[providers[2]].LastCompletedFileBlockIndex = 1;
            file.CopyState[providers[2]].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

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

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes, payload.Data.Length);

                // assert that the contents of the payload match.
                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(File.ReadAllBytes(".\\TestFiles\\TransferPayloads\\payload9.data"), payload.Data));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BackupFileSetBlockAsSentThrowsOnInvalidBlockNumber()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            file.SetBlockAsSent(-5, OzetteLibrary.StorageProviders.StorageProviderTypes.Azure);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BackupFileSetBlockAsSentThrowsOnFileAlreadySynced()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);
            file.OverallState = OzetteLibrary.Files.FileStatus.Synced;

            file.SetBlockAsSent(2, OzetteLibrary.StorageProviders.StorageProviderTypes.Azure);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BackupFileSetBlockAsSentThrowsOnMissingCopyState()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);
            file.OverallState = OzetteLibrary.Files.FileStatus.InProgress;

            file.SetBlockAsSent(2, OzetteLibrary.StorageProviders.StorageProviderTypes.Azure);
        }

        [TestMethod]
        public void BackupFileSetBlockAsSentCorrectlySetsCopyStateAndOverallState1()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = 1, Name = nameof(StorageProviderTypes.AWS), Type = ProviderTypes.Storage }
            });

            Assert.AreEqual(OzetteLibrary.Files.FileStatus.Unsynced, file.OverallState);

            // this example file has 5 total blocks

            file.SetBlockAsSent(0, OzetteLibrary.StorageProviders.StorageProviderTypes.AWS);

            Assert.AreEqual(OzetteLibrary.Files.FileStatus.InProgress, file.OverallState);
            Assert.AreEqual(OzetteLibrary.Files.FileStatus.InProgress, file.CopyState[OzetteLibrary.StorageProviders.StorageProviderTypes.AWS].SyncStatus);
        }

        [TestMethod]
        public void BackupFileSetBlockAsSentCorrectlySetsCopyStateAndOverallState2()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = 1, Name = nameof(StorageProviderTypes.AWS), Type = ProviderTypes.Storage }
            });

            Assert.AreEqual(OzetteLibrary.Files.FileStatus.Unsynced, file.OverallState);

            // this example file has 5 total blocks

            file.SetBlockAsSent(1, OzetteLibrary.StorageProviders.StorageProviderTypes.AWS);

            Assert.AreEqual(OzetteLibrary.Files.FileStatus.InProgress, file.OverallState);
            Assert.AreEqual(OzetteLibrary.Files.FileStatus.InProgress, file.CopyState[OzetteLibrary.StorageProviders.StorageProviderTypes.AWS].SyncStatus);
        }

        [TestMethod]
        public void BackupFileSetBlockAsSentCorrectlySetsCopyStateAndOverallState3()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = 1, Name = nameof(StorageProviderTypes.AWS), Type = ProviderTypes.Storage }
            });

            Assert.AreEqual(OzetteLibrary.Files.FileStatus.Unsynced, file.OverallState);

            // this example file has 5 total blocks

            file.SetBlockAsSent(3, OzetteLibrary.StorageProviders.StorageProviderTypes.AWS);

            Assert.AreEqual(OzetteLibrary.Files.FileStatus.InProgress, file.OverallState);
            Assert.AreEqual(OzetteLibrary.Files.FileStatus.InProgress, file.CopyState[OzetteLibrary.StorageProviders.StorageProviderTypes.AWS].SyncStatus);
        }

        [TestMethod]
        public void BackupFileSetBlockAsSentCorrectlySetsCopyStateAndOverallState4()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            file.ResetCopyState(new ProviderCollection()
            {
                new Provider() { ID = 1, Name = nameof(StorageProviderTypes.AWS), Type = ProviderTypes.Storage }
            });

            Assert.AreEqual(OzetteLibrary.Files.FileStatus.Unsynced, file.OverallState);

            // this example file has 5 total blocks

            file.SetBlockAsSent(4, OzetteLibrary.StorageProviders.StorageProviderTypes.AWS);

            Assert.AreEqual(OzetteLibrary.Files.FileStatus.Synced, file.OverallState);
            Assert.AreEqual(OzetteLibrary.Files.FileStatus.Synced, file.CopyState[OzetteLibrary.StorageProviders.StorageProviderTypes.AWS].SyncStatus);
        }

        [TestMethod]
        public void BackupFileGetRemoteFileNameReturnsCorrectlyFormattedNameForAzureProvider()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            // reset auto-generated id for stable test result.
            file.FileID = new Guid("387ef266-5635-4224-b8d3-980880ae1258");

            var remoteName = file.GetRemoteFileName(OzetteLibrary.StorageProviders.StorageProviderTypes.Azure);
            var expected = "ozette-file-387ef266-5635-4224-b8d3-980880ae1258-v1.mp3";

            Assert.IsNotNull(remoteName);
            Assert.AreEqual(expected, remoteName);
        }

        [TestMethod]
        public void BackupFileGetRemoteFileNameReturnsCorrectlyFormattedExtensionlessNameForAzureProvider()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\AnExtensionlessFile"), source);

            // reset auto-generated id for stable test result.
            file.FileID = new Guid("387ef266-5635-4224-b8d3-980880ae1258");

            var remoteName = file.GetRemoteFileName(OzetteLibrary.StorageProviders.StorageProviderTypes.Azure);
            var expected = "ozette-file-387ef266-5635-4224-b8d3-980880ae1258-v1";

            Assert.IsNotNull(remoteName);
            Assert.AreEqual(expected, remoteName);
        }

        [TestMethod]
        public void BackupFileGetRemoteFileNameWithLaterRevisionReturnsCorrectlyFormattedNameForAzureProvider()
        {
            var source = new LocalSourceLocation() { ID = 1, Priority = FileBackupPriority.Low };
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), source);

            // reset auto-generated id for stable test result.
            file.FileID = new Guid("387ef266-5635-4224-b8d3-980880ae1258");

            // force set the version
            file.FileRevisionNumber = 4;

            var remoteName = file.GetRemoteFileName(OzetteLibrary.StorageProviders.StorageProviderTypes.Azure);
            var expected = "ozette-file-387ef266-5635-4224-b8d3-980880ae1258-v4.mp3";

            Assert.IsNotNull(remoteName);
            Assert.AreEqual(expected, remoteName);
        }
    }
}
