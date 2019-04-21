namespace ArchivialLibrary.Constants
{
    /// <summary>
    /// A constants class for Database operations.
    /// </summary>
    public static class Database
    {
        /// <summary>
        /// A string constant for the name of the database.
        /// </summary>
        public const string DatabaseName = "ArchivialDB";

        /// <summary>
        /// A string constant for the sqlexpress default instance.
        /// </summary>
        public const string DefaultSqlExpressInstanceConnectionString = "Data Source=.\\SQLExpress;Initial Catalog=Master;Integrated Security=SSPI;";

        /// <summary>
        /// A string constant for the sqlexpress user account.
        /// </summary>
        public const string DefaultSqlExpressUserAccount = "NT SERVICE\\MSSQL$SQLEXPRESS";
    }
}
