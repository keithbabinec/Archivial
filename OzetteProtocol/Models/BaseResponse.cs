using System;

namespace OzetteProtocol.Models
{
    /// <summary>
    /// The base response type that all server responses should inherit from.
    /// </summary>
    [Serializable]
    public abstract class BaseResponse : BaseTransaction
    {
        /// <summary>
        /// A unique identifier for the client in the transaction.
        /// </summary>
        public Guid ClientID { get; set; }

        /// <summary>
        /// A flag to indicate if the message was correctly received or processed.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The error that occurred during processing, otherwise null.
        /// </summary>
        public Exception Error { get; set; }
    }
}
