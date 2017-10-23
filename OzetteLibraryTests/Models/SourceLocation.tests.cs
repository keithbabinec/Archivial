using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzetteLibrary.Models.Exceptions;
using System;

namespace OzetteLibraryTests.Models
{
    [TestClass()]
    public class SourceLocationTests
    {
        [TestMethod()]
        [ExpectedException(typeof(SourceLocationInvalidFolderPathException))]
        public void SourceLocationValidateThrowsExceptionWhenInvalidFolderPathIsProvided()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.Validate();
        }

        [TestMethod()]
        [ExpectedException(typeof(SourceLocationInvalidFolderPathException))]
        public void SourceLocationValidateThrowsExceptionWhenInvalidFolderPathIsProvided2()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory + "\\somefolderthatdoesntexist";
            loc.Validate();
        }

        [TestMethod()]
        [ExpectedException(typeof(SourceLocationInvalidFileMatchFilterException))]
        public void SourceLocationValidateThrowsExceptionWhenInvalidFileMatchPatternIsProvided()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.FileMatchFilter = "aaaa";
            loc.Validate();
        }

        [TestMethod()]
        [ExpectedException(typeof(SourceLocationInvalidFileMatchFilterException))]
        public void SourceLocationValidateThrowsExceptionWhenInvalidFileMatchPatternIsProvided2()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.FileMatchFilter = "test.mp3";
            loc.Validate();
        }

        [TestMethod()]
        [ExpectedException(typeof(SourceLocationInvalidRevisionCountException))]
        public void SourceLocationValidateThrowsExceptionWhenInvalidRevisionCountProvided()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 0;
            loc.Validate();
        }

        [TestMethod()]
        [ExpectedException(typeof(SourceLocationInvalidRevisionCountException))]
        public void SourceLocationValidateThrowsExceptionWhenInvalidRevisionCountProvided2()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = -15;
            loc.Validate();
        }

        [TestMethod()]
        public void SourceLocationValidatePassesValidExample1()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 1;
            loc.Validate();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SourceLocationValidatePassesValidExample2()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 10;
            loc.Validate();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SourceLocationValidatePassesValidExample3()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 12345678;
            loc.Validate();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SourceLocationValidatePassesValidExample4()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 1;
            loc.FileMatchFilter = "";
            loc.Validate();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SourceLocationValidatePassesValidExample5()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 1;
            loc.FileMatchFilter = "*";
            loc.Validate();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SourceLocationValidatePassesValidExample6()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 1;
            loc.FileMatchFilter = "*.*";
            loc.Validate();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SourceLocationValidatePassesValidExample7()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 1;
            loc.FileMatchFilter = "test*";
            loc.Validate();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SourceLocationValidatePassesValidExample8()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 1;
            loc.FileMatchFilter = "test*.doc";
            loc.Validate();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SourceLocationValidatePassesValidExample9()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 1;
            loc.FileMatchFilter = "*.doc";
            loc.Validate();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SourceLocationValidatePassesValidExample10()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 1;
            loc.FileMatchFilter = "test.*";
            loc.Validate();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SourceLocationValidatePassesValidExample11()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 1;
            loc.FileMatchFilter = "t?st";
            loc.Validate();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SourceLocationValidatePassesValidExample12()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 1;
            loc.FileMatchFilter = "t?st.doc";
            loc.Validate();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SourceLocationValidatePassesValidExample13()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 1;
            loc.FileMatchFilter = "t?st.*";
            loc.Validate();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SourceLocationValidatePassesValidExample14()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 1;
            loc.FileMatchFilter = "t?st.do?";
            loc.Validate();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SourceLocationValidatePassesValidExample15()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 1;
            loc.FileMatchFilter = "*.d?";
            loc.Validate();

            Assert.IsTrue(true);
        }
    }
}
