using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace OzetteLibraryTests.Models
{
    [TestClass()]
    public class ClientFileTests
    {
        [TestMethod()]
        public void ClientFileResetCopyStateWithExistingTargetsCorrectlyResetsState1()
        {
            // should not throw when no copy state is present

            var file = new OzetteLibrary.Models.ClientFile();
            file.ResetCopyState();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void ClientFileResetCopyStateWithExistingTargetsCorrectlyResetsState2()
        {
            var file = new OzetteLibrary.Models.ClientFile();
            file.CopyState = new Dictionary<int, OzetteLibrary.Models.TargetCopyState>();
            file.CopyState.Add(0, new OzetteLibrary.Models.TargetCopyState() { TargetStatus = OzetteLibrary.Models.FileStatus.InProgress });
            file.ResetCopyState();

            Assert.AreEqual(1, file.CopyState.Count);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, file.CopyState[0].TargetStatus);
        }

        [TestMethod()]
        public void ClientFileResetCopyStateWithExistingTargetsCorrectlyResetsState3()
        {
            var file = new OzetteLibrary.Models.ClientFile();
            file.CopyState = new Dictionary<int, OzetteLibrary.Models.TargetCopyState>();
            file.CopyState.Add(0, new OzetteLibrary.Models.TargetCopyState() { TargetStatus = OzetteLibrary.Models.FileStatus.InProgress });
            file.CopyState.Add(1, new OzetteLibrary.Models.TargetCopyState() { TargetStatus = OzetteLibrary.Models.FileStatus.OutOfDate });
            file.CopyState.Add(2, new OzetteLibrary.Models.TargetCopyState() { TargetStatus = OzetteLibrary.Models.FileStatus.Synced });
            file.ResetCopyState();

            Assert.AreEqual(3, file.CopyState.Count);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, file.CopyState[0].TargetStatus);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, file.CopyState[1].TargetStatus);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, file.CopyState[2].TargetStatus);
        }

        [TestMethod()]
        public void ClientFileResetCopyStateWithNewTargetsCorrectlyResetsState1()
        {
            var newTargets = new OzetteLibrary.Models.Targets();
            newTargets.Add(new OzetteLibrary.Models.Target() { ID = 2 });

            var file = new OzetteLibrary.Models.ClientFile();
            file.CopyState = new Dictionary<int, OzetteLibrary.Models.TargetCopyState>();
            file.CopyState.Add(1, new OzetteLibrary.Models.TargetCopyState() { TargetStatus = OzetteLibrary.Models.FileStatus.InProgress });

            file.ResetCopyState(newTargets);

            Assert.AreEqual(1, file.CopyState.Count);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, file.CopyState[2].TargetStatus);
            Assert.AreEqual(2, file.CopyState[2].TargetID);
        }

        [TestMethod()]
        public void ClientFileResetCopyStateWithNewTargetsCorrectlyResetsState2()
        {
            var newTargets = new OzetteLibrary.Models.Targets();
            newTargets.Add(new OzetteLibrary.Models.Target() { ID = 2 });
            newTargets.Add(new OzetteLibrary.Models.Target() { ID = 4 });
            newTargets.Add(new OzetteLibrary.Models.Target() { ID = 8 });

            var file = new OzetteLibrary.Models.ClientFile();
            file.CopyState = new Dictionary<int, OzetteLibrary.Models.TargetCopyState>();
            file.CopyState.Add(1, new OzetteLibrary.Models.TargetCopyState() { TargetStatus = OzetteLibrary.Models.FileStatus.InProgress });
            file.CopyState.Add(5, new OzetteLibrary.Models.TargetCopyState() { TargetStatus = OzetteLibrary.Models.FileStatus.InProgress });

            file.ResetCopyState(newTargets);

            Assert.AreEqual(3, file.CopyState.Count);

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, file.CopyState[2].TargetStatus);
            Assert.AreEqual(2, file.CopyState[2].TargetID);

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, file.CopyState[4].TargetStatus);
            Assert.AreEqual(4, file.CopyState[4].TargetID);

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, file.CopyState[8].TargetStatus);
            Assert.AreEqual(8, file.CopyState[8].TargetID);
        }
    }
}
