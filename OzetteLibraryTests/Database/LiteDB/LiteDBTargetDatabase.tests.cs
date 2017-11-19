using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace OzetteLibraryTests.Database.LiteDB
{
    [TestClass()]
    public class LiteDBTargetDatabaseTests
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LiteDBTargetDatabaseConstructorThrowsWhenNoLoggerIsProvided()
        {
            OzetteLibrary.Database.LiteDB.LiteDBTargetDatabase db = new OzetteLibrary.Database.LiteDB.LiteDBTargetDatabase(null);
        }

        [TestMethod()]
        public void LiteDBTargetDatabaseCanBeInstantiated()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();
            OzetteLibrary.Database.LiteDB.LiteDBTargetDatabase db = new OzetteLibrary.Database.LiteDB.LiteDBTargetDatabase(logger);
            Assert.IsNotNull(db);
        }
    }
}
