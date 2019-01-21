using OzetteLibrary.Folders;

namespace OzetteLibrary.CommandLine.Arguments
{
    /// <summary>
    /// A set of Ozette source removal arguments.
    /// </summary>
    public class RemoveSourceArguments : ArgumentBase
    {
        /// <summary>
        /// The ID of the source to remove.
        /// </summary>
        public int SourceID { get; set; }

        /// <summary>
        /// The type of source location to remove.
        /// </summary>
        public SourceLocationType SourceType { get; set; }
    }
}
