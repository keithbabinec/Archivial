using OzetteLibrary.CommandLine;
using OzetteLibrary.CommandLine.Arguments;
using OzetteLibrary.CommandLine.Commands;
using OzetteLibrary.Logging.Default;
using System;
using System.Security.Principal;
using System.Text;

namespace OzetteCmd
{
    /// <summary>
    /// Contains command line functionality for Ozette.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main entrypoint.
        /// </summary>
        /// <param name="args"></param>
        static int Main(string[] args)
        {
            var parser = new Parser();
            ArgumentBase argumentObj;

            if (parser.Parse(args, out argumentObj))
            {
                var command = GetCommandFromArguments(argumentObj);

                if (RequiresRelaunchWithElevation(command.GetType()))
                {
                    Console.Error.WriteLine("The requested command requires elevated (administrator) permissions to execute. Please re-launch the program with Run-As Administrator permissions.");
                    return 2;
                }

                if (command.Run(argumentObj))
                {
                    // command completed successfully
                    return 0;
                }
                else
                {
                    // command failed.
                    return 3;
                }
            }
            else
            {
                // arguments error
                DisplayHelp();
                return 1;
            }
        }

        /// <summary>
        /// Prints the help message to the console.
        /// </summary>
        static void DisplayHelp()
        {
            var help = new StringBuilder();
            help.AppendLine("-----------------------------------------------------");
            help.AppendLine("--- Ozette Cloud Backup command line utility");
            help.AppendLine();
            help.AppendLine("General usage:");
            help.AppendLine("\tOzetteCmd.exe <command> --Option1Name Option1Value --Option2Name Option2Value");
            help.AppendLine();
            help.AppendLine("Commands:");
            help.AppendLine();
            help.AppendLine("OzetteCmd.exe install");
            help.AppendLine();
            help.AppendLine("  Description:");
            help.AppendLine("\tInstalls the Ozette Cloud Backup software on this computer.");
            help.AppendLine("  Arguments:");
            help.AppendLine("\t[--installdirectory]\tAn optional value to specify the installation folder.");
            help.AppendLine();
            help.AppendLine("OzetteCmd.exe list-providers");
            help.AppendLine();
            help.AppendLine("  Description:");
            help.AppendLine("\tLists the currently configured cloud backup providers (Azure, AWS, etc).");
            help.AppendLine();
            help.AppendLine("OzetteCmd.exe configure-azure");
            help.AppendLine();
            help.AppendLine("  Description:");
            help.AppendLine("\tConfigures Azure as a cloud backup provider for this computer.");
            help.AppendLine("  Arguments:");
            help.AppendLine("\t--azurestorageaccountname\tThe name of the Azure storage account to backup files to.");
            help.AppendLine("\t--azurestorageaccounttoken\tThe access token for the Azure storage account.");
            help.AppendLine();
            help.AppendLine("OzetteCmd.exe remove-provider");
            help.AppendLine();
            help.AppendLine("  Description:");
            help.AppendLine("\tRemoves a cloud backup provider (Azure, AWS, etc) with specified ID. Run list-providers to see the current providers with IDs.");
            help.AppendLine("  Arguments:");
            help.AppendLine("\t--providerid\tThe ID of the provider to remove.");
            help.AppendLine();
            help.AppendLine("OzetteCmd.exe list-sources");
            help.AppendLine();
            help.AppendLine("  Description:");
            help.AppendLine("\tLists the currently configured backup source directories (folders to backup).");
            help.AppendLine();
            help.AppendLine("OzetteCmd.exe add-localsource");
            help.AppendLine();
            help.AppendLine("  Description:");
            help.AppendLine("\tAdds a local directory to the list of backup sources (folders to backup).");
            help.AppendLine("  Arguments:");
            help.AppendLine("\t--folderpath\tThe full folder path that should be backed up.");
            help.AppendLine("\t[--priority]\tThe backup priority (specify \"Low\", \"Medium\", or \"High\"). Defaults to Medium if omitted.");
            help.AppendLine("\t[--revisions]\tThe number of revisions to store (specify a number, such as 1, 2, 3, etc). Defaults to 1 if omitted.");
            help.AppendLine("\t[--matchfilter]\tAn optional wildcard match filter that scopes this source to only certain files.");
            help.AppendLine();
            help.AppendLine("OzetteCmd.exe add-netsource");
            help.AppendLine();
            help.AppendLine("  Description:");
            help.AppendLine("\tAdds a remote/UNC directory to the list of backup sources (folders to backup).");
            help.AppendLine("  Arguments:");
            help.AppendLine("\t--uncpath\tThe full UNC folder path that should be backed up.");
            help.AppendLine("\t[--credentialname]\tThe name of the network credential to lookup (if authentication is required).");
            help.AppendLine("\t[--priority]\tThe backup priority (specify \"Low\", \"Medium\", or \"High\"). Defaults to Medium if omitted.");
            help.AppendLine("\t[--revisions]\tThe number of revisions to store (specify a number, such as 1, 2, 3, etc). Defaults to 1 if omitted.");
            help.AppendLine("\t[--matchfilter]\tAn optional wildcard match filter that scopes this source to only certain files.");
            help.AppendLine();
            help.AppendLine("OzetteCmd.exe remove-source");
            help.AppendLine();
            help.AppendLine("  Description:");
            help.AppendLine("\tRemoves a backup source with specified ID. Run list-sources to see the current sources with IDs.");
            help.AppendLine("  Arguments:");
            help.AppendLine("\t--sourceid\tThe ID of the backup source to remove.");

            Console.WriteLine(help.ToString());
        }

