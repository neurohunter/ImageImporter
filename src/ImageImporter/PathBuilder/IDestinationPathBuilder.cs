using System;

namespace ImageImporter.PathBuilder
{
    public interface IDestinationPathBuilder
    {
        string BuildDestinationDirectoryPath(string rootDirectory, DateTime dateTime, FileKind fileKind);
    }
}
