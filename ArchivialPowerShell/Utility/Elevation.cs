using System.Security.Principal;

namespace ArchivialPowerShell.Utility
{
    public static class Elevation
    {
        public static bool IsRunningElevated()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
