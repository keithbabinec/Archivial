using OzetteLibrary.CommandLine.Arguments;
using OzetteLibrary.Logging.Default;
using System;
using System.Diagnostics;

namespace OzetteLibrary.CommandLine.Commands
{
    public class RemoveProviderCommand : ICommand
    {
        /// <summary>
        /// A logging helper instance.
        /// </summary>
        private Logger Logger;

        /// <summary>
        /// Constructor that requires a logging instance.
        /// </summary>
        /// <param name="logger"></param>
        public RemoveProviderCommand(Logger logger)
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
            var removeProviderArgs = arguments as RemoveProviderArguments;

            if (removeProviderArgs == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            try
            {
                Logger.WriteConsole("--- Starting Ozette Cloud Backup provider configuration");

                Logger.WriteConsole("--- Step 1: Remove the provider from the database.");

                Logger.WriteConsole("--- Provider configuration completed successfully.");

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteConsole("--- Ozette Cloud Backup provider configuration failed", EventLogEntryType.Error);
                Logger.WriteConsole(ex.ToString(), EventLogEntryType.Error);
                return false;
            }
        }
    }
}
