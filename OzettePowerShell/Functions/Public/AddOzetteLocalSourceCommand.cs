using OzetteLibrary.Exceptions;
using OzetteLibrary.Files;
using OzetteLibrary.Folders;
using OzettePowerShell.Utility;
using System;
using System.Linq;
using System.Management.Automation;

namespace OzettePowerShell.Functions.Public
{
    /// <summary>
    ///   <para type="synopsis">Adds a local folder to the Ozette backup folders list.</para>
    ///   <para type="description">A Local Source is a folder on your computer (or a directly attached external drive) that you would like Ozette to backup and automatically monitor for new and updated files.</para>
    ///   <para type="description">The priority of the source determines how frequently it will be scanned for changes. The automated scanning schedule for Low priority sources is once every 48 hours. Medium priority sources are scanned every 12 hours. High priority sources are scanned every hour.</para>
    ///   <para type="description">The optional MatchFilter parameter allows you to narrow the scope of files in the folder to be monitored. For example, by file extension. Any windows file path wildcard expression will be accepted here.</para>
    /// </summary>
    /// <example>
    ///   <code>C:\> Add-OzetteLocalSource -FolderPath "C:\users\test\documents" -Priority High -Revisions 3</code>
    ///   <para>Adds the specified folder to backup with high priority, and to retain up to 3 revisions of file history.</para>
    ///   <para></para>
    /// </example>
    /// <example>
    ///   <code>C:\> Add-OzetteLocalSource -FolderPath "C:\users\test\music\playlists" -Priority High -Revisions 3 -MatchFilter *.m3u</code>
    ///   <para>Adds the specified folder to backup with high priority, but only files that match the wildcard extension filter.</para>
    ///   <para></para>
    /// </example>
    [Cmdlet(VerbsCommon.Add, "OzetteLocalSource")]
    public class AddOzetteLocalSourceCommand : BaseOzetteCmdlet
    {
        /// <summary>
        ///   <para type="description">Specify the folder path that should be backed up and monitored.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string FolderPath { get; set; }

        /// <summary>
        ///   <para type="description">Specify the priority of this source (which determines how frequently it will be scanned for changes).</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        [ValidateSet("Low", "Medium", "High")]
        public string Priority { get; set; }

        /// <summary>
        ///   <para type="description">Specify the maximum number of revisions to store in the cloud for the files in this folder.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        public int Revisions { get; set; }

        /// <summary>
        ///   <para type="description">Optionally specify a wildcard expression to filter the files to be backed up or monitored.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        [ValidateNotNullOrEmpty]
        public string MatchFilter = OzetteLibrary.Constants.CommandLine.DefaultSourceMatchFilter;

        protected override void ProcessRecord()
        {
            FileBackupPriority priorityEnum;
            if (!Enum.TryParse(Priority, out priorityEnum))
            {
                throw new ArgumentException(nameof(Priority) + " value was not valid.");
            }

            if (Revisions <= 0)
            {
                throw new ArgumentException(nameof(Revisions) + " value must be at least 1.");
            }

            var db = GetDatabaseConnection();

            WriteVerbose("Querying for existing scan sources to check for duplicates.");

            var allSources = db.GetSourceLocationsAsync().GetAwaiter().GetResult();
            var allLocalSources = allSources.Where(x => x is LocalSourceLocation).ToList();

            if (allLocalSources.Any(x => string.Equals(x.Path, FolderPath, StringComparison.CurrentCultureIgnoreCase)
                                      && string.Equals(x.FileMatchFilter, MatchFilter, StringComparison.CurrentCultureIgnoreCase)))
            {
                // there already exists a source with this folder location and match filter.
                throw new SourceLocationException("Unable to add source: the specified folder and match filter combination is already listed as a source.");
            }
            else
            {
                WriteVerbose("No duplicate sources found.");
            }

            var newSource = new LocalSourceLocation();
            newSource.Path = FolderPath;
            newSource.FileMatchFilter = MatchFilter;
            newSource.RevisionCount = Revisions;
            newSource.Priority = priorityEnum;

            WriteVerbose("Validating the source parameters are acceptable.");
            newSource.ValidateParameters();
            WriteVerbose("The specified scan source has normal parameters.");

            WriteVerbose("Saving the source to the database.");

            db.SetSourceLocationAsync(newSource).GetAwaiter().GetResult();

            WriteVerbose("Successfully saved source to the database.");
        }
    }
}
