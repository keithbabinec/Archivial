using OzetteLibrary.CommandLine;
using OzetteLibrary.CommandLine.Commands;
using OzetteLibrary.Logging;
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
        static void Main(string[] args)
        {
            var parser = new Parser();
            Arguments parsed;

            if (parser.Parse(args, out parsed))
            {
                if (parsed is InstallationArguments)
                {
                    var logger = InitializeLogger(OzetteLibrary.Constants.Logging.InstallationComponentName);
                    var installArgs = parsed as InstallationArguments;

                    Install.Run(logger, installArgs);
                }
                else
                {
                    throw new NotImplementedException("Args type not implemented: " + parsed.GetType().FullName);
                }
            }
            else
            {
                DisplayHelp();
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
            help.AppendLine("\t\t[--databasepath]\tAn optional value to specify the database file path.");
            help.AppendLine();
            help.AppendLine("\tconfigure-encryption");
            help.AppendLine("\t\t--protectioniv\tA passphrase to customize the encryption.");
            help.AppendLine();
            help.AppendLine("\tconfigure-azure");
            help.AppendLine("\t\t--azurestorageaccountname\tThe name of the Azure storage account to backup files to.");
            help.AppendLine("\t\t--azurestorageaccounttoken\tThe access token for the Azure storage account.");

            Console.WriteLine(help.ToString());
        }

        /// <summary>
        /// Initializes the logging component.
        /// </summary>
        /// <param name="componentName"></param>
        /// <returns></returns>
        static ILogger InitializeLogger(string componentName)
        {
            var logger = new Logger(componentName);

            logger.Start(
                Properties.Settings.Default.EventlogName,
                Properties.Settings.Default.EventlogName,
                Properties.Settings.Default.LogFilesDirectory);

            return logger;
        }
    }
}
