using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzetteLibrary.Exceptions;
using System;

namespace OzetteLibraryTests.Folders
{
    [TestClass()]
    public class SourceLocationTests
    {
        [TestMethod()]
        [ExpectedException(typeof(SourceLocationInvalidFolderPathException))]
        public void SourceLocationValidateThrowsExceptionWhenInvalidFolderPathIsProvided()
        {
            var loc = new OzetteLibrary.Folders.SourceLocation();
            loc.Validate();
        }

        [TestMethod()]
        [ExpectedException(typeof(SourceLocationInvalidFolderPathException))]
        public void SourceLocationValidateThrowsExceptionWhenInvalidFolderPathIsProvided2()
        {
            var loc = new OzetteLibrary.Folders.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory + "\\somefolderthatdoesntexist";
            loc.Validate();
        }

        [TestMethod()]
        [ExpectedException(typeof(SourceLocationInvalidFileMatchFilterException))]
        public void SourceLocationValidateThrowsExceptionWhenInvalidFileMatchPatternIsProvided()
        {
            var loc = new OzetteLibrary.Folders.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.FileMatchFilter = "aaaa";
            loc.Validate();
        }

        [TestMethod()]
        [ExpectedException(typeof(SourceLocationInvalidFileMatchFilterException))]
        public void SourceLocationValidateThrowsExceptionWhenInvalidFileMatchPatternIsProvided2()
        {
            var loc = new OzetteLibrary.Folders.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.FileMatchFilter = "test.mp3";
            loc.Validate();
        }

        [TestMethod()]
        [ExpectedException(typeof(SourceLocationInvalidRevisionCountException))]
        public void SourceLocationValidateThrowsExceptionWhenInvalidRevisionCountProvided()
        {
            var loc = new OzetteLibrary.Folders.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 0;
            loc.Validate();
        }

        [TestMethod()]
        [ExpectedException(typeof(SourceLocationInvalidRevisionCountException))]
        public void SourceLocationValidateThrowsExceptionWhenInvalidRevisionCountProvided2()
        {
            var loc = new OzetteLibrary.Folders.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = -15;
            loc.Validate();
        }

        [TestMethod()]
        [ExpectedException(typeof(SourceLocationInvalidIDException))]
        public void SourceLocationValidateThrowsExceptionWhenInvalidIDProvided()
        {
            var loc = new OzetteLibrary.Folders.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 1;
            loc.ID = 0;
            loc.Validate();
        }

        [TestMethod()]
        [ExpectedException(typeof(SourceLocationInvalidIDException))]
        public void SourceLocationValidateThrowsExceptionWhenInvalidIDProvided2()
        {
            var loc = new OzetteLibrary.Folders.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 1;
            loc.ID = -10;
            loc.Validate();
        }

