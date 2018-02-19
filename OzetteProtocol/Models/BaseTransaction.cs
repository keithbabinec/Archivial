using System;

namespace OzetteProtocol.Models
{
    /// <summary>
    /// The base transaction type that all messages and responses should inherit from.
    /// </summary>
    /// <remarks>
    /// A transaction in this context represents a single message and a single response.
    /// </remarks>
    [Serializable]
    public abstract class BaseTransaction
    {
        /// <summary>
        /// A unique identifier for the transaction.
        /// </summary>
        public Guid TransactionID { get; set; }
    }
}
