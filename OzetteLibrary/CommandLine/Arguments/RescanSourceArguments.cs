using OzetteLibrary.Folders;

namespace OzetteLibrary.CommandLine.Arguments
{
    /// <summary>
    /// A set of Ozette source rescan arguments.
    /// </summary>
    public class RescanSourceArguments : ArgumentBase
    {
        /// <summary>
        /// The ID of the source to rescan.
        /// </summary>
        public int SourceID { get; set; }

        /// <summary>
        /// The type of source location to rescan.
        /// </summary>
        public SourceLocationType SourceType { get; set; }
    }
}
