using OzetteLibrary.CommandLine.Arguments;
using OzetteLibrary.Logging.Default;
using System;
using System.Diagnostics;

namespace OzetteLibrary.CommandLine.Commands
{
    public class ListNetCredentialsCommand : ICommand
    {
        /// <summary>
        /// A logging helper instance.
        /// </summary>
        private Logger Logger;

        /// <summary>
        /// Constructor that requires a logging instance.
        /// </summary>
        /// <param name="logger"></param>
        public ListNetCredentialsCommand(Logger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Logger = logger;
        }

        /// <summary>
        /// Runs the list-netcredential command.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Run(ArgumentBase arguments)
        {
            var listCredsArgs = arguments as ListNetCredentialArguments;

            if (listCredsArgs == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            try
            {
                Logger.WriteConsole("--- Starting Ozette Cloud Backup credential configuration");

                Logger.WriteConsole("--- Step 1: List the network credentials from the database.");
                ListNetCreds(listCredsArgs);

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
        /// Lists net credentials
        /// </summary>
        /// <param name="arguments"></param>
        private void ListNetCreds(ListNetCredentialArguments arguments)
        {
            throw new NotImplementedException();
        }
    }
}
