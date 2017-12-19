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
    }
}
