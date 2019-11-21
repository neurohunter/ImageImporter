using System;

namespace ImageImporter
{
    public class ImportEventArgs : EventArgs
    {
        public ImportEventArgs(string sourceDirectory, string destinationDirectory, int numberOfFiles)
        {
            SourceDirectory = sourceDirectory;
            DestinationDirectory = destinationDirectory;
            NumberOfFiles = numberOfFiles;
        }

        public string SourceDirectory {get;}
        public string DestinationDirectory {get;}
        public int NumberOfFiles {get;}
    }

    public class FileEventArgs : EventArgs
    {
        public FileEventArgs(string filename, string subDirectory, string errorMessage)
        {
            Filename = filename;
            SubDirectory = subDirectory;
            ErrorMessage = errorMessage;
        }

        public FileEventArgs(string filename, string subDirectory):this(filename, subDirectory, string.Empty){ }

        public string Filename {get;}
        public string SubDirectory {get;}
        public string ErrorMessage {get;}
    }
}