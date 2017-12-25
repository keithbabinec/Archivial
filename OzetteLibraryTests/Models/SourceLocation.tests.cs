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
        [ExpectedException(typeof(SourceLocationInvalidIDException))]
        public void SourceLocationValidateThrowsExceptionWhenInvalidIDProvided()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 1;
            loc.ID = 0;
            loc.Validate();
        }

        [TestMethod()]
        [ExpectedException(typeof(SourceLocationInvalidIDException))]
        public void SourceLocationValidateThrowsExceptionWhenInvalidIDProvided2()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 1;
            loc.ID = -10;
            loc.Validate();
        }

        [TestMethod()]
        public void SourceLocationValidatePassesValidExample1()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 1;
            loc.ID = 1;
            loc.Validate();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SourceLocationValidatePassesValidExample2()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 10;
            loc.ID = 1;
            loc.Validate();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SourceLocationValidatePassesValidExample3()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 12345678;
            loc.ID = 1;
            loc.Validate();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SourceLocationValidatePassesValidExample4()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 1;
            loc.ID = 1;
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
            loc.ID = 1;
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
            loc.ID = 1;
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
            loc.ID = 1;
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
            loc.ID = 1;
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
            loc.ID = 1;
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
            loc.ID = 1;
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
            loc.ID = 1;
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
            loc.ID = 1;
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
            loc.ID = 1;
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
            loc.ID = 1;
            loc.Validate();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SourceLocationValidatePassesValidExample15()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 1;
            loc.ID = 1;
            loc.FileMatchFilter = "*.d?";
            loc.Validate();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SourceLocationShouldScanExample1()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.LastCompletedScan = null;

            var options = new OzetteLibrary.ServiceCore.ServiceOptions();
            options.LowPriorityScanFrequencyInHours = 72;
            options.MedPriorityScanFrequencyInHours = 24;
            options.HighPriorityScanFrequencyInHours = 1;

            Assert.IsTrue(loc.ShouldScan(options));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SourceLocationShouldScanExample2()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.LastCompletedScan = null;

            loc.ShouldScan(null);
        }

        [TestMethod()]
        public void SourceLocationShouldScanExample3()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.LastCompletedScan = DateTime.Now.AddMinutes(-15);
            loc.Priority = OzetteLibrary.Models.FileBackupPriority.High;

            var options = new OzetteLibrary.ServiceCore.ServiceOptions();
            options.HighPriorityScanFrequencyInHours = 1;

            Assert.IsFalse(loc.ShouldScan(options));
        }

        [TestMethod()]
        public void SourceLocationShouldScanExample4()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.LastCompletedScan = DateTime.Now.AddMinutes(-59);
            loc.Priority = OzetteLibrary.Models.FileBackupPriority.High;

            var options = new OzetteLibrary.ServiceCore.ServiceOptions();
            options.HighPriorityScanFrequencyInHours = 1;

            Assert.IsFalse(loc.ShouldScan(options));
        }

        [TestMethod()]
        public void SourceLocationShouldScanExample5()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.LastCompletedScan = DateTime.Now.AddMinutes(-61);
            loc.Priority = OzetteLibrary.Models.FileBackupPriority.High;

            var options = new OzetteLibrary.ServiceCore.ServiceOptions();
            options.HighPriorityScanFrequencyInHours = 1;

            Assert.IsTrue(loc.ShouldScan(options));
        }

        [TestMethod()]
        public void SourceLocationShouldScanExample6()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.LastCompletedScan = DateTime.Now.AddMinutes(-125);
            loc.Priority = OzetteLibrary.Models.FileBackupPriority.High;

            var options = new OzetteLibrary.ServiceCore.ServiceOptions();
            options.HighPriorityScanFrequencyInHours = 1;

            Assert.IsTrue(loc.ShouldScan(options));
        }

        [TestMethod()]
        public void SourceLocationShouldScanExample7()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.LastCompletedScan = DateTime.Now.AddHours(-15);
            loc.Priority = OzetteLibrary.Models.FileBackupPriority.Medium;

            var options = new OzetteLibrary.ServiceCore.ServiceOptions();
            options.MedPriorityScanFrequencyInHours = 24;

            Assert.IsFalse(loc.ShouldScan(options));
        }

        [TestMethod()]
        public void SourceLocationShouldScanExample8()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.LastCompletedScan = DateTime.Now.AddHours(-23);
            loc.Priority = OzetteLibrary.Models.FileBackupPriority.Medium;

            var options = new OzetteLibrary.ServiceCore.ServiceOptions();
            options.MedPriorityScanFrequencyInHours = 24;

            Assert.IsFalse(loc.ShouldScan(options));
        }

        [TestMethod()]
        public void SourceLocationShouldScanExample9()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.LastCompletedScan = DateTime.Now.AddHours(-25);
            loc.Priority = OzetteLibrary.Models.FileBackupPriority.Medium;

            var options = new OzetteLibrary.ServiceCore.ServiceOptions();
            options.MedPriorityScanFrequencyInHours = 24;

            Assert.IsTrue(loc.ShouldScan(options));
        }

        [TestMethod()]
        public void SourceLocationShouldScanExample10()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.LastCompletedScan = DateTime.Now.AddHours(-125);
            loc.Priority = OzetteLibrary.Models.FileBackupPriority.Medium;

            var options = new OzetteLibrary.ServiceCore.ServiceOptions();
            options.MedPriorityScanFrequencyInHours = 24;

            Assert.IsTrue(loc.ShouldScan(options));
        }

        [TestMethod()]
        public void SourceLocationShouldScanExample11()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.LastCompletedScan = DateTime.Now.AddHours(-15);
            loc.Priority = OzetteLibrary.Models.FileBackupPriority.Low;

            var options = new OzetteLibrary.ServiceCore.ServiceOptions();
            options.LowPriorityScanFrequencyInHours = 72;

            Assert.IsFalse(loc.ShouldScan(options));
        }

        [TestMethod()]
        public void SourceLocationShouldScanExample12()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.LastCompletedScan = DateTime.Now.AddHours(-71);
            loc.Priority = OzetteLibrary.Models.FileBackupPriority.Low;

            var options = new OzetteLibrary.ServiceCore.ServiceOptions();
            options.LowPriorityScanFrequencyInHours = 72;

            Assert.IsFalse(loc.ShouldScan(options));
        }

        [TestMethod()]
        public void SourceLocationShouldScanExample13()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.LastCompletedScan = DateTime.Now.AddHours(-73);
            loc.Priority = OzetteLibrary.Models.FileBackupPriority.Low;

            var options = new OzetteLibrary.ServiceCore.ServiceOptions();
            options.LowPriorityScanFrequencyInHours = 72;

            Assert.IsTrue(loc.ShouldScan(options));
        }

        [TestMethod()]
        public void SourceLocationShouldScanExample14()
        {
            var loc = new OzetteLibrary.Models.SourceLocation();
            loc.LastCompletedScan = DateTime.Now.AddHours(-1250);
            loc.Priority = OzetteLibrary.Models.FileBackupPriority.Low;

            var options = new OzetteLibrary.ServiceCore.ServiceOptions();
            options.LowPriorityScanFrequencyInHours = 72;

            Assert.IsTrue(loc.ShouldScan(options));
        }
    }
}
