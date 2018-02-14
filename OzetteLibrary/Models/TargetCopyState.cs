using System;

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
            if (target == null)
            {
                throw new ArgumentException(nameof(target));
            }

            TargetID = target.ID;
            ResetState();
        }

        /// <summary>
        /// Target this file is associated with.
        /// </summary>
        public int TargetID { get; set; }

        /// <summary>
        /// The state of the copy from this file to the target.
        /// </summary>
        public FileStatus TargetStatus { get; set; }

        /// <summary>
        /// The last completed file transfer block.
        /// </summary>
        /// <remarks>
        /// This index starts at 0, not 1.
        /// </remarks>
        public int LastCompletedFileChunkIndex { get; set; }

        /// <summary>
        /// Resets copy state back to unsynced.
        /// </summary>
        public void ResetState()
        {
            TargetStatus = FileStatus.Unsynced;
            LastCompletedFileChunkIndex = -1;
        }
    }
}
