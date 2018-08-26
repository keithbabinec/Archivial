using System.Collections.Generic;

namespace OzetteLibrary.Providers
{
    /// <summary>
    /// A collection class for provider connections.
    /// </summary>
    public class ProviderConnectionsCollection : Dictionary<ProviderTypes, IProviderFileOperations>
    {
    }
}
