using System;
using System.Collections.Generic;
using ShellProgressBar;

namespace ImageImporter.Cli.Framework
{
    class Program
    {
        private static ProgressBar progressBar;

        static void Main(string[] args)
        {
            var parser = SetupParser();
            var parseResult = parser.Parse(args);
            if (!parseResult.HasErrors && !parseResult.HelpCalled)
            {
                IImageImporter imageImporter = new ImageImporter();
                imageImporter.ImportStarted += ImportStarted;
                imageImporter.FileCopied += FileCopied;
                imageImporter.FileFailed += FileFailed;
                imageImporter.ImportFinished += ImportFinished;
                var arguments = parser.Object;
                imageImporter.Initialize(arguments.ConfigurationPath, arguments.OutputDirectory, arguments.RawFileExtensions, arguments.NonRawFileExtensions, arguments.VideoFileExtensions, string.Empty);
                imageImporter.Import(arguments.InputDirectory);                
            }
            else
            {
                if (!parseResult.HelpCalled)
                {
                    Console.WriteLine(parseResult.ErrorText);
                }
            }
        }

        private static void ImportStarted(object sender, ImportEventArgs e)
        {
            var progressBarOptions = new ProgressBarOptions
            {
                ProgressBarOnBottom = true,
                ProgressCharacter = '=',
                CollapseWhenFinished = false,
                DisplayTimeInRealTime = true,
                ForeGroundColorDone = ConsoleColor.Green,
                ForeGroundColor = ConsoleColor.Yellow,
            };
            progressBar = new ProgressBar(e.NumberOfFiles, $"Importing files from {e.SourceDirectory} to {e.DestinationDirectory}", progressBarOptions);
        }

        private static void FileCopied(object sender, FileEventArgs e)
        {
            progressBar.Tick($"Copying {e.Filename} to {e.SubDirectory}");
        }

        private static void FileFailed(object sender, FileEventArgs e)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error when copying {e.Filename} to {e.SubDirectory}: {e.ErrorMessage}");
            Console.ForegroundColor = previousColor;
        }

        private static void ImportFinished(object sender, ImportEventArgs e)
        {
            progressBar.Dispose();
        }

        private static Fclp.FluentCommandLineParser<Arguments> SetupParser()
        {
            var parser = new Fclp.FluentCommandLineParser<Arguments>();
            parser.SetupHelp("?", "help", "h")
                .Callback(t => Console.WriteLine(t))
                .UseForEmptyArgs();
            parser.Setup(a => a.InputDirectory)
                .As('i', "input-directory")
                .WithDescription("Input directory with image files")
                .Required();                
            parser.Setup(a => a.OutputDirectory)
                .As('o', "output-directory")
                .WithDescription("Directory to put files to (will be created if absent)")
                .SetDefault(Environment.CurrentDirectory);
            parser.Setup(a => a.ConfigurationPath)
                .As('c', "configuration-path")
                .WithDescription("Path to a configuration xml file");
            parser.Setup<List<string>>(a => a.RawFileExtensions)
                .As('r', "raw-types")
                .WithDescription("Extensions for RAW images (e.g. -r .ext1 .ext2)")
                .SetDefault(new List<string>());
            parser.Setup<List<string>>(a => a.NonRawFileExtensions)
                .As('n', "non-raw-types")
                .WithDescription("Extensions for non-RAW images (e.g. JPEG) (i.e. -r .ext1 .ext2)")
                .SetDefault(new List<string>());
            parser.Setup<List<string>>(a => a.VideoFileExtensions)
                .As('v', "video-types")
                .WithDescription("Extensions for video files (i.e. -r .ext1 .ext2)")
                .SetDefault(new List<string>());
            return parser;
        }
    }
}
