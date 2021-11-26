using System.IO;

namespace ImageImporter.FileProcessor
{
    /// <inheritdoc />
    public class NullFileProcessor:FileProcessor
    {
        /// <inheritdoc />
        public override string Process(string inputFileName, FileKind fileKind, string outputDirectory)
        {
            return string.Empty;
        }
    }
}
