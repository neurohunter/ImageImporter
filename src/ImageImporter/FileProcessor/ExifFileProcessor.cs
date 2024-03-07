using System;
using System.ComponentModel;
using System.Globalization;
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
        public override string Process(string inputFileName, FileKind fileKind, string outputDirectory)
        {
            DateTime dateTimeTaken = DateTime.Now;
            try
            {
                var metadataDirectories = ImageMetadataReader.ReadMetadata(inputFileName);
                try
                {
                    // try getting date and time from DateTimeDigitezed tag
                    var tagCollection = metadataDirectories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                    dateTimeTaken = tagCollection.GetDateTime(ExifSubIfdDirectory.TagDateTimeDigitized);
                }
                catch(MetadataException)
                {
                    try
                    {
                        // if no DateTimeDigitized tag found - try getting date and time from DateTime tag
                        var tagCollection = metadataDirectories.OfType<ExifDirectoryBase>().FirstOrDefault();
                        dateTimeTaken = tagCollection.GetDateTime(ExifDirectoryBase.TagDateTime);
                    }
                    catch (MetadataException)
                    {
                        try
                        {
                            // if no DateTime tag found - try to guess the date from file name
                            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputFileName);
                            var dateFileNamePart = fileNameWithoutExtension.Split('_')[0];
                            dateTimeTaken = DateTime.ParseExact(dateFileNamePart, "yyyyMMdd", CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            dateTimeTaken = File.GetLastWriteTime(inputFileName);
                        }
                    }
                }
                catch(Exception)
                {   
                    throw;
                }
                return CreateDestinationPath(outputDirectory, dateTimeTaken.Date.ToString("yyyy_MM_dd"), fileKind.GetAttributeOfType<DescriptionAttribute>().Description, Path.GetFileName(inputFileName));
            }
            catch (Exception e)
            {
                throw new FileProcessorException($"Cannot read EXIF metadata from {inputFileName}", e);
            }
        }
    }
}
