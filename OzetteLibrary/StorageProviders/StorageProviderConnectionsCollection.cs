using System.Collections.Generic;

namespace OzetteLibrary.StorageProviders
{
    /// <summary>
    /// A collection class for provider connections.
    /// </summary>
    public class StorageProviderConnectionsCollection : Dictionary<StorageProviderTypes, IStorageProviderFileOperations>
    {
    }
}
