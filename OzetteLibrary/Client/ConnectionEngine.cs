using OzetteLibrary.Database;
using OzetteLibrary.Engine;
using OzetteLibrary.Events;
using OzetteLibrary.Folders;
using OzetteLibrary.Logging;
using OzetteLibrary.StorageProviders;
using OzetteLibrary.Secrets;
using OzetteLibrary.ServiceCore;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using OzetteLibrary.MessagingProviders;

namespace OzetteLibrary.Client
{
    /// <summary>
    /// Contains core connection engine functionality.
    /// </summary>
    public class ConnectionEngine : BaseEngine
    {
        /// <summary>
        /// Constructor that accepts a database and logger.
        /// </summary>
        /// <param name="database">The client database connection.</param>
        /// <param name="logger">A logging instance.</param>
        /// <param name="storageProviders">A collection of cloud backup storage provider connections.</param>
        /// <param name="messagingProviders">A collection of messaging provider connections.</param>
        /// <param name="instanceID">An optional parameter to specify the engine instance ID. Default value is 0.</param>
        public ConnectionEngine(IClientDatabase database,
                                ILogger logger,
                                StorageProviderConnectionsCollection storageProviders,
                                MessagingProviderConnectionsCollection messagingProviders,
                                int instanceID = 0)
            : base(database, logger, storageProviders, messagingProviders, instanceID) { }

        /// <summary>
        /// Begins to start the connection engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStart()
        {
            if (Running == true)
            {
                throw new InvalidOperationException("The engine cannot be started, it is already running.");
            }

            Running = true;

            Thread pl = new Thread(() => ProcessLoop());
            pl.Start();
        }

        /// <summary>
        /// Begins to stop the connection engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStop()
        {
            if (Running == true)
            {
                Running = false;
            }
        }

