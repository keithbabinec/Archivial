using System;

namespace ArchivialLibrary.Events
{
    /// <summary>
    /// A custom event args object for when an engine stops.
    /// </summary>
    public class EngineStoppedEventArgs : EventArgs
    {
        /// <summary>
        /// An exception, if thrown.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// The stop reason.
        /// </summary>
        public EngineStoppedReason Reason { get; set; }

        /// <summary>
        /// The ID of the engine instance.
        /// </summary>
        public int EngineID { get; set; }

        /// <summary>
        /// Default/empty constructor.
        /// </summary>
        public EngineStoppedEventArgs()
        {
        }

        /// <summary>
        /// Constructor that accepts an <c>EngineStoppedReason</c>.
        /// </summary>
        /// <param name="reason">Reason for stopping.</param>
        /// <param name="engineID">The engine instance ID.</param>
        public EngineStoppedEventArgs(EngineStoppedReason reason, int engineID)
        {
            Reason = reason;
            EngineID = engineID;
        }

        /// <summary>
        /// Constructor that accepts an <c>Exception</c>.
        /// </summary>
        /// <remarks>
        /// The stop reason is automatically set to 'Failed'.
        /// </remarks>
        /// <param name="reason">Reason for stopping.</param>
        /// <param name="engineID">The engine instance ID.</param>
        public EngineStoppedEventArgs(Exception ex, int engineID)
        {
            Reason = EngineStoppedReason.Failed;
            Exception = ex;
            EngineID = engineID;
        }
    }
}