        [TestMethod()]
        public void SourceLocationValidatePassesValidExample1()
        {
            var loc = new OzetteLibrary.Folders.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 1;
            loc.ID = 1;
            loc.Validate();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SourceLocationValidatePassesValidExample2()
        {
            var loc = new OzetteLibrary.Folders.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 10;
            loc.ID = 1;
            loc.Validate();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SourceLocationValidatePassesValidExample3()
        {
            var loc = new OzetteLibrary.Folders.SourceLocation();
            loc.FolderPath = Environment.CurrentDirectory;
            loc.RevisionCount = 12345678;
            loc.ID = 1;
            loc.Validate();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SourceLocationValidatePassesValidExample4()
        {
            var loc = new OzetteLibrary.Folders.SourceLocation();
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
            var loc = new OzetteLibrary.Folders.SourceLocation();
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
            var loc = new OzetteLibrary.Folders.SourceLocation();
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
            var loc = new OzetteLibrary.Folders.SourceLocation();
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
            var loc = new OzetteLibrary.Folders.SourceLocation();
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
            var loc = new OzetteLibrary.Folders.SourceLocation();
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
            var loc = new OzetteLibrary.Folders.SourceLocation();
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
            var loc = new OzetteLibrary.Folders.SourceLocation();
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
            var loc = new OzetteLibrary.Folders.SourceLocation();
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
            var loc = new OzetteLibrary.Folders.SourceLocation();
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
            var loc = new OzetteLibrary.Folders.SourceLocation();
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
            var loc = new OzetteLibrary.Folders.SourceLocation();
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
            var loc = new OzetteLibrary.Folders.SourceLocation();
            loc.LastCompletedScan = null;

            var options = new OzetteLibrary.Folders.ScanFrequencies();
            options.LowPriorityScanFrequencyInHours = 72;
            options.MedPriorityScanFrequencyInHours = 24;
            options.HighPriorityScanFrequencyInHours = 1;

            Assert.IsTrue(loc.ShouldScan(options));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SourceLocationShouldScanExample2()
        {
            var loc = new OzetteLibrary.Folders.SourceLocation();
            loc.LastCompletedScan = null;

            loc.ShouldScan(null);
        }

        [TestMethod()]
        public void SourceLocationShouldScanExample3()
        {
            var loc = new OzetteLibrary.Folders.SourceLocation();
            loc.LastCompletedScan = DateTime.Now.AddMinutes(-15);
            loc.Priority = OzetteLibrary.Files.FileBackupPriority.High;

            var options = new OzetteLibrary.Folders.ScanFrequencies();
            options.HighPriorityScanFrequencyInHours = 1;

            Assert.IsFalse(loc.ShouldScan(options));
        }

        [TestMethod()]
        public void SourceLocationShouldScanExample4()
        {
            var loc = new OzetteLibrary.Folders.SourceLocation();
            loc.LastCompletedScan = DateTime.Now.AddMinutes(-59);
            loc.Priority = OzetteLibrary.Files.FileBackupPriority.High;

            var options = new OzetteLibrary.Folders.ScanFrequencies();
            options.HighPriorityScanFrequencyInHours = 1;

            Assert.IsFalse(loc.ShouldScan(options));
        }

        [TestMethod()]
        public void SourceLocationShouldScanExample5()
        {
            var loc = new OzetteLibrary.Folders.SourceLocation();
            loc.LastCompletedScan = DateTime.Now.AddMinutes(-61);
            loc.Priority = OzetteLibrary.Files.FileBackupPriority.High;

            var options = new OzetteLibrary.Folders.ScanFrequencies();
            options.HighPriorityScanFrequencyInHours = 1;

            Assert.IsTrue(loc.ShouldScan(options));
        }

        [TestMethod()]
        public void SourceLocationShouldScanExample6()
        {
            var loc = new OzetteLibrary.Folders.SourceLocation();
            loc.LastCompletedScan = DateTime.Now.AddMinutes(-125);
            loc.Priority = OzetteLibrary.Files.FileBackupPriority.High;

            var options = new OzetteLibrary.Folders.ScanFrequencies();
            options.HighPriorityScanFrequencyInHours = 1;

            Assert.IsTrue(loc.ShouldScan(options));
        }

        [TestMethod()]
        public void SourceLocationShouldScanExample7()
        {
            var loc = new OzetteLibrary.Folders.SourceLocation();
            loc.LastCompletedScan = DateTime.Now.AddHours(-15);
            loc.Priority = OzetteLibrary.Files.FileBackupPriority.Medium;

            var options = new OzetteLibrary.Folders.ScanFrequencies();
            options.MedPriorityScanFrequencyInHours = 24;

            Assert.IsFalse(loc.ShouldScan(options));
        }

        [TestMethod()]
        public void SourceLocationShouldScanExample8()
        {
            var loc = new OzetteLibrary.Folders.SourceLocation();
            loc.LastCompletedScan = DateTime.Now.AddHours(-23);
            loc.Priority = OzetteLibrary.Files.FileBackupPriority.Medium;

            var options = new OzetteLibrary.Folders.ScanFrequencies();
            options.MedPriorityScanFrequencyInHours = 24;

            Assert.IsFalse(loc.ShouldScan(options));
        }

        [TestMethod()]
        public void SourceLocationShouldScanExample9()
        {
            var loc = new OzetteLibrary.Folders.SourceLocation();
            loc.LastCompletedScan = DateTime.Now.AddHours(-25);
            loc.Priority = OzetteLibrary.Files.FileBackupPriority.Medium;

            var options = new OzetteLibrary.Folders.ScanFrequencies();
            options.MedPriorityScanFrequencyInHours = 24;

            Assert.IsTrue(loc.ShouldScan(options));
        }

        [TestMethod()]
        public void SourceLocationShouldScanExample10()
        {
            var loc = new OzetteLibrary.Folders.SourceLocation();
            loc.LastCompletedScan = DateTime.Now.AddHours(-125);
            loc.Priority = OzetteLibrary.Files.FileBackupPriority.Medium;

            var options = new OzetteLibrary.Folders.ScanFrequencies();
            options.MedPriorityScanFrequencyInHours = 24;

            Assert.IsTrue(loc.ShouldScan(options));
        }

        [TestMethod()]
        public void SourceLocationShouldScanExample11()
        {
            var loc = new OzetteLibrary.Folders.SourceLocation();
            loc.LastCompletedScan = DateTime.Now.AddHours(-15);
            loc.Priority = OzetteLibrary.Files.FileBackupPriority.Low;

            var options = new OzetteLibrary.Folders.ScanFrequencies();
            options.LowPriorityScanFrequencyInHours = 72;

            Assert.IsFalse(loc.ShouldScan(options));
        }

        [TestMethod()]
        public void SourceLocationShouldScanExample12()
        {
            var loc = new OzetteLibrary.Folders.SourceLocation();
            loc.LastCompletedScan = DateTime.Now.AddHours(-71);
            loc.Priority = OzetteLibrary.Files.FileBackupPriority.Low;

            var options = new OzetteLibrary.Folders.ScanFrequencies();
            options.LowPriorityScanFrequencyInHours = 72;

            Assert.IsFalse(loc.ShouldScan(options));
        }

        [TestMethod()]
        public void SourceLocationShouldScanExample13()
        {
            var loc = new OzetteLibrary.Folders.SourceLocation();
            loc.LastCompletedScan = DateTime.Now.AddHours(-73);
            loc.Priority = OzetteLibrary.Files.FileBackupPriority.Low;

            var options = new OzetteLibrary.Folders.ScanFrequencies();
            options.LowPriorityScanFrequencyInHours = 72;

            Assert.IsTrue(loc.ShouldScan(options));
        }

        [TestMethod()]
        public void SourceLocationShouldScanExample14()
        {
            var loc = new OzetteLibrary.Folders.SourceLocation();
            loc.LastCompletedScan = DateTime.Now.AddHours(-1250);
            loc.Priority = OzetteLibrary.Files.FileBackupPriority.Low;

            var options = new OzetteLibrary.Folders.ScanFrequencies();
            options.LowPriorityScanFrequencyInHours = 72;

            Assert.IsTrue(loc.ShouldScan(options));
        }

        [TestMethod()]
        [ExpectedException(typeof(SourceLocationsDuplicateIDException))]
        public void SourceLocationsValidateThrowsExceptionOnDuplicateIDs()
        {
            var loc1 = new OzetteLibrary.Folders.SourceLocation();
            loc1.FolderPath = Environment.CurrentDirectory;
            loc1.RevisionCount = 1;
            loc1.ID = 1;
            loc1.FileMatchFilter = "*";

            var loc2 = new OzetteLibrary.Folders.SourceLocation();
            loc2.FolderPath = Environment.CurrentDirectory;
            loc2.RevisionCount = 1;
            loc2.ID = 2;
            loc2.FileMatchFilter = "*";

            var loc3 = new OzetteLibrary.Folders.SourceLocation();
            loc3.FolderPath = Environment.CurrentDirectory;
            loc3.RevisionCount = 1;
            loc3.ID = 2;
            loc3.FileMatchFilter = "*";

            var locations = new OzetteLibrary.Folders.SourceLocations();
            locations.Add(loc1);
            locations.Add(loc2);
            locations.Add(loc3);

            // should throw
            locations.Validate();
        }

        [TestMethod()]
        [ExpectedException(typeof(SourceLocationInvalidRevisionCountException))]
        public void SourceLocationsValidateCallsValidateOnSourcesInsideCollection()
        {
            var loc1 = new OzetteLibrary.Folders.SourceLocation();
            loc1.FolderPath = Environment.CurrentDirectory;
            loc1.RevisionCount = 1;
            loc1.ID = 1;
            loc1.FileMatchFilter = "*";

            var loc2 = new OzetteLibrary.Folders.SourceLocation();
            loc2.FolderPath = Environment.CurrentDirectory;
            loc2.RevisionCount = 1;
            loc2.ID = 2;
            loc2.FileMatchFilter = "*";

            var loc3 = new OzetteLibrary.Folders.SourceLocation();
            loc3.FolderPath = Environment.CurrentDirectory;
            loc3.RevisionCount = 0; // this should cause a validation error
            loc3.ID = 3;
            loc3.FileMatchFilter = "*";

            var locations = new OzetteLibrary.Folders.SourceLocations();
            locations.Add(loc1);
            locations.Add(loc2);
            locations.Add(loc3);

            // should throw
            locations.Validate();
        }

        [TestMethod()]
        public void SourceLocationsValidateDoesNotThrowOnAllValidSources()
        {
            var loc1 = new OzetteLibrary.Folders.SourceLocation();
            loc1.FolderPath = Environment.CurrentDirectory;
            loc1.RevisionCount = 1;
            loc1.ID = 1;
            loc1.FileMatchFilter = "*";

            var loc2 = new OzetteLibrary.Folders.SourceLocation();
            loc2.FolderPath = Environment.CurrentDirectory;
            loc2.RevisionCount = 1;
            loc2.ID = 2;
            loc2.FileMatchFilter = "*";

            var loc3 = new OzetteLibrary.Folders.SourceLocation();
            loc3.FolderPath = Environment.CurrentDirectory;
            loc3.RevisionCount = 1;
            loc3.ID = 3;
            loc3.FileMatchFilter = "*";

            var locations = new OzetteLibrary.Folders.SourceLocations();
            locations.Add(loc1);
            locations.Add(loc2);
            locations.Add(loc3);

            // should throw
            locations.Validate();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SourceLocationsValidateDoesNotThrowOnSingleValidSource()
        {
            var loc1 = new OzetteLibrary.Folders.SourceLocation();
            loc1.FolderPath = Environment.CurrentDirectory;
            loc1.RevisionCount = 1;
            loc1.ID = 1;
            loc1.FileMatchFilter = "*";

            var locations = new OzetteLibrary.Folders.SourceLocations();
            locations.Add(loc1);

            // should throw
            locations.Validate();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        [ExpectedException(typeof(SourceLocationInvalidIDException))]
        public void SourceLocationsValidateDoesThrowOnSingleInvalidSource()
        {
            var loc1 = new OzetteLibrary.Folders.SourceLocation();
            loc1.FolderPath = Environment.CurrentDirectory;
            loc1.RevisionCount = 1;
            loc1.ID = 0;
            loc1.FileMatchFilter = "*";

            var locations = new OzetteLibrary.Folders.SourceLocations();
            locations.Add(loc1);

            // should throw
            locations.Validate();

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SourceLocationsCollectionHasSameContentExample1()
        {
            var loc1 = new OzetteLibrary.Folders.SourceLocation();
            loc1.FolderPath = Environment.CurrentDirectory;
            loc1.RevisionCount = 1;
            loc1.ID = 1;
            loc1.FileMatchFilter = "*";

            var loc2 = new OzetteLibrary.Folders.SourceLocation();
            loc2.FolderPath = Environment.CurrentDirectory;
            loc2.RevisionCount = 1;
            loc2.ID = 2;
            loc2.FileMatchFilter = "*";

            var loc3 = new OzetteLibrary.Folders.SourceLocation();
            loc3.FolderPath = Environment.CurrentDirectory;
            loc3.RevisionCount = 1;
            loc3.ID = 3;
            loc3.FileMatchFilter = "*";

            var locations1 = new OzetteLibrary.Folders.SourceLocations();
            locations1.Add(loc1);
            locations1.Add(loc2);
            locations1.Add(loc3);

            var locations2 = new OzetteLibrary.Folders.SourceLocations();
            locations2.Add(loc1);
            locations2.Add(loc2);
            locations2.Add(loc3);

            Assert.IsTrue(locations1.CollectionHasSameContent(locations2));
        }

        [TestMethod()]
        public void SourceLocationsCollectionHasSameContentExample2()
        {
            var loc1 = new OzetteLibrary.Folders.SourceLocation();
            loc1.FolderPath = Environment.CurrentDirectory;
            loc1.RevisionCount = 1;
            loc1.ID = 1;
            loc1.FileMatchFilter = "*";

            var locations1 = new OzetteLibrary.Folders.SourceLocations();
            locations1.Add(loc1);

            var locations2 = new OzetteLibrary.Folders.SourceLocations();
            locations2.Add(loc1);

            Assert.IsTrue(locations1.CollectionHasSameContent(locations2));
        }

        [TestMethod()]
        public void SourceLocationsCollectionHasSameContentExample3()
        {
            var locations1 = new OzetteLibrary.Folders.SourceLocations();
            var locations2 = new OzetteLibrary.Folders.SourceLocations();

            Assert.IsTrue(locations1.CollectionHasSameContent(locations2));
        }

        [TestMethod()]
        public void SourceLocationsCollectionHasSameContentExample4()
        {
            var loc1 = new OzetteLibrary.Folders.SourceLocation();
            loc1.FolderPath = Environment.CurrentDirectory;
            loc1.RevisionCount = 1;
            loc1.ID = 1;
            loc1.FileMatchFilter = "*";

            var loc2 = new OzetteLibrary.Folders.SourceLocation();
            loc2.FolderPath = Environment.CurrentDirectory;
            loc2.RevisionCount = 1;
            loc2.ID = 2;
            loc2.FileMatchFilter = "*";

            var locations1 = new OzetteLibrary.Folders.SourceLocations();
            locations1.Add(loc1);
            locations1.Add(loc2);

            var locations2 = new OzetteLibrary.Folders.SourceLocations();
            locations2.Add(loc1);

            Assert.IsFalse(locations1.CollectionHasSameContent(locations2));
        }

        [TestMethod()]
        public void SourceLocationsCollectionHasSameContentExample5()
        {
            var loc1 = new OzetteLibrary.Folders.SourceLocation();
            loc1.FolderPath = Environment.CurrentDirectory;
            loc1.RevisionCount = 1;
            loc1.ID = 1;
            loc1.FileMatchFilter = "*";

            var loc2 = new OzetteLibrary.Folders.SourceLocation();
            loc2.FolderPath = Environment.CurrentDirectory;
            loc2.RevisionCount = 1;
            loc2.ID = 2;
            loc2.FileMatchFilter = "*";

            var locations1 = new OzetteLibrary.Folders.SourceLocations();
            locations1.Add(loc1);
            
            var locations2 = new OzetteLibrary.Folders.SourceLocations();
            locations2.Add(loc1);
            locations2.Add(loc2);

            Assert.IsFalse(locations1.CollectionHasSameContent(locations2));
        }

        [TestMethod()]
        public void SourceLocationsCollectionHasSameContentExample6()
        {
            var loc1 = new OzetteLibrary.Folders.SourceLocation();
            loc1.FolderPath = Environment.CurrentDirectory;
            loc1.RevisionCount = 1;
            loc1.ID = 1;
            loc1.FileMatchFilter = "*";

            var loc2 = new OzetteLibrary.Folders.SourceLocation();
            loc2.FolderPath = Environment.CurrentDirectory;
            loc2.RevisionCount = 1;
            loc2.ID = 2;
            loc2.FileMatchFilter = "*";

            var loc3 = new OzetteLibrary.Folders.SourceLocation();
            loc3.FolderPath = Environment.CurrentDirectory;
            loc3.RevisionCount = 1;
            loc3.ID = 3;
            loc3.FileMatchFilter = "*";

            var loc4 = new OzetteLibrary.Folders.SourceLocation();
            loc4.FolderPath = Environment.CurrentDirectory;
            loc4.RevisionCount = 1;
            loc4.ID = 4;
            loc4.FileMatchFilter = "*";

            var loc5 = new OzetteLibrary.Folders.SourceLocation();
            loc5.FolderPath = Environment.CurrentDirectory;
            loc5.RevisionCount = 1;
            loc5.ID = 5;
            loc5.FileMatchFilter = "*";

            var loc6 = new OzetteLibrary.Folders.SourceLocation();
            loc6.FolderPath = Environment.CurrentDirectory;
            loc6.RevisionCount = 1;
            loc6.ID = 6;
            loc6.FileMatchFilter = "*";

            var locations1 = new OzetteLibrary.Folders.SourceLocations();
            locations1.Add(loc1);
            locations1.Add(loc2);
            locations1.Add(loc3);

            var locations2 = new OzetteLibrary.Folders.SourceLocations();
            locations2.Add(loc4);
            locations2.Add(loc5);
            locations2.Add(loc6);

            Assert.IsFalse(locations1.CollectionHasSameContent(locations2));
        }

        [TestMethod()]
        public void SourceLocationsCollectionHasSameContentExample7()
        {
            var loc1 = new OzetteLibrary.Folders.SourceLocation();
            loc1.FolderPath = Environment.CurrentDirectory;
            loc1.RevisionCount = 1;
            loc1.ID = 1;
            loc1.FileMatchFilter = "*";

            var loc2 = new OzetteLibrary.Folders.SourceLocation();
            loc2.FolderPath = Environment.CurrentDirectory;
            loc2.RevisionCount = 1;
            loc2.ID = 1;
            loc2.FileMatchFilter = "*.*"; // different

            var locations1 = new OzetteLibrary.Folders.SourceLocations();
            locations1.Add(loc1);

            var locations2 = new OzetteLibrary.Folders.SourceLocations();
            locations2.Add(loc2);

            Assert.IsFalse(locations1.CollectionHasSameContent(locations2));
        }

        [TestMethod()]
        public void SourceLocationsCollectionHasSameContentExample8()
        {
            var loc1 = new OzetteLibrary.Folders.SourceLocation();
            loc1.FolderPath = Environment.CurrentDirectory;
            loc1.RevisionCount = 1;
            loc1.ID = 1;
            loc1.FileMatchFilter = "*";

            var loc2 = new OzetteLibrary.Folders.SourceLocation();
            loc2.FolderPath = Environment.CurrentDirectory;
            loc2.RevisionCount = 2; // different
            loc2.ID = 1;
            loc2.FileMatchFilter = "*"; 

            var locations1 = new OzetteLibrary.Folders.SourceLocations();
            locations1.Add(loc1);

            var locations2 = new OzetteLibrary.Folders.SourceLocations();
            locations2.Add(loc2);

            Assert.IsFalse(locations1.CollectionHasSameContent(locations2));
        }

        [TestMethod()]
        public void SourceLocationsCollectionHasSameContentExample9()
        {
            var loc1 = new OzetteLibrary.Folders.SourceLocation();
            loc1.FolderPath = Environment.CurrentDirectory;
            loc1.RevisionCount = 1;
            loc1.ID = 1;
            loc1.FileMatchFilter = "*";

            var loc2 = new OzetteLibrary.Folders.SourceLocation();
            loc2.FolderPath = Environment.CurrentDirectory + "\\Dir1"; // different
            loc2.RevisionCount = 1; 
            loc2.ID = 1;
            loc2.FileMatchFilter = "*";

            var locations1 = new OzetteLibrary.Folders.SourceLocations();
            locations1.Add(loc1);

            var locations2 = new OzetteLibrary.Folders.SourceLocations();
            locations2.Add(loc2);

            Assert.IsFalse(locations1.CollectionHasSameContent(locations2));
        }

        [TestMethod()]
        public void SourceLocationsCollectionHasSameContentExample10()
        {
            var loc1 = new OzetteLibrary.Folders.SourceLocation();
            loc1.Priority = OzetteLibrary.Files.FileBackupPriority.Medium;
            loc1.FolderPath = Environment.CurrentDirectory;
            loc1.RevisionCount = 1;
            loc1.ID = 1;
            loc1.FileMatchFilter = "*";

            var loc2 = new OzetteLibrary.Folders.SourceLocation();
            loc2.Priority = OzetteLibrary.Files.FileBackupPriority.Low; // different
            loc2.FolderPath = Environment.CurrentDirectory;
            loc2.RevisionCount = 1;
            loc2.ID = 1;
            loc2.FileMatchFilter = "*";

            var locations1 = new OzetteLibrary.Folders.SourceLocations();
            locations1.Add(loc1);

            var locations2 = new OzetteLibrary.Folders.SourceLocations();
            locations2.Add(loc2);

            Assert.IsFalse(locations1.CollectionHasSameContent(locations2));
        }

        [TestMethod()]
        public void SourceLocationsCollectionHasSameContentExample11()
        {
            var loc1A = new OzetteLibrary.Folders.SourceLocation();
            loc1A.FolderPath = Environment.CurrentDirectory;
            loc1A.RevisionCount = 1;
            loc1A.ID = 1;
            loc1A.FileMatchFilter = "*";

            var loc2A = new OzetteLibrary.Folders.SourceLocation();
            loc2A.FolderPath = Environment.CurrentDirectory;
            loc2A.RevisionCount = 1;
            loc2A.ID = 2;
            loc2A.FileMatchFilter = "*";

            var loc3A = new OzetteLibrary.Folders.SourceLocation();
            loc3A.FolderPath = Environment.CurrentDirectory;
            loc3A.RevisionCount = 1;
            loc3A.ID = 3;
            loc3A.FileMatchFilter = "*.mp4"; // different

            var loc1B = new OzetteLibrary.Folders.SourceLocation();
            loc1B.FolderPath = Environment.CurrentDirectory;
            loc1B.RevisionCount = 1;
            loc1B.ID = 1;
            loc1B.FileMatchFilter = "*";

            var loc2B = new OzetteLibrary.Folders.SourceLocation();
            loc2B.FolderPath = Environment.CurrentDirectory;
            loc2B.RevisionCount = 1;
            loc2B.ID = 2;
            loc2B.FileMatchFilter = "*";

            var loc3B = new OzetteLibrary.Folders.SourceLocation();
            loc3B.FolderPath = Environment.CurrentDirectory;
            loc3B.RevisionCount = 1;
            loc3B.ID = 3;
            loc3B.FileMatchFilter = "*";

            var locations1 = new OzetteLibrary.Folders.SourceLocations();
            locations1.Add(loc1A);
            locations1.Add(loc2A);
            locations1.Add(loc3A);

            var locations2 = new OzetteLibrary.Folders.SourceLocations();
            locations2.Add(loc1B);
            locations2.Add(loc2B);
            locations2.Add(loc3B);

            Assert.IsFalse(locations1.CollectionHasSameContent(locations2));
        }

        [TestMethod()]
        public void SourceLocationsCollectionHasSameContentExample12()
        {
            var loc1A = new OzetteLibrary.Folders.SourceLocation();
            loc1A.FolderPath = Environment.CurrentDirectory;
            loc1A.RevisionCount = 1;
            loc1A.ID = 1;
            loc1A.FileMatchFilter = "*";

            var loc2A = new OzetteLibrary.Folders.SourceLocation();
            loc2A.FolderPath = Environment.CurrentDirectory;
            loc2A.RevisionCount = 5; // different
            loc2A.ID = 2;
            loc2A.FileMatchFilter = "*";

            var loc3A = new OzetteLibrary.Folders.SourceLocation();
            loc3A.FolderPath = Environment.CurrentDirectory;
            loc3A.RevisionCount = 1;
            loc3A.ID = 3;
            loc3A.FileMatchFilter = "*";

            var loc1B = new OzetteLibrary.Folders.SourceLocation();
            loc1B.FolderPath = Environment.CurrentDirectory;
            loc1B.RevisionCount = 1;
            loc1B.ID = 1;
            loc1B.FileMatchFilter = "*";

            var loc2B = new OzetteLibrary.Folders.SourceLocation();
            loc2B.FolderPath = Environment.CurrentDirectory;
            loc2B.RevisionCount = 1;
            loc2B.ID = 2;
            loc2B.FileMatchFilter = "*";

            var loc3B = new OzetteLibrary.Folders.SourceLocation();
            loc3B.FolderPath = Environment.CurrentDirectory;
            loc3B.RevisionCount = 1;
            loc3B.ID = 3;
            loc3B.FileMatchFilter = "*";

            var locations1 = new OzetteLibrary.Folders.SourceLocations();
            locations1.Add(loc1A);
            locations1.Add(loc2A);
            locations1.Add(loc3A);

            var locations2 = new OzetteLibrary.Folders.SourceLocations();
            locations2.Add(loc1B);
            locations2.Add(loc2B);
            locations2.Add(loc3B);

            Assert.IsFalse(locations1.CollectionHasSameContent(locations2));
        }

        [TestMethod()]
        public void SourceLocationsCollectionHasSameContentExample13()
        {
            var loc1A = new OzetteLibrary.Folders.SourceLocation();
            loc1A.FolderPath = Environment.CurrentDirectory;
            loc1A.RevisionCount = 1;
            loc1A.ID = 1;
            loc1A.FileMatchFilter = "*";

            var loc2A = new OzetteLibrary.Folders.SourceLocation();
            loc2A.FolderPath = Environment.CurrentDirectory + "\\Dir1"; // different
            loc2A.RevisionCount = 1; 
            loc2A.ID = 2;
            loc2A.FileMatchFilter = "*";

            var loc3A = new OzetteLibrary.Folders.SourceLocation();
            loc3A.FolderPath = Environment.CurrentDirectory;
            loc3A.RevisionCount = 1;
            loc3A.ID = 3;
            loc3A.FileMatchFilter = "*";

            var loc1B = new OzetteLibrary.Folders.SourceLocation();
            loc1B.FolderPath = Environment.CurrentDirectory;
            loc1B.RevisionCount = 1;
            loc1B.ID = 1;
            loc1B.FileMatchFilter = "*";

            var loc2B = new OzetteLibrary.Folders.SourceLocation();
            loc2B.FolderPath = Environment.CurrentDirectory;
            loc2B.RevisionCount = 1;
            loc2B.ID = 2;
            loc2B.FileMatchFilter = "*";

            var loc3B = new OzetteLibrary.Folders.SourceLocation();
            loc3B.FolderPath = Environment.CurrentDirectory;
            loc3B.RevisionCount = 1;
            loc3B.ID = 3;
            loc3B.FileMatchFilter = "*";

            var locations1 = new OzetteLibrary.Folders.SourceLocations();
            locations1.Add(loc1A);
            locations1.Add(loc2A);
            locations1.Add(loc3A);

            var locations2 = new OzetteLibrary.Folders.SourceLocations();
            locations2.Add(loc1B);
            locations2.Add(loc2B);
            locations2.Add(loc3B);

            Assert.IsFalse(locations1.CollectionHasSameContent(locations2));
        }

        [TestMethod()]
        public void SourceLocationsCollectionHasSameContentExample14()
        {
            var loc1A = new OzetteLibrary.Folders.SourceLocation();
            loc1A.FolderPath = Environment.CurrentDirectory;
            loc1A.Priority = OzetteLibrary.Files.FileBackupPriority.High; // different
            loc1A.RevisionCount = 1;
            loc1A.ID = 1;
            loc1A.FileMatchFilter = "*";

            var loc2A = new OzetteLibrary.Folders.SourceLocation();
            loc2A.FolderPath = Environment.CurrentDirectory;
            loc2A.RevisionCount = 1;
            loc2A.ID = 2;
            loc2A.FileMatchFilter = "*";

            var loc3A = new OzetteLibrary.Folders.SourceLocation();
            loc3A.FolderPath = Environment.CurrentDirectory;
            loc3A.RevisionCount = 1;
            loc3A.ID = 3;
            loc3A.FileMatchFilter = "*";

            var loc1B = new OzetteLibrary.Folders.SourceLocation();
            loc1B.FolderPath = Environment.CurrentDirectory;
            loc1B.RevisionCount = 1;
            loc1B.ID = 1;
            loc1B.FileMatchFilter = "*";

            var loc2B = new OzetteLibrary.Folders.SourceLocation();
            loc2B.FolderPath = Environment.CurrentDirectory;
            loc2B.RevisionCount = 1;
            loc2B.ID = 2;
            loc2B.FileMatchFilter = "*";

            var loc3B = new OzetteLibrary.Folders.SourceLocation();
            loc3B.FolderPath = Environment.CurrentDirectory;
            loc3B.RevisionCount = 1;
            loc3B.ID = 3;
            loc3B.FileMatchFilter = "*";

            var locations1 = new OzetteLibrary.Folders.SourceLocations();
            locations1.Add(loc1A);
            locations1.Add(loc2A);
            locations1.Add(loc3A);

            var locations2 = new OzetteLibrary.Folders.SourceLocations();
            locations2.Add(loc1B);
            locations2.Add(loc2B);
            locations2.Add(loc3B);

            Assert.IsFalse(locations1.CollectionHasSameContent(locations2));
        }
    }
}
