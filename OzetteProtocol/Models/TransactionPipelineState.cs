namespace OzetteProtocol.Models
{
    /// <summary>
    /// Describes the possible states for a transaction in the communication pipeline.
    /// </summary>
    public enum TransactionPipelineState
    {
        /// <summary>
        /// Message has no status.
        /// </summary>
        None = 0,

        /// <summary>
        /// Message has been queued for sending.
        /// </summary>
        Queued = 1,

        /// <summary>
        /// Message sending is in progress.
        /// </summary>
        Sending = 2,

        /// <summary>
        /// Message has been sent.
        /// </summary>
        Sent = 3,

        /// <summary>
        /// Message has been received by the target.
        /// </summary>
        Received = 4,

        /// <summary>
        /// Message transfer has failed.
        /// </summary>
        Failed = 5
    }
}
