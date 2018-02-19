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
            var targetID1 = Guid.NewGuid();

            var target = new OzetteLibrary.Models.Target()
            {
                ID = targetID1,
                Name = "TestTarget"
            };

            var copyState = new OzetteLibrary.Models.TargetCopyState(target);

            Assert.AreEqual(targetID1, copyState.TargetID);
        }

        [TestMethod()]
        public void TargetCopyStateCorrectlyInitializesCopyStateAsUnsynced1()
        {
            var copyState = new OzetteLibrary.Models.TargetCopyState();

            Assert.AreEqual(Guid.Empty, copyState.TargetID);
            Assert.AreEqual(0, copyState.LastCompletedFileChunkIndex);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, copyState.TargetStatus);
        }

        [TestMethod()]
        public void TargetCopyStateCorrectlyInitializesCopyStateAsUnsynced2()
        {
            var targetID1 = Guid.NewGuid();

            var target = new OzetteLibrary.Models.Target()
            {
                ID = targetID1,
                Name = "TestTarget"
            };

            var copyState = new OzetteLibrary.Models.TargetCopyState(target);

            Assert.AreEqual(targetID1, copyState.TargetID);
            Assert.AreEqual(-1, copyState.LastCompletedFileChunkIndex);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, copyState.TargetStatus);
        }

        [TestMethod()]
        public void TargetCopyStateResetStateCorrectlyResetsProgress()
        {
            var targetID1 = Guid.NewGuid();
            var copyState = new OzetteLibrary.Models.TargetCopyState();
            copyState.TargetID = targetID1;
            copyState.LastCompletedFileChunkIndex = 100;
            copyState.TargetStatus = OzetteLibrary.Models.FileStatus.InProgress;
            copyState.ResetState();

            Assert.AreEqual(targetID1, copyState.TargetID);
            Assert.AreEqual(-1, copyState.LastCompletedFileChunkIndex);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, copyState.TargetStatus);
        }

        [TestMethod()]
        public void TargetCopyStateResetStateCorrectlyResetsProgress2()
        {
            var targetID1 = Guid.NewGuid();
            var copyState = new OzetteLibrary.Models.TargetCopyState();
            copyState.TargetID = targetID1;
            copyState.LastCompletedFileChunkIndex = 100;
            copyState.TargetStatus = OzetteLibrary.Models.FileStatus.Synced;
            copyState.ResetState();

            Assert.AreEqual(targetID1, copyState.TargetID);
            Assert.AreEqual(-1, copyState.LastCompletedFileChunkIndex);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, copyState.TargetStatus);
        }
    }
}
