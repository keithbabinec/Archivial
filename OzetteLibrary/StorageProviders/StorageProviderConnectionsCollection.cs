using System.Collections.Generic;

namespace OzetteLibrary.StorageProviders
{
    /// <summary>
    /// A collection class for storage provider connections.
    /// </summary>
    public class StorageProviderConnectionsCollection : Dictionary<StorageProviderTypes, IStorageProviderFileOperations>
    {
    }
}
