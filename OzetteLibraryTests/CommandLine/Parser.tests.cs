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
            Assert.AreEqual(OzetteLibrary.Constants.CommandLine.DefaultDatabaseFilePath, installArgs.DatabasePath);
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
            Assert.AreEqual(OzetteLibrary.Constants.CommandLine.DefaultDatabaseFilePath, installArgs.DatabasePath);
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
            Assert.AreEqual(OzetteLibrary.Constants.CommandLine.DefaultDatabaseFilePath, installArgs.DatabasePath);
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
            Assert.AreEqual(OzetteLibrary.Constants.CommandLine.DefaultDatabaseFilePath, installArgs.DatabasePath);
        }

        [TestMethod]
        public void ParserCanParseInstallCommandWithCustomDbFilepath()
        {
            string[] arguments = { "install", "--databasepath", "C:\\path\\to\\my.db" };
            Arguments parsed;

            var parser = new Parser();

            Assert.IsTrue(parser.Parse(arguments, out parsed));
            Assert.IsInstanceOfType(parsed, typeof(InstallationArguments));

            var installArgs = parsed as InstallationArguments;

            Assert.AreEqual(OzetteLibrary.Constants.CommandLine.DefaultInstallLocation, installArgs.InstallDirectory);
            Assert.AreEqual("C:\\path\\to\\my.db", installArgs.DatabasePath);
        }

        [TestMethod]
        public void ParserCanParseInstallCommandWithBothCustomArgs()
        {
            string[] arguments = { "install", "--databasepath", "C:\\path\\to\\my.db", "--installdirectory", "C:\\path" };
            Arguments parsed;

            var parser = new Parser();

            Assert.IsTrue(parser.Parse(arguments, out parsed));
            Assert.IsInstanceOfType(parsed, typeof(InstallationArguments));

            var installArgs = parsed as InstallationArguments;

            Assert.AreEqual("C:\\path", installArgs.InstallDirectory);
            Assert.AreEqual("C:\\path\\to\\my.db", installArgs.DatabasePath);
        }
    }
}
