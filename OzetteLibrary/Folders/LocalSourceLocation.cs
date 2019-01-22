using OzetteLibrary.Exceptions;

namespace OzetteLibrary.Folders
{
    /// <summary>
    /// Describes a requested source backup location on the local computer.
    /// </summary>
    public class LocalSourceLocation : SourceLocation
    {
        /// <summary>
        /// Formats the Source Location string properties.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Type=Local, ID={0}, Path='{1}', Filter='{2}', Priority={3}, RevisionCount={4}",
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
            ValidateFolderPath();
            ValidateFileMatchFilter();
            ValidateRevisionCount();
        }

        /// <summary>
        /// Validates that the folder path is usable.
        /// </summary>
        private void ValidateFolderPath()
        {
            if (string.IsNullOrWhiteSpace(Path))
            {
                throw new SourceLocationInvalidLocalFolderPathException(this.ToString());
            }
            if (Path.StartsWith("\\\\") == true)
            {
                throw new SourceLocationInvalidLocalFolderPathException(this.ToString());
            }
        }
    }
}
