using System.Collections.Generic;

namespace ArchivialLibrary.MessagingProviders
{
    /// <summary>
    /// A collection class for Messaging provider connections.
    /// </summary>
    public class MessagingProviderConnectionsCollection : Dictionary<MessagingProviderTypes, IMessagingProviderOperations>
    {
    }
}
