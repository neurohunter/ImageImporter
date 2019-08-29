using System;
using System.Collections.Generic;
using System.IO;
using MetadataExtractor;
using System.Linq;
using MetadataExtractor.Formats.Exif;

namespace ImageDownloader
{
    /// <summary>
    /// FIle metatype
    /// </summary>
    public enum ImageKind
    {
        /// <summary>
        /// Unidentified
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// RAW image
        /// </summary>
        RAW = 1,
        /// <summary>
        /// JPG (non-RAW) image
        /// </summary>
        JPG = 2,
        /// <summary>
        /// Video file
        /// </summary>
        Video = 3
    }

    public class ImageDownloader : IImageDownloader
    {
        Configuration.Configuration m_Configuration;
        private readonly ConfigurationProvider m_ConfigurationProvider;

        public ImageDownloader()
        { 
            m_ConfigurationProvider = new ConfigurationProvider();
            m_Configuration = m_ConfigurationProvider.ProvideDefaultConfiguration();            
        }
        
        /// <inheritdoc/>
        public void Initialize(string configurationFilePath)
        {
            m_Configuration = m_ConfigurationProvider.InitializeFromFile(configurationFilePath);
        }
        
        /// <inheritdoc/>
        public void Download(string inputDrirectory)
        {
            //TODO just call full overload, move the rest out
            var outputDirectory = !string.IsNullOrEmpty(m_Configuration.Destination) ? m_Configuration.Destination : Environment.CurrentDirectory;
            Download(inputDrirectory, string.Empty);
        }

        /// <inheritdoc/>
        public void Download(string inputDirectory, string outputDirectory)
        {
            //TODO properly call ful download
            Download(inputDirectory, outputDirectory, null, null, null, string.Empty);
        }

        /// <inheritdoc/>
        public void Download(string inputDirectory, string outputDirectory, IEnumerable<string> rawFiles, IEnumerable<string> nonRawFiles, IEnumerable<string> videoFiles, string pattern)
        {
            // TODO initialize configuration and use it afterwards, do some smartguessing if parameters are not provided
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
                    var fileClass = ClassifyFile(fileInfo.Extension.ToLower());
                    switch (fileClass)
                    {
                        case ImageKind.Undefined:
                            subDirectory = "MISC";
                            break;
                        case ImageKind.RAW:
                            subDirectory = "RAW";
                            break;
                        case ImageKind.JPG:
                            subDirectory = "JPG";
                            break;
                        case ImageKind.Video:
                            subDirectory = "VIDEO";
                            break;                        
                    }
                    var destinationPath = CreateDestinationPath(outputDirectory, dateTimeTaken, subDirectory, fileInfo.Name);
                    File.Copy(file, destinationPath, false);
                }

            }
        }

        /// <summary>
        /// Detects if a file is of a certain metatype
        /// </summary>
        /// <param name="fileExtension">File extension</param>
        /// <returns>File metatype</returns>
        private ImageKind ClassifyFile(string fileExtension)
        {
            if (m_Configuration.FileTypes.RawFileRypes.Contains(fileExtension))
            {
                return ImageKind.RAW;
            }
            else
            { 
                if (m_Configuration.FileTypes.NonRawFileRypes.Contains(fileExtension))
                {
                    return ImageKind.JPG;
                }
                else
                {
                    if (m_Configuration.FileTypes.VideoFileTypes.Contains(fileExtension))
                    {
                        return ImageKind.Video;
                    }
                    else
                    {
                        return ImageKind.Undefined;
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
