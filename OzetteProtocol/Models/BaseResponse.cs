using System;

namespace OzetteProtocol.Models
{
    /// <summary>
    /// The base response type that all server responses should inherit from.
    /// </summary>
    [Serializable]
    public abstract class BaseResponse
    {
        /// <summary>
        /// A unique identifier for the transaction.
        /// </summary>
        public Guid TransactionID { get; set; }

        /// <summary>
        /// A unique identifier for the client in the transaction.
        /// </summary>
        public Guid ClientID { get; set; }

        /// <summary>
        /// A unique identifier for the target in the transaction.
        /// </summary>
        public Guid TargetID { get; set; }
    }
}
