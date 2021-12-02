using System;

namespace ImageImporter.PathBuilder
{
    /// <summary>
    /// An interface to create a directory to move file to
    /// </summary>
    public interface IDestinationPathBuilder
    {
        string BuildDestinationDirectoryPath(string rootDirectory, DateTime dateTime, FileKind fileKind);
    }
}