        /// <summary>
        /// Returns the matching command object for the specified arguments.
        /// </summary>
        /// <param name="arguments"><c>Arguments</c></param>
        /// <returns><c>ICommand</c></returns>
        static ICommand GetCommandFromArguments(ArgumentBase arguments)
        {
            Logger logger = new Logger(OzetteLibrary.Constants.Logging.CommandLineComponentName);
            ICommand command = null;

            if (arguments is InstallationArguments)
            {
                command = new InstallCommand(logger);
            }
            else if (arguments is AddLocalSourceArguments)
            {
                command = new AddLocalSourceCommand(logger);
            }
            else if (arguments is AddNetSourceArguments)
            {
                command = new AddNetSourceCommand(logger);
            }
            else if (arguments is ConfigureAzureArguments)
            {
                command = new ConfigureAzureCommand(logger);
            }
            else if (arguments is ListSourcesArguments)
            {
                command = new ListSourcesCommand(logger);
            }
            else if (arguments is ListProvidersArguments)
            {
                command = new ListProvidersCommand(logger);
            }
            else if (arguments is RemoveSourceArguments)
            {
                command = new RemoveSourceCommand(logger);
            }
            else if (arguments is RemoveProviderArguments)
            {
                command = new RemoveProviderCommand(logger);
            }
            else
            {
                throw new NotImplementedException(arguments.GetType().FullName);
            }

            return command;
        }

        /// <summary>
        /// Checks if the program should exit and re-launch with elevation.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        static bool RequiresRelaunchWithElevation(Type type)
        {
            Attribute[] attributes = Attribute.GetCustomAttributes(type);

            foreach (var attribute in attributes)
            {
                if (attribute is RequiresElevation)
                {
                    // this type requires elevation
                    // check if we are elevated or not.

                    WindowsIdentity identity = WindowsIdentity.GetCurrent();
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    var isAdministrator = principal.IsInRole(WindowsBuiltInRole.Administrator);

                    if (isAdministrator)
                    {
                        // we are already elevated.
                        // no need to re-launch with elevation.
                        return false;
                    }
                    else
                    {
                        // we are not elevated, and this command requires it.
                        // request re-launch with elevation.
                        return true;
                    }
                }
            }

            // No RequiresElevation attribute was found on the type.
            // we can run this command regardless of current elevation state- no requirement to relaunch.
            return false;
        }
    }
}
