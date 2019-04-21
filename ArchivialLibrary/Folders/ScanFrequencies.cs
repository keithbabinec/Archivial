namespace ArchivialLibrary.Folders
{
    /// <summary>
    /// Describes an options class for how often scans should occur.
    /// </summary>
    public class ScanFrequencies
    {
        /// <summary>
        /// The low priority scan timeframe.
        /// </summary>
        public int LowPriorityScanFrequencyInHours { get; set; }

        /// <summary>
        /// The medium priority scan timeframe.
        /// </summary>
        public int MedPriorityScanFrequencyInHours { get; set; }

        /// <summary>
        /// The high priority scan timeframe.
        /// </summary>
        public int HighPriorityScanFrequencyInHours { get; set; }

        /// <summary>
        /// The meta priority scan timeframe.
        /// </summary>
        public int MetaPriorityScanFrequencyInHours { get; set; }
    }
}
