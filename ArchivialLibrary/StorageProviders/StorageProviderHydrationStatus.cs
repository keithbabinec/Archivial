namespace ArchivialLibrary.StorageProviders
{
    /// <summary>
    /// Describes possible target cloud providers.
    /// </summary>
    public enum StorageProviderHydrationStatus
    {
        /// <summary>
        /// No status (not moving).
        /// </summary>
        None = 1,

        /// <summary>
        /// Moving from an active tier into a low-access/archive tier.
        /// </summary>
        MovingToArchiveTier = 2,

        /// <summary>
        /// Moving from a low-access/archive tier into an active tier.
        /// </summary>
        MovingToActiveTier = 3
    }
}
