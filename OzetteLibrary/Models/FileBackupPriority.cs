namespace OzetteLibrary.Models
{
    /// <summary>
    /// Describes the possible priorities for a file to be backed up.
    /// </summary>
    public enum FileBackupPriority
    {
        /// <summary>
        /// The default state: unset.
        /// </summary>
        Unset = 0,

        /// <summary>
        /// Low priority.
        /// </summary>
        Low = 1,

        /// <summary>
        /// Medium priority.
        /// </summary>
        Medium = 2,

        /// <summary>
        /// High priority.
        /// </summary>
        High = 3
    }
}
