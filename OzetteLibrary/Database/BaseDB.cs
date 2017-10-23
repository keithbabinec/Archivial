using LiteDB;

namespace OzetteLibrary.Database
{
    /// <summary>
    /// Base functionality for client and target databases.
    /// </summary>
    public class BaseDB : IDatabase
    {
        /// <summary>
        /// A reference to the database (interface).
        /// </summary>
        private LiteDatabase database { get; set; }

        /// <summary>
        /// Constructor that takes a database file path.
        /// </summary>
        /// <remarks>
        /// Initializes the database if it is not present.
        /// </remarks>
        /// <param name="databaseFilePath"></param>
        public BaseDB(string databaseFilePath)
        {
            database = new LiteDatabase(databaseFilePath);
        }

        /// <summary>
        /// Destructor that cleans up database resources.
        /// </summary>
        ~BaseDB()
        {
            if (database != null)
            {
                database.Dispose();
            }
        }
    }
}
