using System.Management.Automation;

namespace OzettePowerShell.Functions.Private
{
    public class BaseOzetteCmdlet : Cmdlet
    {
        internal string ActivityName { get; set; }

        internal int ActivityID { get; set; }

        internal void WriteProgress(int Percentage, string Description)
        {
            base.WriteProgress(
                new ProgressRecord(ActivityID, ActivityName, Description) { PercentComplete = Percentage }
            );
        }

        internal void WriteVerboseAndProgress(int Percentage, string Description)
        {
            base.WriteProgress(
                new ProgressRecord(ActivityID, ActivityName, Description) { PercentComplete = Percentage }
            );

            base.WriteVerbose(Description);
        }
    }
}
