using System;

namespace OzetteLibrary.Folders
{
    /// <summary>
    /// Describes a requested source backup location on a network/SMB share source.
    /// </summary>
    public class NetworkSourceLocation : SourceLocation
    {
        /// <summary>
        /// The folder path to backup.
        /// </summary>
        public string UncPath { get; set; }

        /// <summary>
        /// The name of the credential lookup key.
        /// </summary>
        public string CredentialName { get; set; }

        /// <summary>
        /// Formats the Source Location string properties.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Type=Network, Credential={0}, ID={1}, Path='{2}', Filter='{3}', Priority={4}, RevisionCount={5}",
                CredentialName,
                ID,
                UncPath,
                FileMatchFilter == null ? "(none)" : FileMatchFilter,
                Priority,
                RevisionCount);
        }

        /// <summary>
        /// Validates that a source configuration is usable.
        /// </summary>
        public override void Validate()
        {
            ValidateUncPath();
            ValidateFileMatchFilter();
            ValidateRevisionCount();
            ValidateID();
        }

        /// <summary>
        /// Validates that the folder path is usable.
        /// </summary>
        private void ValidateUncPath()
        {
            throw new NotImplementedException();
        }
    }
}
