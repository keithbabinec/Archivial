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

            Assert.AreEqual("C:\\dir01", sources[0].FolderPath);
            Assert.AreEqual(FileBackupPriority.High, sources[0].Priority);

            Assert.AreEqual("C:\\dir02", sources[1].FolderPath);
            Assert.AreEqual(FileBackupPriority.Medium, sources[1].Priority);

            Assert.AreEqual("C:\\dir03", sources[2].FolderPath);
            Assert.AreEqual(FileBackupPriority.Low, sources[2].Priority);

            Assert.AreEqual("C:\\dir04", sources[3].FolderPath);
            Assert.AreEqual(FileBackupPriority.Low, sources[3].Priority);

            Assert.AreEqual("C:\\dir05", sources[4].FolderPath);
            Assert.AreEqual(FileBackupPriority.High, sources[4].Priority);

            Assert.AreEqual("C:\\dir06", sources[5].FolderPath);
            Assert.AreEqual(FileBackupPriority.Low, sources[5].Priority);

            Assert.AreEqual("C:\\dir07", sources[6].FolderPath);
            Assert.AreEqual(FileBackupPriority.High, sources[6].Priority);

            Assert.AreEqual("C:\\dir08", sources[7].FolderPath);
            Assert.AreEqual(FileBackupPriority.Medium, sources[7].Priority);

            Assert.AreEqual("C:\\dir09", sources[8].FolderPath);
            Assert.AreEqual(FileBackupPriority.High, sources[8].Priority);

            Assert.AreEqual("C:\\dir10", sources[9].FolderPath);
            Assert.AreEqual(FileBackupPriority.Medium, sources[9].Priority);

            Assert.AreEqual("C:\\dir11", sources[10].FolderPath);
            Assert.AreEqual(FileBackupPriority.Low, sources[10].Priority);

            Assert.AreEqual("C:\\dir12", sources[11].FolderPath);
            Assert.AreEqual(FileBackupPriority.High, sources[11].Priority);
        }

        [TestMethod()]
        public void SourcesLoaderCanCorrectlySortByPriority()
        {
            OzetteLibrary.Client.Sources.Loader load = new OzetteLibrary.Client.Sources.Loader();

            var sources = load.LoadSourcesFile(".\\TestFiles\\SourceLocation\\SourcesExample2.json");
            var sorted = load.SortSources(sources);

            Assert.IsNotNull(sorted);
            Assert.IsTrue(sorted.Count == 12);

            Assert.AreEqual("C:\\dir01", sorted[0].FolderPath);
            Assert.AreEqual(FileBackupPriority.High, sorted[0].Priority);

            Assert.AreEqual("C:\\dir05", sorted[1].FolderPath);
            Assert.AreEqual(FileBackupPriority.High, sorted[1].Priority);

            Assert.AreEqual("C:\\dir07", sorted[2].FolderPath);
            Assert.AreEqual(FileBackupPriority.High, sorted[2].Priority);

            Assert.AreEqual("C:\\dir09", sorted[3].FolderPath);
            Assert.AreEqual(FileBackupPriority.High, sorted[3].Priority);

            Assert.AreEqual("C:\\dir12", sorted[4].FolderPath);
            Assert.AreEqual(FileBackupPriority.High, sorted[4].Priority);

            Assert.AreEqual("C:\\dir02", sorted[5].FolderPath);
            Assert.AreEqual(FileBackupPriority.Medium, sorted[5].Priority);

            Assert.AreEqual("C:\\dir08", sorted[6].FolderPath);
            Assert.AreEqual(FileBackupPriority.Medium, sorted[6].Priority);

            Assert.AreEqual("C:\\dir10", sorted[7].FolderPath);
            Assert.AreEqual(FileBackupPriority.Medium, sorted[7].Priority);

            Assert.AreEqual("C:\\dir03", sorted[8].FolderPath);
            Assert.AreEqual(FileBackupPriority.Low, sorted[8].Priority);

            Assert.AreEqual("C:\\dir04", sorted[9].FolderPath);
            Assert.AreEqual(FileBackupPriority.Low, sorted[9].Priority);

            Assert.AreEqual("C:\\dir06", sorted[10].FolderPath);
            Assert.AreEqual(FileBackupPriority.Low, sorted[10].Priority);

            Assert.AreEqual("C:\\dir11", sorted[11].FolderPath);
            Assert.AreEqual(FileBackupPriority.Low, sorted[11].Priority);
        }

        [TestMethod()]
        public void SourcesLoaderCanCorrectlySortByPriorityAndPath()
        {
            OzetteLibrary.Client.Sources.Loader load = new OzetteLibrary.Client.Sources.Loader();

            var sources = load.LoadSourcesFile(".\\TestFiles\\SourceLocation\\SourcesExample3.json");
            var sorted = load.SortSources(sources);

            Assert.IsNotNull(sorted);
            Assert.IsTrue(sorted.Count == 9);

            Assert.AreEqual("C:\\dir1", sorted[0].FolderPath);
            Assert.AreEqual(FileBackupPriority.High, sorted[0].Priority);

            Assert.AreEqual("C:\\dir5", sorted[1].FolderPath);
            Assert.AreEqual(FileBackupPriority.High, sorted[1].Priority);

            Assert.AreEqual("C:\\dir7", sorted[2].FolderPath);
            Assert.AreEqual(FileBackupPriority.High, sorted[2].Priority);

            Assert.AreEqual("C:\\dir9", sorted[3].FolderPath);
            Assert.AreEqual(FileBackupPriority.High, sorted[3].Priority);

            Assert.AreEqual("C:\\dir2", sorted[4].FolderPath);
            Assert.AreEqual(FileBackupPriority.Medium, sorted[4].Priority);

            Assert.AreEqual("C:\\dir8", sorted[5].FolderPath);
            Assert.AreEqual(FileBackupPriority.Medium, sorted[5].Priority);

            Assert.AreEqual("C:\\dir3", sorted[6].FolderPath);
            Assert.AreEqual(FileBackupPriority.Low, sorted[6].Priority);

            Assert.AreEqual("C:\\dir4", sorted[7].FolderPath);
            Assert.AreEqual(FileBackupPriority.Low, sorted[7].Priority);

            Assert.AreEqual("C:\\dir6", sorted[8].FolderPath);
            Assert.AreEqual(FileBackupPriority.Low, sorted[8].Priority);
        }
    }
}
