using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace ImageImporter.FileProcessor
{
    /// <summary>
    /// Specific file processor for HEIC files produces by Apple devices
    /// </summary>
    public class HeicFileProcessor : FileProcessor
    {
        /// </<inheritdoc/>
        public override string Process(string inputFileName, FileKind fileKind, string outputDirectory)
        {
            var dateString = ReadDateFromFile(inputFileName);
            return CreateDestinationPath(outputDirectory, dateString, fileKind.GetAttributeOfType<DescriptionAttribute>().Description, Path.GetFileNameWithoutExtension(inputFileName));
        }

        /// <summary>
        /// Reads date digitized tag from file
        /// </summary>
        /// <param name="inputFile">File to get tag from</param>
        /// <returns>String representation of date</returns>
        private string ReadDateFromFile(string inputFileName)
        {
            return DateTime.Now.Date.ToString("yyyy_MM_dd");
        }
    }
}
