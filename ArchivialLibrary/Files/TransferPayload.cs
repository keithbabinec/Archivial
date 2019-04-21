﻿using ArchivialLibrary.StorageProviders;
using System;
using System.Collections.Generic;

namespace ArchivialLibrary.Files
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
        public List<StorageProviderTypes> DestinationProviders { get; set; }

        /// <summary>
        /// The block number currently associated with this transfer payload.
        /// </summary>
        public long CurrentBlockNumber { get; set; }

        /// <summary>
        /// The total number of blocks in this file.
        /// </summary>
        public long TotalBlocks { get; set; }

        /// <summary>
        /// The actual payload/file data chunk.
        /// </summary>
        public byte[] Data { get; set; }
    }
}
