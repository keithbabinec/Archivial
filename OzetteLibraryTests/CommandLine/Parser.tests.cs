using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzetteLibrary.CommandLine;

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
        public void ParserCanParseConfigureEncryptionCommandWithValidArgs()
        {
            string[] arguments = { "configure-encryption", "--protectioniv", "mykey" };
            Arguments parsed;

            var parser = new Parser();

            Assert.IsTrue(parser.Parse(arguments, out parsed));
            Assert.IsInstanceOfType(parsed, typeof(ConfigureEncryptionArguments));

            var installArgs = parsed as ConfigureEncryptionArguments;

            Assert.AreEqual("mykey", installArgs.ProtectionIv);
        }

        [TestMethod]
        public void ParserReturnsFalseWhenConfigureEncryptionHasNoArgsPassed()
        {
            string[] arguments = { "configure-encryption" };
            Arguments parsed;

            var parser = new Parser();

            Assert.IsFalse(parser.Parse(arguments, out parsed));
            Assert.IsNull(parsed);
        }

        [TestMethod]
        public void ParserReturnsFalseWhenConfigureEncryptionWhenWrongOptionProvided()
        {
            string[] arguments = { "configure-encryption", "--wrongoption", "mykey" };
            Arguments parsed;

            var parser = new Parser();

            Assert.IsFalse(parser.Parse(arguments, out parsed));
            Assert.IsNull(parsed);
        }
    }
}
