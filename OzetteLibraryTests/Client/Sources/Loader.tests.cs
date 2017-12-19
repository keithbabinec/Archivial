using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzetteLibrary.Models;
using System;

namespace OzetteLibraryTests.Client.Sources
{
    [TestClass()]
    public class LoaderTests
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void SourcesLoaderThrowsExceptionWhenNoFileIsProvided()
        {
            OzetteLibrary.Client.Sources.Loader load = new OzetteLibrary.Client.Sources.Loader();
            load.LoadSourcesFile(null);
        }

        [TestMethod()]
        public void SourcesLoaderSafelyReturnsEmptySourcesCollectionFromEmptySourceFile()
        {
            OzetteLibrary.Client.Sources.Loader load = new OzetteLibrary.Client.Sources.Loader();

            var sources = load.LoadSourcesFile(".\\TestFiles\\SourceLocation\\EmptySourcesFile.json");

            Assert.IsNotNull(sources);
            Assert.AreEqual(0, sources.Count);
            Assert.AreEqual(typeof(OzetteLibrary.Models.SourceLocations), sources.GetType());
        }

        [TestMethod()]
        public void SourcesLoaderCanLoadExample1()
        {
            OzetteLibrary.Client.Sources.Loader load = new OzetteLibrary.Client.Sources.Loader();

            var sources = load.LoadSourcesFile(".\\TestFiles\\SourceLocation\\SourcesExample1.json");

            Assert.IsNotNull(sources);
            Assert.IsTrue(sources.Count == 2);

            Assert.AreEqual("C:\\documents", sources[0].FolderPath);
            Assert.AreEqual("*", sources[0].FileMatchFilter);
            Assert.AreEqual(FileBackupPriority.High, sources[0].Priority);
            Assert.AreEqual(3, sources[0].RevisionCount);

            Assert.AreEqual("C:\\music", sources[1].FolderPath);
            Assert.AreEqual("*.mp3", sources[1].FileMatchFilter);
            Assert.AreEqual(FileBackupPriority.Medium, sources[1].Priority);
            Assert.AreEqual(1, sources[1].RevisionCount);
        }

        [TestMethod()]
        public void SourcesLoaderCanLoadExample2()
        {
            OzetteLibrary.Client.Sources.Loader load = new OzetteLibrary.Client.Sources.Loader();

            var sources = load.LoadSourcesFile(".\\TestFiles\\SourceLocation\\SourcesExample2.json");

            Assert.IsNotNull(sources);
            Assert.IsTrue(sources.Count == 12);

            Assert.AreEqual("C:\\dir1", sources[0].FolderPath);
            Assert.AreEqual(FileBackupPriority.High, sources[0].Priority);

            Assert.AreEqual("C:\\dir2", sources[1].FolderPath);
            Assert.AreEqual(FileBackupPriority.Medium, sources[1].Priority);

            Assert.AreEqual("C:\\dir3", sources[2].FolderPath);
            Assert.AreEqual(FileBackupPriority.Low, sources[2].Priority);

            Assert.AreEqual("C:\\dir4", sources[3].FolderPath);
            Assert.AreEqual(FileBackupPriority.Low, sources[3].Priority);

            Assert.AreEqual("C:\\dir5", sources[4].FolderPath);
            Assert.AreEqual(FileBackupPriority.High, sources[4].Priority);

            Assert.AreEqual("C:\\dir6", sources[5].FolderPath);
            Assert.AreEqual(FileBackupPriority.Low, sources[5].Priority);

            Assert.AreEqual("C:\\dir7", sources[6].FolderPath);
            Assert.AreEqual(FileBackupPriority.High, sources[6].Priority);

            Assert.AreEqual("C:\\dir8", sources[7].FolderPath);
            Assert.AreEqual(FileBackupPriority.Medium, sources[7].Priority);

            Assert.AreEqual("C:\\dir9", sources[8].FolderPath);
            Assert.AreEqual(FileBackupPriority.High, sources[8].Priority);

            Assert.AreEqual("C:\\dir10", sources[9].FolderPath);
            Assert.AreEqual(FileBackupPriority.Medium, sources[9].Priority);

            Assert.AreEqual("C:\\dir11", sources[10].FolderPath);
            Assert.AreEqual(FileBackupPriority.Low, sources[10].Priority);

            Assert.AreEqual("C:\\dir12", sources[11].FolderPath);
            Assert.AreEqual(FileBackupPriority.High, sources[11].Priority);
        }
    }
}
