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
        public void LiteDBClientDatabaseConstructorThrowsWhenNoDatabaseConnectionStringIsProvided()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            string dbConString = null;

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(dbConString, logger);
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
        public void LiteDBClientDatabaseCanBeInstantiatedWithConnectionString()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();
            var dbConString = "fake-connection-string";

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(dbConString, logger);

            Assert.IsNotNull(db);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseCanRunPrepareDatabaseWithoutExceptions()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();
        }

        [TestMethod()]
        public void LiteDBClientDatabaseCanRunPrepareDatabaseMultipleTimesWithoutExceptions()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();
            db.PrepareDatabase();
            db.PrepareDatabase();
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseGetClientFileThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.GetClientFile(null, null, null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseGetTargetsThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.GetTargets();
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseAddClientFileThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.AddClientFile(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LiteDBClientDatabaseUpdateClientFileThrowsIfPrepareDatabaseHasNotBeenCalled()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.UpdateClientFile(null);
        }

        [TestMethod()]
        public void LiteDBClientDatabaseGetTargetsReturnsEmptyCollectionInsteadOfNull()
        {
            OzetteLibrary.Logging.Mock.MockLogger logger = new OzetteLibrary.Logging.Mock.MockLogger();

            var ms = new MemoryStream();

            OzetteLibrary.Database.LiteDB.LiteDBClientDatabase db =
                new OzetteLibrary.Database.LiteDB.LiteDBClientDatabase(ms, logger);

            db.PrepareDatabase();

            var result = db.GetTargets();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }
    }
}
