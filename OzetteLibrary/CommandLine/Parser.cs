using System.Collections.Generic;

namespace OzetteLibrary.CommandLine
{
    /// <summary>
    /// Contains functionality for parsing Ozette command line arguments.
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// Parses the provided arguments into an <c>Arguments</c> object.
        /// </summary>
        /// <example>
        /// A few examples:
        /// ozettecmd.exe install
        /// ozettecmd.exe install --installdirectory "C:\path"
        /// ozettecmd.exe configure-encryption --protectioniv "passphrase"
        /// </example>
        /// <param name="args">The raw arguments from the commandline.</param>
        /// <param name="parsed">An output parameter for the parsed object.</param>
        /// <returns>True if successfully parsed, otherwise false.</returns>
        public bool Parse(string[] args, out Arguments parsed)
        {
            if (args.Length == 0)
            {
                // no args provided.
                parsed = null;
                return false;
            }

            var baseCommand = args[0].ToLower();

            if (baseCommand == "install")
            {
                return ParseInstallArgs(args, out parsed);
            }
            else if (baseCommand == "configure-encryption")
            {
                return ParseConfigureEncryptionArgs(args, out parsed);
            }
            else if (baseCommand == "configure-azure")
            {
                return ParseConfigureAzureArgs(args, out parsed);
            }
            else
            {
                // unexpected/no base command provided.
                parsed = null;
                return false;
            }
        }

        /// <summary>
        /// Parses the provided arguments into an <c>InstallationArguments</c> object.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="parsed"></param>
        /// <returns></returns>
        private bool ParseInstallArgs(string[] args, out Arguments parsed)
        {
            // initialize args object with default
            var installArgs = new InstallationArguments();
            var map = ExtractArguments(args);

            if (map.ContainsKey("installdirectory"))
            {
                installArgs.InstallDirectory = map["installdirectory"];
            }
            else
            {
                // apply default
                installArgs.InstallDirectory = Constants.CommandLine.DefaultInstallLocation;
            }

            parsed = installArgs;
            return true;
        }

        /// <summary>
        /// Parses the provided arguments into an <c>ConfigureEncryptionArguments</c> object.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="parsed"></param>
        /// <returns></returns>
        private bool ParseConfigureEncryptionArgs(string[] args, out Arguments parsed)
        {
            // initialize args object with default
            var configArgs = new ConfigureEncryptionArguments();
            var map = ExtractArguments(args);

            if (map.ContainsKey("protectioniv"))
            {
                configArgs.ProtectionIv = map["protectioniv"];
            }
            else
            {
                // required argument was not found.
                parsed = null;
                return false;
            }

            parsed = configArgs;
            return true;
        }

        /// <summary>
        /// Parses the provided arguments into an <c>ConfigureAzureArguments</c> object.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="parsed"></param>
        /// <returns></returns>
        private bool ParseConfigureAzureArgs(string[] args, out Arguments parsed)
        {
            // initialize args object with default
            var configArgs = new ConfigureAzureArguments();
            var map = ExtractArguments(args);

            if (map.ContainsKey("azurestorageaccountname"))
            {
                configArgs.AzureStorageAccountName = map["azurestorageaccountname"];
            }
            else
            {
                // required argument was not found.
                parsed = null;
                return false;
            }

            if (map.ContainsKey("azurestorageaccounttoken"))
            {
                configArgs.AzureStorageAccountToken = map["azurestorageaccounttoken"];
            }
            else
            {
                // required argument was not found.
                parsed = null;
                return false;
            }

            parsed = configArgs;
            return true;
        }

        /// <summary>
        /// Returns a dictionary map of the provided arguments.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Dictionary<string, string> ExtractArguments(string[] args)
        {
            var map = new Dictionary<string, string>();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("--"))
                {
                    if (i + 1 > args.Length || args[i+1].StartsWith("--"))
                    {
                        // there is no next argumnt
                        // or the next argument is another parameter option
                        // this means no value is provided. assume a switch param.
                        map.Add(args[i].ToLower().Substring(2), "true");
                    }
                    else
                    {
                        map.Add(args[i].ToLower().Substring(2), args[i + 1]);
                    }
                }
            }

            return map;
        }
    }
}
