using System.IO;

namespace ImageImporter.FileProcessor
{
    /// <summary>
    /// Processes a single file kind according to rules defined for it
    /// </summary>
    public interface IFileProcessor
    {
        /// <summary>
        /// Process a single file
        /// </summary>
        /// <param name="inputFilePath">File path to process</param>
        /// <param name="fileKind">File kind to process</param>
        /// <param name="outputDirectory">Directory to put file to</param>
        /// <returns>Path to put file to</returns>
        string Process(string inputFilePath, FileKind fileKind, string outputDirectory);
    }
}