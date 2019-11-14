using System.IO;

namespace ImageImporter.FileProcessor
{
    public abstract class FileProcessor:IFileProcessor
    {
        /// <inheritdoc />
        public abstract string Process(FileInfo inputFile, FileKind fileKind, string outputDirectory);

        /// <summary>
        /// Creates a destination file path
        /// </summary>
        /// <param name="rootDirectory">Root output directory</param>
        /// <param name="templateDirectory">Subdirectory from a template</param>
        /// <param name="fileKindDirectory">Subdirectory based on file kind</param>
        /// <param name="fileName">File to store</param>
        /// <returns>Path to store a file</returns>'
        /// <remarks>Creates a destination directory if it does not exist</remarks>
        /// <example>Build a path like <paramref name="rootDirectory"/>\<paramref name="templateDirectory"/>\<paramref name="fileKindDirectory"/>\<paramref name="fileName"/></example>
        public string CreateDestinationPath(string rootDirectory, string templateDirectory, string fileKindDirectory, string fileName)
        {
            string destinationDirectory = Path.Combine(rootDirectory, templateDirectory, fileKindDirectory);
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }
            return Path.Combine(destinationDirectory, fileName);
        }
    }
}
