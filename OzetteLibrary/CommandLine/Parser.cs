using OzetteLibrary.CommandLine.Arguments;
using OzetteLibrary.Exceptions;
using OzetteLibrary.Files;
using OzetteLibrary.Folders;
using System;
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
        /// <param name="args">The raw arguments from the commandline.</param>
        /// <param name="parsed">An output parameter for the parsed object.</param>
        /// <returns>True if successfully parsed, otherwise false.</returns>
        public bool Parse(string[] args, out ArgumentBase parsed)
        {
            if (args.Length == 0)
            {
                // no args provided.
                parsed = null;
                return false;
            }

            var baseCommand = args[0].ToLower();

            if (baseCommand == "list-providers")
            {
                // command has no additional arguments
                parsed = new ListProvidersArguments();
                return true;
            }
            else if (baseCommand == "remove-source")
            {
                return ParseRemoveSourceArgs(args, out parsed);
            }
            else if (baseCommand == "rescan-source")
            {
                return ParseRescanSourceArgs(args, out parsed);
            }
            else if (baseCommand == "remove-provider")
            {
                return ParseRemoveProviderArgs(args, out parsed);
            }
            else if (baseCommand == "remove-netcredential")
            {
                return ParseRemoveNetCredentialArgs(args, out parsed);
            }
            else if (baseCommand == "show-status")
            {
                // command has no additional arguments
                parsed = new ShowStatusArguments();
                return true;
            }
            else
            {
                // unexpected/no base command provided.
                parsed = null;
                return false;
            }
        }

        /// <summary>
        /// Parses the provided arguments into an <c>RemoveNetCredentialArguments</c> object.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="parsed"></param>
        /// <returns></returns>
        private bool ParseRemoveNetCredentialArgs(string[] args, out ArgumentBase parsed)
        {
            // initialize args object with default
            var configArgs = new RemoveNetCredentialArguments();
            var map = ExtractArguments(args);

            if (map.ContainsKey("credentialname"))
            {
                configArgs.CredentialName = map["credentialname"];
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
        /// Parses the provided arguments into an <c>RemoveSourceArguments</c> object.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="parsed"></param>
        /// <returns></returns>
        private bool ParseRemoveSourceArgs(string[] args, out ArgumentBase parsed)
        {
            // initialize args object with default
            var remSrcArgs = new RemoveSourceArguments();
            var map = ExtractArguments(args);

            if (map.ContainsKey("sourceid"))
            {
                var sourceId = map["sourceid"];
                int parsedId;

                if (int.TryParse(sourceId, out parsedId))
                {
                    remSrcArgs.SourceID = parsedId;
                }
                else
                {
                    // required argument was not valid.
                    parsed = null;
                    return false;
                }
            }
            else
            {
                // required argument was not found.
                parsed = null;
                return false;
            }

            if (map.ContainsKey("sourcetype"))
            {
                var sourceType = map["sourcetype"];
                SourceLocationType parsedLocationType;

                if (Enum.TryParse(sourceType, true, out parsedLocationType))
                {
                    remSrcArgs.SourceType = parsedLocationType;
                }
                else
                {
                    // an optional argument was specified, but was not given a valid value.
                    throw new SourceLocationException("Invalid source type value.");
                }
            }
            else
            {
                // required argument was not found.
                parsed = null;
                return false;
            }

            parsed = remSrcArgs;
            return true;
        }

        /// <summary>
        /// Parses the provided arguments into an <c>RescanSourceArguments</c> object.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="parsed"></param>
        /// <returns></returns>
        private bool ParseRescanSourceArgs(string[] args, out ArgumentBase parsed)
        {
            // initialize args object with default
            var scanSrcArgs = new RescanSourceArguments();
            var map = ExtractArguments(args);

            if (map.ContainsKey("sourceid"))
            {
                var sourceId = map["sourceid"];
                int parsedId;

                if (int.TryParse(sourceId, out parsedId))
                {
                    scanSrcArgs.SourceID = parsedId;
                }
                else
                {
                    // required argument was not valid.
                    parsed = null;
                    return false;
                }
            }
            else
            {
                // required argument was not found.
                parsed = null;
                return false;
            }

            if (map.ContainsKey("sourcetype"))
            {
                var sourceType = map["sourcetype"];
                SourceLocationType parsedLocationType;

                if (Enum.TryParse(sourceType, true, out parsedLocationType))
                {
                    scanSrcArgs.SourceType = parsedLocationType;
                }
                else
                {
                    // an optional argument was specified, but was not given a valid value.
                    throw new SourceLocationException("Invalid source type value.");
                }
            }
            else
            {
                // required argument was not found.
                parsed = null;
                return false;
            }

            parsed = scanSrcArgs;
            return true;
        }

        /// <summary>
        /// Parses the provided arguments into an <c>RemoveProviderArguments</c> object.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="parsed"></param>
        /// <returns></returns>
        private bool ParseRemoveProviderArgs(string[] args, out ArgumentBase parsed)
        {
            // initialize args object with default
            var remSrcArgs = new RemoveProviderArguments();
            var map = ExtractArguments(args);

            if (map.ContainsKey("providerid"))
            {
                var providerId = map["providerid"];
                int parsedId;

                if (int.TryParse(providerId, out parsedId))
                {
                    remSrcArgs.ProviderID = parsedId;
                }
                else
                {
                    // required argument was not valid.
                    parsed = null;
                    return false;
                }
            }
            else
            {
                // required argument was not found.
                parsed = null;
                return false;
            }

            parsed = remSrcArgs;
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
