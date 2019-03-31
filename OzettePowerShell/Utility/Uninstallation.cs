using OzetteLibrary.Constants;
using OzetteLibrary.Database.SQLServer;
using OzetteLibrary.ServiceCore;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;

namespace OzettePowerShell.Utility
{
    public static class Uninstallation
    {
        /// <summary>
        /// Stops the client windows service.
        /// </summary>
        public static void StopClientService()
        {
            var existingServices = ServiceController.GetServices();
            var ozetteService = existingServices.FirstOrDefault(x => x.ServiceName == "OzetteCloudBackup");

            if (ozetteService != null && ozetteService.Status != ServiceControllerStatus.Stopped)
            {
                ozetteService.Stop();

                try
                {
                    ozetteService.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(60));
                }
                catch (System.ServiceProcess.TimeoutException)
                {
                    throw new Exception("Failed to stop the OzetteCloudBackup windows service.");
                }

                // wait for the process to exit as well.

                var timeoutTime = DateTime.Now.Add(TimeSpan.FromSeconds(60));

                while (true)
                {
                    var processCheck = Process.GetProcessesByName("OzetteClientAgent.exe");

                    if (processCheck == null || processCheck.Length == 0)
                    {
                        break;
                    }

                    if (DateTime.Now > timeoutTime)
                    {
                        throw new Exception("Failed to stop the OzetteCloudBackup windows service, the process is still running and may be frozen.");
                    }
                }
            }
        }

        /// <summary>
        /// Removes the client windows service.
        /// </summary>
        public static void DeleteClientService()
        {
            var existingServices = ServiceController.GetServices();

            if (existingServices.Any(x => x.ServiceName == "OzetteCloudBackup"))
            {
                var removalArgs = "delete \"OzetteCloudBackup\"";

                Process removeServiceProcess = new Process();
                removeServiceProcess.StartInfo = new ProcessStartInfo()
                {
                    FileName = "sc.exe",
                    Arguments = removalArgs
                };

                removeServiceProcess.Start();
                removeServiceProcess.WaitForExit();

                if (removeServiceProcess.ExitCode != 0)
                {
                    throw new Exception("Failed to remove the windows service. Sc.exe returned an error code: " + removeServiceProcess.ExitCode);
                }
            }
        }

        /// <summary>
        /// Removes the installation directories.
        /// </summary>
        public static void DeleteInstallationDirectories()
        {
            var installDirectory = CoreSettings.InstallationDirectory;
            int attempts = 0;

            while (true)
            {
                try
                {
                    if (Directory.Exists(installDirectory))
                    {
                        Directory.Delete(installDirectory, true);
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                    if (attempts > 5)
                    {
                        throw;
                    }
                    else
                    {
                        attempts++;
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                    }
                }
            }
        }

        /// <summary>
        /// Removes the event log source.
        /// </summary>
        public static void DeleteEventLogContents()
        {
            if (EventLog.Exists(CoreSettings.EventlogName))
            {
                var log = new EventLog(CoreSettings.EventlogName, Environment.MachineName, CoreSettings.EventlogName);
                log.Clear();
            }
        }

        /// <summary>
        /// Removes the core settings.
        /// </summary>
        public static void DeleteCoreSettings()
        {
            var settings = new string[]
            {
                BootstrapSettingNames.InstallationDirectory,
                BootstrapSettingNames.LogFilesDirectory,
                BootstrapSettingNames.LogFilesArchiveDirectory,
                BootstrapSettingNames.LogFilesRetentionInDays,
                BootstrapSettingNames.DatabaseDirectory,
                BootstrapSettingNames.DatabaseBackupsDirectory,
                BootstrapSettingNames.DatabaseBackupsRetentionInDays,
                BootstrapSettingNames.EventlogName,
                BootstrapSettingNames.BackupEngineInstancesCount,
                BootstrapSettingNames.DatabasePublishIsRequired,
                BootstrapSettingNames.DatabaseConnectionString,
                BootstrapSettingNames.ProtectionIV
            };

            foreach (var setting in settings)
            {
                CoreSettings.RemoveCoreSetting(setting);
            }
        }
    }
}
