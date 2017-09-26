namespace OzetteLibrary.Database
{
    /// <summary>
    /// The target agent database implementation.
    /// </summary>
    public class TargetDB : BaseDB
    {
        /// <summary>
        /// Constructor that takes a database file path.
        /// </summary>
        /// <param name="databaseFilePath"></param>
        public TargetDB(string databaseFilePath) : base(databaseFilePath)
        {
        }
    }
}
