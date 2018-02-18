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
            var file = new OzetteLibrary.Models.ClientFile();
            file.OverallState = OzetteLibrary.Models.FileStatus.Synced;

            Assert.IsFalse(file.HasDataToTransfer());
        }

        [TestMethod()]
        public void ClientFileHasDataToTransferReturnsValidTrueExample1()
        {
            var file = new OzetteLibrary.Models.ClientFile();
            file.OverallState = OzetteLibrary.Models.FileStatus.OutOfDate;

            Assert.IsTrue(file.HasDataToTransfer());
        }

        [TestMethod()]
        public void ClientFileHasDataToTransferReturnsValidTrueExample2()
        {
            var file = new OzetteLibrary.Models.ClientFile();
            file.OverallState = OzetteLibrary.Models.FileStatus.Unsynced;

            Assert.IsTrue(file.HasDataToTransfer());
        }

        [TestMethod()]
        public void ClientFileHasDataToTransferReturnsValidTrueExample3()
        {
            var file = new OzetteLibrary.Models.ClientFile();
            file.OverallState = OzetteLibrary.Models.FileStatus.InProgress;

            Assert.IsTrue(file.HasDataToTransfer());
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

            // generate a payload for the first block (index 0)

            file.ResetCopyState(targets);

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(9, payload.TotalBlocks);
                Assert.AreEqual(0, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationTargetIDs);
                Assert.AreEqual(1, payload.DestinationTargetIDs.Count);
                Assert.AreEqual(1, payload.DestinationTargetIDs[0]);

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferChunkSizeBytes, payload.Data.Length);

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[]
                {
                    181, 214, 35, 173, 93, 152, 78, 193, 4, 144, 156, 99, 85, 215, 93, 93, 98, 85, 204, 128
                }));
            }
        }

        [TestMethod()]
        public void ClientFileGenerateNextTransferPayloadReturnsCorrectPayload2()
        {
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Models.ClientFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Models.FileBackupPriority.Medium);
            var targets = new OzetteLibrary.Models.Targets();

            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 100,
                Name = "t1",
                Port = 80,
                RootDirectory = "C:\\backup\\incoming",
                Url = "http:\\\\fake-backup-location.com"
            });

            // generate a payload for the first block (index 0)

            file.ResetCopyState(targets);
            
            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(9, payload.TotalBlocks);
                Assert.AreEqual(0, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationTargetIDs);
                Assert.AreEqual(1, payload.DestinationTargetIDs.Count);
                Assert.AreEqual(100, payload.DestinationTargetIDs[0]);

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferChunkSizeBytes, payload.Data.Length);

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[]
                {
                    188, 2, 134, 85, 252, 193, 68, 239, 20, 109, 45, 159, 115, 190, 66, 79, 237, 85,
                    152, 169, 42, 111, 116, 189, 159, 41, 135, 185, 178, 53, 162, 26,
                }));
            }
        }

        [TestMethod()]
        public void ClientFileGenerateNextTransferPayloadReturnsCorrectPayload3()
        {
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Models.ClientFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Models.FileBackupPriority.Low);
            var targets = new OzetteLibrary.Models.Targets();

            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 4,
                Name = "t1",
                Port = 80,
                RootDirectory = "C:\\backup\\incoming",
                Url = "http:\\\\fake-backup-location.com"
            });

            // generate a payload for the second block (index 1)

            file.ResetCopyState(targets);
            file.CopyState[targets[0].ID].LastCompletedFileChunkIndex = 0;
            file.CopyState[targets[0].ID].TargetStatus = OzetteLibrary.Models.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(9, payload.TotalBlocks);
                Assert.AreEqual(1, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationTargetIDs);
                Assert.AreEqual(1, payload.DestinationTargetIDs.Count);
                Assert.AreEqual(4, payload.DestinationTargetIDs[0]);

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferChunkSizeBytes, payload.Data.Length);

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[]
                {
                    195, 216, 15, 17, 146, 78, 106, 226, 24, 67, 148, 215, 196, 100, 62, 114, 94, 174, 244, 112
                }));
            }
        }

        [TestMethod()]
        public void ClientFileGenerateNextTransferPayloadReturnsCorrectPayload4()
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

            // generate a payload for a block in the middle (index 5)

            file.ResetCopyState(targets);
            file.CopyState[targets[0].ID].LastCompletedFileChunkIndex = 4;
            file.CopyState[targets[0].ID].TargetStatus = OzetteLibrary.Models.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(9, payload.TotalBlocks);
                Assert.AreEqual(5, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationTargetIDs);
                Assert.AreEqual(1, payload.DestinationTargetIDs.Count);
                Assert.AreEqual(1, payload.DestinationTargetIDs[0]);

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferChunkSizeBytes, payload.Data.Length);

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[]
                {
                    87, 235, 10, 73, 101, 181, 223, 125, 207, 62, 245, 133, 49, 181, 131, 199, 111, 104, 153, 89
                }));
            }
        }

        [TestMethod()]
        public void ClientFileGenerateNextTransferPayloadReturnsCorrectPayload5()
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

            // generate a payload for the final block (index 8)

            file.ResetCopyState(targets);
            file.CopyState[targets[0].ID].LastCompletedFileChunkIndex = 7;
            file.CopyState[targets[0].ID].TargetStatus = OzetteLibrary.Models.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(9, payload.TotalBlocks);
                Assert.AreEqual(8, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationTargetIDs);
                Assert.AreEqual(1, payload.DestinationTargetIDs.Count);
                Assert.AreEqual(1, payload.DestinationTargetIDs[0]);

                Assert.AreEqual(102809, payload.Data.Length); // partial chunk

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[]
                {
                    250, 27, 250, 217, 229, 10, 217, 143, 211, 205, 186, 171, 83, 35, 218, 172, 40, 4, 138, 110
                }));
            }
        }

        [TestMethod()]
        public void ClientFileGenerateNextTransferPayloadReturnsCorrectPayload6()
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

            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 2,
                Name = "t2",
                Port = 80,
                RootDirectory = "C:\\backup\\incoming",
                Url = "http:\\\\fake-backup-location.com"
            });

            // generate a payload for the final block (index 8)

            file.ResetCopyState(targets);
            file.CopyState[targets[0].ID].LastCompletedFileChunkIndex = 7;
            file.CopyState[targets[0].ID].TargetStatus = OzetteLibrary.Models.FileStatus.InProgress;

            file.CopyState[targets[1].ID].LastCompletedFileChunkIndex = 7;
            file.CopyState[targets[1].ID].TargetStatus = OzetteLibrary.Models.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(9, payload.TotalBlocks);
                Assert.AreEqual(8, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationTargetIDs);
                Assert.AreEqual(2, payload.DestinationTargetIDs.Count);
                Assert.AreEqual(1, payload.DestinationTargetIDs[0]);
                Assert.AreEqual(2, payload.DestinationTargetIDs[1]);

                Assert.AreEqual(102809, payload.Data.Length); // partial chunk

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[]
                {
                    250, 27, 250, 217, 229, 10, 217, 143, 211, 205, 186, 171, 83, 35, 218, 172, 40, 4, 138, 110
                }));
            }
        }

        [TestMethod()]
        public void ClientFileGenerateNextTransferPayloadReturnsCorrectPayload7()
        {
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Models.ClientFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Models.FileBackupPriority.Low);
            var targets = new OzetteLibrary.Models.Targets();

            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 3,
                Name = "t3",
                Port = 80,
                RootDirectory = "C:\\backup\\incoming",
                Url = "http:\\\\fake-backup-location.com"
            });

            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 4,
                Name = "t4",
                Port = 80,
                RootDirectory = "C:\\backup\\incoming",
                Url = "http:\\\\fake-backup-location.com"
            });

            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 5,
                Name = "t5",
                Port = 80,
                RootDirectory = "C:\\backup\\incoming",
                Url = "http:\\\\fake-backup-location.com"
            });

            // generate a payload for a block in the middle (index 5)

            file.ResetCopyState(targets);
            file.CopyState[targets[0].ID].LastCompletedFileChunkIndex = 4;
            file.CopyState[targets[0].ID].TargetStatus = OzetteLibrary.Models.FileStatus.InProgress;

            file.CopyState[targets[1].ID].LastCompletedFileChunkIndex = 4;
            file.CopyState[targets[1].ID].TargetStatus = OzetteLibrary.Models.FileStatus.InProgress;

            file.CopyState[targets[2].ID].LastCompletedFileChunkIndex = 4;
            file.CopyState[targets[2].ID].TargetStatus = OzetteLibrary.Models.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(9, payload.TotalBlocks);
                Assert.AreEqual(5, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationTargetIDs);
                Assert.AreEqual(3, payload.DestinationTargetIDs.Count);
                Assert.AreEqual(3, payload.DestinationTargetIDs[0]);
                Assert.AreEqual(4, payload.DestinationTargetIDs[1]);
                Assert.AreEqual(5, payload.DestinationTargetIDs[2]);

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferChunkSizeBytes, payload.Data.Length);

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[]
                {
                    87, 235, 10, 73, 101, 181, 223, 125, 207, 62, 245, 133, 49, 181, 131, 199, 111, 104, 153, 89
                }));
            }
        }

        [TestMethod()]
        public void ClientFileGenerateNextTransferPayloadReturnsCorrectPayload8()
        {
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Models.ClientFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Models.FileBackupPriority.Low);
            var targets = new OzetteLibrary.Models.Targets();

            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 3,
                Name = "t3",
                Port = 80,
                RootDirectory = "C:\\backup\\incoming",
                Url = "http:\\\\fake-backup-location.com"
            });

            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 4,
                Name = "t4",
                Port = 80,
                RootDirectory = "C:\\backup\\incoming",
                Url = "http:\\\\fake-backup-location.com"
            });

            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 5,
                Name = "t5",
                Port = 80,
                RootDirectory = "C:\\backup\\incoming",
                Url = "http:\\\\fake-backup-location.com"
            });

            // generate a payload for a block in the middle (index 5)

            file.ResetCopyState(targets);
            file.CopyState[targets[0].ID].LastCompletedFileChunkIndex = 8;
            file.CopyState[targets[0].ID].TargetStatus = OzetteLibrary.Models.FileStatus.Synced;

            file.CopyState[targets[1].ID].LastCompletedFileChunkIndex = 4;
            file.CopyState[targets[1].ID].TargetStatus = OzetteLibrary.Models.FileStatus.InProgress;

            file.CopyState[targets[2].ID].LastCompletedFileChunkIndex = 4;
            file.CopyState[targets[2].ID].TargetStatus = OzetteLibrary.Models.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(9, payload.TotalBlocks);
                Assert.AreEqual(5, payload.CurrentBlockNumber);

                Assert.IsNotNull(payload.DestinationTargetIDs);
                Assert.AreEqual(2, payload.DestinationTargetIDs.Count);
                Assert.AreEqual(4, payload.DestinationTargetIDs[0]);
                Assert.AreEqual(5, payload.DestinationTargetIDs[1]);

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferChunkSizeBytes, payload.Data.Length);

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[]
                {
                    87, 235, 10, 73, 101, 181, 223, 125, 207, 62, 245, 133, 49, 181, 131, 199, 111, 104, 153, 89
                }));
            }
        }

        [TestMethod()]
        public void ClientFileGenerateNextTransferPayloadReturnsCorrectPayload9()
        {
            var hasher = new OzetteLibrary.Crypto.Hasher(new OzetteLibrary.Logging.Mock.MockLogger());
            var file = new OzetteLibrary.Models.ClientFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Models.FileBackupPriority.Low);
            var targets = new OzetteLibrary.Models.Targets();

            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 3,
                Name = "t3",
                Port = 80,
                RootDirectory = "C:\\backup\\incoming",
                Url = "http:\\\\fake-backup-location.com"
            });

            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 4,
                Name = "t4",
                Port = 80,
                RootDirectory = "C:\\backup\\incoming",
                Url = "http:\\\\fake-backup-location.com"
            });

            targets.Add(new OzetteLibrary.Models.Target()
            {
                ID = 5,
                Name = "t5",
                Port = 80,
                RootDirectory = "C:\\backup\\incoming",
                Url = "http:\\\\fake-backup-location.com"
            });

            // generate a payload for a block in the middle (index 5)

            file.ResetCopyState(targets);
            file.CopyState[targets[0].ID].LastCompletedFileChunkIndex = 8;
            file.CopyState[targets[0].ID].TargetStatus = OzetteLibrary.Models.FileStatus.Synced;

            file.CopyState[targets[1].ID].LastCompletedFileChunkIndex = 6;
            file.CopyState[targets[1].ID].TargetStatus = OzetteLibrary.Models.FileStatus.InProgress;

            file.CopyState[targets[2].ID].LastCompletedFileChunkIndex = 4;
            file.CopyState[targets[2].ID].TargetStatus = OzetteLibrary.Models.FileStatus.InProgress;

            using (var filestream = new FileStream(file.FullSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var payload = file.GenerateNextTransferPayload(filestream, hasher);

                Assert.IsNotNull(payload);

                Assert.AreEqual(9, payload.TotalBlocks);
                Assert.AreEqual(5, payload.CurrentBlockNumber);

                // ensure we are taking the minimum block number that could be sent.

                Assert.IsNotNull(payload.DestinationTargetIDs);
                Assert.AreEqual(1, payload.DestinationTargetIDs.Count);
                Assert.AreEqual(5, payload.DestinationTargetIDs[0]);

                Assert.AreEqual(OzetteLibrary.Constants.Transfers.TransferChunkSizeBytes, payload.Data.Length);

                Assert.IsTrue(hasher.CheckTwoByteHashesAreTheSame(payload.ExpectedHash, new byte[]
                {
                    87, 235, 10, 73, 101, 181, 223, 125, 207, 62, 245, 133, 49, 181, 131, 199, 111, 104, 153, 89
                }));
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClientFileSetBlockAsSentThrowsOnInvalidBlockNumber()
        {
            var targetIDs = new List<int>() { 1, 2, 3 };
            var file = new OzetteLibrary.Models.ClientFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Models.FileBackupPriority.Low);

            file.SetBlockAsSent(-5, targetIDs);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClientFileSetBlockAsSentThrowsOnInvalidDestinations1()
        {
            var file = new OzetteLibrary.Models.ClientFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Models.FileBackupPriority.Low);

            file.SetBlockAsSent(3, null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClientFileSetBlockAsSentThrowsOnInvalidDestinations2()
        {
            var targetIDs = new List<int>() { };
            var file = new OzetteLibrary.Models.ClientFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Models.FileBackupPriority.Low);

            file.SetBlockAsSent(2, targetIDs);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ClientFileSetBlockAsSentThrowsOnFileAlreadySynced()
        {
            var targetIDs = new List<int>() { 1, 2, 3 };
            var file = new OzetteLibrary.Models.ClientFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Models.FileBackupPriority.Low);
            file.OverallState = OzetteLibrary.Models.FileStatus.Synced;

            file.SetBlockAsSent(2, targetIDs);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ClientFileSetBlockAsSentThrowsOnMissingCopyState()
        {
            var targetIDs = new List<int>() { 1, 2, 3 };
            var file = new OzetteLibrary.Models.ClientFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Models.FileBackupPriority.Low);
            file.OverallState = OzetteLibrary.Models.FileStatus.InProgress;

            file.SetBlockAsSent(2, targetIDs);
        }

        [TestMethod()]
        public void ClientFileSetBlockAsSentCorrectlySetsCopyStateAndOverallState1()
        {
            var targets = new OzetteLibrary.Models.Targets();
            targets.Add(new OzetteLibrary.Models.Target() { ID = 1, Name = "t1" });

            var targetIDs = new List<int>() { targets[0].ID };

            var file = new OzetteLibrary.Models.ClientFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Models.FileBackupPriority.Low);
            file.ResetCopyState(targets);

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, file.OverallState);

            // this example file has 9 total blocks

            file.SetBlockAsSent(0, targetIDs);

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.InProgress, file.OverallState);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.InProgress, file.CopyState[targets[0].ID].TargetStatus);
        }

        [TestMethod()]
        public void ClientFileSetBlockAsSentCorrectlySetsCopyStateAndOverallState2()
        {
            var targets = new OzetteLibrary.Models.Targets();
            targets.Add(new OzetteLibrary.Models.Target() { ID = 1, Name = "t1" });

            var targetIDs = new List<int>() { targets[0].ID };

            var file = new OzetteLibrary.Models.ClientFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Models.FileBackupPriority.Low);
            file.ResetCopyState(targets);

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, file.OverallState);

            // this example file has 9 total blocks

            file.SetBlockAsSent(4, targetIDs);

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.InProgress, file.OverallState);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.InProgress, file.CopyState[targets[0].ID].TargetStatus);
        }

        [TestMethod()]
        public void ClientFileSetBlockAsSentCorrectlySetsCopyStateAndOverallState3()
        {
            var targets = new OzetteLibrary.Models.Targets();
            targets.Add(new OzetteLibrary.Models.Target() { ID = 1, Name = "t1" });

            var targetIDs = new List<int>() { targets[0].ID };

            var file = new OzetteLibrary.Models.ClientFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Models.FileBackupPriority.Low);
            file.ResetCopyState(targets);

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, file.OverallState);

            // this example file has 9 total blocks

            file.SetBlockAsSent(8, targetIDs);

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.InProgress, file.OverallState);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.InProgress, file.CopyState[targets[0].ID].TargetStatus);
        }

        [TestMethod()]
        public void ClientFileSetBlockAsSentCorrectlySetsCopyStateAndOverallState4()
        {
            var targets = new OzetteLibrary.Models.Targets();
            targets.Add(new OzetteLibrary.Models.Target() { ID = 1, Name = "t1" });

            var targetIDs = new List<int>() { targets[0].ID };

            var file = new OzetteLibrary.Models.ClientFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Models.FileBackupPriority.Low);
            file.ResetCopyState(targets);

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, file.OverallState);

            // this example file has 9 total blocks

            file.SetBlockAsSent(9, targetIDs);

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Synced, file.OverallState);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Synced, file.CopyState[targets[0].ID].TargetStatus);
        }

        [TestMethod()]
        public void ClientFileSetBlockAsSentCorrectlySetsCopyStateAndOverallState5()
        {
            var targets = new OzetteLibrary.Models.Targets();
            targets.Add(new OzetteLibrary.Models.Target() { ID = 1, Name = "t1" });
            targets.Add(new OzetteLibrary.Models.Target() { ID = 2, Name = "t2" });
            targets.Add(new OzetteLibrary.Models.Target() { ID = 3, Name = "t3" });

            var targetIDs = new List<int>() { targets[0].ID, targets[1].ID, targets[2].ID };

            var file = new OzetteLibrary.Models.ClientFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Models.FileBackupPriority.Low);
            file.ResetCopyState(targets);

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, file.OverallState);

            // this example file has 9 total blocks

            file.SetBlockAsSent(2, targetIDs);

            // state: all are in progress

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.InProgress, file.OverallState);

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.InProgress, file.CopyState[targets[0].ID].TargetStatus);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.InProgress, file.CopyState[targets[1].ID].TargetStatus);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.InProgress, file.CopyState[targets[2].ID].TargetStatus);
        }

        [TestMethod()]
        public void ClientFileSetBlockAsSentCorrectlySetsCopyStateAndOverallState6()
        {
            var targets = new OzetteLibrary.Models.Targets();
            targets.Add(new OzetteLibrary.Models.Target() { ID = 1, Name = "t1" });
            targets.Add(new OzetteLibrary.Models.Target() { ID = 2, Name = "t2" });
            targets.Add(new OzetteLibrary.Models.Target() { ID = 3, Name = "t3" });

            var targetIDs = new List<int>() { targets[2].ID };

            var file = new OzetteLibrary.Models.ClientFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Models.FileBackupPriority.Low);
            file.ResetCopyState(targets);

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, file.OverallState);

            // this example file has 9 total blocks

            file.SetBlockAsSent(5, targetIDs);

            // state: at least one in progress

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.InProgress, file.OverallState);

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, file.CopyState[targets[0].ID].TargetStatus);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, file.CopyState[targets[1].ID].TargetStatus);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.InProgress, file.CopyState[targets[2].ID].TargetStatus);
        }

        [TestMethod()]
        public void ClientFileSetBlockAsSentCorrectlySetsCopyStateAndOverallState7()
        {
            var targets = new OzetteLibrary.Models.Targets();
            targets.Add(new OzetteLibrary.Models.Target() { ID = 1, Name = "t1" });
            targets.Add(new OzetteLibrary.Models.Target() { ID = 2, Name = "t2" });
            targets.Add(new OzetteLibrary.Models.Target() { ID = 3, Name = "t3" });

            var targetIDs = new List<int>() { targets[0].ID, targets[1].ID, targets[2].ID };

            var file = new OzetteLibrary.Models.ClientFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Models.FileBackupPriority.Low);
            file.ResetCopyState(targets);

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, file.OverallState);

            // this example file has 9 total blocks

            file.SetBlockAsSent(9, targetIDs);

            // state: all targets synced.

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Synced, file.OverallState);

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Synced, file.CopyState[targets[0].ID].TargetStatus);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Synced, file.CopyState[targets[1].ID].TargetStatus);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Synced, file.CopyState[targets[2].ID].TargetStatus);
        }

        [TestMethod()]
        public void ClientFileSetBlockAsSentCorrectlySetsCopyStateAndOverallState8()
        {
            var targets = new OzetteLibrary.Models.Targets();
            targets.Add(new OzetteLibrary.Models.Target() { ID = 1, Name = "t1" });
            targets.Add(new OzetteLibrary.Models.Target() { ID = 2, Name = "t2" });
            targets.Add(new OzetteLibrary.Models.Target() { ID = 3, Name = "t3" });
            
            var file = new OzetteLibrary.Models.ClientFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Models.FileBackupPriority.Low);
            file.ResetCopyState(targets);

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, file.OverallState);

            // this example file has 9 total blocks

            file.CopyState[targets[2].ID].TargetStatus = OzetteLibrary.Models.FileStatus.OutOfDate;
            file.SetBlockAsSent(9, new List<int>() { targets[0].ID });
            file.SetBlockAsSent(9, new List<int>() { targets[1].ID });

            // state: something is out of date on at least one target.

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.OutOfDate, file.OverallState);

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Synced, file.CopyState[targets[0].ID].TargetStatus);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Synced, file.CopyState[targets[1].ID].TargetStatus);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.OutOfDate, file.CopyState[targets[2].ID].TargetStatus);
        }

        [TestMethod()]
        public void ClientFileSetBlockAsSentCorrectlySetsCopyStateAndOverallState9()
        {
            var targets = new OzetteLibrary.Models.Targets();
            targets.Add(new OzetteLibrary.Models.Target() { ID = 1, Name = "t1" });
            targets.Add(new OzetteLibrary.Models.Target() { ID = 2, Name = "t2" });
            targets.Add(new OzetteLibrary.Models.Target() { ID = 3, Name = "t3" });

            var file = new OzetteLibrary.Models.ClientFile(new FileInfo(".\\TestFiles\\Hasher\\MediumFile.mp3"), OzetteLibrary.Models.FileBackupPriority.Low);
            file.ResetCopyState(targets);

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, file.OverallState);

            // this example file has 9 total blocks

            file.CopyState[targets[2].ID].TargetStatus = OzetteLibrary.Models.FileStatus.Unsynced;
            file.SetBlockAsSent(9, new List<int>() { targets[0].ID });
            file.SetBlockAsSent(9, new List<int>() { targets[1].ID });

            // state: something is unsynced on at least one target.

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, file.OverallState);

            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Synced, file.CopyState[targets[0].ID].TargetStatus);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Synced, file.CopyState[targets[1].ID].TargetStatus);
            Assert.AreEqual(OzetteLibrary.Models.FileStatus.Unsynced, file.CopyState[targets[2].ID].TargetStatus);
        }
    }
}
