using OzetteLibrary.Providers;
using System;
using System.Collections.Generic;

namespace OzetteLibrary.Files
{
    /// <summary>
    /// Describes the contents of a single transfer stream payload. 
    /// </summary>
    public class TransferPayload
    {
        /// <summary>
        /// A unique file identifier.
        /// </summary>
        public Guid FileID { get; set; }

        /// <summary>
        /// A set of destination providers for this payload.
        /// </summary>
        public List<ProviderTypes> DestinationProviders { get; set; }

        /// <summary>
        /// The block number currently associated with this transfer payload.
        /// </summary>
        public long CurrentBlockNumber { get; set; }

        /// <summary>
        /// The total number of blocks in this file.
        /// </summary>
        public long TotalBlocks { get; set; }

        /// <summary>
        /// The expected hash of this payload data chunk.
        /// </summary>
        public byte[] ExpectedHash { get; set; }

        /// <summary>
        /// The actual payload/file data chunk.
        /// </summary>
        public byte[] Data { get; set; }
    }
}
