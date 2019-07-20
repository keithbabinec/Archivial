using ArchivialLibrary.Files;
using ArchivialPowerShell.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace ArchivialPowerShell.Functions.Public
{
    [Cmdlet(VerbsCommon.Find, "ArchivialFilesToRestore")]
    public class FindArchivialFilesToRestoreCommand : BaseArchivialCmdlet
    {
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

            WriteVerbose("Quering for files that match the search input.");

            // TODO: implementation
        }
    }
}
