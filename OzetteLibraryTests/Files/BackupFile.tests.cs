using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace OzetteLibraryTests.Files
{
    [TestClass()]
    public class BackupFileTests
    {
        private const int DefaultBlockSizeBytes = 1024;

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksHandlesZeroFileLength()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 0;
            Assert.AreEqual(0, file.CalculateTotalFileBlocks(DefaultBlockSizeBytes));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void BackupFileGetTotalFileBlocksThrowsWhenProvidedZeroBlockSize()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 1024;
            file.CalculateTotalFileBlocks(0);
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks1()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 1024;
            Assert.AreEqual(1, file.CalculateTotalFileBlocks(DefaultBlockSizeBytes));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks2()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 2048;
            Assert.AreEqual(2, file.CalculateTotalFileBlocks(DefaultBlockSizeBytes));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks3()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 4096;
            Assert.AreEqual(4, file.CalculateTotalFileBlocks(DefaultBlockSizeBytes));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks4()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 598016;
            Assert.AreEqual(584, file.CalculateTotalFileBlocks(DefaultBlockSizeBytes));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks5()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 1023;
            Assert.AreEqual(1, file.CalculateTotalFileBlocks(DefaultBlockSizeBytes));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks6()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 1;
            Assert.AreEqual(1, file.CalculateTotalFileBlocks(DefaultBlockSizeBytes));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks7()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 1025;
            Assert.AreEqual(2, file.CalculateTotalFileBlocks(DefaultBlockSizeBytes));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks8()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 1500;
            Assert.AreEqual(2, file.CalculateTotalFileBlocks(DefaultBlockSizeBytes));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks9()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 2046;
            Assert.AreEqual(2, file.CalculateTotalFileBlocks(DefaultBlockSizeBytes));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks10()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 2050;
            Assert.AreEqual(3, file.CalculateTotalFileBlocks(DefaultBlockSizeBytes));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks11()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 189643547895;
            Assert.AreEqual(18519878, file.CalculateTotalFileBlocks(10240));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks12()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 43414941001;
            Assert.AreEqual(4239741, file.CalculateTotalFileBlocks(10240));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks13()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 7744931200569;
            Assert.AreEqual(167955484, file.CalculateTotalFileBlocks(46113));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks14()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 1647999899992;
            Assert.AreEqual(228539718, file.CalculateTotalFileBlocks(7211));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks15()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.FileSizeBytes = 15677400000;
            Assert.AreEqual(154, file.CalculateTotalFileBlocks(102400000));
        }

        [TestMethod()]
        public void BackupFileHasDataToTransferReturnsValidFalseExample1()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.OverallState = OzetteLibrary.Files.FileStatus.Synced;

            Assert.IsFalse(file.HasDataToTransfer());
        }

        [TestMethod()]
        public void BackupFileHasDataToTransferReturnsValidTrueExample1()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.OverallState = OzetteLibrary.Files.FileStatus.OutOfDate;

            Assert.IsTrue(file.HasDataToTransfer());
        }

        [TestMethod()]
        public void BackupFileHasDataToTransferReturnsValidTrueExample2()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.OverallState = OzetteLibrary.Files.FileStatus.Unsynced;

            Assert.IsTrue(file.HasDataToTransfer());
        }

        [TestMethod()]
        public void BackupFileHasDataToTransferReturnsValidTrueExample3()
        {
            var file = new OzetteLibrary.Files.BackupFile();
            file.OverallState = OzetteLibrary.Files.FileStatus.InProgress;

            Assert.IsTrue(file.HasDataToTransfer());
        }

        [TestMethod()]
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
                new OzetteLibrary.Providers.ProviderFileStatus() {
                    Provider = provider1,
                    SyncStatus = OzetteLibrary.Files.FileStatus.InProgress
                });

            file.ResetCopyState(new OzetteLibrary.Providers.ProviderTypes[] { provider1, provider2 });

            Assert.AreEqual(2, file.CopyState.Count);
            Assert.AreEqual(OzetteLibrary.Files.FileStatus.Unsynced, file.CopyState[provider1].SyncStatus);
            Assert.AreEqual(OzetteLibrary.Files.FileStatus.Unsynced, file.CopyState[provider2].SyncStatus);
        }

        [TestMethod()]
        public void BackupFileResetCopyStateCorrectlyResetsState2()
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
                    SyncStatus = OzetteLibrary.Files.FileStatus.Synced
                });

            file.CopyState.Add(
                provider2,
                new OzetteLibrary.Providers.ProviderFileStatus()
                {
                    Provider = provider2,
                    SyncStatus = OzetteLibrary.Files.FileStatus.Synced
                });

            file.ResetCopyState(new OzetteLibrary.Providers.ProviderTypes[] { provider1 });

            Assert.AreEqual(1, file.CopyState.Count);
            Assert.AreEqual(OzetteLibrary.Files.FileStatus.Unsynced, file.CopyState[provider1].SyncStatus);
        }
    }
}
