using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace OzetteLibraryTests.Models
{
    [TestClass()]
    public class BackupFileTests
    {
        private const int DefaultChunkSizeBytes = 1024;

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksHandlesZeroFileLength()
        {
            var file = new OzetteLibrary.Models.BackupFile();
            file.FileSizeBytes = 0;
            Assert.AreEqual(0, file.GetTotalFileBlocks(DefaultChunkSizeBytes));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void BackupFileGetTotalFileBlocksThrowsWhenProvidedZeroChunkSize()
        {
            var file = new OzetteLibrary.Models.BackupFile();
            file.FileSizeBytes = 1024;
            file.GetTotalFileBlocks(0);
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks1()
        {
            var file = new OzetteLibrary.Models.BackupFile();
            file.FileSizeBytes = 1024;
            Assert.AreEqual(1, file.GetTotalFileBlocks(DefaultChunkSizeBytes));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks2()
        {
            var file = new OzetteLibrary.Models.BackupFile();
            file.FileSizeBytes = 2048;
            Assert.AreEqual(2, file.GetTotalFileBlocks(DefaultChunkSizeBytes));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks3()
        {
            var file = new OzetteLibrary.Models.BackupFile();
            file.FileSizeBytes = 4096;
            Assert.AreEqual(4, file.GetTotalFileBlocks(DefaultChunkSizeBytes));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks4()
        {
            var file = new OzetteLibrary.Models.BackupFile();
            file.FileSizeBytes = 598016;
            Assert.AreEqual(584, file.GetTotalFileBlocks(DefaultChunkSizeBytes));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks5()
        {
            var file = new OzetteLibrary.Models.BackupFile();
            file.FileSizeBytes = 1023;
            Assert.AreEqual(1, file.GetTotalFileBlocks(DefaultChunkSizeBytes));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks6()
        {
            var file = new OzetteLibrary.Models.BackupFile();
            file.FileSizeBytes = 1;
            Assert.AreEqual(1, file.GetTotalFileBlocks(DefaultChunkSizeBytes));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks7()
        {
            var file = new OzetteLibrary.Models.BackupFile();
            file.FileSizeBytes = 1025;
            Assert.AreEqual(2, file.GetTotalFileBlocks(DefaultChunkSizeBytes));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks8()
        {
            var file = new OzetteLibrary.Models.BackupFile();
            file.FileSizeBytes = 1500;
            Assert.AreEqual(2, file.GetTotalFileBlocks(DefaultChunkSizeBytes));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks9()
        {
            var file = new OzetteLibrary.Models.BackupFile();
            file.FileSizeBytes = 2046;
            Assert.AreEqual(2, file.GetTotalFileBlocks(DefaultChunkSizeBytes));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks10()
        {
            var file = new OzetteLibrary.Models.BackupFile();
            file.FileSizeBytes = 2050;
            Assert.AreEqual(3, file.GetTotalFileBlocks(DefaultChunkSizeBytes));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks11()
        {
            var file = new OzetteLibrary.Models.BackupFile();
            file.FileSizeBytes = 189643547895;
            Assert.AreEqual(18519878, file.GetTotalFileBlocks(10240));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks12()
        {
            var file = new OzetteLibrary.Models.BackupFile();
            file.FileSizeBytes = 43414941001;
            Assert.AreEqual(4239741, file.GetTotalFileBlocks(10240));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks13()
        {
            var file = new OzetteLibrary.Models.BackupFile();
            file.FileSizeBytes = 7744931200569;
            Assert.AreEqual(167955484, file.GetTotalFileBlocks(46113));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks14()
        {
            var file = new OzetteLibrary.Models.BackupFile();
            file.FileSizeBytes = 1647999899992;
            Assert.AreEqual(228539718, file.GetTotalFileBlocks(7211));
        }

        [TestMethod()]
        public void BackupFileGetTotalFileBlocksReturnsCorrectNumberOfBlocks15()
        {
            var file = new OzetteLibrary.Models.BackupFile();
            file.FileSizeBytes = 15677400000;
            Assert.AreEqual(154, file.GetTotalFileBlocks(102400000));
        }
    }
}
