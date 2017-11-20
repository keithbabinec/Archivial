namespace OzetteLibrary.Database
{
    /// <summary>
    /// A generic database interface for the target database.
    /// </summary>
    public interface ITargetDatabase : IDatabase
    {
        /// <summary>
        /// Runs database preparation steps.
        /// </summary>
        /// <remarks>
        /// This step prepares any tables, indexes, identity mappings, etc. for use.
        /// This action is idempotent (safe to run over and over), but should be run once at each application startup.
        /// </remarks>
        void PrepareDatabase();
    }
}
