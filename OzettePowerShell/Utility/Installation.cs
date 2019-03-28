﻿using OzetteLibrary.ServiceCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.ServiceProcess;

namespace OzettePowerShell.Utility
{
    public static class Installation
    {
        /// <summary>
        /// Sets the core application settings.
        /// </summary>
        /// <param name="arguments"></param>
        public static void CreateCoreSettings(string installationDirectory)
        {
            // set the core settings.
            CoreSettings.InstallationDirectory = installationDirectory;
            CoreSettings.LogFilesDirectory = Path.Combine(installationDirectory, "Logs");
            CoreSettings.LogFilesArchiveDirectory = Path.Combine(CoreSettings.LogFilesDirectory, "Archive");
            CoreSettings.LogFilesRetentionInDays = 30;
            CoreSettings.DatabaseDirectory = Path.Combine(installationDirectory, "Database");
            CoreSettings.DatabaseBackupsDirectory = Path.Combine(CoreSettings.DatabaseDirectory, "Backups");
            CoreSettings.DatabaseBackupsRetentionInDays = 7;
            CoreSettings.EventlogName = "Ozette";
            CoreSettings.BackupEngineInstanceCount = 4;

            // setting this flag indicates publish is required on next service startup.
            CoreSettings.DatabasePublishIsRequired = true;

            var dbConnectionString = string.Format("Data Source=.\\SQLExpress;Initial Catalog={0};Integrated Security=SSPI;", OzetteLibrary.Constants.Database.DatabaseName);
            CoreSettings.DatabaseConnectionString = dbConnectionString;

            // this entropy/iv key is used only for saving/retrieving app secrets (like storage config tokens).
            // it is not used for encrypting files in the cloud.
            // the iv key must be 16 bytes.
            var encryptionIvBytes = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(encryptionIvBytes);
            var encryptionIvString = Convert.ToBase64String(encryptionIvBytes);
            CoreSettings.ProtectionIv = encryptionIvString;
        }

        /// <summary>
        /// Creates a custom event log and event source.
        /// </summary>
        public static void CreateEventLogSource()
        {
            if (EventLog.Exists(CoreSettings.EventlogName) == false)
            {
                EventLog.CreateEventSource(CoreSettings.EventlogName, CoreSettings.EventlogName);
            }
        }

        /// <summary>
        /// Creates the installation directories.
        /// </summary>
        public static void CreateInstallationDirectories()
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
                        OzetteLibrary.Constants.Database.DefaultSqlExpressUserAccount,
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
        public static void CopyProgramFiles()
        {
            var sourcePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // expected file manifest
            var fileManifest = new List<string>()
            {
                "Microsoft.Azure.KeyVault.Core.dll",
                "Microsoft.Azure.KeyVault.Core.xml",
                "Microsoft.Data.Tools.Schema.Sql.dll",
                "Microsoft.Data.Tools.Utilities.dll",
                "Microsoft.IdentityModel.Logging.dll",
                "Microsoft.IdentityModel.Tokens.dll",
                "Microsoft.IdentityModel.Tokens.xml",
                "Microsoft.SqlServer.Dac.dll",
                "Microsoft.SqlServer.Dac.xml",
                "Microsoft.SqlServer.TransactSql.ScriptDom.dll",
                "Microsoft.SqlServer.TransactSql.ScriptDom.xml",
                "Microsoft.SqlServer.Types.dll",
                "Microsoft.WindowsAzure.Storage.dll",
                "Microsoft.WindowsAzure.Storage.xml",
                "NCrontab.dll",
                "Newtonsoft.Json.dll",
                "Newtonsoft.Json.xml",
                "OzetteClientAgent.exe",
                "OzetteClientAgent.exe.config",
                "OzetteDB.dacpac",
                "OzetteDB.dll",
                "OzetteLibrary.dll",
                "System.IdentityModel.Tokens.Jwt.dll",
                "System.IdentityModel.Tokens.Jwt.xml",
                "System.Management.Automation.dll",
                "System.Management.Automation.xml",
                "Twilio.dll",
                "Twilio.xml"
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
        public static void CreateClientService()
        {
            var existingServices = ServiceController.GetServices();

            if (existingServices.Any(x => x.ServiceName == "OzetteCloudBackup") == false)
            {
                var installArgs = string.Format(
                    "create \"OzetteCloudBackup\" binPath= \"{0}\" start= \"auto\" DisplayName= \"Ozette Cloud Backup Agent\"",
                    Path.Combine(CoreSettings.InstallationDirectory, "OzetteClientAgent.exe"));

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
                    "description \"OzetteCloudBackup\" \"Ozette Cloud Backup is a data backup client service that copies data to cloud providers like Azure and AWS.\"",
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
        public static void StartClientService()
        {
            Process startService = new Process();
            startService.StartInfo = new ProcessStartInfo()
            {
                FileName = "sc.exe",
                Arguments = "start OzetteCloudBackup"
            };

            startService.Start();
            startService.WaitForExit();

            if (startService.ExitCode != 0)
            {
                throw new Exception("Failed to start the windows service. Sc.exe returned an error code: " + startService.ExitCode);
            }
        }
    }
}
