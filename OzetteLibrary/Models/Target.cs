using System;

namespace OzetteLibrary.Models
{
    /// <summary>
    /// Describes a single backup target (destination).
    /// </summary>
    public class Target
    {
        /// <summary>
        /// An unique identifier for the target to be referenced by files.
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// The name of the target.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The URL to connect to the target on.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The port to connect to the target on.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The root directory for file storage on the target system.
        /// </summary>
        public string RootDirectory { get; set; }

        /// <summary>
        /// The availability state of the target.
        /// </summary>
        public TargetAvailabilityState Availability { get; set; }
    }
}
