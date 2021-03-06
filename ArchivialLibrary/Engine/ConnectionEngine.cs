﻿using ArchivialLibrary.Database;
using ArchivialLibrary.Events;
using ArchivialLibrary.Folders;
using ArchivialLibrary.Logging;
using ArchivialLibrary.Secrets;
using ArchivialLibrary.ServiceCore;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ArchivialLibrary.Engine
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
        /// <param name="instanceID">A parameter to specify the engine instance ID.</param>
        /// <param name="coreSettings">The core settings accessor.</param>
        public ConnectionEngine(IClientDatabase database,
                                ILogger logger,
                                int instanceID,
                                ICoreSettings coreSettings)
            : base(database, logger, instanceID, coreSettings) { }

        /// <summary>
        /// Begins to start the connection engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStart()
        {
            Thread pl = new Thread(() => ProcessLoopAsync().Wait());
            pl.Start();
        }

        /// <summary>
        /// Begins to stop the connection engine, returns immediately to the caller.
        /// </summary>
        public override void BeginStop()
        {
            CancelSource.Cancel();
            Logger.WriteTraceMessage("Connection engine is shutting down by request.", InstanceID);
        }

        /// <summary>
        /// Core processing loop.
        /// </summary>
        private async Task ProcessLoopAsync()
        {
            try
            {
                while (true)
                {
                    // query source locations from the database.
                    var sources = await SafeGetSourcesAsync().ConfigureAwait(false);
                    if (sources != null && sources.Count > 0)
                    {
                        foreach (var source in sources)
                        {
                            // if we have a network source, validate it.
                            // ensure it is connected or re-established as needed.
                            if (source is NetworkSourceLocation)
                            {
                                await SafeVerifyNetSourceConnectionStateAsync(source as NetworkSourceLocation).ConfigureAwait(false);
                            }
                        }
                    }

                    await WaitAsync(TimeSpan.FromMinutes(1)).ConfigureAwait(false);

                    if (CancelSource.Token.IsCancellationRequested)
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
        private async Task<SourceLocations> SafeGetSourcesAsync()
        {
            try
            {
                return await Database.GetSourceLocationsAsync().ConfigureAwait(false);
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
        private async Task SafeVerifyNetSourceConnectionStateAsync(NetworkSourceLocation netSource)
        {
            var dir = new DirectoryInfo(netSource.Path);
            if (dir.Exists)
            {
                // we are already connected or have access.
                // this is either an unauthenticated location or we have already establish a connection through net use.
                await UpdateNetSourceConnectionStateAsync(netSource, true, false).ConfigureAwait(false);
                return;
            }

            // force disconnect and reconnect

            if (netSource.CredentialName == null)
            {
                Logger.WriteTraceError(string.Format(
                    "Unable to connect to network source location: {0}.",
                    netSource.Path, netSource.CredentialName));

                await UpdateNetSourceConnectionStateAsync(netSource, false, true).ConfigureAwait(false);
                return;
            }

            Logger.WriteTraceMessage("Attempting to connect to network source location: " + netSource.Path);

            var creds = await GetNetSourceCredentialsAsync(netSource.CredentialName).ConfigureAwait(false);

            if (creds == null)
            {
                Logger.WriteTraceError(string.Format(
                    "Unable to connect to network source location: {0}. The specified credential ({1}) was not found in the stored credentials.", 
                    netSource.Path, netSource.CredentialName));

                await UpdateNetSourceConnectionStateAsync(netSource, false, true).ConfigureAwait(false);
                return;
            }

            DisconnectNetUse(netSource);

            if (!TryConnectNetUse(netSource, creds.Item1, creds.Item2))
            {
                await UpdateNetSourceConnectionStateAsync(netSource, false, true).ConfigureAwait(false);
                return;
            }

            // validate again: do we have access now after disconnect/reconnect?
            // if we dont have access after running net use, then we can consider this connection failed.

            var freshDir = new DirectoryInfo(netSource.Path);
            if (freshDir.Exists)
            {
                Logger.WriteTraceMessage("Successfully connected to network source location: " + netSource.Path);
                await UpdateNetSourceConnectionStateAsync(netSource, true, false).ConfigureAwait(false);
            }
            else
            {
                Logger.WriteTraceMessage("Failed to connect to network source location. The directory is still not accessible after running NET USE command: " + netSource.Path);
                await UpdateNetSourceConnectionStateAsync(netSource, false, true).ConfigureAwait(false);
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
        private async Task<Tuple<string,string>> GetNetSourceCredentialsAsync(string credentialName)
        {
            var dbCredNames = await Database.GetNetCredentialsAsync().ConfigureAwait(false);

            var foundCred = dbCredNames.FirstOrDefault(x => string.Equals(x.CredentialName, credentialName, StringComparison.CurrentCultureIgnoreCase));

            if (foundCred == null)
            {
                return null;
            }

            try
            {
                var scope = System.Security.Cryptography.DataProtectionScope.LocalMachine;
                var settingName = Constants.RuntimeSettingNames.ProtectionIV;
                var protectionIvEncodedString = await Database.GetApplicationOptionAsync(settingName).ConfigureAwait(false);
                var ivBytes = Convert.FromBase64String(protectionIvEncodedString);
                var pds = new ProtectedDataStore(Database, scope, ivBytes);

                var result = new Tuple<string, string>(
                    await pds.GetApplicationSecretAsync(string.Format(Constants.Formats.NetCredentialUserNameKeyLookup, foundCred.CredentialName)).ConfigureAwait(false),
                    await pds.GetApplicationSecretAsync(string.Format(Constants.Formats.NetCredentialUserPasswordKeyLookup, foundCred.CredentialName)).ConfigureAwait(false)
                );

                return result;
            }
            catch (Exception ex)
            {
                string err = string.Format("Failed to lookup network credentials in the database: {0}. An error occurred: {1}", credentialName, ex.ToString());
                Logger.WriteTraceError(err);

                return null;
            }
        }

        /// <summary>
        /// Updates the network source connection state in the database.
        /// </summary>
        /// <param name="netSource"></param>
        /// <param name="isConnected"></param>
        /// <param name="isFailed"></param>
        private async Task UpdateNetSourceConnectionStateAsync(NetworkSourceLocation netSource, bool isConnected, bool isFailed)
        {
            try
            {
                netSource.IsConnected = isConnected;
                netSource.IsFailed = isFailed;
                netSource.LastConnectionCheck = DateTime.Now;

                await Database.SetSourceLocationAsync(netSource).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                string err = "Failed to save source locations to the database.";
                Logger.WriteSystemEvent(err, ex, Logger.GenerateFullContextStackTrace(), Constants.EventIDs.FailedToSaveScanSources, true);
            }
        }
    }
}
