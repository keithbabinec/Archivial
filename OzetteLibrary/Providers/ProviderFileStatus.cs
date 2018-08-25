using OzetteLibrary.Constants;
using OzetteLibrary.Exceptions;
using OzetteLibrary.Files;
using System;
using System.Collections.Generic;

namespace OzetteLibrary.Providers
{
    /// <summary>
    /// Describes the status for a file under any cloud provider.
    /// </summary>
    public class ProviderFileStatus
    {
        /// <summary>
        /// Default/empty constructor.
        /// </summary>
        public ProviderFileStatus()
        {
        }

        /// <summary>
        /// A constructor that accepts a <c>ProviderTypes</c> enumeration.
        /// </summary>
        /// <param name="provider"></param>
        public ProviderFileStatus(ProviderTypes provider)
        {
            Provider = provider;
            ResetState();
        }

        /// <summary>
        /// A collection of metadata properties assigned to the file in the cloud provider.
        /// </summary>
        public IDictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// The type of cloud storage provider.
        /// </summary>
        public ProviderTypes Provider { get; set; }

        /// <summary>
        /// A status enumeration to describe sync state.
        /// </summary>
        public FileStatus SyncStatus { get; set; }

        /// <summary>
        /// The last completed file transfer block, if multiple blocks are required.
        /// </summary>
        /// <remarks>
        /// This index starts at 0, not 1.
        /// </remarks>
        public int LastCompletedFileBlockIndex { get; set; }

        /// <summary>
        /// Resets copy state back to unsynced.
        /// </summary>
        public void ResetState()
        {
            SyncStatus = FileStatus.Unsynced;
            LastCompletedFileBlockIndex = -1;
            Metadata = null;
        }

        /// <summary>
        /// Applies the metadata found in the provider state to this object.
        /// </summary>
        /// <param name="providerMetadata"></param>
        public void ApplyMetadataToState(IDictionary<string, string> providerMetadata)
        {
            if (providerMetadata == null)
            {
                throw new ArgumentNullException(nameof(providerMetadata));
            }

            if (Provider == ProviderTypes.Azure)
            {
                if (!providerMetadata.ContainsKey(ProviderMetadata.ProviderSyncStatusKeyName))
                {
                    throw new ProviderMetadataMissingException(ProviderMetadata.ProviderSyncStatusKeyName);
                }
                if (!providerMetadata.ContainsKey(ProviderMetadata.ProviderLastCompletedFileBlockIndexKeyName))
                {
                    throw new ProviderMetadataMissingException(ProviderMetadata.ProviderLastCompletedFileBlockIndexKeyName);
                }

                FileStatus parsedStatus;
                if (!Enum.TryParse(providerMetadata[ProviderMetadata.ProviderSyncStatusKeyName], out parsedStatus))
                {
                    throw new ProviderMetadataMalformedException(ProviderMetadata.ProviderSyncStatusKeyName);
                }

                int parsedLastBlock;
                if (!int.TryParse(providerMetadata[ProviderMetadata.ProviderLastCompletedFileBlockIndexKeyName], out parsedLastBlock))
                {
                    throw new ProviderMetadataMalformedException(ProviderMetadata.ProviderLastCompletedFileBlockIndexKeyName);
                }

                SyncStatus = parsedStatus;
                LastCompletedFileBlockIndex = parsedLastBlock;
                Metadata = providerMetadata;
            }
            else
            {
                throw new NotImplementedException("Provider is not implemented: " + Provider.ToString());
            }
        }
    }
}
