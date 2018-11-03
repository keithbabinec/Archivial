using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzetteLibrary.CommandLine;
using OzetteLibrary.CommandLine.Arguments;
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
            ArgumentBase parsed;

            var parser = new Parser();

            Assert.IsFalse(parser.Parse(arguments, out parsed));
        }

        [TestMethod]
        public void ParserCanParseInstallCommandWithDefaults()
        {
            string[] arguments = { "install" };
            ArgumentBase parsed;

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
            ArgumentBase parsed;

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
            ArgumentBase parsed;

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
            ArgumentBase parsed;

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
            ArgumentBase parsed;

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
            ArgumentBase parsed;

            var parser = new Parser();

            Assert.IsFalse(parser.Parse(arguments, out parsed));
            Assert.IsNull(parsed);
        }

        [TestMethod]
        public void ParserReturnsFalseWhenConfigureAzureIsMissingStorageAccount()
        {
            string[] arguments = { "configure-azure", "--azurestorageaccounttoken", "mytoken" };
            ArgumentBase parsed;

            var parser = new Parser();

            Assert.IsFalse(parser.Parse(arguments, out parsed));
            Assert.IsNull(parsed);
        }

        [TestMethod]
        public void ParserReturnsFalseWhenConfigureAzureIsMissingStorageToken()
        {
            string[] arguments = { "configure-azure", "--azurestorageaccountname", "myaccount" };
            ArgumentBase parsed;

            var parser = new Parser();

            Assert.IsFalse(parser.Parse(arguments, out parsed));
            Assert.IsNull(parsed);
        }

        [TestMethod]
        public void ParserReturnsFalseWhenAddSourceHasNoArgsPassed()
        {
            string[] arguments = { "add-localsource" };
            ArgumentBase parsed;

            var parser = new Parser();

            Assert.IsFalse(parser.Parse(arguments, out parsed));
            Assert.IsNull(parsed);
        }

        [TestMethod]
        public void ParserCanParseAddSourceCommandWithOnlyRequiredArgs()
        {
            string[] arguments = { "add-localsource", "--folderpath", "C:\\test\\folder" };
            ArgumentBase parsed;

            var parser = new Parser();

            Assert.IsTrue(parser.Parse(arguments, out parsed));
            Assert.IsInstanceOfType(parsed, typeof(AddLocalSourceArguments));

            var sourceArgs = parsed as AddLocalSourceArguments;

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
            string[] arguments = { "add-localsource", "--folderpath", "C:\\test\\folder", "--priority", "high", "--revisions", "3", "--matchfilter", "*.mp3" };
            ArgumentBase parsed;

            var parser = new Parser();

            Assert.IsTrue(parser.Parse(arguments, out parsed));
            Assert.IsInstanceOfType(parsed, typeof(AddLocalSourceArguments));

            var sourceArgs = parsed as AddLocalSourceArguments;

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
            string[] arguments = { "add-localsource", "--folderpath", "C:\\test\\folder", "--priority", "low", "--revisions", "not a number", "--matchfilter", "*.mp3" };
            ArgumentBase parsed;

            var parser = new Parser();
            parser.Parse(arguments, out parsed); // should throw due to invalid revision number (must be a number).
        }

        [TestMethod]
        [ExpectedException(typeof(SourceLocationInvalidFileBackupPriorityException))]
        public void ParserShouldThrowExceptionWhenAddSourceHasInvalidPriority()
        {
            string[] arguments = { "add-localsource", "--folderpath", "C:\\test\\folder", "--priority", "critical", "--revisions", "3", "--matchfilter", "*.mp3" };
            ArgumentBase parsed;

            var parser = new Parser();
            parser.Parse(arguments, out parsed); // should throw due to 'critical' file backup priority (not a valid value).
        }

        [TestMethod]
        public void ParserReturnsFalseWhenAddNetSourceHasNoArgsPassed()
        {
            string[] arguments = { "add-netsource" };
            ArgumentBase parsed;

            var parser = new Parser();

            Assert.IsFalse(parser.Parse(arguments, out parsed));
            Assert.IsNull(parsed);
        }

        [TestMethod]
        public void ParserCanParseAddNetSourceCommandWithOnlyRequiredArgs()
        {
            string[] arguments = { "add-netsource", "--uncpath", "\\\\networkshare\\public\\media\\playlists" };
            ArgumentBase parsed;

            var parser = new Parser();

            Assert.IsTrue(parser.Parse(arguments, out parsed));
            Assert.IsInstanceOfType(parsed, typeof(AddNetSourceArguments));

            var sourceArgs = parsed as AddNetSourceArguments;

            // pass through
            Assert.AreEqual("\\\\networkshare\\public\\media\\playlists", sourceArgs.UncPath);

            // defaults
            Assert.AreEqual(OzetteLibrary.Constants.CommandLine.DefaultSourceMatchFilter, sourceArgs.Matchfilter);
            Assert.AreEqual(OzetteLibrary.Constants.CommandLine.DefaultSourcePriority, sourceArgs.Priority);
            Assert.AreEqual(OzetteLibrary.Constants.CommandLine.DefaultSourceRevisionCount, sourceArgs.Revisions);
        }

        [TestMethod]
        public void ParserCanParseAddNetSourceCommandWithOptionalArgs()
        {
            string[] arguments = { "add-netsource", "--uncpath", "\\\\networkshare\\public\\media\\playlists", "--credentialname", "network-device-name", "--priority", "high", "--revisions", "3", "--matchfilter", "*.m3u" };
            ArgumentBase parsed;

            var parser = new Parser();

            Assert.IsTrue(parser.Parse(arguments, out parsed));
            Assert.IsInstanceOfType(parsed, typeof(AddNetSourceArguments));

            var sourceArgs = parsed as AddNetSourceArguments;

            // pass through
            Assert.AreEqual("\\\\networkshare\\public\\media\\playlists", sourceArgs.UncPath);
            Assert.AreEqual("network-device-name", sourceArgs.CredentialName);
            Assert.AreEqual("*.m3u", sourceArgs.Matchfilter);
            Assert.AreEqual(OzetteLibrary.Files.FileBackupPriority.High, sourceArgs.Priority);
            Assert.AreEqual(3, sourceArgs.Revisions);
        }

        [TestMethod]
        [ExpectedException(typeof(SourceLocationInvalidRevisionCountException))]
        public void ParserShouldThrowExceptionWhenAddNetSourceHasInvalidRevision()
        {
            string[] arguments = { "add-netsource", "--uncpath", "\\\\networkshare\\public\\media\\playlists", "--credentialname", "network-device-name", "--priority", "low", "--revisions", "not a number", "--matchfilter", "*.m3u" };
            ArgumentBase parsed;

            var parser = new Parser();
            parser.Parse(arguments, out parsed); // should throw due to invalid revision number (must be a number).
        }

        [TestMethod]
        [ExpectedException(typeof(SourceLocationInvalidFileBackupPriorityException))]
        public void ParserShouldThrowExceptionWhenAddNetSourceHasInvalidPriority()
        {
            string[] arguments = { "add-netsource", "--uncpath", "\\\\networkshare\\public\\media\\playlists", "--credentialname", "network-device-name", "--priority", "critical", "--revisions", "3", "--matchfilter", "*.m3u" };
            ArgumentBase parsed;

            var parser = new Parser();
            parser.Parse(arguments, out parsed); // should throw due to 'critical' file backup priority (not a valid value).
        }

        [TestMethod]
        public void ParserReturnsFalseWhenRemoveProviderHasNoArgsPassed()
        {
            string[] arguments = { "remove-provider" };
            ArgumentBase parsed;

            var parser = new Parser();

            Assert.IsFalse(parser.Parse(arguments, out parsed));
            Assert.IsNull(parsed);
        }

        [TestMethod]
        public void ParserReturnsFalseWhenRemoveSourceHasNoArgsPassed()
        {
            string[] arguments = { "remove-source" };
            ArgumentBase parsed;

            var parser = new Parser();

            Assert.IsFalse(parser.Parse(arguments, out parsed));
            Assert.IsNull(parsed);
        }

        [TestMethod]
        public void ParserReturnsFalseWhenRemoveProviderWhenInvalidIdPassed()
        {
            string[] arguments = { "remove-provider", "--providerid", "nope" };
            ArgumentBase parsed;

            var parser = new Parser();

            Assert.IsFalse(parser.Parse(arguments, out parsed));
            Assert.IsNull(parsed);
        }

        [TestMethod]
        public void ParserReturnsFalseWhenRemoveSourceWhenInvalidIdPassed()
        {
            string[] arguments = { "remove-source", "--sourceid", "nope" };
            ArgumentBase parsed;

            var parser = new Parser();

            Assert.IsFalse(parser.Parse(arguments, out parsed));
            Assert.IsNull(parsed);
        }

        [TestMethod]
        public void ParserCanParseRemoveProviderCommandWithExpectedArgs()
        {
            string[] arguments = { "remove-provider", "--providerid", "1" };
            ArgumentBase parsed;

            var parser = new Parser();

            Assert.IsTrue(parser.Parse(arguments, out parsed));
            Assert.IsNotNull(parsed);
            Assert.IsInstanceOfType(parsed, typeof(RemoveProviderArguments));
            Assert.AreEqual(1, (parsed as RemoveProviderArguments).ProviderID);
        }

        [TestMethod]
        public void ParserCanParseRemoveSourceCommandWithExpectedArgs()
        {
            string[] arguments = { "remove-source", "--sourceid", "3" };
            ArgumentBase parsed;

            var parser = new Parser();

            Assert.IsTrue(parser.Parse(arguments, out parsed));
            Assert.IsNotNull(parsed);
            Assert.IsInstanceOfType(parsed, typeof(RemoveSourceArguments));
            Assert.AreEqual(3, (parsed as RemoveSourceArguments).SourceID);
        }
    }
}
