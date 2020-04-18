using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace ImageImporter.FileProcessor
{
    /// <inheritdoc />
    public class ExifFileProcessor:FileProcessor
    {
        /// <inheritdoc />
        public override string Process(FileInfo inputFile, FileKind fileKind, string outputDirectory)
        {
            DateTime dateTimeTaken = DateTime.Now;
            try
            {
                var metadataDirectories = ImageMetadataReader.ReadMetadata(inputFile.FullName);
                try
                {
                    var tagCollection = metadataDirectories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                    dateTimeTaken = tagCollection.GetDateTime(ExifSubIfdDirectory.TagDateTimeDigitized);
                }
                catch(MetadataException)
                {
                    var tagCollection = metadataDirectories.OfType<ExifDirectoryBase>().FirstOrDefault();
                    dateTimeTaken = tagCollection.GetDateTime(ExifDirectoryBase.TagDateTime);
                }
                catch(Exception)
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
