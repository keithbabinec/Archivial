using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace OzetteLibraryTests.Files
{
    [TestClass]
    public class BackupFileTests
    {
        private const int DefaultBlockSizeBytes = 1024;

        [TestMethod]
        public void BackupFileGetTotalFileBlocksHandlesZeroFileLength()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 0;
            Assert.AreEqual(0, file.CalculateTotalFileBlocks(DefaultBlockSizeBytes));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BackupFileGetTotalFileBlocksThrowsWhenProvidedZeroBlockSize()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 1024;
            file.CalculateTotalFileBlocks(0);
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks1()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 1024;
            Assert.AreEqual(1, file.CalculateTotalFileBlocks(DefaultBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks2()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 2048;
            Assert.AreEqual(2, file.CalculateTotalFileBlocks(DefaultBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks3()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 4096;
            Assert.AreEqual(4, file.CalculateTotalFileBlocks(DefaultBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks4()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 598016;
            Assert.AreEqual(584, file.CalculateTotalFileBlocks(DefaultBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks5()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 1023;
            Assert.AreEqual(1, file.CalculateTotalFileBlocks(DefaultBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks6()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 1;
            Assert.AreEqual(1, file.CalculateTotalFileBlocks(DefaultBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks7()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 1025;
            Assert.AreEqual(2, file.CalculateTotalFileBlocks(DefaultBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks8()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 1500;
            Assert.AreEqual(2, file.CalculateTotalFileBlocks(DefaultBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks9()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 2046;
            Assert.AreEqual(2, file.CalculateTotalFileBlocks(DefaultBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks10()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 2050;
            Assert.AreEqual(3, file.CalculateTotalFileBlocks(DefaultBlockSizeBytes));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks11()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 189643547895;
            Assert.AreEqual(18519878, file.CalculateTotalFileBlocks(10240));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks12()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 43414941001;
            Assert.AreEqual(4239741, file.CalculateTotalFileBlocks(10240));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks13()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 7744931200569;
            Assert.AreEqual(167955484, file.CalculateTotalFileBlocks(46113));
        }

        [TestMethod]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks14()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 1647999899992;
            Assert.AreEqual(228539718, file.CalculateTotalFileBlocks(7211));
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

                Assert.AreEqual(9, payload.TotalBlocks);
                Assert.AreEqual(0, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationProviders);
                Assert.AreEqual(1, payload.DestinationProviders.Count);
                Assert.AreEqual(OzetteLibrary.Providers.ProviderTypes.AWS, payload.DestinationProviders[0]);

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes, payload.Data.Length);

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[]
                {
                    181, 214, 35, 173, 93, 152, 78, 193, 4, 144, 156, 99, 85, 215, 93, 93, 98, 85, 204, 128
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

                Assert.AreEqual(9, payload.TotalBlocks);
                Assert.AreEqual(0, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationProviders);
                Assert.AreEqual(1, payload.DestinationProviders.Count);
                Assert.AreEqual(OzetteLibrary.Providers.ProviderTypes.AWS, payload.DestinationProviders[0]);

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes, payload.Data.Length);

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[]
                {
                    188, 2, 134, 85, 252, 193, 68, 239, 20, 109, 45, 159, 115, 190, 66, 79, 237, 85,
                    152, 169, 42, 111, 116, 189, 159, 41, 135, 185, 178, 53, 162, 26,
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

                Assert.AreEqual(9, payload.TotalBlocks);
                Assert.AreEqual(1, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationProviders);
                Assert.AreEqual(1, payload.DestinationProviders.Count);
                Assert.AreEqual(OzetteLibrary.Providers.ProviderTypes.AWS, payload.DestinationProviders[0]);

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes, payload.Data.Length);

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[]
                {
                    195, 216, 15, 17, 146, 78, 106, 226, 24, 67, 148, 215, 196, 100, 62, 114, 94, 174, 244, 112
                }));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload4()
        {
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Files.FileBackupPriority.Low);

            // generate a payload for a block in the middle (index 5)

            file.ResetCopyState(
                new OzetteLibrary.Providers.ProvidersCollection()
                {
                    new OzetteLibrary.Providers.Provider() { Type = OzetteLibrary.Providers.ProviderTypes.AWS, Enabled = true }
                });

            file.CopyState[OzetteLibrary.Providers.ProviderTypes.AWS].LastCompletedFileBlockIndex = 4;
            file.CopyState[OzetteLibrary.Providers.ProviderTypes.AWS].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(9, payload.TotalBlocks);
                Assert.AreEqual(5, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationProviders);
                Assert.AreEqual(1, payload.DestinationProviders.Count);
                Assert.AreEqual(OzetteLibrary.Providers.ProviderTypes.AWS, payload.DestinationProviders[0]);

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes, payload.Data.Length);

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[]
                {
                    87, 235, 10, 73, 101, 181, 223, 125, 207, 62, 245, 133, 49, 181, 131, 199, 111, 104, 153, 89
                }));
            }
        }

        [TestMethod]
        public void BackupFileGenerateNextTransferPayloadReturnsCorrectPayload5()
        {
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Files.BackupFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Files.FileBackupPriority.Low);

            // generate a payload for the final block (index 8)

            file.ResetCopyState(
                new OzetteLibrary.Providers.ProvidersCollection()
                {
                    new OzetteLibrary.Providers.Provider() { Type = OzetteLibrary.Providers.ProviderTypes.AWS, Enabled = true }
                });

            file.CopyState[OzetteLibrary.Providers.ProviderTypes.AWS].LastCompletedFileBlockIndex = 7;
            file.CopyState[OzetteLibrary.Providers.ProviderTypes.AWS].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(9, payload.TotalBlocks);
                Assert.AreEqual(8, payload.CurrentBlockNumber);

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

            // generate a payload for the final block (index 8)

            file.ResetCopyState(
                new OzetteLibrary.Providers.ProvidersCollection()
                {
                    new OzetteLibrary.Providers.Provider() { Type = providers[0], Enabled = true },
                    new OzetteLibrary.Providers.Provider() { Type = providers[1], Enabled = true }
                });

            file.CopyState[providers[0]].LastCompletedFileBlockIndex = 7;
            file.CopyState[providers[0]].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

            file.CopyState[providers[1]].LastCompletedFileBlockIndex = 7;
            file.CopyState[providers[1]].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(9, payload.TotalBlocks);
                Assert.AreEqual(8, payload.CurrentBlockNumber);

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

            // generate a payload for a block in the middle (index 5)

            file.ResetCopyState(
                new OzetteLibrary.Providers.ProvidersCollection()
                {
                    new OzetteLibrary.Providers.Provider() { Type = providers[0], Enabled = true },
                    new OzetteLibrary.Providers.Provider() { Type = providers[1], Enabled = true },
                    new OzetteLibrary.Providers.Provider() { Type = providers[2], Enabled = true }
                });

            file.CopyState[providers[0]].LastCompletedFileBlockIndex = 4;
            file.CopyState[providers[0]].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

            file.CopyState[providers[1]].LastCompletedFileBlockIndex = 4;
            file.CopyState[providers[1]].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

            file.CopyState[providers[2]].LastCompletedFileBlockIndex = 4;
            file.CopyState[providers[2]].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(9, payload.TotalBlocks);
                Assert.AreEqual(5, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationProviders);
                Assert.AreEqual(3, payload.DestinationProviders.Count);
                Assert.AreEqual(providers[0], payload.DestinationProviders[0]);
                Assert.AreEqual(providers[1], payload.DestinationProviders[1]);
                Assert.AreEqual(providers[2], payload.DestinationProviders[2]);

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes, payload.Data.Length);

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[]
                {
                    87, 235, 10, 73, 101, 181, 223, 125, 207, 62, 245, 133, 49, 181, 131, 199, 111, 104, 153, 89
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

            // generate a payload for a block in the middle (index 5)
            // this file is already synced in the first provider, so only two destination providers should be returned.

            file.ResetCopyState(
                new OzetteLibrary.Providers.ProvidersCollection()
                {
                    new OzetteLibrary.Providers.Provider() { Type = providers[0], Enabled = true },
                    new OzetteLibrary.Providers.Provider() { Type = providers[1], Enabled = true },
                    new OzetteLibrary.Providers.Provider() { Type = providers[2], Enabled = true }
                });

            file.CopyState[providers[0]].LastCompletedFileBlockIndex = 8;
            file.CopyState[providers[0]].SyncStatus = OzetteLibrary.Files.FileStatus.Synced;

            file.CopyState[providers[1]].LastCompletedFileBlockIndex = 4;
            file.CopyState[providers[1]].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

            file.CopyState[providers[2]].LastCompletedFileBlockIndex = 4;
            file.CopyState[providers[2]].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(9, payload.TotalBlocks);
                Assert.AreEqual(5, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationProviders);
                Assert.AreEqual(2, payload.DestinationProviders.Count);
                Assert.AreEqual(providers[1], payload.DestinationProviders[0]);
                Assert.AreEqual(providers[2], payload.DestinationProviders[1]);

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes, payload.Data.Length);

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[]
                {
                    87, 235, 10, 73, 101, 181, 223, 125, 207, 62, 245, 133, 49, 181, 131, 199, 111, 104, 153, 89
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

            // generate a payload for a block in the middle (index 5)
            // this file is already synced in the first provider, and the second provider is further along.
            // thus only the third provider should be returned, as that is the next available block to send.

            file.ResetCopyState(
                new OzetteLibrary.Providers.ProvidersCollection()
                {
                    new OzetteLibrary.Providers.Provider() { Type = providers[0], Enabled = true },
                    new OzetteLibrary.Providers.Provider() { Type = providers[1], Enabled = true },
                    new OzetteLibrary.Providers.Provider() { Type = providers[2], Enabled = true }
                });

            file.CopyState[providers[0]].LastCompletedFileBlockIndex = 8;
            file.CopyState[providers[0]].SyncStatus = OzetteLibrary.Files.FileStatus.Synced;

            file.CopyState[providers[1]].LastCompletedFileBlockIndex = 6;
            file.CopyState[providers[1]].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

            file.CopyState[providers[2]].LastCompletedFileBlockIndex = 4;
            file.CopyState[providers[2]].SyncStatus = OzetteLibrary.Files.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(9, payload.TotalBlocks);
                Assert.AreEqual(5, payload.CurrentBlockNumber);

                // ensure we are taking the minimum block number that could be sent.

                Assert.IsNotNull(payload.DestinationProviders);
                Assert.AreEqual(1, payload.DestinationProviders.Count);
                Assert.AreEqual(providers[2], payload.DestinationProviders[0]);

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferBlockSizeBytes, payload.Data.Length);

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[]
                {
                    87, 235, 10, 73, 101, 181, 223, 125, 207, 62, 245, 133, 49, 181, 131, 199, 111, 104, 153, 89
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

            // this example file has 9 total blocks

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

            // this example file has 9 total blocks

            file.SetBlockAsSent(4, OzetteLibrary.Providers.ProviderTypes.AWS);

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

            // this example file has 9 total blocks

            file.SetBlockAsSent(8, OzetteLibrary.Providers.ProviderTypes.AWS);

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

            // this example file has 9 total blocks

            file.SetBlockAsSent(9, OzetteLibrary.Providers.ProviderTypes.AWS);

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
            var expected = "ozette-file-387ef266-5635-4224-b8d3-980880ae1258";

            Assert.IsNotNull(remoteName);
            Assert.AreEqual(expected, remoteName);
        }
    }
}
