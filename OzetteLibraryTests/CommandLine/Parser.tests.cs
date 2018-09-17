using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzetteLibrary.CommandLine;
using OzetteLibrary.Exceptions;

namespace OzetteLibraryTests.CommandLine
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void ParserReturnsFalseWhenNoArgsProvided()
        {
            string[] arguments = { };
            Arguments parsed;

            var parser = new Parser();

            Assert.IsFalse(parser.Parse(arguments, out parsed));
        }

        [TestMethod]
        public void ParserCanParseInstallCommandWithDefaults()
        {
            string[] arguments = { "install" };
            Arguments parsed;

            var parser = new Parser();

            Assert.IsTrue(parser.Parse(arguments, out parsed));
            Assert.IsInstanceOfType(parsed, typeof(InstallationArguments));

            var installArgs = parsed as InstallationArguments;

            Assert.AreEqual(OzetteLibrary.Constants.CommandLine.DefaultInstallLocation, installArgs.InstallDirectory);
        }

        [TestMethod]
        public void ParserCanParseInstallCommandWithUppercaseAndDefaults()
        {
            string[] arguments = { "INSTALL" };
            Arguments parsed;

            var parser = new Parser();

            Assert.IsTrue(parser.Parse(arguments, out parsed));
            Assert.IsInstanceOfType(parsed, typeof(InstallationArguments));

            var installArgs = parsed as InstallationArguments;

            Assert.AreEqual(OzetteLibrary.Constants.CommandLine.DefaultInstallLocation, installArgs.InstallDirectory);
        }

        [TestMethod]
        public void ParserCanParseInstallCommandWithCustomInstallDir()
        {
            string[] arguments = { "install", "--installdirectory", "C:\\path" };
            Arguments parsed;

            var parser = new Parser();

            Assert.IsTrue(parser.Parse(arguments, out parsed));
            Assert.IsInstanceOfType(parsed, typeof(InstallationArguments));

            var installArgs = parsed as InstallationArguments;

            Assert.AreEqual("C:\\path", installArgs.InstallDirectory);
        }

        [TestMethod]
        public void ParserCanParseInstallCommandWithCustomInstallDirUppercaseArgname()
        {
            string[] arguments = { "install", "--INSTALLDIRECTORY", "C:\\path" };
            Arguments parsed;

            var parser = new Parser();

            Assert.IsTrue(parser.Parse(arguments, out parsed));
            Assert.IsInstanceOfType(parsed, typeof(InstallationArguments));

            var installArgs = parsed as InstallationArguments;

            Assert.AreEqual("C:\\path", installArgs.InstallDirectory);
        }

        [TestMethod]
        public void ParserCanParseConfigureAzureCommandWithBothArgs()
        {
            string[] arguments = { "configure-azure", "--azurestorageaccountname", "myaccount", "--azurestorageaccounttoken", "mytoken" };
            Arguments parsed;

            var parser = new Parser();

            Assert.IsTrue(parser.Parse(arguments, out parsed));
            Assert.IsInstanceOfType(parsed, typeof(ConfigureAzureArguments));

            var installArgs = parsed as ConfigureAzureArguments;

            Assert.AreEqual("myaccount", installArgs.AzureStorageAccountName);
            Assert.AreEqual("mytoken", installArgs.AzureStorageAccountToken);
        }

        [TestMethod]
        public void ParserReturnsFalseWhenConfigureAzureHasNoArgsPassed()
        {
            string[] arguments = { "configure-azure" };
            Arguments parsed;

            var parser = new Parser();

            Assert.IsFalse(parser.Parse(arguments, out parsed));
            Assert.IsNull(parsed);
        }

        [TestMethod]
        public void ParserReturnsFalseWhenConfigureAzureIsMissingStorageAccount()
        {
            string[] arguments = { "configure-azure", "--azurestorageaccounttoken", "mytoken" };
            Arguments parsed;

            var parser = new Parser();

            Assert.IsFalse(parser.Parse(arguments, out parsed));
            Assert.IsNull(parsed);
        }

        [TestMethod]
        public void ParserReturnsFalseWhenConfigureAzureIsMissingStorageToken()
        {
            string[] arguments = { "configure-azure", "--azurestorageaccountname", "myaccount" };
            Arguments parsed;

            var parser = new Parser();

            Assert.IsFalse(parser.Parse(arguments, out parsed));
            Assert.IsNull(parsed);
        }

        [TestMethod]
        public void ParserReturnsFalseWhenAddSourceHasNoArgsPassed()
        {
            string[] arguments = { "add-source" };
            Arguments parsed;

            var parser = new Parser();

            Assert.IsFalse(parser.Parse(arguments, out parsed));
            Assert.IsNull(parsed);
        }

        [TestMethod]
        public void ParserCanParseAddSourceCommandWithOnlyRequiredArgs()
        {
            string[] arguments = { "add-source", "--folderpath", "C:\\test\\folder" };
            Arguments parsed;

            var parser = new Parser();

            Assert.IsTrue(parser.Parse(arguments, out parsed));
            Assert.IsInstanceOfType(parsed, typeof(AddSourceArguments));

            var sourceArgs = parsed as AddSourceArguments;

            // pass through
            Assert.AreEqual("C:\\test\\folder", sourceArgs.FolderPath);

            // defaults
            Assert.AreEqual(OzetteLibrary.Constants.CommandLine.DefaultSourceMatchFilter, sourceArgs.Matchfilter);
            Assert.AreEqual(OzetteLibrary.Constants.CommandLine.DefaultSourcePriority, sourceArgs.Priority);
            Assert.AreEqual(OzetteLibrary.Constants.CommandLine.DefaultSourceRevisionCount, sourceArgs.Revisions);
        }

        [TestMethod]
        public void ParserCanParseAddSourceCommandWithOptionalArgs()
        {
            string[] arguments = { "add-source", "--folderpath", "C:\\test\\folder", "--priority", "high", "--revisions", "3", "--matchfilter", "*.mp3" };
            Arguments parsed;

            var parser = new Parser();

            Assert.IsTrue(parser.Parse(arguments, out parsed));
            Assert.IsInstanceOfType(parsed, typeof(AddSourceArguments));

            var sourceArgs = parsed as AddSourceArguments;

            // pass through
            Assert.AreEqual("C:\\test\\folder", sourceArgs.FolderPath);
            Assert.AreEqual("*.mp3", sourceArgs.Matchfilter);
            Assert.AreEqual(OzetteLibrary.Files.FileBackupPriority.High, sourceArgs.Priority);
            Assert.AreEqual(3, sourceArgs.Revisions);
        }

        [TestMethod]
        [ExpectedException(typeof(SourceLocationInvalidRevisionCountException))]
        public void ParserShouldThrowExceptionWhenAddSourceHasInvalidRevision()
        {
            string[] arguments = { "add-source", "--folderpath", "C:\\test\\folder", "--priority", "low", "--revisions", "not a number", "--matchfilter", "*.mp3" };
            Arguments parsed;

            var parser = new Parser();
            parser.Parse(arguments, out parsed); // should throw due to invalid revision number (must be a number).
        }

        [TestMethod]
        [ExpectedException(typeof(SourceLocationInvalidFileBackupPriorityException))]
        public void ParserShouldThrowExceptionWhenAddSourceHasInvalidPriority()
        {
            string[] arguments = { "add-source", "--folderpath", "C:\\test\\folder", "--priority", "critical", "--revisions", "3", "--matchfilter", "*.mp3" };
            Arguments parsed;

            var parser = new Parser();
            parser.Parse(arguments, out parsed); // should throw due to 'critical' file backup priority (not a valid value).
        }
    }
}
