using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzetteLibrary.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzetteLibraryTests.Database
{
    [TestClass]
    public class DatabaseBackupSchedulerTests
    {
        DatabaseBackupScheduler Scheduler { get; set; }

        [TestInitialize]
        public void TestSetup()
        {
            Scheduler = new DatabaseBackupScheduler();
        }

        [TestMethod]
        public void NextDatabaseBackupCorrectlyReturnsFullBackupIfNoBackupsTaken()
        {
            var recentBackups = new DatabaseBackupStatus()
            {
                LastFullBackup = null,
                LastDifferentialBackup = null,
                LastTransactionLogBackup = null
            };

            var result = Scheduler.NextDatabaseBackup(recentBackups);

            Assert.IsNotNull(result);
            Assert.AreEqual(DatabaseBackupType.Full, result.Value);
        }

        [TestMethod]
        public void NextDatabaseBackupCorrectlyReturnsNoBackupWhenNoBackupsNeedToBeTaken()
        {
            var now = DateTime.Now;

            var recentBackups = new DatabaseBackupStatus()
            {
                LastFullBackup = now.AddHours(-23),
                LastDifferentialBackup = now.AddHours(-3),
                LastTransactionLogBackup = now.AddMinutes(-29)
            };

            var result = Scheduler.NextDatabaseBackup(recentBackups);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void NextDatabaseBackupCorrectlyReturnsFullBackupIfNoFullBackupIn24Hours()
        {
            var now = DateTime.Now;

            var recentBackups = new DatabaseBackupStatus()
            {
                LastFullBackup = now.AddHours(-24),
                LastDifferentialBackup = now.AddHours(-2),
                LastTransactionLogBackup = now.AddMinutes(-5)
            };

            var result = Scheduler.NextDatabaseBackup(recentBackups);

            Assert.IsNotNull(result);
            Assert.AreEqual(DatabaseBackupType.Full, result.Value);
        }

        [TestMethod]
        public void NextDatabaseBackupCorrectlyReturnsDifferentialBackupIfNoDiffBackupsIn4Hours()
        {
            var now = DateTime.Now;

            var recentBackups = new DatabaseBackupStatus()
            {
                LastFullBackup = now.AddHours(-22),
                LastDifferentialBackup = now.AddHours(-4),
                LastTransactionLogBackup = now.AddMinutes(-5)
            };

            var result = Scheduler.NextDatabaseBackup(recentBackups);

            Assert.IsNotNull(result);
            Assert.AreEqual(DatabaseBackupType.Differential, result.Value);
        }

        [TestMethod]
        public void NextDatabaseBackupCorrectlyReturnsLogBackupIfNoLogBackupsIn30Minutes()
        {
            var now = DateTime.Now;

            var recentBackups = new DatabaseBackupStatus()
            {
                LastFullBackup = now.AddHours(-20),
                LastDifferentialBackup = now.AddHours(-3),
                LastTransactionLogBackup = now.AddMinutes(-30)
            };

            var result = Scheduler.NextDatabaseBackup(recentBackups);

            Assert.IsNotNull(result);
            Assert.AreEqual(DatabaseBackupType.TransactionLog, result.Value);
        }

        [TestMethod]
        public void NextDatabaseBackupCorrectlyPrioritizesLastFullOverLastDifferential()
        {
            var now = DateTime.Now;

            var recentBackups = new DatabaseBackupStatus()
            {
                LastFullBackup = now.AddHours(-25),
                LastDifferentialBackup = now.AddHours(-5),
                LastTransactionLogBackup = now.AddMinutes(-35)
            };

            var result = Scheduler.NextDatabaseBackup(recentBackups);

            Assert.IsNotNull(result);
            Assert.AreEqual(DatabaseBackupType.Full, result.Value);
        }

        [TestMethod]
        public void NextDatabaseBackupCorrectlyPrioritizesLastDifferentialOverLastTransactionLog()
        {
            var now = DateTime.Now;

            var recentBackups = new DatabaseBackupStatus()
            {
                LastFullBackup = now.AddHours(-20),
                LastDifferentialBackup = now.AddHours(-5),
                LastTransactionLogBackup = now.AddMinutes(-35)
            };

            var result = Scheduler.NextDatabaseBackup(recentBackups);

            Assert.IsNotNull(result);
            Assert.AreEqual(DatabaseBackupType.Differential, result.Value);
        }
    }
}
