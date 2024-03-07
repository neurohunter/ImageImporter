using System.IO;

namespace ImageImporter.FileProcessor
{
    /// <summary>
    /// An interface to retrieve date and time information based on file type
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