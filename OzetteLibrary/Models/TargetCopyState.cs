namespace OzetteLibrary.Models
{
    /// <summary>
    /// Contains the state of an individual file against one target (destination).
    /// </summary>
    public class TargetCopyState
    {
        /// <summary>
        /// Default/empty constructor.
        /// </summary>
        public TargetCopyState()
        {
        }

        /// <summary>
        /// A constructor that accepts a <c>Target</c> object.
        /// </summary>
        /// <param name="target"></param>
        public TargetCopyState(Target target)
        {
            TargetID = target.ID;
        }

        /// <summary>
        /// Target this file is associated with.
        /// </summary>
        public int TargetID { get; set; }

        /// <summary>
        /// The state of the copy from this file to the 
        /// </summary>
        public FileStatus OverallStatus { get; set; }

        /// <summary>
        /// The last completed file transfer block.
        /// </summary>
        public int LastCompletedFileChunk { get; set; }

        /// <summary>
        /// The total number of file transfer blocks.
        /// </summary>
        public int TotalFileChunks { get; set; }
    }
}
