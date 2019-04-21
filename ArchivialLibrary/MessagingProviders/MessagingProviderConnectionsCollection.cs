using System.Collections.Generic;

namespace OzetteLibrary.MessagingProviders
{
    /// <summary>
    /// A collection class for Messaging provider connections.
    /// </summary>
    public class MessagingProviderConnectionsCollection : Dictionary<MessagingProviderTypes, IMessagingProviderOperations>
    {
    }
}
