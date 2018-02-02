namespace OzetteLibrary.Models
{
    /// <summary>
    /// Describes the possible availability states of a target.
    /// </summary>
    public enum TargetAvailabilityState
    {
        /// <summary>
        /// The default state: unknown (we haven't checked).
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The target is available for communication.
        /// </summary>
        Available = 1,

        /// <summary>
        /// The target is failed.
        /// </summary>
        Failed = 2,

        /// <summary>
        /// The target is disabled.
        /// </summary>
        Disabled = 3
    }
}
