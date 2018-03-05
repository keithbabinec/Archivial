using System;

namespace OzetteProtocol.Models
{
    /// <summary>
    /// The base message type that all client messages should inherit from.
    /// </summary>
    [Serializable]
    public abstract class BaseMessage
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
        /// The client's hostname.
        /// </summary>
        public string ClientHostname { get; set; }

        /// <summary>
        /// The client's port number.
        /// </summary>
        public int ClientPort { get; set; }

        /// <summary>
        /// A unique identifier for the target in the transaction.
        /// </summary>
        public Guid TargetID { get; set; }
    }
}
