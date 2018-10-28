using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

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
            var provider1 = OzetteLibrary.Providers.ProviderTypes.AWS;
            var provider2 = OzetteLibrary.Providers.ProviderTypes.Azure;

            var file = new OzetteLibrary.Files.BackupFile();

            file.CopyState = new System.Collections.Generic.Dictionary
                <OzetteLibrary.Providers.ProviderTypes,
                OzetteLibrary.Providers.ProviderFileStatus>();

            file.CopyState.Add(
                provider1,
                new OzetteLibrary.Providers.ProviderFileStatus()
                {
                    Provider = provider1,
                    SyncStatus = OzetteLibrary.Files.FileStatus.InProgress
                });

            file.ResetCopyState(
                new OzetteLibrary.Providers.ProvidersCollection()
                {
                    new OzetteLibrary.Providers.Provider() { Type = provider1, Enabled = true },
                    new OzetteLibrary.Providers.Provider() { Type = provider2, Enabled = true }
                });

            Assert.AreEqual(2, file.CopyState.Count);
            Assert.AreEqual(OzetteLibrary.Files.FileStatus.Unsynced, file.CopyState[provider1].SyncStatus);
            Assert.AreEqual(OzetteLibrary.Files.FileStatus.Unsynced, file.CopyState[provider2].SyncStatus);
        }

        [TestMethod]
        public void BackupFileResetCopyStateCorrectlyResetsState2()
        {
            var provider1 = OzetteLibrary.Providers.ProviderTypes.AWS;
            var provider2 = OzetteLibrary.Providers.ProviderTypes.Azure;

            var file = new OzetteLibrary.Files.BackupFile();

            file.CopyState = new Dictionary
                <OzetteLibrary.Providers.ProviderTypes,
                OzetteLibrary.Providers.ProviderFileStatus>();

            file.CopyState.Add(
                provider1,
                new OzetteLibrary.Providers.ProviderFileStatus()
                {
                    Provider = provider1,
                    SyncStatus = OzetteLibrary.Files.FileStatus.Synced
                });

            file.CopyState.Add(
                provider2,
                new OzetteLibrary.Providers.ProviderFileStatus()
                {
                    Provider = provider2,
                    SyncStatus = OzetteLibrary.Files.FileStatus.Synced
                });

            file.ResetCopyState(
                new OzetteLibrary.Providers.ProvidersCollection()
                {
                    new OzetteLibrary.Providers.Provider() { Type = provider1, Enabled = true }
                });

            Assert.AreEqual(1, file.CopyState.Count);
            Assert.AreEqual(OzetteLibrary.Files.FileStatus.Unsynced, file.CopyState[provider1].SyncStatus);
        }

        [TestMethod]
        public void BackupFileResetCopyStateCorrectlyResetsState3()
        {
            var provider1 = OzetteLibrary.Providers.ProviderTypes.AWS;
            var provider2 = OzetteLibrary.Providers.ProviderTypes.Azure;

            var file = new OzetteLibrary.Files.BackupFile();

            file.CopyState = new Dictionary
                <OzetteLibrary.Providers.ProviderTypes,
                OzetteLibrary.Providers.ProviderFileStatus>();

            file.CopyState.Add(
                provider1,
                new OzetteLibrary.Providers.ProviderFileStatus()
                {
                    Provider = provider1,
                    SyncStatus = OzetteLibrary.Files.FileStatus.InProgress
                });

            file.ResetCopyState(
                new OzetteLibrary.Providers.ProvidersCollection()
                {
                    // the difference here is that a disabled provider should not be registered in copystate

                    new OzetteLibrary.Providers.Provider() { Type = provider1, Enabled = false },
                    new OzetteLibrary.Providers.Provider() { Type = provider2, Enabled = true }
                });

            Assert.AreEqual(1, file.CopyState.Count);
            Assert.AreEqual(provider2, file.CopyState[OzetteLibrary.Providers.ProviderTypes.Azure].Provider);
            Assert.AreEqual(OzetteLibrary.Files.FileStatus.Unsynced, file.CopyState[OzetteLibrary.Providers.ProviderTypes.Azure].SyncStatus);
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
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Files.FileBackupPriority.Low);

            // copy state is inconsistent

            file.ResetCopyState(
                new OzetteLibrary.Providers.ProvidersCollection()
                {
                    new OzetteLibrary.Providers.Provider() { Type = OzetteLibrary.Providers.ProviderTypes.AWS, Enabled = true }
                });

            file.CopyState[OzetteLibrary.Providers.ProviderTypes.AWS].SyncStatus = OzetteLibrary.Files.FileStatus.Synced;
            file.OverallState = OzetteLibrary.Files.FileStatus.Unsynced;

            using (var filestream = new FileStream(".\\TestFiles\\Hasher\\SmallFile.txt", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                file.GenerateNextTransferPayload(filestream, new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger()));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload1()
        {
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Files.FileBackupPriority.Low);

            file.ResetCopyState(
                new OzetteLibrary.Providers.ProvidersCollection()
                {
                    new OzetteLibrary.Providers.Provider() { Type = OzetteLibrary.Providers.ProviderTypes.AWS, Enabled = true }
                });

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(5, payload.TotalBlocks);
                Assert.AreEqual(0, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationProviders);
                Assert.AreEqual(1, payload.DestinationProviders.Count);
                Assert.AreEqual(OzetteLibrary.Providers.ProviderTypes.AWS, payload.DestinationProviders[0]);

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes, payload.Data.Length);

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[]
                {
                    104,190,157,129,20,64,203,47,12,223,69,124,46,207,146,245,145,182,132,186
                }));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload2()
        {
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Files.FileBackupPriority.Medium);

            file.ResetCopyState(
                new OzetteLibrary.Providers.ProvidersCollection()
                {
                    new OzetteLibrary.Providers.Provider() { Type = OzetteLibrary.Providers.ProviderTypes.AWS, Enabled = true }
                });

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(5, payload.TotalBlocks);
                Assert.AreEqual(0, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationProviders);
                Assert.AreEqual(1, payload.DestinationProviders.Count);
                Assert.AreEqual(OzetteLibrary.Providers.ProviderTypes.AWS, payload.DestinationProviders[0]);

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes, payload.Data.Length);

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[]
                {
                    87,42,186,67,68,10,37,216,48,58,2,219,171,171,115,137,238,242,144,168,93,135,96,249,169,49,17,123,22,30,252,81
                }));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload3()
        {
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Files.FileBackupPriority.Low);

            // generate a payload for the second block (index 1)

            file.ResetCopyState(
                new OzetteLibrary.Providers.ProvidersCollection()
                {
                    new OzetteLibrary.Providers.Provider() { Type = OzetteLibrary.Providers.ProviderTypes.AWS, Enabled = true }
                });

            file.CopyState[OzetteLibrary.Providers.ProviderTypes.AWS].LastCompletedFileBlockIndex = 0;
            file.CopyState[OzetteLibrary.Providers.ProviderTypes.AWS].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(5, payload.TotalBlocks);
                Assert.AreEqual(1, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationProviders);
                Assert.AreEqual(1, payload.DestinationProviders.Count);
                Assert.AreEqual(OzetteLibrary.Providers.ProviderTypes.AWS, payload.DestinationProviders[0]);

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes, payload.Data.Length);

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[]
                {
                    219,24,62,65,143,24,145,76,71,142,207,220,183,68,220,193,27,199,6,90
                }));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload4()
        {
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Files.FileBackupPriority.Low);

            // generate a payload for a block in the middle (index 3)

            file.ResetCopyState(
                new OzetteLibrary.Providers.ProvidersCollection()
                {
                    new OzetteLibrary.Providers.Provider() { Type = OzetteLibrary.Providers.ProviderTypes.AWS, Enabled = true }
                });

            file.CopyState[OzetteLibrary.Providers.ProviderTypes.AWS].LastCompletedFileBlockIndex = 2;
            file.CopyState[OzetteLibrary.Providers.ProviderTypes.AWS].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(5, payload.TotalBlocks);
                Assert.AreEqual(3, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationProviders);
                Assert.AreEqual(1, payload.DestinationProviders.Count);
                Assert.AreEqual(OzetteLibrary.Providers.ProviderTypes.AWS, payload.DestinationProviders[0]);

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes, payload.Data.Length);

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[]
                {
                    62,53,249,165,158,29,2,214,101,66,11,113,2,175,24,160,31,144,25,105
                }));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload5()
        {
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Files.FileBackupPriority.Low);

            // generate a payload for the final block (index 4)

            file.ResetCopyState(
                new OzetteLibrary.Providers.ProvidersCollection()
                {
                    new OzetteLibrary.Providers.Provider() { Type = OzetteLibrary.Providers.ProviderTypes.AWS, Enabled = true }
                });

            file.CopyState[OzetteLibrary.Providers.ProviderTypes.AWS].LastCompletedFileBlockIndex = 3;
            file.CopyState[OzetteLibrary.Providers.ProviderTypes.AWS].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(5, payload.TotalBlocks);
                Assert.AreEqual(4, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationProviders);
                Assert.AreEqual(1, payload.DestinationProviders.Count);
                Assert.AreEqual(OzetteLibrary.Providers.ProviderTypes.AWS, payload.DestinationProviders[0]);

                Assert.AreEqual(102809, payload.Data.Length); // partial chunk

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[]
                {
                    250, 27, 250, 217, 229, 10, 217, 143, 211, 205, 186, 171, 83, 35, 218, 172, 40, 4, 138, 110
                }));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload6()
        {
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Files.FileBackupPriority.Low);

            var providers = new OzetteLibrary.Providers.ProviderTypes[] { OzetteLibrary.Providers.ProviderTypes.AWS, OzetteLibrary.Providers.ProviderTypes.Azure };

            // generate a payload for the final block (index 4) for multiple providers

            file.ResetCopyState(
                new OzetteLibrary.Providers.ProvidersCollection()
                {
                    new OzetteLibrary.Providers.Provider() { Type = providers[0], Enabled = true },
                    new OzetteLibrary.Providers.Provider() { Type = providers[1], Enabled = true }
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

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[]
                {
                    250, 27, 250, 217, 229, 10, 217, 143, 211, 205, 186, 171, 83, 35, 218, 172, 40, 4, 138, 110
                }));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload7()
        {
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Files.FileBackupPriority.Low);

            var providers = new OzetteLibrary.Providers.ProviderTypes[] {
                OzetteLibrary.Providers.ProviderTypes.AWS,
                OzetteLibrary.Providers.ProviderTypes.Azure,
                OzetteLibrary.Providers.ProviderTypes.Google
            };

            // generate a payload for a block in the middle (index 3) for multiple providers.

            file.ResetCopyState(
                new OzetteLibrary.Providers.ProvidersCollection()
                {
                    new OzetteLibrary.Providers.Provider() { Type = providers[0], Enabled = true },
                    new OzetteLibrary.Providers.Provider() { Type = providers[1], Enabled = true },
                    new OzetteLibrary.Providers.Provider() { Type = providers[2], Enabled = true }
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

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[]
                {
                    62,53,249,165,158,29,2,214,101,66,11,113,2,175,24,160,31,144,25,105
                }));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload8()
        {
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Files.FileBackupPriority.Low);

            var providers = new OzetteLibrary.Providers.ProviderTypes[] {
                OzetteLibrary.Providers.ProviderTypes.AWS,
                OzetteLibrary.Providers.ProviderTypes.Azure,
                OzetteLibrary.Providers.ProviderTypes.Google
            };

            // generate a payload for a block in the middle (index 3)
            // this file is already synced in the first provider, so only two destination providers should be returned.

            file.ResetCopyState(
                new OzetteLibrary.Providers.ProvidersCollection()
                {
                    new OzetteLibrary.Providers.Provider() { Type = providers[0], Enabled = true },
                    new OzetteLibrary.Providers.Provider() { Type = providers[1], Enabled = true },
                    new OzetteLibrary.Providers.Provider() { Type = providers[2], Enabled = true }
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

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[]
                {
                    62,53,249,165,158,29,2,214,101,66,11,113,2,175,24,160,31,144,25,105
                }));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload9()
        {
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Files.FileBackupPriority.Low);

            var providers = new OzetteLibrary.Providers.ProviderTypes[] {
                OzetteLibrary.Providers.ProviderTypes.AWS,
                OzetteLibrary.Providers.ProviderTypes.Azure,
                OzetteLibrary.Providers.ProviderTypes.Google
            };

            // generate a payload for a block in the middle (index 2)
            // this file is already synced in the first provider, and the second provider is further along.
            // thus only the third provider should be returned, as that is the next available block to send.

            file.ResetCopyState(
                new OzetteLibrary.Providers.ProvidersCollection()
                {
                    new OzetteLibrary.Providers.Provider() { Type = providers[0], Enabled = true },
                    new OzetteLibrary.Providers.Provider() { Type = providers[1], Enabled = true },
                    new OzetteLibrary.Providers.Provider() { Type = providers[2], Enabled = true }
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

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[]
                {
                    111,40,74,161,120,65,67,23,252,204,239,135,18,152,53,141,219,111,38,180
                }));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BackupFileSetBlockAsSentThrowsOnInvalidBlockNumber()
        {
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Files.FileBackupPriority.Low);

            file.SetBlockAsSent(-5, OzetteLibrary.Providers.ProviderTypes.Azure);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BackupFileSetBlockAsSentThrowsOnFileAlreadySynced()
        {
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Files.FileBackupPriority.Low);
            file.OverallState = OzetteLibrary.Files.FileStatus.Synced;

            file.SetBlockAsSent(2, OzetteLibrary.Providers.ProviderTypes.Azure);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BackupFileSetBlockAsSentThrowsOnMissingCopyState()
        {
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Files.FileBackupPriority.Low);
            file.OverallState = OzetteLibrary.Files.FileStatus.InProgress;

            file.SetBlockAsSent(2, OzetteLibrary.Providers.ProviderTypes.Azure);
        }

        [TestMethod]
        public void BackupFileSetBlockAsSentCorrectlySetsCopyStateAndOverallState1()
        {
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Files.FileBackupPriority.Low);

            file.ResetCopyState(
                new OzetteLibrary.Providers.ProvidersCollection()
                {
                    new OzetteLibrary.Providers.Provider() { Type = OzetteLibrary.Providers.ProviderTypes.AWS, Enabled = true }
                });

            Assert.AreEqual(OzetteLibrary.Files.FileStatus.Unsynced, file.OverallState);

            // this example file has 5 total blocks

            file.SetBlockAsSent(0, OzetteLibrary.Providers.ProviderTypes.AWS);

            Assert.AreEqual(OzetteLibrary.Files.FileStatus.InProgress, file.OverallState);
            Assert.AreEqual(OzetteLibrary.Files.FileStatus.InProgress, file.CopyState[OzetteLibrary.Providers.ProviderTypes.AWS].SyncStatus);
        }

        [TestMethod]
        public void BackupFileSetBlockAsSentCorrectlySetsCopyStateAndOverallState2()
        {
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Files.FileBackupPriority.Low);

            file.ResetCopyState(
                new OzetteLibrary.Providers.ProvidersCollection()
                {
                    new OzetteLibrary.Providers.Provider() { Type = OzetteLibrary.Providers.ProviderTypes.AWS, Enabled = true }
                });

            Assert.AreEqual(OzetteLibrary.Files.FileStatus.Unsynced, file.OverallState);

            // this example file has 5 total blocks

            file.SetBlockAsSent(1, OzetteLibrary.Providers.ProviderTypes.AWS);

            Assert.AreEqual(OzetteLibrary.Files.FileStatus.InProgress, file.OverallState);
            Assert.AreEqual(OzetteLibrary.Files.FileStatus.InProgress, file.CopyState[OzetteLibrary.Providers.ProviderTypes.AWS].SyncStatus);
        }

        [TestMethod]
        public void BackupFileSetBlockAsSentCorrectlySetsCopyStateAndOverallState3()
        {
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Files.FileBackupPriority.Low);

            file.ResetCopyState(
                new OzetteLibrary.Providers.ProvidersCollection()
                {
                    new OzetteLibrary.Providers.Provider() { Type = OzetteLibrary.Providers.ProviderTypes.AWS, Enabled = true }
                });

            Assert.AreEqual(OzetteLibrary.Files.FileStatus.Unsynced, file.OverallState);

            // this example file has 5 total blocks

            file.SetBlockAsSent(3, OzetteLibrary.Providers.ProviderTypes.AWS);

            Assert.AreEqual(OzetteLibrary.Files.FileStatus.InProgress, file.OverallState);
            Assert.AreEqual(OzetteLibrary.Files.FileStatus.InProgress, file.CopyState[OzetteLibrary.Providers.ProviderTypes.AWS].SyncStatus);
        }

        [TestMethod]
        public void BackupFileSetBlockAsSentCorrectlySetsCopyStateAndOverallState4()
        {
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Files.FileBackupPriority.Low);

            file.ResetCopyState(
                new OzetteLibrary.Providers.ProvidersCollection()
                {
                    new OzetteLibrary.Providers.Provider() { Type = OzetteLibrary.Providers.ProviderTypes.AWS, Enabled = true }
                });

            Assert.AreEqual(OzetteLibrary.Files.FileStatus.Unsynced, file.OverallState);

            // this example file has 5 total blocks

            file.SetBlockAsSent(4, OzetteLibrary.Providers.ProviderTypes.AWS);

            Assert.AreEqual(OzetteLibrary.Files.FileStatus.Synced, file.OverallState);
            Assert.AreEqual(OzetteLibrary.Files.FileStatus.Synced, file.CopyState[OzetteLibrary.Providers.ProviderTypes.AWS].SyncStatus);
        }

        [TestMethod]
        public void BackupFileGetRemoteFileNameReturnsCorrectlyFormattedNameForAzureProvider()
        {
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Files.FileBackupPriority.Low);

            // reset auto-generated id for stable test result.
            file.FileID = new Guid("387ef266-5635-4224-b8d3-980880ae1258");

            var remoteName = file.GetRemoteFileName(OzetteLibrary.Providers.ProviderTypes.Azure);
            var expected = "ozette-file-387ef266-5635-4224-b8d3-980880ae1258.mp3";

            Assert.IsNotNull(remoteName);
            Assert.AreEqual(expected, remoteName);
        }
    }
}
