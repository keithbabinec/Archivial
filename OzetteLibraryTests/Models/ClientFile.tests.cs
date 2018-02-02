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
        public void ClientFileHasDataToTransferReturnsValidFalseExample1()
        {
            // no copy state

            var file = new OzetteLibrary.Models.ClientFile();

            Assert.IsFalse(file.HasDataToTransfer(new OzetteLibrary.Models.Targets()));
        }

        [TestMethod()]
        public void ClientFileHasDataToTransferReturnsValidFalseExample2()
        {
            // no targets

            var file = new OzetteLibrary.Models.ClientFile();
            file.CopyState = new Dictionary<int, OzetteLibrary.Models.TargetCopyState>();
            file.CopyState.Add(1, new OzetteLibrary.Models.TargetCopyState() { TargetStatus = OzetteLibrary.Models.FileStatus.InProgress });

            Assert.IsFalse(file.HasDataToTransfer(null));
        }

        [TestMethod()]
        public void ClientFileHasDataToTransferReturnsValidFalseExample3()
        {
            // all synced already

            var file = new OzetteLibrary.Models.ClientFile();
            file.CopyState = new Dictionary<int, OzetteLibrary.Models.TargetCopyState>();
            file.CopyState.Add(1, new OzetteLibrary.Models.TargetCopyState() { TargetStatus = OzetteLibrary.Models.FileStatus.Synced });
            file.OverallState = OzetteLibrary.Models.FileStatus.Synced;

            var targets = new OzetteLibrary.Models.Targets();
            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 1,
                Availability = OzetteLibrary.Models.TargetAvailabilityState.Available
            });

            Assert.IsFalse(file.HasDataToTransfer(targets));
        }

        [TestMethod()]
        public void ClientFileHasDataToTransferReturnsValidFalseExample4()
        {
            // could sync, but the target is down

            var file = new OzetteLibrary.Models.ClientFile();
            file.CopyState = new Dictionary<int, OzetteLibrary.Models.TargetCopyState>();
            file.CopyState.Add(1, new OzetteLibrary.Models.TargetCopyState() { TargetStatus = OzetteLibrary.Models.FileStatus.Unsynced });
            file.OverallState = OzetteLibrary.Models.FileStatus.Unsynced;

            var targets = new OzetteLibrary.Models.Targets();
            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 1,
                Availability = OzetteLibrary.Models.TargetAvailabilityState.Failed
            });

            Assert.IsFalse(file.HasDataToTransfer(targets));
        }

        [TestMethod()]
        public void ClientFileHasDataToTransferReturnsValidFalseExample5()
        {
            // has multiple targets but one file is already synced.
            // the target of the syncable file is unavailable.

            var file = new OzetteLibrary.Models.ClientFile();
            file.CopyState = new Dictionary<int, OzetteLibrary.Models.TargetCopyState>();
            file.CopyState.Add(1, new OzetteLibrary.Models.TargetCopyState() { TargetStatus = OzetteLibrary.Models.FileStatus.Synced });
            file.CopyState.Add(2, new OzetteLibrary.Models.TargetCopyState() { TargetStatus = OzetteLibrary.Models.FileStatus.Unsynced });

            file.OverallState = OzetteLibrary.Models.FileStatus.Unsynced;

            var targets = new OzetteLibrary.Models.Targets();
            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 1,
                Availability = OzetteLibrary.Models.TargetAvailabilityState.Available
            });
            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 2,
                Availability = OzetteLibrary.Models.TargetAvailabilityState.Failed
            });

            Assert.IsFalse(file.HasDataToTransfer(targets));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ClientFileHasDataToTransferThrowsOnTargetKeyNotFound()
        {
            // has data that can be synced
            // but the target keys don't match.

            var file = new OzetteLibrary.Models.ClientFile();
            file.CopyState = new Dictionary<int, OzetteLibrary.Models.TargetCopyState>();
            file.CopyState.Add(1, new OzetteLibrary.Models.TargetCopyState() { TargetStatus = OzetteLibrary.Models.FileStatus.Unsynced });
            file.OverallState = OzetteLibrary.Models.FileStatus.Unsynced;

            var targets = new OzetteLibrary.Models.Targets();
            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 2,
                Availability = OzetteLibrary.Models.TargetAvailabilityState.Available
            });

            Assert.IsTrue(file.HasDataToTransfer(targets));
        }

        [TestMethod()]
        public void ClientFileHasDataToTransferReturnsValidTrueExample1()
        {
            // has data that can be synced (unsynced)

            var file = new OzetteLibrary.Models.ClientFile();
            file.CopyState = new Dictionary<int, OzetteLibrary.Models.TargetCopyState>();
            file.CopyState.Add(1, new OzetteLibrary.Models.TargetCopyState() { TargetStatus = OzetteLibrary.Models.FileStatus.Unsynced });
            file.OverallState = OzetteLibrary.Models.FileStatus.Unsynced;

            var targets = new OzetteLibrary.Models.Targets();
            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 1,
                Availability = OzetteLibrary.Models.TargetAvailabilityState.Available
            });

            Assert.IsTrue(file.HasDataToTransfer(targets));
        }

        [TestMethod()]
        public void ClientFileHasDataToTransferReturnsValidTrueExample2()
        {
            // has data that can be synced (out of date)

            var file = new OzetteLibrary.Models.ClientFile();
            file.CopyState = new Dictionary<int, OzetteLibrary.Models.TargetCopyState>();
            file.CopyState.Add(1, new OzetteLibrary.Models.TargetCopyState() { TargetStatus = OzetteLibrary.Models.FileStatus.OutOfDate });
            file.OverallState = OzetteLibrary.Models.FileStatus.OutOfDate;

            var targets = new OzetteLibrary.Models.Targets();
            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 1,
                Availability = OzetteLibrary.Models.TargetAvailabilityState.Available
            });

            Assert.IsTrue(file.HasDataToTransfer(targets));
        }

        [TestMethod()]
        public void ClientFileHasDataToTransferReturnsValidTrueExample3()
        {
            // has data that can be synced (in-progress)

            var file = new OzetteLibrary.Models.ClientFile();
            file.CopyState = new Dictionary<int, OzetteLibrary.Models.TargetCopyState>();
            file.CopyState.Add(1, new OzetteLibrary.Models.TargetCopyState() { TargetStatus = OzetteLibrary.Models.FileStatus.InProgress });
            file.OverallState = OzetteLibrary.Models.FileStatus.InProgress;

            var targets = new OzetteLibrary.Models.Targets();
            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 1,
                Availability = OzetteLibrary.Models.TargetAvailabilityState.Available
            });

            Assert.IsTrue(file.HasDataToTransfer(targets));
        }

        [TestMethod()]
        public void ClientFileHasDataToTransferReturnsValidTrueExample4()
        {
            // has multiple targets that can be synced (unsynced)

            var file = new OzetteLibrary.Models.ClientFile();
            file.CopyState = new Dictionary<int, OzetteLibrary.Models.TargetCopyState>();
            file.CopyState.Add(1, new OzetteLibrary.Models.TargetCopyState() { TargetStatus = OzetteLibrary.Models.FileStatus.Unsynced });
            file.CopyState.Add(2, new OzetteLibrary.Models.TargetCopyState() { TargetStatus = OzetteLibrary.Models.FileStatus.Unsynced });

            file.OverallState = OzetteLibrary.Models.FileStatus.Unsynced;

            var targets = new OzetteLibrary.Models.Targets();
            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 1,
                Availability = OzetteLibrary.Models.TargetAvailabilityState.Available
            });
            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 2,
                Availability = OzetteLibrary.Models.TargetAvailabilityState.Available
            });

            Assert.IsTrue(file.HasDataToTransfer(targets));
        }

        [TestMethod()]
        public void ClientFileHasDataToTransferReturnsValidTrueExample5()
        {
            // has multiple targets that can be synced (unsynced)
            // but one target is unavailable.

            var file = new OzetteLibrary.Models.ClientFile();
            file.CopyState = new Dictionary<int, OzetteLibrary.Models.TargetCopyState>();
            file.CopyState.Add(1, new OzetteLibrary.Models.TargetCopyState() { TargetStatus = OzetteLibrary.Models.FileStatus.Unsynced });
            file.CopyState.Add(2, new OzetteLibrary.Models.TargetCopyState() { TargetStatus = OzetteLibrary.Models.FileStatus.Unsynced });

            file.OverallState = OzetteLibrary.Models.FileStatus.Unsynced;

            var targets = new OzetteLibrary.Models.Targets();
            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 1,
                Availability = OzetteLibrary.Models.TargetAvailabilityState.Disabled
            });
            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 2,
                Availability = OzetteLibrary.Models.TargetAvailabilityState.Available
            });

            Assert.IsTrue(file.HasDataToTransfer(targets));
        }

        [TestMethod()]
        public void ClientFileHasDataToTransferReturnsValidTrueExample6()
        {
            // has multiple targets but one file is already synced.

            var file = new OzetteLibrary.Models.ClientFile();
            file.CopyState = new Dictionary<int, OzetteLibrary.Models.TargetCopyState>();
            file.CopyState.Add(1, new OzetteLibrary.Models.TargetCopyState() { TargetStatus = OzetteLibrary.Models.FileStatus.Synced });
            file.CopyState.Add(2, new OzetteLibrary.Models.TargetCopyState() { TargetStatus = OzetteLibrary.Models.FileStatus.Unsynced });

            file.OverallState = OzetteLibrary.Models.FileStatus.Unsynced;

            var targets = new OzetteLibrary.Models.Targets();
            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 1,
                Availability = OzetteLibrary.Models.TargetAvailabilityState.Available
            });
            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 2,
                Availability = OzetteLibrary.Models.TargetAvailabilityState.Available
            });

            Assert.IsTrue(file.HasDataToTransfer(targets));
        }
        
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
            var file = new OzetteLibrary.Models.ClientFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Models.FileBackupPriority.Low);
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
            var file = new OzetteLibrary.Models.ClientFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Models.FileBackupPriority.Low);
            var targets = new OzetteLibrary.Models.Targets();

            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 1,
                Name = "t1",
                Port = 80,
                RootDirectory = "C:\\backup\\incoming",
                Url = "http:\\\\fake-backup-location.com"
            });

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
