using System.IO;

namespace ImageImporter.FileProcessor
{
    /// <inheritdoc />
    public class NullFileProcessor:FileProcessor
    {
        /// <inheritdoc />
        public override string Process(FileInfo inputFile, FileKind fileKind, string outputDirectory)
        {
            return string.Empty;
        }
    }
}
