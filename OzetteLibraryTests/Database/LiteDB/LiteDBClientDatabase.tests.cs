using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace OzetteLibraryTests.Database.LiteDB
{
    [TestClass()]
    public class LiteDBClientDatabaseTests
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LiteDBClientDatabaseConstructorThrowsWhenNoLoggerIsProvided()
        {
            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db = 
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(new MemoryStream(), null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LiteDBClientDatabaseConstructorThrowsWhenNoDatabaseStreamIsProvided()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            MemoryStream ms = null;

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void LiteDBClientDatabaseConstructorThrowsWhenNoDatabaseFileNameIsProvided()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            string dbname = null;

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(dbname, logger);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseCanBeInstantiatedWithMemoryStream()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();
            var mem = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db = 
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(mem, logger);

            Assert.IsNotNull(db);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseCanBeInstantiatedWithFileName()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();
            var dbname = "database.db";

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(dbname, logger);

            Assert.IsNotNull(db);
        }
    }
}
