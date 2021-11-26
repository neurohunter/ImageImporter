using System;
using System.ComponentModel;

namespace ImageImporter.PathBuilder
{
    public class DestinationPathBuilder : IDestinationPathBuilder
    {
        public string BuildDestinationDirectoryPath(string rootDirectory, DateTime dateTime, FileKind fileKind)
        {
            return System.IO.Path.Combine(rootDirectory, dateTime.ToString("yyyy_MM_dd"), fileKind.GetAttributeOfType<DescriptionAttribute>().Description);
        }
    }
}
