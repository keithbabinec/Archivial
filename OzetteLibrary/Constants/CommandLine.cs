using OzetteLibrary.Files;

namespace OzetteLibrary.Constants
{
    /// <summary>
    /// Constants for the command line tooling.
    /// </summary>
    public class CommandLine
    {
        /// <summary>
        /// The default scan source priority.
        /// </summary>
        public const FileBackupPriority DefaultSourcePriority = FileBackupPriority.Medium;

        /// <summary>
        /// The default scan source revision count.
        /// </summary>
        public const int DefaultSourceRevisionCount = 1;

        /// <summary>
        /// The default scan source match filter.
        /// </summary>
        public const string DefaultSourceMatchFilter = "*";
    }
}
