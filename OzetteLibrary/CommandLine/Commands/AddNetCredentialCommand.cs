using OzetteLibrary.CommandLine.Arguments;
using OzetteLibrary.Logging.Default;
using System;
using System.Diagnostics;

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
        public bool Run(ArgumentBase arguments)
        {
            var addNetCredArgs = arguments as AddNetCredentialArguments;

            if (addNetCredArgs == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            try
            {
                Logger.WriteConsole("--- Starting Ozette Cloud Backup credential configuration");

                Logger.WriteConsole("--- Step 1: Adds the network credential to the database.");
                AddNetCred(addNetCredArgs);

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
        private void AddNetCred(AddNetCredentialArguments arguments)
        {
            throw new NotImplementedException();
        }
    }
}
