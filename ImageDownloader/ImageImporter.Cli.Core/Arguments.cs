using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.CommandLine;
using CommandLine;

[assembly:InternalsVisibleTo("ImageImporter.Cli.Core.Tests")]
namespace ImageImporter.Cli.Core
{
    internal class Arguments
    {
        public bool NoPathProvided
        {
            get => OutputDirectory.Equals(System.Environment.CurrentDirectory);
        }

        [Option(
            'i',
            "input-directory",
            Required = true,
            HelpText = "Input directory with image files") ]
        public string InputDirectory { set; get; }
        [Option(
            'o',
            "output-directory",
            SetName = "options",
            Required = false,
            HelpText = "Directory to put files to (will be created if not present)")]
        public string OutputDirectory { set; get; }
        [Option(
            'r',
            "raw-types",
            SetName = "options",
            Required = false,
            HelpText = "Extensions for RAW image types (e.g. .ext1 .ext2")]
        public IEnumerable<string> RawFileExtensions { get; set; }
        [Option(
            'n',
            "non-raw-types",
            SetName = "options",
            Required = false,
            HelpText = "Extensions for non-RAW image types (e.g. .ext1 .ext2")]
        public IEnumerable<string> NonRawFileExtensions { get; set; }
        [Option(
            'v',
            "video-types",
            SetName = "options",
            Required = false,
            HelpText = "Extensions for video image types (e.g. .ext1 .ext2")]
        public IEnumerable<string> VideoFileExtensions { get; set; }
        [Option(
            'c',
            "configuration-path",
            SetName = "file",
            Required = false,
            HelpText = "Path to a configuration file")]
        public string ConfigurationPath { set; get; }

        public Arguments()
        {
            RawFileExtensions = new List<string>();
            NonRawFileExtensions = new List<string>();
            VideoFileExtensions = new List<string>();
        }

        /// <summary>
        /// Parses comman-line arguments into domain object
        /// </summary>
        /// <param name="args">Command-line arguments as a strings array</param>
        /// <returns>Instance of <see cref="Arguments"></returns>
        public static bool Parse(string[] args, out Arguments processedArguments)
        {
            var arguments = new Arguments();
            var parsed = true;
            Parser.Default.ParseArguments<Arguments>(args)
                .WithParsed(a =>
                {
                    arguments.ConfigurationPath = a.ConfigurationPath ?? default;
                    arguments.InputDirectory = a.InputDirectory;
                    arguments.NonRawFileExtensions = a.NonRawFileExtensions;
                    arguments.OutputDirectory = a.OutputDirectory ?? default;
                    arguments.VideoFileExtensions = a.VideoFileExtensions;
                    arguments.RawFileExtensions = a.RawFileExtensions;
                })
                .WithNotParsed(a => parsed = false);
            processedArguments = arguments;
            return parsed;
        }
    }
}
