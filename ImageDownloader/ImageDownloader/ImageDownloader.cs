using System;
using System.IO;
using MetadataExtractor;
using System.Linq;
using MetadataExtractor.Formats.Exif;
using ShellProgressBar;

namespace ImageDownloader
{
    public class ImageDownloader:IImageDownloader
    {
        public void Download(string inputDirectory, string outputDirectory)
        {
            if (!System.IO.Directory.Exists(outputDirectory))
            {
                System.IO.Directory.CreateDirectory(outputDirectory);
            }

            var filesToImport = System.IO.Directory.GetFiles(inputDirectory).ToList();

            var progressBarOptions = new ProgressBarOptions
            {
                ProgressBarOnBottom = true,
                ProgressCharacter = '=',
                CollapseWhenFinished = false,
                DisplayTimeInRealTime = true,
                EnableTaskBarProgress = true,
                ForegroundColorDone = ConsoleColor.Green,
                ForegroundColor = ConsoleColor.Yellow,
                
            };
            using (var progressBar = new ProgressBar(filesToImport.Count, $"Importing files from {inputDirectory} to {outputDirectory}", progressBarOptions))
            {
                foreach (var file in filesToImport)
                {
                    var fileInfo = new FileInfo(file);
                    var metadataDirectories = ImageMetadataReader.ReadMetadata(file);
                    var dateTimeTaken = DateTime.Now;
                    var exifTagDirectory = metadataDirectories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                    if (exifTagDirectory != null)
                    {
                        dateTimeTaken = exifTagDirectory.TryGetDateTime(ExifDirectoryBase.TagDateTimeDigitized, out var dateTime) ? dateTime : dateTimeTaken;
                        var subDirectory = string.Empty;
                        switch (fileInfo.Extension.ToLower())
                        {
                            case ".cr2":
                            case ".arw":
                            case ".cr3":
                                subDirectory = "RAW";
                                break;
                            case ".heic":
                            case ".jpg":
                                subDirectory = "JPG";
                                break;
                            default:
                                break;
                        }

                        var destinationPath = CreateDestinationPath(outputDirectory, dateTimeTaken, subDirectory, fileInfo.Name);
                        File.Copy(file, destinationPath, false);
                        progressBar.Tick($"Copying {fileInfo.Name} to {subDirectory}");
                    }

                }
            }
        }

        private string CreateDestinationPath(string outputDirectory, DateTime dateTimeTaken, string subDirectory, string fileName)
        {
            var dateAsString = dateTimeTaken.Date.ToString("yyyy_MM_dd");
            var directory = Path.Combine(outputDirectory, dateAsString, subDirectory);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            return Path.Combine(directory, fileName);
        }
    }
}
