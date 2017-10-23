using OzetteLibrary.Database;
using OzetteLibrary.Models;
using System;

namespace OzetteLibrary.Client.Sources
{
    /// <summary>
    /// Contains functionality for scanning a source location.
    /// </summary>
    public class Scanner
    {
        /// <summary>
        /// Default constructor that takes a <c>SourceLocation</c> and <c>IDatabase</c> as input.
        /// </summary>
        /// <param name="source"></param>
        public Scanner(SourceLocation source, IDatabase database)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Starts asynchronously scanning a source.
        /// </summary>
        public void BeginScan()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stops scanning a source if it is in-progress.
        /// </summary>
        public void StopScan()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Event triggered when the scan has completed.
        /// </summary>
        public event EventHandler<ScanResults> ScanCompleted;
    }
}
