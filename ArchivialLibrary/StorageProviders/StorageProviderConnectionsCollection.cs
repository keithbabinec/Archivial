using System.Collections.Generic;

namespace ArchivialLibrary.StorageProviders
{
    /// <summary>
    /// A collection class for storage provider connections.
    /// </summary>
    public class StorageProviderConnectionsCollection : Dictionary<StorageProviderTypes, IStorageProviderFileOperations>
    {
    }
}
