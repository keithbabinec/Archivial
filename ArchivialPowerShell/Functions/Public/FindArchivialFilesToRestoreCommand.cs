﻿using ArchivialLibrary.Files;
using ArchivialLibrary.Folders;
using ArchivialPowerShell.Utility;
using System;
using System.Management.Automation;

namespace ArchivialPowerShell.Functions.Public
{
    /// <summary>
    ///   <para type="synopsis">Finds backup files that are available to restore.</para>
    ///   <para type="description">Backup files are available to restore if they have completed an upload to at least one cloud storage provider account.</para>
    ///   <para type="description">This command searches the backup index to find files that are eligible for restore and filters the result using the specified options.</para>
    ///   <para type="description">The output from this command can be passed into Start-ArchivialFileRestore, which is used to initiate the restore.</para>
    /// </summary>
    /// <example>
    ///   <code>C:\> Find-ArchivialFilesToRestore -MatchFilter "*.docx" -LimitResults 100</code>
    ///   <para>Searches for any files that match the extension filter and limits the results to no more than 100 items.</para>
    ///   <para></para>
    /// </example>
    /// <example>
    ///   <code>C:\> Find-ArchivialFilesToRestore -MatchFilter "D:\music\*"</code>
    ///   <para>Searches for any files that contain a match to the specified path. Does not limit the number of results returned.</para>
    ///   <para></para>
    /// </example>
    /// <example>
    ///   <code>C:\> Get-ArchivialNetworkSource | Where Path -like "*\\drobo-nas\documents*" | Find-ArchivialFilesToRestore</code>
    ///   <para>Searches for any files that originated from a Network source that matches the documents path filter.</para>
    ///   <para></para>
    /// </example>
    /// <example>
    ///   <code>C:\> Get-ArchivialLocalSource | Find-ArchivialFilesToRestore</code>
    ///   <para>Searches for any files that originated from any of the Local sources.</para>
    ///   <para></para>
    /// </example>
    /// <example>
    ///   <code>C:\> Find-ArchivialFilesToRestore -FileHash "A37CC82F2876DB6CF59BA29B4EB148C7BF5CC920"</code>
    ///   <para>Searches for any files that match the provided file hash.</para>
    ///   <para></para>
    /// </example>
    [Cmdlet(VerbsCommon.Find, "ArchivialFilesToRestore", DefaultParameterSetName = "ByFilter")]
    public class FindArchivialFilesToRestoreCommand : BaseArchivialCmdlet
    {
        /// <summary>
        ///   <para type="description">Specify a source location to search for files that can be restored.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "BySource", ValueFromPipeline = true)]
        [ValidateNotNull]
        public SourceLocation Source { get; set; }

        /// <summary>
        ///   <para type="description">Specify a file hash to search for files that can be restored.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "ByHash")]
        [ValidateNotNullOrEmpty]
        public string FileHash { get; set; }

        /// <summary>
        ///   <para type="description">Specify a directory/file path filter to search for files that can be restored. This match behaves similar to a 'Contains' operation and supports the * wildcard.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "ByFilter")]
        [ValidateNotNullOrEmpty]
        public string MatchFilter { get; set; }

        /// <summary>
        ///   <para type="description">Specify this option to return all files eligible for restore, without any filtering.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "All")]
        public SwitchParameter All = false;

        /// <summary>
        ///   <para type="description">Optionally specify the maximum number of results to return from a search.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "BySource")]
        [Parameter(Mandatory = false, ParameterSetName = "ByHash")]
        [Parameter(Mandatory = false, ParameterSetName = "ByFilter")]
        [ValidateRange(1, int.MaxValue)]
        public int LimitResults { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FindArchivialFilesToRestoreCommand() : base() { }

        /// <summary>
        /// Secondary constructor for dependency injection.
        /// </summary>
        /// <param name="dependencies"></param>
        public FindArchivialFilesToRestoreCommand(CmdletDependencies dependencies) : base(dependencies) { }

        protected override void ProcessRecord()
        {
            var db = GetDatabaseConnection();

            BackupFileSearchResults results = null;

            if (!string.IsNullOrEmpty(MatchFilter))
            {
                // fts user generated text needs to be wrapped in quotes for the wildcards to work.
                var wrappedFilter = "\"" + MatchFilter + "\"";

                WriteVerbose("Quering for files that match the provided file/directory match filter: " + wrappedFilter);
                results = db.FindArchivialFilesToRestoreByFilter(wrappedFilter, LimitResults).GetAwaiter().GetResult();
            }
            else if (!string.IsNullOrEmpty(FileHash))
            {
                // fts user generated text needs to be wrapped in quotes for the wildcards to work.
                var wrappedHash = "\"" + FileHash + "\"";

                WriteVerbose("Quering for files that match the provided file hash: " + wrappedHash);
                results = db.FindArchivialFilesToRestoreByHash(wrappedHash, LimitResults).GetAwaiter().GetResult();
            }
            else if (Source != null)
            {
                WriteVerbose("Quering for files that match the originating source: " + Source.ID);
                results = db.FindArchivialFilesToRestoreBySource(Source, LimitResults).GetAwaiter().GetResult();
            }
            else if (All == true)
            {
                WriteVerbose("Quering for all files.");
                results = db.FindAllArchivialFilesToRestore().GetAwaiter().GetResult();
            }
            else
            {
                throw new ArgumentException("Valid search parameters were not provided.");
            }

            if (results != null)
            {
                WriteVerbose(string.Format("Found {0} total eligible files.", results.Count));
                WriteObject(results);
            }
        }
    }
}
