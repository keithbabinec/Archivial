using OzetteLibrary.CommandLine;
using OzetteLibrary.CommandLine.Commands;
using OzetteLibrary.Logging.Default;
using System;
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
            Arguments argumentObj;

            if (parser.Parse(args, out argumentObj))
            {
                var command = GetCommandFromArguments(argumentObj);

                if (command.Run(argumentObj))
                {
                    // command completed successfully
                    return 0;
                }
                else
                {
                    // command failed.
                    return 2;
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
            help.AppendLine("\tinstall");
            help.AppendLine("\t\t[--installdirectory]\tAn optional value to specify the installation folder.");
            help.AppendLine();
            help.AppendLine("\tconfigure-azure");
            help.AppendLine("\t\t--azurestorageaccountname\tThe name of the Azure storage account to backup files to.");
            help.AppendLine("\t\t--azurestorageaccounttoken\tThe access token for the Azure storage account.");
            help.AppendLine();
            help.AppendLine("\tadd-source");
            help.AppendLine("\t\t--folderpath\tThe full folder path that should be backed up.");
            help.AppendLine("\t\t[--priority]\tThe backup priority (specify \"Low\", \"Medium\", or \"High\"). Defaults to Medium if omitted.");
            help.AppendLine("\t\t[--revisions]\tThe number of revisions to store (specify a number, such as 1, 2, 3, etc). Defaults to 1 if omitted.");
            help.AppendLine("\t\t[--matchfilter]\tAn optional wildcard match filter that scopes this source to only certain files.");

            Console.WriteLine(help.ToString());
        }

        /// <summary>
        /// Returns the matching command object for the specified arguments.
        /// </summary>
        /// <param name="arguments"><c>Arguments</c></param>
        /// <returns><c>ICommand</c></returns>
        static ICommand GetCommandFromArguments(Arguments arguments)
        {
            Logger logger = new Logger(OzetteLibrary.Constants.Logging.CommandLineComponentName);
            ICommand command = null;

            if (arguments is InstallationArguments)
            {
                command = new InstallCommand(logger);
            }
            else if (arguments is AddSourceArguments)
            {
                command = new AddSourceCommand(logger);
            }
            else if (arguments is ConfigureAzureArguments)
            {
                command = new ConfigureAzureCommand(logger);
            }
            else
            {
                throw new NotImplementedException(arguments.GetType().FullName);
            }

            return command;
        }
    }
}
