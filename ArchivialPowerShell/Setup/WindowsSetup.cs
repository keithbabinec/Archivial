using ArchivialLibrary.Constants;
using ArchivialLibrary.Database;
using ArchivialLibrary.ServiceCore;
using ArchivialPowerShell.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace ArchivialPowerShell.Setup
{
    /// <summary>
    /// The Windows specific implementation of <c>ISetup</c>
    /// </summary>
    public class WindowsSetup : ISetup
    {
        /// <summary>
        /// A reference to the database instance.
        /// </summary>
        private IClientDatabase DatabaseClient { get; set; }

        /// <summary>
        /// A constructor that accepts a database instance.
        /// </summary>
        /// <param name="client"></param>
        public WindowsSetup(IClientDatabase client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            DatabaseClient = client;
        }

        /// <summary>
        /// Gets the installed version of the product.
        /// </summary>
        /// <returns></returns>
        public async Task<Version> GetInstalledVersionAsync()
        {
            // 3 major components should be present to detect a valid installation.
            // - the binaries (lib, svc executable)
            // - the daemon
            // - the database

            var libVersion = GetInstalledBinaryVersion();
            var serviceIsPresent = WindowsServiceIsPresent();
            var databaseIsPresent = await DatabaseClient.DatabaseIsPresentAsync().ConfigureAwait(false);

            if (libVersion == null && !serviceIsPresent && !databaseIsPresent)
            {
                // if all the components are missing, return null (product is not installed).
                return null;
            }
            else if (libVersion != null && serviceIsPresent && databaseIsPresent)
            {
                // if all the components are present, return the version.
                return libVersion;
            }
            else
            {
                // if some of the components are missing (but not all), throw since the installation is damaged.
                throw new CmdletExecutionFailedDamagedProductInstallationException("The Archivial Cloud Backup product installation appears to be damaged or partially installed.");
            }
        }

        /// <summary>
        /// Gets the running PowerShell module version.
        /// </summary>
        /// <returns></returns>
        public Version GetPowerShellModuleVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }

        /// <summary>
        /// Gets the binary version of the installed product.
        /// </summary>
        /// <returns></returns>
        private Version GetInstalledBinaryVersion()
        {
            var programDirectory = CoreSettings.InstallationDirectory;

            if (string.IsNullOrWhiteSpace(programDirectory))
            {
                return null;
            }
            else
            {
                var agentLibPath = Path.Combine(programDirectory, "ArchivialLibrary.dll");

                if (File.Exists(agentLibPath))
                {
                    var agentLib = FileVersionInfo.GetVersionInfo(agentLibPath);
                    return new Version(agentLib.FileVersion);
                }
            }

            return null;
        }

        /// <summary>
        /// Checks to see if the windows service is present.
        /// </summary>
        /// <returns></returns>
        private bool WindowsServiceIsPresent()
        {
            var existingServices = ServiceController.GetServices();
            return existingServices.Any(x => x.ServiceName == "ArchivialCloudBackup");
        }

        /// <summary>
        /// Checks if this process is running elevated.
        /// </summary>
        /// <returns></returns>
        public bool IsRunningElevated()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// Checks if the SQL Server prerequisite is available.
        /// </summary>
        /// <returns></returns>
        public bool SqlServerPrerequisiteIsAvailable()
        {
            var existingServices = ServiceController.GetServices();

            if (existingServices.Any(x => x.ServiceName == Database.DefaultSqlExpressInstanceName) == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the core application settings.
        /// </summary>
        /// <param name="installationDirectory"></param>
        public void CreateCoreSettings(string installationDirectory)
        {
            // set the core settings.
            CoreSettings.InstallationDirectory = installationDirectory;
            CoreSettings.EventlogName = "Archivial";

            // setting this flag indicates publish is required on next service startup.
            CoreSettings.DatabasePublishIsRequired = true;

            var dbConnectionString = string.Format("Data Source=.\\SQLExpress;Initial Catalog={0};Integrated Security=SSPI;", Database.DatabaseName);
            CoreSettings.DatabaseConnectionString = dbConnectionString;
        }

        /// <summary>
        /// Creates a custom event log and event source.
        /// </summary>
        public void CreateEventLogSource()
        {
            if (EventLog.Exists(CoreSettings.EventlogName) == false)
            {
                EventLog.CreateEventSource(CoreSettings.EventlogName, CoreSettings.EventlogName);
            }
        }

        /// <summary>
        /// Creates the installation directories.
        /// </summary>
        public void CreateInstallationDirectories()
        {
            if (Directory.Exists(CoreSettings.InstallationDirectory) == false)
            {
                Directory.CreateDirectory(CoreSettings.InstallationDirectory);
            }

            if (Directory.Exists(CoreSettings.DatabaseDirectory) == false)
            {
                Directory.CreateDirectory(CoreSettings.DatabaseDirectory);
                Directory.CreateDirectory(CoreSettings.DatabaseBackupsDirectory);

                var dirInfo = new DirectoryInfo(CoreSettings.DatabaseDirectory);
                var dirSecurity = dirInfo.GetAccessControl();

                dirSecurity.AddAccessRule(
                    new FileSystemAccessRule(
                        Database.DefaultSqlExpressUserAccount,
                        FileSystemRights.FullControl,
                        InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                        PropagationFlags.None,
                        AccessControlType.Allow
                    )
                );

                dirInfo.SetAccessControl(dirSecurity);
            }

            if (Directory.Exists(CoreSettings.LogFilesDirectory) == false)
            {
                Directory.CreateDirectory(CoreSettings.LogFilesDirectory);
                Directory.CreateDirectory(CoreSettings.LogFilesArchiveDirectory);
            }
        }

        /// <summary>
        /// Copies the program files to the installation directory.
        /// </summary>
        public void CopyProgramFiles()
        {
            var sourcePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // expected file manifest
            var fileManifest = new List<string>()
            {
                "Microsoft.Azure.KeyVault.Core.dll",
                "Microsoft.Data.Tools.Schema.Sql.dll",
                "Microsoft.Data.Tools.Utilities.dll",
                "Microsoft.IdentityModel.Logging.dll",
                "Microsoft.IdentityModel.Tokens.dll",
                "Microsoft.SqlServer.Dac.dll",
                "Microsoft.SqlServer.TransactSql.ScriptDom.dll",
                "Microsoft.SqlServer.Types.dll",
                "Microsoft.WindowsAzure.Storage.dll",
                "NCrontab.dll",
                "Newtonsoft.Json.dll",
                "ArchivialClientAgent.exe",
                "ArchivialClientAgent.exe.config",
                "ArchivialDB.dacpac",
                "ArchivialDB.dll",
                "ArchivialLibrary.dll",
                "System.IdentityModel.Tokens.Jwt.dll",
                "System.Management.Automation.dll",
                "Twilio.dll"
            };

            foreach (var file in fileManifest)
            {
                var sourceFileFullPath = Path.Combine(sourcePath, file);
                var destFileFullPath = Path.Combine(CoreSettings.InstallationDirectory, file);

                if (File.Exists(sourceFileFullPath) == false)
                {
                    throw new FileNotFoundException("A required setup file was missing: " + sourceFileFullPath);
                }

                File.Copy(sourceFileFullPath, destFileFullPath, true);
            }
        }

        /// <summary>
        /// Creates the client windows service.
        /// </summary>
        public void CreateClientService()
        {
            if (!WindowsServiceIsPresent())
            {
                var installArgs = string.Format(
                    "create \"ArchivialCloudBackup\" binPath= \"{0}\" start= \"auto\" DisplayName= \"{1}\" depend= \"{2}\"",
                    Path.Combine(CoreSettings.InstallationDirectory, "ArchivialClientAgent.exe"),
                    "Archivial Cloud Backup Agent",
                    Database.DefaultSqlExpressInstanceName);

                Process createServiceProcess = new Process();
                createServiceProcess.StartInfo = new ProcessStartInfo()
                {
                    FileName = "sc.exe",
                    Arguments = installArgs
                };

                createServiceProcess.Start();
                createServiceProcess.WaitForExit();

                if (createServiceProcess.ExitCode != 0)
                {
                    throw new Exception("Failed to create the windows service. Sc.exe returned an error code: " + createServiceProcess.ExitCode);
                }

                var setDescriptionArgs = string.Format(
                    "description \"ArchivialCloudBackup\" \"Archivial Cloud Backup is a data backup client service that copies data to cloud providers like Azure and AWS.\"",
                    CoreSettings.InstallationDirectory);

                Process setServiceDescriptionProcess = new Process();
                setServiceDescriptionProcess.StartInfo = new ProcessStartInfo()
                {
                    FileName = "sc.exe",
                    Arguments = setDescriptionArgs
                };

                setServiceDescriptionProcess.Start();
                setServiceDescriptionProcess.WaitForExit();

                if (setServiceDescriptionProcess.ExitCode != 0)
                {
                    throw new Exception("Failed to set the windows service description. Sc.exe returned an error code: " + setServiceDescriptionProcess.ExitCode);
                }
            }
        }

        /// <summary>
        /// Starts the client service.
        /// </summary>
        public void StartClientService()
        {
            Process startService = new Process();
            startService.StartInfo = new ProcessStartInfo()
            {
                FileName = "sc.exe",
                Arguments = "start ArchivialCloudBackup"
            };

            startService.Start();
            startService.WaitForExit();

            if (startService.ExitCode != 0)
            {
                throw new Exception("Failed to start the windows service. Sc.exe returned an error code: " + startService.ExitCode);
            }
        }

        /// <summary>
        /// Waits until the first time setup/init is completed.
        /// </summary>
        /// <remarks>
        /// First time setup is done once the database is initialized, so check for the required publish flag/option state.
        /// </remarks>
        public void WaitForFirstTimeSetup()
        {
            var Timeout = DateTime.Now.Add(TimeSpan.FromMinutes(10));

            while (true)
            {
                if (CoreSettings.DatabasePublishIsRequired)
                {
                    // setup is still running.

                    if (DateTime.Now > Timeout)
                    {
                        // we have exceeded the timeout.
                        throw new System.TimeoutException("The Archivial Cloud Backup service has failed to initialize within the expected timeframe (10 minutes). There may be a problem with the service, please see the logs for details.");
                    }
                    else
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(2));
                    }
                }
                else
                {
                    // setup has finished.
                    break;
                }
            }
        }

        /// <summary>
        /// Stops the client windows service.
        /// </summary>
        public void StopClientService()
        {
            var existingServices = ServiceController.GetServices();
            var ArchivialService = existingServices.FirstOrDefault(x => x.ServiceName == "ArchivialCloudBackup");

            if (ArchivialService != null && ArchivialService.Status != ServiceControllerStatus.Stopped)
            {
                ArchivialService.Stop();

                try
                {
                    ArchivialService.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(60));
                }
                catch (System.ServiceProcess.TimeoutException)
                {
                    throw new Exception("Failed to stop the ArchivialCloudBackup windows service.");
                }

                // wait for the process to exit as well.

                var timeoutTime = DateTime.Now.Add(TimeSpan.FromSeconds(60));

                while (true)
                {
                    var processCheck = Process.GetProcessesByName("ArchivialClientAgent.exe");

                    if (processCheck == null || processCheck.Length == 0)
                    {
                        break;
                    }

                    if (DateTime.Now > timeoutTime)
                    {
                        throw new Exception("Failed to stop the ArchivialCloudBackup windows service, the process is still running and may be frozen.");
                    }
                }
            }
        }

        /// <summary>
        /// Removes the client windows service.
        /// </summary>
        public void DeleteClientService()
        {
            var existingServices = ServiceController.GetServices();

            if (existingServices.Any(x => x.ServiceName == "ArchivialCloudBackup"))
            {
                var removalArgs = "delete \"ArchivialCloudBackup\"";

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
        public void DeleteInstallationDirectories()
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
        public void DeleteEventLogContents()
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
        public void DeleteCoreSettings()
        {
            var settings = new string[]
            {
                BootstrapSettingNames.InstallationDirectory,
                BootstrapSettingNames.EventlogName,
                BootstrapSettingNames.DatabasePublishIsRequired,
                BootstrapSettingNames.DatabaseConnectionString
            };

            foreach (var setting in settings)
            {
                CoreSettings.RemoveCoreSetting(setting);
            }
        }
    }
}