        /// <summary>
        /// Core processing loop.
        /// </summary>
        private void ProcessLoop()
        {
            try
            {
                while (true)
                {
                    // query source locations from the database.
                    var sources = SafeGetSources();
                    if (sources != null && sources.Count > 0)
                    {
                        foreach (var source in sources)
                        {
                            // if we have a network source, validate it.
                            // ensure it is connected or re-established as needed.
                            if (source is NetworkSourceLocation)
                            {
                                SafeVerifyNetSourceConnectionState(source as NetworkSourceLocation);
                            }
                        }
                    }

                    ThreadSleepWithStopRequestCheck(TimeSpan.FromSeconds(60));

                    if (Running == false)
                    {
                        OnStopped(new EngineStoppedEventArgs(EngineStoppedReason.StopRequested, InstanceID));
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                OnStopped(new EngineStoppedEventArgs(ex, InstanceID));
            }
        }

        /// <summary>
        /// Grabs the source locations stored in the database.
        /// </summary>
        /// <remarks>
        /// This function is marked as safe and should not throw exceptions.
        /// </remarks>
        /// <returns></returns>
        private SourceLocations SafeGetSources()
        {
            try
            {
                return Database.GetAllSourceLocations();
            }
            catch (Exception ex)
            {
                string err = "Failed to read source locations from the database.";
                Logger.WriteSystemEvent(err, ex, Logger.GenerateFullContextStackTrace(), Constants.EventIDs.FailedToLoadScanSources, true);

                return null;
            }
        }

        /// <summary>
        /// Safely verifies a network source connection state.
        /// </summary>
        /// <remarks>
        /// This function is marked as safe and should not throw exceptions.
        /// </remarks>
        /// <param name="netSource"></param>
        /// <returns></returns>
        private void SafeVerifyNetSourceConnectionState(NetworkSourceLocation netSource)
        {
            var dir = new DirectoryInfo(netSource.Path);
            if (dir.Exists)
            {
                // we are already connected or have access.
                // this is either an unauthenticated location or we have already establish a connection through net use.
                UpdateNetSourceConnectionState(netSource, true, false);
                return;
            }

            // force disconnect and reconnect

            Logger.WriteTraceMessage("Attempting to connect to network source location: " + netSource.Path);

            string netUser = null;
            string netPass = null;

            if (!string.IsNullOrWhiteSpace(netSource.CredentialName))
            {
                if (!TryGetNetSourceCredentials(netSource.CredentialName, out netUser, out netPass))
                {
                    Logger.WriteTraceError(string.Format(
                        "Unable to connect to network source location: {0}. The specified credential ({1}) was not found in the stored credentials.", 
                        netSource.Path, netSource.CredentialName));

                    UpdateNetSourceConnectionState(netSource, false, true);
                    return;
                }
            }

            DisconnectNetUse(netSource);

            if (!TryConnectNetUse(netSource, netUser, netPass))
            {
                UpdateNetSourceConnectionState(netSource, false, true);
                return;
            }

            // validate again: do we have access now after disconnect/reconnect?
            // if we dont have access after running net use, then we can consider this connection failed.

            var freshDir = new DirectoryInfo(netSource.Path);
            if (freshDir.Exists)
            {
                Logger.WriteTraceMessage("Successfully connected to network source location: " + netSource.Path);
                UpdateNetSourceConnectionState(netSource, true, false);
            }
            else
            {
                Logger.WriteTraceMessage("Failed to connect to network source location. The directory is still not accessible after running NET USE command: " + netSource.Path);
                UpdateNetSourceConnectionState(netSource, false, true);
            }
        }

        /// <summary>
        /// Disconnects a NET USE session.
        /// </summary>
        /// <param name="netSource"></param>
        private void DisconnectNetUse(NetworkSourceLocation netSource)
        {
            var process = new Process();
            var startInfo = new ProcessStartInfo()
            {
                FileName = "NET",
                Arguments = string.Format("USE /DELETE {0}", netSource.Path),
            };

            process.StartInfo = startInfo;

            process.Start();
            process.WaitForExit();

            // we don't actually care what happens here, in terms of exit code or stderr.
            // net use will write to stderr if the connection doesn't exist, which is fine/normal for this use case.
        }

        /// <summary>
        /// Connects a NET USE session.
        /// </summary>
        /// <param name="netSource"></param>
        /// <param name="netPass"></param>
        /// <param name="netUser"></param>
        private bool TryConnectNetUse(NetworkSourceLocation netSource, string netUser, string netPass)
        {
            bool authenticated = !string.IsNullOrWhiteSpace(netUser) && !string.IsNullOrWhiteSpace(netPass);

            var process = new Process();
            var startInfo = new ProcessStartInfo()
            {
                FileName = "NET",
                Arguments = authenticated ? 
                    string.Format("USE {0} /USER:{1} {2}", netSource.Path, netUser, netPass) :
                    string.Format("USE {0}", netSource.Path),
                RedirectStandardError = true,
                UseShellExecute = false
            };

            process.StartInfo = startInfo;

            process.Start();
            process.WaitForExit();

            var stdErr = process.StandardError.ReadToEnd();

            if (process.ExitCode != 0 && !string.IsNullOrWhiteSpace(stdErr))
            {
                string err = string.Format("Failed to connect a network source location: {0}. An error occurred calling NET USE. Error: {1}", netSource.Path, stdErr);
                Logger.WriteTraceError(err);
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Tries to get the network source location credentials by name, from the database.
        /// </summary>
        /// <param name="credentialName"></param>
        /// <param name="netUser"></param>
        /// <param name="netPass"></param>
        /// <returns></returns>
        private bool TryGetNetSourceCredentials(string credentialName, out string netUser, out string netPass)
        {
            var dbCredNames = Database.GetNetCredentialsList();

            var foundCred = dbCredNames.FirstOrDefault(x => string.Equals(x.CredentialName, credentialName, StringComparison.CurrentCultureIgnoreCase));

            if (foundCred == null)
            {
                netUser = null;
                netPass = null;
                return false;
            }

            try
            {
                var scope = System.Security.Cryptography.DataProtectionScope.LocalMachine;
                var ivkey = Convert.FromBase64String(CoreSettings.ProtectionIv);
                var pds = new ProtectedDataStore(Database, scope, ivkey);

                netUser = pds.GetApplicationSecret(string.Format(Constants.Formats.NetCredentialUserNameKeyLookup, foundCred.CredentialName));
                netPass = pds.GetApplicationSecret(string.Format(Constants.Formats.NetCredentialUserPasswordKeyLookup, foundCred.CredentialName));
                return true;
            }
            catch (Exception ex)
            {
                string err = string.Format("Failed to lookup network credentials in the database: {0}. An error occurred: {1}", credentialName, ex.ToString());
                Logger.WriteTraceError(err);

                netUser = null;
                netPass = null;
                return false;
            }
        }

        /// <summary>
        /// Updates the network source connection state in the database.
        /// </summary>
        /// <param name="netSource"></param>
        /// <param name="isConnected"></param>
        /// <param name="isFailed"></param>
        private void UpdateNetSourceConnectionState(NetworkSourceLocation netSource, bool isConnected, bool isFailed)
        {
            try
            {
                netSource.IsConnected = isConnected;
                netSource.IsFailed = isFailed;
                netSource.LastConnectionCheck = DateTime.Now;

                Database.UpdateSourceLocation(netSource);
            }
            catch (Exception ex)
            {
                string err = "Failed to save source locations to the database.";
                Logger.WriteSystemEvent(err, ex, Logger.GenerateFullContextStackTrace(), Constants.EventIDs.FailedToSaveScanSources, true);
            }
        }
    }
}
