using System;
using ShellProgressBar;
using ImageDownloader.Core;
using ImageDownloader.InputHandling;

namespace ImageDownloader.Cli
{
    class Program
    {
        private static IImageDownloader _imageDownloader;
        private static IInputHandler _inputHandler;
        private static ProgressBar _progressBar;

        static void Main(string[] args)
        {
            _inputHandler = new InputHandler();
            var arguments = _inputHandler.Parse(args);
            if (arguments != null)
            {
                _imageDownloader = new Core.ImageDownloader(arguments);
                _imageDownloader.ImportStarted += ImportStarted;
                _imageDownloader.FileCopied += FileCopied;
                _imageDownloader.FileFailed += FileFailed;
                _imageDownloader.ImportDone += ImportDone;
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
            _progressBar = new ProgressBar(e.NumberOfFiles, $"Importing files from {e.SourceDirectory} to {e.DestinationDirectory}", progressBarOptions);
        }

        private static void FileCopied(object sender, FileEventArgs e)
        {
            _progressBar.Tick($"Copying {e.Filename} to {e.SubDirectory}");
        }

        private static void FileFailed(object sender, FileEventArgs e)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error when copying {e.Filename} to {e.SubDirectory}: {e.ErrorMessage}");
            Console.ForegroundColor = previousColor;
        }

        private static void ImportDone(object sender, ImportEventArgs e)
        {
            _progressBar.Dispose();
        }
    }
}
