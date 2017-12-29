using System;
using OzetteLibrary.Database;
using OzetteLibrary.Events;
using OzetteLibrary.Logging;
using OzetteLibrary.Models;

namespace OzetteLibrary.Client.Transfer
{
    public class FileSender
    {
        /// <summary>
        /// A constructor that accepts a database and logger.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="logger"></param>
        public FileSender(IClientDatabase database, ILogger logger)
        {
            if (database == null)
            {
                throw new ArgumentNullException(nameof(database));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Database = database;
            Logger = logger;
        }

        /// <summary>
        /// A reference to the database.
        /// </summary>
        private IClientDatabase Database { get; set; }

        /// <summary>
        /// A reference to the logger.
        /// </summary>
        private ILogger Logger { get; set; }

        /// <summary>
        /// Begins a file-send operation.
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>
        public AsyncResult BeginSend(ClientFile File)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stops the in-progress file send operation.
        /// </summary>
        public void StopSend()
        {
            throw new NotImplementedException();
        }
    }
}
