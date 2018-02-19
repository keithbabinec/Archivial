using System;

namespace OzetteProtocol.Models
{
    /// <summary>
    /// The base message type that all client messages should inherit from.
    /// </summary>
    [Serializable]
    public abstract class BaseMessage : BaseTransaction
    {
        /// <summary>
        /// A unique identifier for the target in the transaction.
        /// </summary>
        public Guid TargetID { get; set; }
    }
}
