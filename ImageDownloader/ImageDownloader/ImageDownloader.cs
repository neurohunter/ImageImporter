using System;
using System.IO;
using MetadataExtractor;
using System.Linq;
using MetadataExtractor.Formats.Exif;

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

            foreach(var file in System.IO.Directory.GetFiles(inputDirectory))
            {
                var fileInfo = new FileInfo(file);
                var metadataDirectories = ImageMetadataReader.ReadMetadata(file);
                var dateTimeTaken = DateTime.Now;
                var exifTagDirectory = metadataDirectories.OfType<ExifIfd0Directory>().FirstOrDefault();
                if (exifTagDirectory != null)
                {
                    dateTimeTaken = exifTagDirectory.TryGetDateTime(ExifDirectoryBase.TagDateTime, out var dateTime) ? dateTime : dateTimeTaken;
                    var subDirectory = string.Empty;
                    switch (fileInfo.Extension.ToLower())
                    {
                        case ".cr2":
                        case ".arw":
                        case ".cr3":
                            subDirectory = "RAW";
                            break;
                        case ".jpg":
                            subDirectory = "JPG";
                            break;
                        default:
                            break;
                    }
                    var destinationPath = CreateDestinationPath(outputDirectory, dateTimeTaken, subDirectory, fileInfo.Name);
                    File.Copy(file, destinationPath, false);
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
