using System.IO;

namespace ImageImporter.FileProcessor
{
    /// <summary>
    /// Processes a specific file kind according to rules specific to it
    /// </summary>
    public interface IFileProcessor
    {
        /// <summary>
        /// Process a single file
        /// </summary>
        /// <param name="inputFile">File descriptor to process</param>
        /// <param name="fileKind">File kind to process</param>
        /// <param name="outputDirectory">Directory to put file to</param>
        /// <returns>Path to put file to</returns>
        string Process(FileInfo inputFile, FileKind fileKind, string outputDirectory);
    }
}