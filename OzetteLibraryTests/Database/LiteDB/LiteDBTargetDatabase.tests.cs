using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace OzetteLibraryTests.Database.LiteDB
{
    [TestClass()]
    public class LiteDBTargetDatabaseTests
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LiteDBTargetDatabaseConstructorThrowsWhenNoLoggerIsProvided()
        {
            OzetteLibrary.Database.LiteDB.LiteDBTargetDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBTargetDatabase(new MemoryStream(), null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LiteDBTargetDatabaseConstructorThrowsWhenNoDatabaseStreamIsProvided()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            MemoryStream ms = null;

            OzetteLibrary.Database.LiteDB.LiteDBTargetDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBTargetDatabase(ms, logger);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void LiteDBTargetDatabaseConstructorThrowsWhenNoDatabaseConnectionStringIsProvided()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            string dbConString = null;

            OzetteLibrary.Database.LiteDB.LiteDBTargetDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBTargetDatabase(dbConString, logger);
        }

        [TestMethod()]
        public void LiteDBTargetDatabaseCanBeInstantiatedWithMemoryStream()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();
            var mem = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBTargetDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBTargetDatabase(mem, logger);

            Assert.IsNotNull(db);
        }

        [TestMethod()]
        public void LiteDBTargetDatabaseCanBeInstantiatedWithConnectionString()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();
            var dbConString = "fake-connection-string";

            OzetteLibrary.Database.LiteDB.LiteDBTargetDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBTargetDatabase(dbConString, logger);

            Assert.IsNotNull(db);
        }
    }
}
