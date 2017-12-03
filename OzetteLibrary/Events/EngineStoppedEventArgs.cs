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
    }
}
