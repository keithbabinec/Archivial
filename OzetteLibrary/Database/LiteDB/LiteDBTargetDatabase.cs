using OzetteLibrary.Logging;
using System;

namespace OzetteLibrary.Database.LiteDB
{
    /// <summary>
    /// A LiteDB implementation of the target database.
    /// </summary>
    public class LiteDBTargetDatabase : ITargetDatabase
    {
        /// <summary>
        /// A constructor that requires the logger.
        /// </summary>
        /// <param name="logger"><c>ILogger</c></param>
        public LiteDBTargetDatabase(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Logger = logger;
        }

        /// <summary>
        /// A reference to the logger.
        /// </summary>
        private ILogger Logger;
    }
}
