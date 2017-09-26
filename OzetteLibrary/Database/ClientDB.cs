namespace OzetteLibrary.Database
{
    /// <summary>
    /// The client agent database implementation.
    /// </summary>
    public class ClientDB : BaseDB
    {
        /// <summary>
        /// Constructor that takes a database file path.
        /// </summary>
        /// <param name="databaseFilePath"></param>
        public ClientDB(string databaseFilePath) : base(databaseFilePath)
        {
        }
    }
}
