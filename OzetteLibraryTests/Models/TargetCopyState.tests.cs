using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace OzetteLibraryTests.Models
{
    [TestClass()]
    public class TargetCopyStateTests
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TargetCopyStateConstructorThrowsOnNullTargetInput()
        {
            var copyState = new OzetteLibrary.Models.TargetCopyState(null);
        }

        [TestMethod()]
        public void TargetCopyStateResetStateDoesNotThrowIfNoTargetIsPresent()
        {
            var copyState = new OzetteLibrary.Models.TargetCopyState();
            copyState.ResetState();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void TargetCopyStateCorrectlyAssignsTargetFromConstructor()
        {
            var target = new OzetteLibrary.Models.Target()
            {
                ID = 1,
                Name = "TestTarget"
            };

            var copyState = new OzetteLibrary.Models.TargetCopyState(target);

            Assert.AreEqual(1, copyState.TargetID);
        }

        [TestMethod()]
        public void TargetCopyStateCorrectlyInitializesCopyStateAsUnsynced1()
        {
            var copyState = new OzetteLibrary.Models.TargetCopyState();

            Assert.AreEqual(0, copyState.TargetID);
            Assert.AreEqual(0, copyState.LastCompletedFileChunk);
            Assert.AreEqual(0, copyState.TotalFileChunks);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, copyState.TargetStatus);
        }

        [TestMethod()]
        public void TargetCopyStateCorrectlyInitializesCopyStateAsUnsynced2()
        {
            var target = new OzetteLibrary.Models.Target()
            {
                ID = 1,
                Name = "TestTarget"
            };

            var copyState = new OzetteLibrary.Models.TargetCopyState(target);

            Assert.AreEqual(1, copyState.TargetID);
            Assert.AreEqual(0, copyState.LastCompletedFileChunk);
            Assert.AreEqual(0, copyState.TotalFileChunks);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, copyState.TargetStatus);
        }

        [TestMethod()]
        public void TargetCopyStateResetStateCorrectlyResetsProgress()
        {
            var copyState = new OzetteLibrary.Models.TargetCopyState();
            copyState.TargetID = 1;
            copyState.LastCompletedFileChunk = 100;
            copyState.TotalFileChunks = 150;
            copyState.TargetStatus = OzetteLibrary.Models.FileStatus.InProgress;
            copyState.ResetState();

            Assert.AreEqual(1, copyState.TargetID);
            Assert.AreEqual(0, copyState.LastCompletedFileChunk);
            Assert.AreEqual(0, copyState.TotalFileChunks);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, copyState.TargetStatus);
        }

        [TestMethod()]
        public void TargetCopyStateResetStateCorrectlyResetsProgress2()
        {
            var copyState = new OzetteLibrary.Models.TargetCopyState();
            copyState.TargetID = 1;
            copyState.LastCompletedFileChunk = 100;
            copyState.TotalFileChunks = 100;
            copyState.TargetStatus = OzetteLibrary.Models.FileStatus.Synced;
            copyState.ResetState();

            Assert.AreEqual(1, copyState.TargetID);
            Assert.AreEqual(0, copyState.LastCompletedFileChunk);
            Assert.AreEqual(0, copyState.TotalFileChunks);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, copyState.TargetStatus);
        }
    }
}
