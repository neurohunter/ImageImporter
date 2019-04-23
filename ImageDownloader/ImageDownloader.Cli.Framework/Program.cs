using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.Cli.Framework
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = SetupParser();
            var parseResult = parser.Parse(args);
            if (!parseResult.HasErrors && !parseResult.HelpCalled)
            {
                IImageDownloader imageDownloader = new ImageDownloader();
                imageDownloader.Download(parser.Object.InputDirectory, parser.Object.OutputDirectory);
            }
            else
            {
                if (!parseResult.HelpCalled)
                {
                    Console.WriteLine(parseResult.ErrorText);
                }
            }
            if(!Debugger.IsAttached && !parseResult.HelpCalled)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }

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
                .Required();
            return parser;
        }
    }
}
