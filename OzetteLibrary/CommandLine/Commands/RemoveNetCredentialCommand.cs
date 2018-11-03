using OzetteLibrary.CommandLine.Arguments;
using OzetteLibrary.Logging.Default;
using System;
using System.Diagnostics;

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
            throw new NotImplementedException();
        }
    }
}
