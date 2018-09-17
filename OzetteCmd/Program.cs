using OzetteLibrary.CommandLine;
using OzetteLibrary.CommandLine.Commands;
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
            Arguments parsed;

            if (parser.Parse(args, out parsed))
            {
                if (parsed is InstallationArguments)
                {
                    if (Install.Run(parsed as InstallationArguments))
                    {
                        return 0;
                    }
                    else
                    {
                        return 2;
                    }
                }
                if (parsed is ConfigureAzureArguments)
                {
                    if (ConfigureAzure.Run(parsed as ConfigureAzureArguments))
                    {
                        return 0;
                    }
                    else
                    {
                        return 2;
                    }
                }
                else
                {
                    throw new NotImplementedException("Args type not implemented: " + parsed.GetType().FullName);
                }
            }
            else
            {
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
    }
}
