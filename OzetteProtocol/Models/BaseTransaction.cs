using System;

namespace OzetteProtocol.Models
{
    /// <summary>
    /// The base transaction type that contains a message and response.
    /// </summary>
    /// <remarks>
    /// A transaction in this context represents a single message and a single response.
    /// </remarks>
    [Serializable]
    public class BaseTransaction
    {
        /// <summary>
        /// A unique identifier for the transaction.
        /// </summary>
        public Guid TransactionID { get; set; }

        /// <summary>
        /// A unique identifier for the target in the transaction.
        /// </summary>
        public Guid TargetID { get; set; }

        /// <summary>
        /// The target's hostname.
        /// </summary>
        public string TargetHostname { get; set; }

        /// <summary>
        /// The target's port number.
        /// </summary>
        public int TargetPort { get; set; }

        /// <summary>
        /// The message object.
        /// </summary>
        public BaseMessage Message { get; set; }

        /// <summary>
        /// The response object.
        /// </summary>
        public BaseResponse Response { get; set; }

        /// <summary>
        /// The response status in the pipeline.
        /// </summary>
        public TransactionPipelineState Status { get; set; }

        /// <summary>
        /// The error that occurred during processing, otherwise null.
        /// </summary>
        public Exception Error { get; set; }

        /// <summary>
        /// Sets the pipeline status on the response.
        /// </summary>
        /// <param name="updatedState">The updated state.</param>
        public void SetMessageStatus(TransactionPipelineState updatedState)
        {
            Status = updatedState;

            // TODO: fire event when state changes?
        }

        /// <summary>
        /// Sets the pipeline status on the response.
        /// </summary>
        /// <param name="updatedState">The updated state.</param>
        /// <param name="error">An exception</param>
        public void SetMessageStatus(TransactionPipelineState updatedState, Exception error)
        {
            Status = updatedState;
            Error = error;

            // TODO: fire event when state changes?
        }
    }
}
