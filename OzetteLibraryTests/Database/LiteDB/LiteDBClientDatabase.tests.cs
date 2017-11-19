using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace OzetteLibraryTests.Database.LiteDB
{
    [TestClass()]
    public class LiteDBClientDatabaseTests
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LiteDBClientDatabaseConstructorThrowsWhenNoLoggerIsProvided()
        {
            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db = new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(null);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseCanBeInstantiated()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();
            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db = new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(logger);
            Assert.IsNotNull(db);
        }
    }
}
