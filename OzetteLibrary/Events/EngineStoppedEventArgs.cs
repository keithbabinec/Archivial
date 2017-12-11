using System;

namespace OzetteLibrary.Events
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
        /// Default/empty constructor.
        /// </summary>
        public EngineStoppedEventArgs()
        {
        }

        /// <summary>
        /// Constructor that accepts an <c>EngineStoppedReason</c>.
        /// </summary>
        /// <param name="reason">Reason for stopping.</param>
        public EngineStoppedEventArgs(EngineStoppedReason reason)
        {
            Reason = reason;
        }

        /// <summary>
        /// Constructor that accepts an <c>Exception</c>.
        /// </summary>
        /// <remarks>
        /// The stop reason is automatically set to 'Failed'.
        /// </remarks>
        /// <param name="reason">Reason for stopping.</param>
        public EngineStoppedEventArgs(Exception ex)
        {
            Reason = EngineStoppedReason.Failed;
            Exception = ex;
        }
    }
}
