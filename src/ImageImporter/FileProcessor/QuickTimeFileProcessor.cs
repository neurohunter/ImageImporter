using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ImageImporter.FileProcessor
{
    internal class QuickTimeFileProcessor : FileProcessor
    {
        public override string Process(FileInfo inputFile, FileKind fileKind, string outputDirectory)
        {
            DateTime dateTimeTaken = DateTime.Now;
            try
            {
                var metadataDirectories = ImageMetadataReader.ReadMetadata(inputFile.FullName);
                try
                {
                    var tagCollection = metadataDirectories.OfType<MetadataExtractor.Formats.QuickTime.QuickTimeMovieHeaderDirectory>().FirstOrDefault();
                    dateTimeTaken = tagCollection.GetDateTime(MetadataExtractor.Formats.QuickTime.QuickTimeMovieHeaderDirectory.TagCreated);
                }
                catch (MetadataException)
                {
                    try
                    {
                        var tagCollection = metadataDirectories.OfType<MetadataExtractor.Formats.QuickTime.QuickTimeMetadataHeaderDirectory>().FirstOrDefault();
                        dateTimeTaken = tagCollection.GetDateTime(MetadataExtractor.Formats.QuickTime.QuickTimeMetadataHeaderDirectory.TagCreationDate);
                    }
                    catch (MetadataException)
                    {
                        // if no DateTime tag found - try to guess the date from file name
                        var dateFileNamePart = inputFile.Name.Split('_')[0];
                        dateTimeTaken = DateTime.ParseExact(dateFileNamePart, "yyyyMMdd", CultureInfo.InvariantCulture);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                return CreateDestinationPath(outputDirectory, dateTimeTaken.Date.ToString("yyyy_MM_dd"), fileKind.GetAttributeOfType<DescriptionAttribute>().Description, inputFile.Name);
            }
            catch (Exception e)
            {
                throw new FileProcessorException($"Cannot read EXIF metadata from {inputFile.FullName}", e);
            }
        }
    }
}