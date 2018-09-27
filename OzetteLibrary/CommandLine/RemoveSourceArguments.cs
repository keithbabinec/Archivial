namespace OzetteLibrary.CommandLine
{
    /// <summary>
    /// A set of Ozette source removal arguments.
    /// </summary>
    public class RemoveSourceArguments : Arguments
    {
        /// <summary>
        /// The ID of the source to remove.
        /// </summary>
        public int SourceID { get; set; }
    }
}
