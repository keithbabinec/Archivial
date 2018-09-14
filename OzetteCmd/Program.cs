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
            help.AppendLine(string.Format("----------------------------------------"));
            help.AppendLine(string.Format("--- {0} command line utility help/usage", OzetteLibrary.Constants.Logging.AppName));


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
