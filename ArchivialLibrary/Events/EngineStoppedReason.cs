namespace ArchivialLibrary.Events
{
    /// <summary>
    /// Defines possible reasons that an engine may have stopped.
    /// </summary>
    public enum EngineStoppedReason
    {
        /// <summary>
        /// The stop was requested.
        /// </summary>
        StopRequested = 1,

        /// <summary>
        /// The engine has failed.
        /// </summary>
        Failed = 2
    }
}
