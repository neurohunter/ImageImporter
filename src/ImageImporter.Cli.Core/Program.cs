using ShellProgressBar;
using System;
using System.Threading.Tasks;

namespace ImageImporter.Cli.Core
{
    class Program
    {
        private static ProgressBar progressBar;

        static void Main(string[] args)
        {

            if (Arguments.Parse(args, out var arguments))
            {
                IImageImporter imageImporter = new ImageImporter();
                imageImporter.ImportStarted += ImportStarted;
                imageImporter.FileCopied += FileCopied;
                imageImporter.FileFailed += FileFailed;
                imageImporter.ImportFinished += ImportFinished;
                imageImporter.Initialize(
                    arguments.ConfigurationPath,
                    arguments.OutputDirectory,
                    arguments.RawFileExtensions,
                    arguments.NonRawFileExtensions,
                    arguments.VideoFileExtensions,
                    string.Empty);
                imageImporter.Import(arguments.InputDirectory);
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
                ForegroundColorDone = ConsoleColor.Green,
                ForegroundColor = ConsoleColor.Yellow,
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
    }
}
