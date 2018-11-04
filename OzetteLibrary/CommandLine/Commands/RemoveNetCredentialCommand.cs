using OzetteLibrary.CommandLine.Arguments;
using OzetteLibrary.Database.LiteDB;
using OzetteLibrary.Logging.Default;
using OzetteLibrary.ServiceCore;
using System;
using System.Diagnostics;
using System.Linq;

namespace OzetteLibrary.CommandLine.Commands
{
    public class RemoveNetCredentialCommand : ICommand
    {
        /// <summary>
        /// A logging helper instance.
        /// </summary>
        private Logger Logger;

        /// <summary>
        /// Constructor that requires a logging instance.
        /// </summary>
        /// <param name="logger"></param>
        public RemoveNetCredentialCommand(Logger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Logger = logger;
        }

        /// <summary>
        /// Runs the remove-netcredential command.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Run(ArgumentBase arguments)
        {
            var removeProviderArgs = arguments as RemoveNetCredentialArguments;

            if (removeProviderArgs == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            try
            {
                Logger.WriteConsole("--- Starting Ozette Cloud Backup credential configuration");

                Logger.WriteConsole("--- Step 1: Remove the network credential from the database.");
                RemoveNetCred(removeProviderArgs);

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
        /// Removes the specified credential.
        /// </summary>
        /// <param name="arguments"></param>
        private void RemoveNetCred(RemoveNetCredentialArguments arguments)
        {
            Logger.WriteConsole("Initializing a database connection.");

            var db = new LiteDBClientDatabase(CoreSettings.DatabaseConnectionString);
            db.PrepareDatabase();

            Logger.WriteConsole("Querying for existing network credentials to see if the specified credential exists.");

            var allCredentialsList = db.GetNetCredentialsList();
            var credToRemove = allCredentialsList.FirstOrDefault(x => x.CredentialName == arguments.CredentialName);

            if (credToRemove == null)
            {
                // the credential doesn't exist. nothing to do.
                Logger.WriteConsole("No network credential was found with the specified name. Nothing to remove.");
                return;
            }

            Logger.WriteConsole("Found a matching network credential, removing it now.");

            allCredentialsList.Remove(credToRemove);
            db.SetNetCredentialsList(allCredentialsList);

            // remove provider specific secrets
            db.RemoveApplicationOption(string.Format(Constants.Formats.NetCredentialUserNameKeyLookup, credToRemove.CredentialName));
            db.RemoveApplicationOption(string.Format(Constants.Formats.NetCredentialUserPasswordKeyLookup, credToRemove.CredentialName));

            Logger.WriteConsole("Successfully removed the credential from the database.");
        }
    }
}
