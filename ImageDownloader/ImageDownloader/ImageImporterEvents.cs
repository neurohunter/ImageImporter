using System;

namespace ImageImporter
{
    /// <summary>
    /// Event arguments for import process
    /// </summary>
    public class ImportEventArgs : EventArgs
    {
        public ImportEventArgs(string sourceDirectory, string destinationDirectory, int numberOfFiles)
        {
            SourceDirectory = sourceDirectory;
            DestinationDirectory = destinationDirectory;
            NumberOfFiles = numberOfFiles;
        }
        /// <summary>
        /// Original images location
        /// </summary>
        public string SourceDirectory {get;}
        /// <summary>
        /// Directory to put images to
        /// </summary>
        public string DestinationDirectory {get;}
        /// <summary>
        /// Number of files to process
        /// </summary>
        public int NumberOfFiles {get;}
    }

    /// <summary>
    /// Event arguments for a file handling
    /// </summary>
    public class FileEventArgs : EventArgs
    {
        public FileEventArgs(string filename, string subDirectory, string errorMessage)
        {
            Filename = filename;
            SubDirectory = subDirectory;
            ErrorMessage = errorMessage;
        }

        public FileEventArgs(string filename, string subDirectory):this(filename, subDirectory, string.Empty){ }
        /// <summary>
        /// Filename that is currently processed
        /// </summary>
        public string Filename {get;}
        /// <summary>
        /// Subdirectory based on a file type
        /// </summary>
        public string SubDirectory {get;}
        /// <summary>
        /// File processing error
        /// </summary>
        public string ErrorMessage {get;}
    }
}