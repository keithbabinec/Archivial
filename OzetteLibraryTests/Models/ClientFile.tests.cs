using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

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

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ClientFileGenerateNextTransferPayloadThrowsOnInvalidArgs1()
        {
            // no filestream
            var file = new OzetteLibrary.Models.ClientFile();
            file.GenerateNextTransferPayload(null, new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger()));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ClientFileGenerateNextTransferPayloadThrowsOnInvalidArgs2()
        {
            var file = new OzetteLibrary.Models.ClientFile();

            // no logger
            using (var filestream = new FileStream(".\\TestFiles\\SourceLocation\\EmptySourcesFile.json", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                file.GenerateNextTransferPayload(filestream, null);
            }   
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ClientFileGenerateNextTransferPayloadThrowsOnInvalidFileState1()
        {
            var file = new OzetteLibrary.Models.ClientFile();

            // file is already synced
            using (var filestream = new FileStream(".\\TestFiles\\SourceLocation\\EmptySourcesFile.json", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                file.OverallState = OzetteLibrary.Models.FileStatus.Synced;
                file.GenerateNextTransferPayload(filestream, new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger()));
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ClientFileGenerateNextTransferPayloadThrowsOnInvalidFileState2()
        {
            var file = new OzetteLibrary.Models.ClientFile();

            // file priority is not set
            using (var filestream = new FileStream(".\\TestFiles\\SourceLocation\\EmptySourcesFile.json", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                file.OverallState = OzetteLibrary.Models.FileStatus.Unsynced;
                file.Priority = OzetteLibrary.Models.FileBackupPriority.Unset;
                file.GenerateNextTransferPayload(filestream, new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger()));
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ClientFileGenerateNextTransferPayloadThrowsOnInvalidFileState3()
        {
            var file = new OzetteLibrary.Models.ClientFile();

            // copystate is not set
            using (var filestream = new FileStream(".\\TestFiles\\SourceLocation\\EmptySourcesFile.json", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                file.OverallState = OzetteLibrary.Models.FileStatus.Unsynced;
                file.Priority = OzetteLibrary.Models.FileBackupPriority.Low;
                file.CopyState = null;
                file.GenerateNextTransferPayload(filestream, new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger()));
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ClientFileGenerateNextTransferPayloadThrowsOnInvalidFileState4()
        {
            var file = new OzetteLibrary.Models.ClientFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"));
            var targets = new OzetteLibrary.Models.Targets();

            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 1,
                Name = "t1",
                Port = 80,
                RootDirectory = "C:\\backup\\incoming",
                Url = "http:\\\\fake-backup-location.com"
            });

            // copy state is inconsistent
            file.Priority = OzetteLibrary.Models.FileBackupPriority.Low;
            file.ResetCopyState(targets);
            file.CopyState[targets[0].ID].TargetStatus = OzetteLibrary.Models.FileStatus.Synced;
            file.OverallState = OzetteLibrary.Models.FileStatus.Unsynced;

            using (var filestream = new FileStream(".\\TestFiles\\SourceLocation\\EmptySourcesFile.json", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                file.GenerateNextTransferPayload(filestream, new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger()));
            }
        }

        [TestMethod()]
        public void ClientFileGenerateNextTransferPayloadReturnsCorrectPayload1()
        {
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Models.ClientFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"));
            var targets = new OzetteLibrary.Models.Targets();

            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 1,
                Name = "t1",
                Port = 80,
                RootDirectory = "C:\\backup\\incoming",
                Url = "http:\\\\fake-backup-location.com"
            });

            file.Priority = OzetteLibrary.Models.FileBackupPriority.Low;
            file.ResetCopyState(targets);

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(9, payload.TotalBlocks);
                Assert.AreEqual(0, payload.CurrentBlockNumber);

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferChunkSizeBytes, payload.Data.Length);

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[] 
                {
                    181, 214, 35, 173, 93, 152, 78, 193, 4, 144, 156, 99, 85, 215, 93, 93, 98, 85, 204, 128
                }));
            }
        }
    }
}
