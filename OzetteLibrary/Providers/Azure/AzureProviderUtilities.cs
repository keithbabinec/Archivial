using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;

namespace OzetteLibrary.Providers.Azure
{
    /// <summary>
    /// Provides utility functions for the Azure provider.
    /// </summary>
    public class AzureProviderUtilities
    {
        /// <summary>
        /// Generates a list base64 encoded block identifiers to commit.
        /// </summary>
        /// <remarks>
        /// Azure references block IDs with base64 encoded strings.
        /// One or more block IDs are committed to make a single CloudBlockBlob object.
        /// Generate the list of block IDs up to the current block.
        /// </remarks>
        /// <param name="fileID"></param>
        /// <param name="currentBlockIndex"></param>
        /// <returns></returns>
        public List<string> GenerateListOfBlocksToCommit(Guid fileID, int currentBlockIndex)
        {
            if (fileID == Guid.Empty)
            {
                throw new ArgumentException(nameof(fileID) + " must be provided.");
            }
            if (currentBlockIndex < 0)
            {
                throw new ArgumentException(nameof(currentBlockIndex) + " cannot be negative.");
            }

            List<string> blockIDs = new List<string>();

            for (int i = 0; i <= currentBlockIndex; i++)
            {
                blockIDs.Add(GenerateBlockIdentifierBase64String(fileID, i));
            }

            return blockIDs;
        }

        /// <summary>
        /// Generates a base64 encoded block identifier.
        /// </summary>
        /// <remarks>
        /// Azure references block IDs with base64 encoded strings.
        /// One or more block IDs are committed to make a single CloudBlockBlob object.
        /// </remarks>
        /// <param name="fileID"></param>
        /// <param name="blockNumber"></param>
        /// <returns></returns>
        public string GenerateBlockIdentifierBase64String(Guid fileID, int blockNumber)
        {
            if (fileID == Guid.Empty)
            {
                throw new ArgumentException(nameof(fileID) + " must be provided.");
            }
            if (blockNumber < 0)
            {
                throw new ArgumentException(nameof(blockNumber) + " cannot be negative.");
            }

            return Convert.ToBase64String(
                Encoding.UTF8.GetBytes(
                    string.Format("{0}-{1}", fileID.ToString(), blockNumber.ToString("D8"))
                )
            );
        }

        /// <summary>
        /// Returns the Azure blob resource URI.
        /// </summary>
        /// <param name="storageAccountName">The Azure storage account name.</param>
        /// <param name="containerName">The Azure storage container name.</param>
        /// <param name="blobName">The Azure storage blob name.</param>
        /// <returns>A string formatted as a URI</returns>
        public string GetFileUri(string storageAccountName, string containerName, string blobName)
        {
            if (string.IsNullOrWhiteSpace(storageAccountName))
            {
                throw new ArgumentException(nameof(storageAccountName) + " must be provided.");
            }
            if (string.IsNullOrWhiteSpace(containerName))
            {
                throw new ArgumentException(nameof(containerName) + " must be provided.");
            }
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentException(nameof(blobName) + " must be provided.");
            }

            // example uri:
            // https://myaccount.blob.core.windows.net/mycontainer/myblob 

            return string.Format("https://{0}.blob.core.windows.net/{1}/{2}", storageAccountName, containerName, blobName);
        }

        /// <summary>
        /// Returns the Azure container resource URI.
        /// </summary>
        /// <param name="storageAccountName">The Azure storage account name.</param>
        /// <param name="containerName">The Azure storage container name.</param>
        /// <returns>A string formatted as a URI</returns>
        public string GetContainerUri(string storageAccountName, string containerName)
        {
            if (string.IsNullOrWhiteSpace(storageAccountName))
            {
                throw new ArgumentException(nameof(storageAccountName) + " must be provided.");
            }
            if (string.IsNullOrWhiteSpace(containerName))
            {
                throw new ArgumentException(nameof(containerName) + " must be provided.");
            }

            // example uri:
            // https://myaccount.blob.core.windows.net/mycontainer/myblob 

            return string.Format("https://{0}.blob.core.windows.net/{1}", storageAccountName, containerName);
        }

        /// <summary>
        /// Converts the Azure file rehydration state to a common version usable across all providers.
        /// </summary>
        /// <param name="rehydrationStatus"></param>
        /// <returns></returns>
        public string GetHydrationStatusFromAzureState(RehydrationStatus? rehydrationStatus)
        {
            // note that moving back to archive tier is an instantaneous operation.
            // hence the missing case of 'moving to archive'

            if (rehydrationStatus.HasValue == false || rehydrationStatus.Value == RehydrationStatus.Unknown)
            {
                return ProviderHydrationStatus.None.ToString();
            }
            else if (rehydrationStatus.Value == RehydrationStatus.PendingToCool || rehydrationStatus.Value == RehydrationStatus.PendingToHot)
            {
                return ProviderHydrationStatus.MovingToActiveTier.ToString();
            }
            else
            {
                throw new NotImplementedException("Unexpected RehydrationStatus provided by Azure: " + rehydrationStatus.Value.ToString());
            }
        }
    }
}
