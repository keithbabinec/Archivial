using ArchivialPowerShell.Utility;
using System.Management.Automation;

namespace ArchivialPowerShell.Functions.Public
{
    /// <summary>
    ///   <para type="synopsis">Returns the current status of the cloud backup progress.</para>
    ///   <para type="description">Returns the current status of the cloud backup progress. It includes details such as how many files are backed up, remaining, and percentage complete.</para>
    /// </summary>
    /// <example>
    ///   <code>C:\> Get-ArchivialCloudBackupStatus</code>
    ///   <para>Returns the current status of the cloud backup progress.</para>
    ///   <para></para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "ArchivialCloudBackupStatus")]
    public class GetArchivialCloudBackupStatusCommand : BaseArchivialCmdlet
    {
        protected override void ProcessRecord()
        {
            var db = GetDatabaseConnection();

            WriteVerbose("Querying for backup status.");

            var progress = db.GetBackupProgressAsync().GetAwaiter().GetResult();

            WriteObject(progress);
        }
    }
}
