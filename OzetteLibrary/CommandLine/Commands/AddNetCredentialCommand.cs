using OzetteLibrary.CommandLine.Arguments;
using OzetteLibrary.Database.SQLServer;
using OzetteLibrary.Logging.Default;
using OzetteLibrary.Secrets;
using OzetteLibrary.ServiceCore;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OzetteLibrary.CommandLine.Commands
{
    public class AddNetCredentialCommand : ICommand
    {
        /// <summary>
        /// A logging helper instance.
        /// </summary>
        private Logger Logger;

        /// <summary>
        /// Constructor that requires a logging instance.
        /// </summary>
        /// <param name="logger"></param>
        public AddNetCredentialCommand(Logger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Logger = logger;
        }

        /// <summary>
        /// Runs the remove-provider command.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns>True if successful, otherwise false.</returns>
        public async Task<bool> RunAsync(ArgumentBase arguments)
        {
            var addNetCredArgs = arguments as AddNetCredentialArguments;

            if (addNetCredArgs == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            try
            {
                Logger.WriteConsole("--- Starting Ozette Cloud Backup credential configuration");

                Logger.WriteConsole("--- Step 1: Adds the network credential name to the database.");
                await AddNetCredAsync(addNetCredArgs);

                Logger.WriteConsole("--- Step 2: Encrypt and save credential username and password settings.");
                await EncryptAndSaveAsync(addNetCredArgs);

                Logger.WriteConsole("--- Credential configuration completed successfully.");

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteConsole("--- Ozette Cloud Backup credential configuration failed", EventLogEntryType.Error);
                Logger.WriteConsole(ex.ToString(), EventLogEntryType.Error);
                return false;
            }
        }

        /// <summary>
        /// Adds the specified credential.
        /// </summary>
        /// <param name="arguments"></param>
        private async Task AddNetCredAsync(AddNetCredentialArguments arguments)
        {
            Logger.WriteConsole("Initializing a database connection.");

            var db = new SQLServerClientDatabase(CoreSettings.DatabaseConnectionString);

            Logger.WriteConsole("Fetching current network credentials from the database.");

            var existingCredsList = await db.GetNetCredentialsAsync();

            if (existingCredsList.Any(x => x.CredentialName == arguments.CredentialName) == false)
            {
                Logger.WriteConsole("The specified credential is not configured in the client database. Adding it now.");

                var newcreds = new NetCredential() { CredentialName = arguments.CredentialName };

                await db.AddNetCredentialAsync(newcreds);

                Logger.WriteConsole("Successfully added the network credential.");
            }
            else
            {
                Logger.WriteConsole("The specified network credential is already in the client database. No action required.");
            }
        }

        /// <summary>
        /// Encrypts and saves the net credential settings.
        /// </summary>
        /// <param name="arguments"></param>
        private async Task EncryptAndSaveAsync(AddNetCredentialArguments arguments)
        {
            Logger.WriteConsole("Initializing a database connection.");

            var db = new SQLServerClientDatabase(CoreSettings.DatabaseConnectionString);

            Logger.WriteConsole("Initializing protected data store.");

            var scope = System.Security.Cryptography.DataProtectionScope.LocalMachine;
            var ivkey = Convert.FromBase64String(CoreSettings.ProtectionIv);
            var pds = new ProtectedDataStore(db, scope, ivkey);

            Logger.WriteConsole("Saving encrypted username setting.");

            await pds.SetApplicationSecretAsync(string.Format(Constants.Formats.NetCredentialUserNameKeyLookup, arguments.CredentialName), arguments.ShareUser);

            Logger.WriteConsole("Saving encrypted password setting.");

            await pds.SetApplicationSecretAsync(string.Format(Constants.Formats.NetCredentialUserPasswordKeyLookup, arguments.CredentialName), arguments.SharePassword);
        }
    }
}
