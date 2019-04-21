using OzetteLibrary.Exceptions;
using System;

namespace OzetteLibrary.Folders
{
    /// <summary>
    /// Describes a requested source backup location on a network/SMB share source.
    /// </summary>
    public class NetworkSourceLocation : SourceLocation
    {
        /// <summary>
        /// The name of the credential lookup key.
        /// </summary>
        public string CredentialName { get; set; }

        /// <summary>
        /// A flag to determine if this source is currently connected (usable).
        /// </summary>
        public bool IsConnected { get; set; }

        /// <summary>
        /// A flag to determine if this source is currently in a failed state.
        /// </summary>
        public bool IsFailed { get; set; }

        /// <summary>
        /// A timestamp for when the last time we checked the connection.
        /// </summary>
        public DateTime? LastConnectionCheck { get; set; }

        /// <summary>
        /// Formats the Source Location string properties.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Type=Network, Credential={0}, ID={1}, Path='{2}', Filter='{3}', Priority={4}, RevisionCount={5}",
                CredentialName,
                ID,
                Path,
                FileMatchFilter == null ? "(none)" : FileMatchFilter,
                Priority,
                RevisionCount);
        }

        /// <summary>
        /// Validates that source configuration parameters are valid, but does not validate if the source exists on disk.
        /// </summary>
        public override void ValidateParameters()
        {
            ValidateUncPath();
            ValidateFileMatchFilter();
            ValidateRevisionCount();
        }

        /// <summary>
        /// Validates that the folder path is usable (is a UNC path).
        /// </summary>
        private void ValidateUncPath()
        {
            if (string.IsNullOrWhiteSpace(Path))
            {
                throw new SourceLocationInvalidUncFolderPathException(this.ToString());
            }
            if (Path.StartsWith("\\\\") == false)
            {
                throw new SourceLocationInvalidUncFolderPathException(this.ToString());
            }
            if (string.IsNullOrWhiteSpace(CredentialName))
            {
                throw new SourceLocationInvalidCredentialNameException(this.ToString());
            }
        }
    }
}
