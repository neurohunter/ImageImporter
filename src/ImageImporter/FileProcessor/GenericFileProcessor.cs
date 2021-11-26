using System.ComponentModel;
using System.IO;

namespace ImageImporter.FileProcessor
{
    /// <inheritdoc />
    public class GenericFileProcessor:FileProcessor
    {
        /// <inheritdoc />
        public override string Process(string inputFileName, FileKind fileKind, string outputDirectory)
        {
            return CreateDestinationPath(outputDirectory, File.GetLastAccessTime(inputFileName).Date.ToString("yyyy_MM_dd"), fileKind.GetAttributeOfType<DescriptionAttribute>().Description, Path.GetFileName(inputFileName));
        }
    }
}
